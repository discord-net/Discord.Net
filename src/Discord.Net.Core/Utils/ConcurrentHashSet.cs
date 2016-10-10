using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace Discord
{
    //Based on https://github.com/dotnet/corefx/blob/d0dc5fc099946adc1035b34a8b1f6042eddb0c75/src/System.Threading.Tasks.Parallel/src/System/Threading/PlatformHelper.cs
    //Copyright (c) .NET Foundation and Contributors
    internal static class ConcurrentHashSet
    {
        private const int PROCESSOR_COUNT_REFRESH_INTERVAL_MS = 30000;
        private static volatile int s_processorCount;
        private static volatile int s_lastProcessorCountRefreshTicks;

        public static int DefaultConcurrencyLevel
        {
            get
            {
                int now = Environment.TickCount;
                if (s_processorCount == 0 || (now - s_lastProcessorCountRefreshTicks) >= PROCESSOR_COUNT_REFRESH_INTERVAL_MS)
                {
                    s_processorCount = Environment.ProcessorCount;
                    s_lastProcessorCountRefreshTicks = now;
                }

                return s_processorCount;
            }
        }
    }

    //Based on https://github.com/dotnet/corefx/blob/master/src/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentDictionary.cs
    //Copyright (c) .NET Foundation and Contributors
    [DebuggerDisplay("Count = {Count}")]
    internal class ConcurrentHashSet<T> : IReadOnlyCollection<T>
    {
        private sealed class Tables
        {
            internal readonly Node[] _buckets;
            internal readonly object[] _locks;
            internal volatile int[] _countPerLock;

            internal Tables(Node[] buckets, object[] locks, int[] countPerLock)
            {
                _buckets = buckets;
                _locks = locks;
                _countPerLock = countPerLock;
            }
        }
        private sealed class Node
        {
            internal readonly T _value;
            internal volatile Node _next;
            internal readonly int _hashcode;

            internal Node(T key, int hashcode, Node next)
            {
                _value = key;
                _next = next;
                _hashcode = hashcode;
            }
        }

        private const int DefaultCapacity = 31;
        private const int MaxLockNumber = 1024;

        private static int GetBucket(int hashcode, int bucketCount)
        {
            int bucketNo = (hashcode & 0x7fffffff) % bucketCount;
            return bucketNo;
        }
        private static void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount, int lockCount)
        {
            bucketNo = (hashcode & 0x7fffffff) % bucketCount;
            lockNo = bucketNo % lockCount;
        }
        private static int DefaultConcurrencyLevel => ConcurrentHashSet.DefaultConcurrencyLevel;

        private volatile Tables _tables;
        private readonly IEqualityComparer<T> _comparer; 
        private readonly bool _growLockArray;
        private int _budget;        

        public int Count
        {
            get
            {
                int count = 0;

                int acquiredLocks = 0;
                try
                {
                    AcquireAllLocks(ref acquiredLocks);

                    for (int i = 0; i < _tables._countPerLock.Length; i++)
                        count += _tables._countPerLock[i];
                }
                finally { ReleaseLocks(0, acquiredLocks); }

                return count;
            }
        }
        public bool IsEmpty
        {
            get
            {
                int acquiredLocks = 0;
                try
                {
                    // Acquire all locks
                    AcquireAllLocks(ref acquiredLocks);

                    for (int i = 0; i < _tables._countPerLock.Length; i++)
                    {
                        if (_tables._countPerLock[i] != 0)
                            return false;
                    }
                }
                finally { ReleaseLocks(0, acquiredLocks); }

                return true;
            }
        }
        public ReadOnlyCollection<T> Values
        {
            get
            {
                int locksAcquired = 0;
                try
                {
                    AcquireAllLocks(ref locksAcquired);
                    List<T> values = new List<T>();

                    for (int i = 0; i < _tables._buckets.Length; i++)
                    {
                        Node current = _tables._buckets[i];
                        while (current != null)
                        {
                            values.Add(current._value);
                            current = current._next;
                        }
                    }

                    return new ReadOnlyCollection<T>(values);
                }
                finally { ReleaseLocks(0, locksAcquired); }
            }
        }

        public ConcurrentHashSet() 
            : this(DefaultConcurrencyLevel, DefaultCapacity, true, EqualityComparer<T>.Default) { }
        public ConcurrentHashSet(int concurrencyLevel, int capacity) 
            : this(concurrencyLevel, capacity, false, EqualityComparer<T>.Default) { }
        public ConcurrentHashSet(IEnumerable<T> collection) 
            : this(collection, EqualityComparer<T>.Default) { }
        public ConcurrentHashSet(IEqualityComparer<T> comparer) 
            : this(DefaultConcurrencyLevel, DefaultCapacity, true, comparer) { }
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) 
            : this(comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            InitializeFromCollection(collection);
        }
        public ConcurrentHashSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : this(concurrencyLevel, DefaultCapacity, false, comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            InitializeFromCollection(collection);
        }        
        public ConcurrentHashSet(int concurrencyLevel, int capacity, IEqualityComparer<T> comparer)
            : this(concurrencyLevel, capacity, false, comparer) { }
        internal ConcurrentHashSet(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<T> comparer)
        {
            if (concurrencyLevel < 1) throw new ArgumentOutOfRangeException(nameof(concurrencyLevel));
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
                        
            if (capacity < concurrencyLevel)
                capacity = concurrencyLevel;

            object[] locks = new object[concurrencyLevel];
            for (int i = 0; i < locks.Length; i++)
                locks[i] = new object();

            int[] countPerLock = new int[locks.Length];
            Node[] buckets = new Node[capacity];
            _tables = new Tables(buckets, locks, countPerLock);

            _comparer = comparer;
            _growLockArray = growLockArray;
            _budget = buckets.Length / locks.Length;
        }
        private void InitializeFromCollection(IEnumerable<T> collection)
        {
            foreach (var value in collection)
            {
                if (value == null) throw new ArgumentNullException("key");

                if (!TryAddInternal(value, _comparer.GetHashCode(value), false))
                    throw new ArgumentException();
            }

            if (_budget == 0)
                _budget = _tables._buckets.Length / _tables._locks.Length;
        }
        
        public bool ContainsKey(T value)
        {
            if (value == null) throw new ArgumentNullException("key");
            return ContainsKeyInternal(value, _comparer.GetHashCode(value));
        }
        private bool ContainsKeyInternal(T value, int hashcode)
        {
            Tables tables = _tables;

            int bucketNo = GetBucket(hashcode, tables._buckets.Length);
            
            Node n = Volatile.Read(ref tables._buckets[bucketNo]);

            while (n != null)
            {
                if (hashcode == n._hashcode && _comparer.Equals(n._value, value))
                    return true;
                n = n._next;
            }
            
            return false;
        }

        public bool TryAdd(T value)
        {
            if (value == null)  throw new ArgumentNullException("key");
            return TryAddInternal(value, _comparer.GetHashCode(value), true);
        }
        private bool TryAddInternal(T value, int hashcode, bool acquireLock)
        {
            while (true)
            {
                int bucketNo, lockNo;

                Tables tables = _tables;
                GetBucketAndLockNo(hashcode, out bucketNo, out lockNo, tables._buckets.Length, tables._locks.Length);

                bool resizeDesired = false;
                bool lockTaken = false;
                try
                {
                    if (acquireLock)
                        Monitor.Enter(tables._locks[lockNo], ref lockTaken);

                    if (tables != _tables)
                        continue;

                    Node prev = null;
                    for (Node node = tables._buckets[bucketNo]; node != null; node = node._next)
                    {
                        if (hashcode == node._hashcode && _comparer.Equals(node._value, value))
                            return false;
                        prev = node;
                    }

                    Volatile.Write(ref tables._buckets[bucketNo], new Node(value, hashcode, tables._buckets[bucketNo]));
                    checked { tables._countPerLock[lockNo]++; }

                    if (tables._countPerLock[lockNo] > _budget)
                        resizeDesired = true;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(tables._locks[lockNo]);
                }

                if (resizeDesired)
                    GrowTable(tables);
                
                return true;
            }
        }

        public bool TryRemove(T value)
        {
            if (value == null) throw new ArgumentNullException("key");
            return TryRemoveInternal(value);
        }        
        private bool TryRemoveInternal(T value)
        {
            int hashcode = _comparer.GetHashCode(value);
            while (true)
            {
                Tables tables = _tables;

                int bucketNo, lockNo;
                GetBucketAndLockNo(hashcode, out bucketNo, out lockNo, tables._buckets.Length, tables._locks.Length);

                lock (tables._locks[lockNo])
                {
                    if (tables != _tables)
                        continue;

                    Node prev = null;
                    for (Node curr = tables._buckets[bucketNo]; curr != null; curr = curr._next)
                    {
                        if (hashcode == curr._hashcode && _comparer.Equals(curr._value, value))
                        {
                            if (prev == null)
                                Volatile.Write(ref tables._buckets[bucketNo], curr._next);
                            else
                                prev._next = curr._next;

                            value = curr._value;
                            tables._countPerLock[lockNo]--;
                            return true;
                        }
                        prev = curr;
                    }
                }

                value = default(T);
                return false;
            }
        }
        
        public void Clear()
        {
            int locksAcquired = 0;
            try
            {
                AcquireAllLocks(ref locksAcquired);

                Tables newTables = new Tables(new Node[DefaultCapacity], _tables._locks, new int[_tables._countPerLock.Length]);
                _tables = newTables;
                _budget = Math.Max(1, newTables._buckets.Length / newTables._locks.Length);
            }
            finally
            {
                ReleaseLocks(0, locksAcquired);
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            Node[] buckets = _tables._buckets;

            for (int i = 0; i < buckets.Length; i++)
            {
                Node current = Volatile.Read(ref buckets[i]);

                while (current != null)
                {
                    yield return current._value;
                    current = current._next;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void GrowTable(Tables tables)
        {
            const int MaxArrayLength = 0X7FEFFFFF;
            int locksAcquired = 0;
            try
            {
                AcquireLocks(0, 1, ref locksAcquired);
                if (tables != _tables)
                    return;

                long approxCount = 0;
                for (int i = 0; i < tables._countPerLock.Length; i++)
                    approxCount += tables._countPerLock[i];

                if (approxCount < tables._buckets.Length / 4)
                {
                    _budget = 2 * _budget;
                    if (_budget < 0)
                        _budget = int.MaxValue;
                    return;
                }

                int newLength = 0;
                bool maximizeTableSize = false;
                try
                {
                    checked
                    {
                        newLength = tables._buckets.Length * 2 + 1;
                        while (newLength % 3 == 0 || newLength % 5 == 0 || newLength % 7 == 0)
                            newLength += 2;

                        if (newLength > MaxArrayLength)
                            maximizeTableSize = true;
                    }
                }
                catch (OverflowException)
                {
                    maximizeTableSize = true;
                }

                if (maximizeTableSize)
                {
                    newLength = MaxArrayLength;
                    _budget = int.MaxValue;
                }

                AcquireLocks(1, tables._locks.Length, ref locksAcquired);

                object[] newLocks = tables._locks;

                if (_growLockArray && tables._locks.Length < MaxLockNumber)
                {
                    newLocks = new object[tables._locks.Length * 2];
                    Array.Copy(tables._locks, 0, newLocks, 0, tables._locks.Length);
                    for (int i = tables._locks.Length; i < newLocks.Length; i++)
                        newLocks[i] = new object();
                }

                Node[] newBuckets = new Node[newLength];
                int[] newCountPerLock = new int[newLocks.Length];

                for (int i = 0; i < tables._buckets.Length; i++)
                {
                    Node current = tables._buckets[i];
                    while (current != null)
                    {
                        Node next = current._next;
                        int newBucketNo, newLockNo;
                        GetBucketAndLockNo(current._hashcode, out newBucketNo, out newLockNo, newBuckets.Length, newLocks.Length);

                        newBuckets[newBucketNo] = new Node(current._value, current._hashcode, newBuckets[newBucketNo]);

                        checked { newCountPerLock[newLockNo]++; }

                        current = next;
                    }
                }

                _budget = Math.Max(1, newBuckets.Length / newLocks.Length);
                _tables = new Tables(newBuckets, newLocks, newCountPerLock);
            }
            finally { ReleaseLocks(0, locksAcquired); }
        }

        private void AcquireAllLocks(ref int locksAcquired)
        {
            AcquireLocks(0, 1, ref locksAcquired);
            AcquireLocks(1, _tables._locks.Length, ref locksAcquired);
        }
        private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
        {
            object[] locks = _tables._locks;

            for (int i = fromInclusive; i < toExclusive; i++)
            {
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(locks[i], ref lockTaken);
                }
                finally
                {
                    if (lockTaken)
                        locksAcquired++;
                }
            }
        }
        private void ReleaseLocks(int fromInclusive, int toExclusive)
        {
            for (int i = fromInclusive; i < toExclusive; i++)
                Monitor.Exit(_tables._locks[i]);
        }                
    }
}