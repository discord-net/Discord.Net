using System;

namespace Discord
{
    internal static class Preconditions
    {
        //Objects
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }
        public static void NotNull<T>(Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(name);
            else return new ArgumentNullException(name, msg);
        }

        //Strings
        public static void NotEmpty(string obj, string name, string msg = null) { if (obj.Length == 0) throw CreateNotEmptyException(name, msg); }
        public static void NotEmpty(Optional<string> obj, string name, string msg = null) { if (obj.IsSpecified && obj.Value.Length == 0) throw CreateNotEmptyException(name, msg); }
        public static void NotNullOrEmpty(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Length == 0) throw CreateNotEmptyException(name, msg);
        }
        public static void NotNullOrEmpty(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }
        public static void NotNullOrWhitespace(string obj, string name, string msg = null)
        {
            if (obj == null) throw CreateNotNullException(name, msg);
            if (obj.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
        }
        public static void NotNullOrWhitespace(Optional<string> obj, string name, string msg = null)
        {
            if (obj.IsSpecified)
            {
                if (obj.Value == null) throw CreateNotNullException(name, msg);
                if (obj.Value.Trim().Length == 0) throw CreateNotEmptyException(name, msg);
            }
        }        

        private static ArgumentException CreateNotEmptyException(string name, string msg)
        {
            if (msg == null) return new ArgumentException("Argument cannot be blank", name);
            else return new ArgumentException(name, msg);
        }

        //Numerics
        public static void NotEqual(sbyte obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(byte obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(short obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ushort obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(int obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(uint obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(long obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ulong obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(sbyte? obj, sbyte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(byte? obj, byte value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(short? obj, short value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ushort? obj, ushort value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(int? obj, int value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(uint? obj, uint value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(long? obj, long value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(ulong? obj, ulong value, string name, string msg = null) { if (obj == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<sbyte?> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<byte?> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<short?> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ushort?> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<int?> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<uint?> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<long?> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }
        public static void NotEqual(Optional<ulong?> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value == value) throw CreateNotEqualException(name, msg, value); }

        private static ArgumentException CreateNotEqualException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value may not be equal to {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void AtLeast(sbyte obj, sbyte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(byte obj, byte value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(short obj, short value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(ushort obj, ushort value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(int obj, int value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(uint obj, uint value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(long obj, long value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(ulong obj, ulong value, string name, string msg = null) { if (obj < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }
        public static void AtLeast(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value < value) throw CreateAtLeastException(name, msg, value); }

        private static ArgumentException CreateAtLeastException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be at least {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void GreaterThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(byte obj, byte value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(short obj, short value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(ushort obj, ushort value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(int obj, int value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(uint obj, uint value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(long obj, long value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(ulong obj, ulong value, string name, string msg = null) { if (obj <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }
        public static void GreaterThan(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value <= value) throw CreateGreaterThanException(name, msg, value); }

        private static ArgumentException CreateGreaterThanException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be greater than {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void AtMost(sbyte obj, sbyte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(byte obj, byte value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(short obj, short value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(ushort obj, ushort value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(int obj, int value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(uint obj, uint value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(long obj, long value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(ulong obj, ulong value, string name, string msg = null) { if (obj > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }
        public static void AtMost(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value > value) throw CreateAtMostException(name, msg, value); }

        private static ArgumentException CreateAtMostException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be at most {value}", name);
            else return new ArgumentException(msg, name);
        }

        public static void LessThan(sbyte obj, sbyte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(byte obj, byte value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(short obj, short value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(ushort obj, ushort value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(int obj, int value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(uint obj, uint value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(long obj, long value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(ulong obj, ulong value, string name, string msg = null) { if (obj >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<sbyte> obj, sbyte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<byte> obj, byte value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<short> obj, short value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<ushort> obj, ushort value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<int> obj, int value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<uint> obj, uint value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<long> obj, long value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }
        public static void LessThan(Optional<ulong> obj, ulong value, string name, string msg = null) { if (obj.IsSpecified && obj.Value >= value) throw CreateLessThanException(name, msg, value); }

        private static ArgumentException CreateLessThanException<T>(string name, string msg, T value)
        {
            if (msg == null) return new ArgumentException($"Value must be less than {value}", name);
            else return new ArgumentException(msg, name);
        }

        // Bulk Delete
        public static void YoungerThanTwoWeeks(ulong[] collection, string name)
        {
            var minimum = DateTimeUtils.ToSnowflake(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(14)));
            for (var i = 0; i < collection.Length; i++)
            {
                if (collection[i] <= minimum) 
                    throw new ArgumentOutOfRangeException(name, "Messages must be younger than two weeks old.");
            }
        }
    }
}
