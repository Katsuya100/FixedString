using Katuusagi.ConstExpressionForUnity;
using Katuusagi.ILPostProcessorCommon;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Katuusagi.FixedString
{
    public static class FixedStringUtils
    {
        public static ulong DeBruijn64Sequence
        {
            [ConstExpression]
            get
            {
                ulong value = 0;
                ulong flag = 1;
                for (int i = 1; i < 64; i++)
                {
                    value = (value << 1) | flag;
                    flag = 1 & ((value >> 5) + (value >> 0));
                }

                value = value >> 5;

                return value;
            }
        }

        public static ReadOnlyArray<int> DeBruijn64_BitTable
        {
            [ConstExpression]
            get
            {
                var result = new int[64];
                var sequence = DeBruijn64Sequence;
                for (int i = 0; i < result.Length; i++)
                {
                    result[sequence >> 58] = i;
                    sequence <<= 1;
                }

                return result;
            }
        }

        public static ReadOnlyArray<int> DeBruijn64_FirstBitTable
        {
            [ConstExpression]
            get
            {
                var bitTable = DeBruijn64_BitTable;
                var result = new int[bitTable.Count];
                for (int i = 0; i < bitTable.Count; i++)
                {
                    result[i] = bitTable[i] >> 2 << 2;
                }

                return result;
            }
        }

        public static ReadOnlyArray<int> DeBruijn64_ByteTable
        {
            [ConstExpression]
            get
            {
                var bitTable = DeBruijn64_BitTable;
                var result = new int[bitTable.Count];
                for (int i = 0; i < bitTable.Count; i++)
                {
                    result[i] = bitTable[i] >> 3;
                }

                return result;
            }
        }

        public static readonly bool IsLittleEndian;

        static FixedStringUtils()
        {
            IsLittleEndian = BitConverter.IsLittleEndian;
        }

        [ConstExpression(CalculationFailedWarning = false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyArray<byte> ToUTF8(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        [ConstExpression(CalculationFailedWarning = false)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this string str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in Span<char> str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in ReadOnlySpan<char> str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in Span<byte> buffer)
        {
            return buffer.Length <= 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in ReadOnlySpan<byte> buffer)
        {
            return buffer.Length <= 16;
        }
    }
}
