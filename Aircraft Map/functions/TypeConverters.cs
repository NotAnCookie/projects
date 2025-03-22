using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace airportapp.functions
{
    internal static class TypeConverters
    {
        public static UInt64 Ch_UInt64(object obj)
        {
            if (UInt64.TryParse((string)obj, out UInt64 parsedUInt64))
            {
                return parsedUInt64;
            }
            else
            {
                throw new ArgumentException("Failed to parse UInt64 value.");
            }
        }

        public static UInt16 Ch_UInt16(object obj)
        {
            if (UInt16.TryParse((string)obj, out UInt16 parsedUInt16))
            {
                return parsedUInt16;
            }
            else
            {
                throw new ArgumentException("Failed to parse UInt16 value.");
            }
        }

        public static List<UInt64> Ch_UInt64_list(object obj) // UInt64 changer for arrays
        {
            string[] in_string = ((string)obj).Substring(1, ((string)obj).Length - 2).Split(';');
            List<UInt64> in_uint64 = new List<UInt64>();
            for (int i = 0; i < in_string.Length; i++)
            {
                if (UInt64.TryParse(in_string[i], out UInt64 parsedUInt64))
                {
                    in_uint64.Add(parsedUInt64);
                }
                else
                {
                    throw new ArgumentException("Failed to parse UInt64 array value.");
                }
            }
            return in_uint64;
        }

        public static Single Ch_Single(object obj)
        {
            if (Single.TryParse((string)obj, NumberStyles.Float, CultureInfo.InvariantCulture, out Single parsedSingle))
            {
                return parsedSingle;
            }
            else
            {
                throw new ArgumentException("Failed to parse Single value.");
            }
        }
    }
}
