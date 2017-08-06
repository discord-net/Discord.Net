// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Binary;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Utf8;

namespace System.Text.Json
{
    public struct JsonObject
    {
        private BufferPool _pool;
        private OwnedBuffer<byte> _dbMemory;
        private ReadOnlySpan<byte> _db; 
        private ReadOnlySpan<byte> _values;

        public static JsonObject Parse(ReadOnlySpan<byte> utf8Json)
        {
            var parser = new JsonParser();
            var result = parser.Parse(utf8Json);
            return result;
        }

        public static JsonObject Parse(ReadOnlySpan<byte> utf8Json, BufferPool pool = null)
        {
            var parser = new JsonParser();
            var result = parser.Parse(utf8Json, pool);
            return result;
        }

        internal JsonObject(ReadOnlySpan<byte> values, ReadOnlySpan<byte> db, BufferPool pool = null, OwnedBuffer<byte> dbMemory = null)
        {
            _db = db;
            _values = values;
            _pool = pool;
            _dbMemory = dbMemory;
        }

        public bool TryGetValue(Utf8String propertyName, out JsonObject value)
        {
            var record = Record;

            if (record.Length == 0) {
                throw new KeyNotFoundException();
            }
            if (record.Type != JsonValueType.Object) {
                throw new InvalidOperationException();
            }

            for (int i = DbRow.Size; i <= _db.Length; i += DbRow.Size) {
                record = _db.Slice(i).Read<DbRow>();

                if (!record.IsSimpleValue) {
                    i += record.Length * DbRow.Size;
                    continue;
                }

                if (new Utf8String(_values.Slice(record.Location, record.Length)) == propertyName) {
                    int newStart = i + DbRow.Size;
                    int newEnd = newStart + DbRow.Size;

                    record = _db.Slice(newStart).Read<DbRow>();

                    if (!record.IsSimpleValue) {
                        newEnd = newEnd + DbRow.Size * record.Length;
                    }

                    value = new JsonObject(_values, _db.Slice(newStart, newEnd - newStart));
                    return true;
                }

                var valueType = _db.Slice(i + DbRow.Size + 8).Read<JsonValueType>();
                if (valueType != JsonValueType.Object && valueType != JsonValueType.Array) {
                    i += DbRow.Size;
                }
            }

            value = default;
            return false;
        }

        public bool TryGetValue(string propertyName, out JsonObject value)
        {
            var record = Record;
            
            if (record.Length == 0) {
                throw new KeyNotFoundException();
            }

            if (record.Type != JsonValueType.Object) {
                throw new InvalidOperationException();
            }

            for (int i = DbRow.Size; i <= _db.Length; i += DbRow.Size) {
                record = _db.Slice(i).Read<DbRow>();

                if (!record.IsSimpleValue) {
                    i += record.Length * DbRow.Size;
                    continue;
                }

                if (new Utf8String(_values.Slice(record.Location, record.Length)) == propertyName) {
                    int newStart = i + DbRow.Size;
                    int newEnd = newStart + DbRow.Size;

                    record = _db.Slice(newStart).Read<DbRow>();

                    if (!record.IsSimpleValue) {
                        newEnd = newEnd + DbRow.Size * record.Length;
                    }

                    value = new JsonObject(_values, _db.Slice(newStart, newEnd - newStart));
                    return true;
                }

                var valueType = _db.Slice(i + DbRow.Size + 8).Read<JsonValueType>();
                if (valueType != JsonValueType.Object && valueType != JsonValueType.Array) {
                    i += DbRow.Size;
                }
            }

            value = default;
            return false;
        }

        public JsonObject this[Utf8String name]
        {
            get {
                JsonObject value;
                if(TryGetValue(name, out value)) {
                    return value;
                }
                throw new KeyNotFoundException();
            }
        }

        public JsonObject this[string name] {
            get {
                JsonObject value;
                if (TryGetValue(name, out value)) {
                    return value;
                }
                throw new KeyNotFoundException();
            }
        }

        public JsonObject this[int index] {
            get {
                var record = Record;

                if (index < 0 || index >= record.Length) {
                    throw new IndexOutOfRangeException();
                }

                if (record.Type != JsonValueType.Array) {
                    throw new InvalidOperationException();
                }

                int counter = 0;
                for (int i = DbRow.Size; i <= _db.Length; i += DbRow.Size) {
                    record = _db.Slice(i).Read<DbRow>();

                    if (index == counter) {
                        int newStart = i;
                        int newEnd = i + DbRow.Size;

                        if (!record.IsSimpleValue) {
                            newEnd = newEnd + DbRow.Size * record.Length;
                        }
                        return new JsonObject(_values, _db.Slice(newStart, newEnd - newStart));
                    }

                    if (!record.IsSimpleValue) {
                        i += record.Length * DbRow.Size;
                    }

                    counter++;
                }

                throw new IndexOutOfRangeException();
            }
        }

        public int ArrayLength
        {
            get
            {
                var record = Record;
                if (record.Type != JsonValueType.Array)
                {
                    throw new InvalidOperationException();
                }
                return record.Length; 
            }
        }

        public static explicit operator string(JsonObject json)
        {
            var utf8 = (Utf8String)json;
            return utf8.ToString();
        }

        public static explicit operator Utf8String(JsonObject json)
        {
            var record = json.Record;
            if (!record.IsSimpleValue) {
                throw new InvalidCastException();
            }

            return new Utf8String(json._values.Slice(record.Location, record.Length));
        }

        public static explicit operator bool(JsonObject json)
        {
            var record = json.Record;
            if (!record.IsSimpleValue) {
                throw new InvalidCastException();
            }

            if (record.Length < 4 || record.Length > 5) {
                throw new InvalidCastException();
            }

            var slice = json._values.Slice(record.Location);

            bool result;
            if(!PrimitiveParser.InvariantUtf8.TryParseBoolean(slice, out result)){
                throw new InvalidCastException();
            }
            return result;
        }

        public static explicit operator int(JsonObject json)
        {
            var record = json.Record;
            if (!record.IsSimpleValue) {
                throw new InvalidCastException();
            }

            var slice = json._values.Slice(record.Location);

            int result;
            if (!PrimitiveParser.InvariantUtf8.TryParseInt32(slice, out result)) {
                throw new InvalidCastException();
            }
            return result;
        }

        public static explicit operator double(JsonObject json)
        {
            var record = json.Record;
            if (!record.IsSimpleValue) {
                throw new InvalidCastException();
            }

            int count = record.Location;
            bool isNegative = false;
            var nextByte = json._values[count];
            if (nextByte == '-') {
                isNegative = true;
                count++;
                nextByte = json._values[count];
            }

            if (nextByte < '0' || nextByte > '9' || count - record.Location >= record.Length) {
                throw new InvalidCastException();
            }

            int integerPart = 0;
            while (nextByte >= '0' && nextByte <= '9' && count - record.Location < record.Length) {
                int digit = nextByte - '0';
                integerPart = integerPart * 10 + digit;
                count++;
                nextByte = json._values[count];
            }

            double result = integerPart;

            int decimalPart = 0;
            if (nextByte == '.') {
                count++;
                int numberOfDigits = count;
                nextByte = json._values[count];
                while (nextByte >= '0' && nextByte <= '9' && count - record.Location < record.Length) {
                    int digit = nextByte - '0';
                    decimalPart = decimalPart * 10 + digit;
                    count++;
                    nextByte = json._values[count];
                }
                numberOfDigits = count - numberOfDigits;
                double divisor = Math.Pow(10, numberOfDigits);
                result += decimalPart / divisor;
            }

            int exponentPart = 0;
            bool isExpNegative = false;
            if (nextByte == 'e' || nextByte == 'E') {
                count++;
                nextByte = json._values[count];
                if (nextByte == '-' || nextByte == '+') {
                    if (nextByte == '-') {
                        isExpNegative = true;
                    }
                    count++;
                }
                nextByte = json._values[count];
                while (nextByte >= '0' && nextByte <= '9' && count - record.Location < record.Location) {
                    int digit = nextByte - '0';
                    exponentPart = exponentPart * 10 + digit;
                    count++;
                    nextByte = json._values[count];
                }

                result *= (Math.Pow(10, isExpNegative ? exponentPart * -1 : exponentPart));
            }

            if (count - record.Location > record.Length) {
                throw new InvalidCastException();
            }

            return isNegative ? result * -1 : result;

        }

        internal DbRow Record => _db.Read<DbRow>();
        public JsonValueType Type => _db.Slice(8).Read<JsonValueType>();

        public enum JsonValueType : byte
        {
            String = 0,
            Number = 1,
            Object = 2,
            Array  = 3,
            True   = 4,
            False  = 5,
            Null   = 6
        }

        public bool HasValue()
        {
            var record = Record;

            if (!record.IsSimpleValue) {
                if (record.Length == 0) return false;
                return true;
            } else {
                if (_values[record.Location - 1] == '"' && _values[record.Location + 4] == '"') {
                    return true;
                }
                return (_values[record.Location] != 'n' || _values[record.Location + 1] != 'u' || _values[record.Location + 2] != 'l' || _values[record.Location + 3] != 'l');
            }
        }

        public void Dispose()
        {
            if (_pool == null) throw new InvalidOperationException("only root object can (and should) be disposed.");
            _db = ReadOnlySpan<byte>.Empty;
            _values = ReadOnlySpan<byte>.Empty;
            _dbMemory.Dispose();
            _dbMemory = null;
        }
    }
}
