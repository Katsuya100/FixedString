//#define FIXED_STRING_ENDIAN_SAFE
using Katuusagi.ILPostProcessorCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Katuusagi.FixedString
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FixedString16Bytes : IComparable, IComparable<FixedString16Bytes>, IComparable<string>,
                                           IEquatable<FixedString16Bytes>, IEquatable<string>,
                                           IEnumerable<char>
    {
        public const int Size = 16;
        private static readonly int[] FirstBitTable = FixedStringUtils.DeBruijn64_FirstBitTable.ToArray();
        private static readonly int[] ByteTable = FixedStringUtils.DeBruijn64_ByteTable.ToArray();

        [FieldOffset(0)]
        public long _1;

        [FieldOffset(8)]
        public long _2;

        [SerializeField]
        [FieldOffset(0)]
        private FixedMemory16Bytes _memory;

        public int Length
        {
            get
            {
                var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref _memory), Size);
                int nullIdx = 0;
                if (_2 != 0)
                {
                    var x = _2;
                    x |= (x >> 1);
                    x |= (x >> 2);
                    x |= (x >> 4);
                    x |= (x >> 8);
                    x |= (x >> 16);
                    x |= (x >> 32);
                    var highest = (ulong)(x - (long)((ulong)x >> 1));
                    var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                    var offsetByte = ByteTable[tableIndex];
                    nullIdx = 8 + offsetByte + 1;
                }
                else if (_1 != 0)
                {
                    var x = _1;
                    x |= (x >> 1);
                    x |= (x >> 2);
                    x |= (x >> 4);
                    x |= (x >> 8);
                    x |= (x >> 16);
                    x |= (x >> 32);
                    var highest = (ulong)(x - (long)((ulong)x >> 1));
                    var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                    var offsetByte = ByteTable[tableIndex];
                    nullIdx = offsetByte + 1;
                }

                if (nullIdx < Size)
                {
                    buffer = buffer.Slice(0, nullIdx);
                }
                return Encoding.UTF8.GetCharCount(buffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in FixedString16Bytes other)
        {
            _1 = _2 = 0;
            _memory = other._memory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in string str)
        {
            _1 = _2 = 0;
            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            _memory = UnsafeUtility.As<byte, FixedMemory16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in ReadOnlySpan<char> str)
        {
            _1 = _2 = 0;
            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            _memory = UnsafeUtility.As<byte, FixedMemory16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in ReadOnlySpan<byte> buffer)
        {
            _1 = _2 = 0;
            if (buffer.Length < Size)
            {
                _memory = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref _memory), Size);
                buffer.CopyTo(span);
                return;
            }
            _memory = UnsafeUtility.As<byte, FixedMemory16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in ReadOnlyArray<byte> buffer)
        {
            _1 = _2 = 0;
            if (buffer.Count < Size)
            {
                _memory = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref _memory), Size);
                buffer.CopyTo(span);
                return;
            }
            _memory = UnsafeUtility.As<byte, FixedMemory16Bytes>(ref MemoryMarshal.GetReference<byte>(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(string str)
        {
            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(in Span<char> str)
        {
            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(in ReadOnlySpan<char> str)
        {
            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator FixedString16Bytes(in Span<byte> buffer)
        {
            if (buffer.Length < Size)
            {
                FixedString16Bytes result = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<FixedString16Bytes, byte>(ref result), Size);
                buffer.CopyTo(span);
                return result;
            }

            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator FixedString16Bytes(in ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < Size)
            {
                FixedString16Bytes result = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<FixedString16Bytes, byte>(ref result), Size);
                buffer.CopyTo(span);
                return result;
            }

            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator FixedString16Bytes(ReadOnlyArray<byte> buffer)
        {
            if (buffer.Count < Size)
            {
                FixedString16Bytes result = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<FixedString16Bytes, byte>(ref result), Size);
                buffer.CopyTo(span);
                return result;
            }

            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference<byte>(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(in FixedString16Bytes fstr)
        {
            var memory = fstr._memory;
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref memory), Size);
            int nullIdx = 0;
            if (fstr._2 != 0)
            {
                var x = fstr._2;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = 8 + offsetByte + 1;
            }
            else if (fstr._1 != 0)
            {
                var x = fstr._1;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = offsetByte + 1;
            }

            if (nullIdx < Size)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref _memory), Size);
            int nullIdx = 0;
            if (_2 != 0)
            {
                var x = _2;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = 8 + offsetByte + 1;
            }
            else if (_1 != 0)
            {
                var x = _1;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = offsetByte + 1;
            }

            if (nullIdx < Size)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<char> result)
        {
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<FixedMemory16Bytes, byte>(ref _memory), Size);
            int nullIdx = 0;
            if (_2 != 0)
            {
                var x = _2;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = 8 + offsetByte + 1;
            }
            else if (_1 != 0)
            {
                var x = _1;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offsetByte = ByteTable[tableIndex];
                nullIdx = offsetByte + 1;
            }

            if (nullIdx < Size)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            Encoding.UTF8.GetChars(buffer, result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<byte> result)
        {
            MemoryMarshal.Write(result, ref _memory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConvertableTo(in ReadOnlySpan<char> str)
        {
            var count = Encoding.UTF8.GetByteCount(str);
            return count >= Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsConvertableTo(in ReadOnlySpan<byte> buffer)
        {
            return buffer.Length >= Size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(FixedString16Bytes other)
        {
            if (_1 != other._1)
            {
                var diffBit = _1 ^ other._1;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offset = FirstBitTable[tableIndex];
                var lv = (byte)(_1 >> offset);
                var rv = (byte)(other._1 >> offset);
                return lv - rv;
            }

            if (_2 != other._2)
            {
                var diffBit = _2 ^ other._2;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offset = FirstBitTable[tableIndex];
                var lv = (byte)(_2 >> offset);
                var rv = (byte)(other._2 >> offset);
                return lv - rv;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(string str)
        {
            var byteCount = Encoding.UTF8.GetByteCount(str);
            if (byteCount > Size)
            {
                return -1;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            if (_1 != other._1)
            {
                var diffBit = _1 ^ other._1;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offset = FirstBitTable[tableIndex];
                var lv = (byte)(_1 >> offset);
                var rv = (byte)(other._1 >> offset);
                return lv - rv;
            }

            if (_2 != other._2)
            {
                var diffBit = _2 ^ other._2;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                var offset = FirstBitTable[tableIndex];
                var lv = (byte)(_2 >> offset);
                var rv = (byte)(other._2 >> offset);
                return lv - rv;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(object obj)
        {
            {
                if (obj is FixedString16Bytes other)
                {
                    if (_1 != other._1)
                    {
                        var diffBit = _1 ^ other._1;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                        var offset = FirstBitTable[tableIndex];
                        var lv = (byte)(_1 >> offset);
                        var rv = (byte)(other._1 >> offset);
                        return lv - rv;
                    }

                    if (_2 != other._2)
                    {
                        var diffBit = _2 ^ other._2;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                        var offset = FirstBitTable[tableIndex];
                        var lv = (byte)(_2 >> offset);
                        var rv = (byte)(other._2 >> offset);
                        return lv - rv;
                    }

                    return 0;
                }
            }

            {
                if (obj is string str)
                {
                    var byteCount = Encoding.UTF8.GetByteCount(str);
                    if (byteCount > Size)
                    {
                        return -1;
                    }

                    Span<byte> buffer = stackalloc byte[Size];
                    Encoding.UTF8.GetBytes(str, buffer);
                    ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
                    if (_1 != other._1)
                    {
                        var diffBit = _1 ^ other._1;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                        var offset = FirstBitTable[tableIndex];
                        var lv = (byte)(_1 >> offset);
                        var rv = (byte)(other._1 >> offset);
                        return lv - rv;
                    }

                    if (_2 != other._2)
                    {
                        var diffBit = _2 ^ other._2;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64Sequence) >> 58);
                        var offset = FirstBitTable[tableIndex];
                        var lv = (byte)(_2 >> offset);
                        var rv = (byte)(other._2 >> offset);
                        return lv - rv;
                    }

                    return 0;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FixedString16Bytes other)
        {
            return _1 == other._1 && _2 == other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(string str)
        {
            var byteCount = Encoding.UTF8.GetByteCount(str);
            if (byteCount > Size)
            {
                return false;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(str, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            return _1 == other._1 && _2 == other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            {
                if (obj is FixedString16Bytes other)
                {
                    return _1 == other._1 && _2 == other._2;
                }
            }
            {
                if (obj is string str)
                {
                    var byteCount = Encoding.UTF8.GetByteCount(str);
                    if (byteCount > Size)
                    {
                        return false;
                    }

                    Span<byte> buffer = stackalloc byte[Size];
                    Encoding.UTF8.GetBytes(str, buffer);
                    ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
                    return _1 == other._1 && _2 == other._2;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in FixedString16Bytes left, in FixedString16Bytes right)
        {
            return left._1 == right._1 && left._2 == right._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in FixedString16Bytes left, string right)
        {
            var byteCount = Encoding.UTF8.GetByteCount(right);
            if (byteCount > Size)
            {
                return false;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(right, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            return left._1 == other._1 && left._2 == other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(string left, in FixedString16Bytes right)
        {
            var byteCount = Encoding.UTF8.GetByteCount(left);
            if (byteCount > Size)
            {
                return false;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(left, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            return right._1 == other._1 && right._2 == other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in FixedString16Bytes left, in FixedString16Bytes right)
        {
            return left._1 != right._1 || left._2 != right._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in FixedString16Bytes left, string right)
        {
            var byteCount = Encoding.UTF8.GetByteCount(right);
            if (byteCount > Size)
            {
                return false;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(right, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            return left._1 != other._1 || left._2 != other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(string left, in FixedString16Bytes right)
        {
            var byteCount = Encoding.UTF8.GetByteCount(left);
            if (byteCount > Size)
            {
                return false;
            }

            Span<byte> buffer = stackalloc byte[Size];
            Encoding.UTF8.GetBytes(left, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
            return right._1 != other._1 || right._2 != other._2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                ulong k1 = (ulong)_1;
                ulong k2 = (ulong)_2;

#if FIXED_STRING_ENDIAN_SAFE
                if (!FixedStringUtils.IsLittleEndian)
                {
                    k1 = ((k1 & 0xff00000000000000L) >> 56) |
                         ((k1 & 0x00ff000000000000L) >> 40) |
                         ((k1 & 0x0000ff0000000000L) >> 24) |
                         ((k1 & 0x000000ff00000000L) >> 8) |
                         ((k1 & 0x00000000ff000000L) << 8) |
                         ((k1 & 0x0000000000ff0000L) << 24) |
                         ((k1 & 0x000000000000ff00L) << 40) |
                         ((k1 & 0x00000000000000ffL) << 56);
                    k2 = ((k2 & 0xff00000000000000L) >> 56) |
                         ((k2 & 0x00ff000000000000L) >> 40) |
                         ((k2 & 0x0000ff0000000000L) >> 24) |
                         ((k2 & 0x000000ff00000000L) >> 8) |
                         ((k2 & 0x00000000ff000000L) << 8) |
                         ((k2 & 0x0000000000ff0000L) << 24) |
                         ((k2 & 0x000000000000ff00L) << 40) |
                         ((k2 & 0x00000000000000ffL) << 56);
                }
#endif
                var h1 = ((k1 * 0x9E3779B97F4A7C15) >> 31) | ((k1 * 0x9E3779B97F4A7C15) << (64 - 31)) * 0xBF58476D1CE4E5B9;
                h1 = ((h1 >> 27) | (h1 << (64 - 27))) * 0x94D049BB133111EB;
                var h2 = ((k2 * 0x9E3779B97F4A7C15) >> 33) | ((k2 * 0x9E3779B97F4A7C15) << (64 - 33)) * 0xBF58476D1CE4E5B9;
                h2 = (h2 >> 29) | (h2 << (64 - 29)) * 0x94D049BB133111EB;
                return (int)(((h1 ^ h2) >> 32) ^ (h1 ^ h2));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<char> GetEnumerator()
        {
            return ToString().GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
