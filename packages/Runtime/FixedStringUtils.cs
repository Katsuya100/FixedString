using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Katuusagi.FixedString
{
    public static class FixedStringUtils
    {
        public const ulong DeBruijn64 = 0x03F566ED27179461UL;

        public static readonly bool IsLittleEndian;
        public static readonly IReadOnlyList<int> DeBruijn64_BitTable;
        public static readonly IReadOnlyList<int> DeBruijn64_FirstBitTable;
        public static readonly IReadOnlyList<int> DeBruijn64_ByteTable;

        static FixedStringUtils()
        {
            IsLittleEndian = BitConverter.IsLittleEndian;
            DeBruijn64_BitTable = new int[] { 0, 1, 59, 2, 60, 40, 54, 3, 61, 32, 49, 41, 55, 19, 35, 4, 62, 52, 30, 33, 50, 12, 14, 42, 56, 16, 27, 20, 36, 23, 44, 5, 63, 58, 39, 53, 31, 48, 18, 34, 51, 29, 11, 13, 15, 26, 22, 43, 57, 38, 47, 17, 28, 10, 25, 21, 37, 46, 9, 24, 45, 8, 7, 6 };
            DeBruijn64_FirstBitTable = new int[] { 0, 0, 56, 0, 56, 40, 48, 0, 56, 32, 48, 40, 48, 16, 32, 0, 56, 48, 24, 32, 48, 8, 8, 40, 56, 16, 24, 16, 32, 16, 40, 0, 56, 56, 32, 48, 24, 48, 16, 32, 48, 24, 8, 8, 8, 24, 16, 40, 56, 32, 40, 16, 24, 8, 24, 16, 32, 40, 8, 24, 40, 8, 0, 0 };
            DeBruijn64_ByteTable = new int[] { 0, 0, 7, 0, 7, 5, 6, 0, 7, 4, 6, 5, 6, 2, 4, 0, 7, 6, 3, 4, 6, 1, 1, 5, 7, 2, 3, 2, 4, 2, 5, 0, 7, 7, 4, 6, 3, 6, 2, 4, 6, 3, 1, 1, 1, 3, 2, 5, 7, 4, 5, 2, 3, 1, 3, 2, 4, 5, 1, 3, 5, 1, 0, 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this string str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= FixedString16Bytes.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in Span<char> str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= FixedString16Bytes.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in ReadOnlySpan<char> str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count <= FixedString16Bytes.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in Span<byte> buffer)
        {
            return buffer.Length <= FixedString16Bytes.Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsConvertableToFixedString16Bytes(this in ReadOnlySpan<byte> buffer)
        {
            return buffer.Length <= FixedString16Bytes.Size;
        }
    }
}
