using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal static class ObjectExtensions
    {
        private const long Int53Max = (1L << 53) - 1;
        private const long Int53Min = -Int53Max;

        public static (double Min, double Max) GetSupportedNumericalRange(this object o)
            => Type.GetTypeCode(o.GetType()) switch
            {
                TypeCode.Byte => (byte.MinValue, byte.MaxValue),
                TypeCode.SByte => (sbyte.MinValue, sbyte.MaxValue),
                TypeCode.Int16 => (short.MinValue, short.MaxValue),
                TypeCode.UInt16 => (ushort.MinValue, ushort.MaxValue),
                TypeCode.Int32 => (int.MinValue, int.MaxValue),
                TypeCode.UInt32 => (uint.MinValue, uint.MaxValue),
                TypeCode.UInt64 => (ulong.MinValue, Int53Max),
                _ => (Int53Min, Int53Max)
            };

        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
