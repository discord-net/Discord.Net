using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Serialization
{
    public class ModelMap<TModel>
        where TModel : class, new()
    {
        private struct PropertyEntry
        {
            public int hashCode;             // Lower 31 bits of hash code, -1 if unused
            public int next;                 // Index of next entry, -1 if last
            public ReadOnlyBuffer<byte> key; // Key of entry
            public PropertyMap value;        // Value of entry
        }

        private int[] _buckets;
        private PropertyEntry[] _entries;
        private int _count;
        private int _freeList;
        private int _freeCount;

        public PropertyMap[] Properties { get; }

        public ModelMap(List<PropertyMap> properties)
        {
            Properties = properties.ToArray();

            int size = HashHelpers.GetPrime(Properties.Length);
            _buckets = new int[size];
            for (int i = 0; i < _buckets.Length; i++) _buckets[i] = -1;
            _entries = new PropertyEntry[size];
            _freeList = -1;

            for (int i = 0; i < properties.Count; i++)
                AddProperty(properties[i].Utf8Key, properties[i]);
        }       

        public void AddProperty(ReadOnlyBuffer<byte> key, PropertyMap value)
        {
            int hashCode = GetKeyHashCode(key) & 0x7FFFFFFF;
            int targetBucket = hashCode % _buckets.Length;

            for (int i = _buckets[targetBucket]; i >= 0; i = _entries[i].next)
            {
                if (_entries[i].hashCode == hashCode && KeyEquals(_entries[i].key, key))
                    throw new ArgumentException("Duplicate property", nameof(key));
            }
            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                _freeList = _entries[index].next;
                _freeCount--;
            }
            else
            {
                if (_count == _entries.Length)
                {
                    Resize();
                    targetBucket = hashCode % _buckets.Length;
                }
                index = _count;
                _count++;
            }

            _entries[index].hashCode = hashCode;
            _entries[index].next = _buckets[targetBucket];
            _entries[index].key = key;
            _entries[index].value = value;
            _buckets[targetBucket] = index;
        }
        private void Resize()
        {
            int newSize = HashHelpers.ExpandPrime(_count);

            var newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; i++)
                newBuckets[i] = -1;

            var newEntries = new PropertyEntry[newSize];
            Array.Copy(_entries, 0, newEntries, 0, _count);

            for (int i = 0; i < _count; i++)
            {
                if (newEntries[i].hashCode >= 0)
                {
                    int bucket = newEntries[i].hashCode % newSize;
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
            }
            _buckets = newBuckets;
            _entries = newEntries;
        }

        private int FindEntry(ReadOnlyBuffer<byte> key)
        {
            if (_buckets != null)
            {
                int hashCode = GetKeyHashCode(key) & 0x7FFFFFFF;
                for (int i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].next)
                {
                    if (_entries[i].hashCode == hashCode && KeyEquals(_entries[i].key, key))
                        return i;
                }
            }
            return -1;
        }
        private int FindEntry(ReadOnlySpan<byte> key)
        {
            if (_buckets != null)
            {
                int hashCode = GetKeyHashCode(key) & 0x7FFFFFFF;
                for (int i = _buckets[hashCode % _buckets.Length]; i >= 0; i = _entries[i].next)
                {
                    if (_entries[i].hashCode == hashCode && KeyEquals(_entries[i].key, key))
                        return i;
                }
            }
            return -1;
        }

        public bool TryGetProperty(ReadOnlyBuffer<byte> key, out PropertyMap value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = _entries[i].value;
                return true;
            }
            value = default;
            return false;
        }
        public bool TryGetProperty(ReadOnlySpan<byte> key, out PropertyMap value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = _entries[i].value;
                return true;
            }
            value = default;
            return false;
        }

        private bool KeyEquals(ReadOnlyBuffer<byte> x, ReadOnlyBuffer<byte> y) => x.Span.SequenceEqual(y.Span);
        private bool KeyEquals(ReadOnlyBuffer<byte> x, ReadOnlySpan<byte> y) => x.Span.SequenceEqual(y);

        private int GetKeyHashCode(ReadOnlyBuffer<byte> obj) => GetKeyHashCode(obj.Span);
        private int GetKeyHashCode(ReadOnlySpan<byte> obj)
        {
            //From Utf8String
            //TODO: Replace when they do
            unchecked
            {
                if (obj.Length <= 4)
                {
                    int hash = obj.Length;
                    for (int i = 0; i < obj.Length; i++)
                    {
                        hash <<= 8;
                        hash ^= obj[i];
                    }
                    return hash;
                }
                else
                {
                    int hash = obj.Length;
                    hash ^= obj[0];
                    hash <<= 8;
                    hash ^= obj[1];
                    hash <<= 8;
                    hash ^= obj[obj.Length - 2];
                    hash <<= 8;
                    hash ^= obj[obj.Length - 1];
                    return hash;
                }
            }
        }
    }

    internal static class HashHelpers
    {
        public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, 8639249, 10367101,
            12440537, 14928671, 17914409, 21497293, 25796759, 30956117, 37147349, 44576837, 53492207, 64190669,
            77028803, 92434613, 110921543, 133105859, 159727031, 191672443, 230006941, 276008387, 331210079,
            397452101, 476942527, 572331049, 686797261, 824156741, 988988137, 1186785773, 1424142949, 1708971541,
            2050765853, MaxPrimeArrayLength };

        public static int GetPrime(int min)
        {
            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min) return prime;
            }
            return min;
        }
        
        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
                return MaxPrimeArrayLength;
            return GetPrime(newSize);
        }
        
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
    }
}