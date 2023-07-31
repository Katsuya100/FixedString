//#define USE_FIXED_STRING_BURST
//#define FIXED_STRING_ENDIAN_SAFE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Katuusagi.FixedString
{
#if USE_FIXED_STRING_BURST
    [Unity.Burst.BurstCompile]
#endif
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FixedString16Bytes : IComparable, IComparable<FixedString16Bytes>, IComparable<string>,
                                           IEquatable<FixedString16Bytes>, IEquatable<string>,
                                           IEnumerable<char>
    {
        public const int Size = 16;

        [FieldOffset(0)]
        private int4 _value;

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
                var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<int4, byte>(ref _value), Size);
                int nullIdx = GetNullIndex();
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
            _memory = default;
            _value = other._value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in string str)
        {
            _1 = _2 = 0;
            _memory = default;
            Span<byte> buffer = stackalloc byte[Size];
#if USE_FIXED_STRING_BURST
            _value = StringTo<int4>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            _value = UnsafeUtility.As<byte, int4>(ref MemoryMarshal.GetReference(buffer));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in ReadOnlySpan<char> str)
        {
            _1 = _2 = 0;
            _memory = default;
            Span<byte> buffer = stackalloc byte[Size];
#if USE_FIXED_STRING_BURST
            _value = StringTo<int4>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            _value = UnsafeUtility.As<byte, int4>(ref MemoryMarshal.GetReference(buffer));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedString16Bytes(in ReadOnlySpan<byte> buffer)
        {
            _1 = _2 = 0;
            _memory = default;
            if (buffer.Length < Size)
            {
                _value = default;
                var span = MemoryMarshal.CreateSpan(ref UnsafeUtility.As<int4, byte>(ref _value), Size);
                buffer.CopyTo(span);
                return;
            }

#if USE_FIXED_STRING_BURST
            _value = BinaryAs<int4>(buffer);
#else
            _value = UnsafeUtility.As<byte, int4>(ref MemoryMarshal.GetReference(buffer));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(string str)
        {
            Span<byte> buffer = stackalloc byte[Size];
#if USE_FIXED_STRING_BURST
            return StringTo<FixedString16Bytes>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(in Span<char> str)
        {
            Span<byte> buffer = stackalloc byte[Size];
#if USE_FIXED_STRING_BURST
            return StringTo<FixedString16Bytes>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FixedString16Bytes(in ReadOnlySpan<char> str)
        {
            Span<byte> buffer = stackalloc byte[Size];
#if USE_FIXED_STRING_BURST
            return StringTo<FixedString16Bytes>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
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

#if USE_FIXED_STRING_BURST
            return BinaryAs<FixedString16Bytes>(buffer);
#else
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
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

#if USE_FIXED_STRING_BURST
            return BinaryAs<FixedString16Bytes>(buffer);
#else
            return UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator string(in FixedString16Bytes fstr)
        {
            var value = fstr._value;
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<int4, byte>(ref value), Size);
            int nullIdx = fstr.GetNullIndex();
            if (nullIdx != -1)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<int4, byte>(ref _value), Size);
            int nullIdx = GetNullIndex();
            if (nullIdx != -1)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<char> result)
        {
            var buffer = MemoryMarshal.CreateReadOnlySpan(ref UnsafeUtility.As<int4, byte>(ref _value), Size);
            int nullIdx = GetNullIndex();
            if (nullIdx != -1)
            {
                buffer = buffer.Slice(0, nullIdx);
            }
            Encoding.UTF8.GetChars(buffer, result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Span<byte> result)
        {
            MemoryMarshal.Write(result, ref _value);
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
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
                var lv = (byte)(_1 >> offset);
                var rv = (byte)(other._1 >> offset);
                return lv - rv;
            }

            if (_2 != other._2)
            {
                var diffBit = _2 ^ other._2;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
            if (_1 != other._1)
            {
                var diffBit = _1 ^ other._1;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
                var lv = (byte)(_1 >> offset);
                var rv = (byte)(other._1 >> offset);
                return lv - rv;
            }

            if (_2 != other._2)
            {
                var diffBit = _2 ^ other._2;
                var lowest = (ulong)(diffBit & -diffBit);
                var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
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
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                        var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
                        var lv = (byte)(_1 >> offset);
                        var rv = (byte)(other._1 >> offset);
                        return lv - rv;
                    }

                    if (_2 != other._2)
                    {
                        var diffBit = _2 ^ other._2;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                        var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
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
#if USE_FIXED_STRING_BURST
                    ref var other = ref StringTo<FixedString16Bytes>(str, buffer);
#else
                    Encoding.UTF8.GetBytes(str, buffer);
                    ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
                    if (_1 != other._1)
                    {
                        var diffBit = _1 ^ other._1;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                        var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
                        var lv = (byte)(_1 >> offset);
                        var rv = (byte)(other._1 >> offset);
                        return lv - rv;
                    }

                    if (_2 != other._2)
                    {
                        var diffBit = _2 ^ other._2;
                        var lowest = (ulong)(diffBit & -diffBit);
                        var tableIndex = (int)((lowest * FixedStringUtils.DeBruijn64) >> 58);
                        var offset = FixedStringUtils.DeBruijn64_FirstBitTable[tableIndex];
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
#if USE_FIXED_STRING_BURST
            return EqualCheck(this, other);
#else
            return _1 == other._1 && _2 == other._2;
#endif
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(str, buffer);
#else
            Encoding.UTF8.GetBytes(str, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
            return EqualCheck(this, other);
#else
            return _1 == other._1 && _2 == other._2;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            {
                if (obj is FixedString16Bytes other)
                {
#if USE_FIXED_STRING_BURST
                    return EqualCheck(this, other);
#else
                    return _1 == other._1 && _2 == other._2;
#endif
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
#if USE_FIXED_STRING_BURST
                    ref var other = ref StringTo<FixedString16Bytes>(str, buffer);
#else
                    Encoding.UTF8.GetBytes(str, buffer);
                    ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
                    return EqualCheck(this, other);
#else
                    return _1 == other._1 && _2 == other._2;
#endif
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in FixedString16Bytes left, in FixedString16Bytes right)
        {
#if USE_FIXED_STRING_BURST
            return EqualCheck(left, right);
#else
            return left._1 == right._1 && left._2 == right._2;
#endif
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(right, buffer);
#else
            Encoding.UTF8.GetBytes(right, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
            return EqualCheck(left, other);
#else
            return left._1 == other._1 && left._2 == other._2;
#endif
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(left, buffer);
#else
            Encoding.UTF8.GetBytes(left, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
            return EqualCheck(right, other);
#else
            return right._1 == other._1 && right._2 == other._2;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in FixedString16Bytes left, in FixedString16Bytes right)
        {
#if USE_FIXED_STRING_BURST
            return !EqualCheck(left, right);
#else
            return left._1 != right._1 || left._2 != right._2;
#endif
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(right, buffer);
#else
            Encoding.UTF8.GetBytes(right, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
            return !EqualCheck(left, other);
#else
            return left._1 != other._1 || left._2 != other._2;
#endif
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
#if USE_FIXED_STRING_BURST
            ref var other = ref StringTo<FixedString16Bytes>(left, buffer);
#else
            Encoding.UTF8.GetBytes(left, buffer);
            ref var other = ref UnsafeUtility.As<byte, FixedString16Bytes>(ref MemoryMarshal.GetReference(buffer));
#endif
#if USE_FIXED_STRING_BURST
            return !EqualCheck(right, other);
#else
            return right._1 != other._1 || right._2 != other._2;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
#if FIXED_STRING_ENDIAN_SAFE
            if (!FixedStringUtils.IsLittleEndian)
            {
                var x = (int)((_value.x & 0xff000000) >> 24) | ((_value.x & 0x00ff0000) >> 8) | ((_value.x & 0x0000ff00) << 8) | ((_value.x & 0x000000ff) << 24);
                var y = (int)((_value.y & 0xff000000) >> 24) | ((_value.y & 0x00ff0000) >> 8) | ((_value.y & 0x0000ff00) << 8) | ((_value.y & 0x000000ff) << 24);
                var z = (int)((_value.z & 0xff000000) >> 24) | ((_value.z & 0x00ff0000) >> 8) | ((_value.z & 0x0000ff00) << 8) | ((_value.z & 0x000000ff) << 24);
                var w = (int)((_value.w & 0xff000000) >> 24) | ((_value.w & 0x00ff0000) >> 8) | ((_value.w & 0x0000ff00) << 8) | ((_value.w & 0x000000ff) << 24);
                unchecked
                {
                    return ((((17 * 23 + x) * 23 + y) * 23 + z) * 23 + w);
                }
            }
#endif
            unchecked
            {
                return ((((17 * 23 + _value.x) * 23 + _value.y) * 23 + _value.z) * 23 + _value.w);
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

#if USE_FIXED_STRING_BURST
        [Unity.Burst.BurstCompile]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool EqualCheck(in FixedString16Bytes left, in FixedString16Bytes right)
        {
            return left._1 == right._1 && left._2 == right._2;
        }

#if USE_FIXED_STRING_BURST
        [Unity.Burst.BurstCompile]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref T BinaryAs<T>(in ReadOnlySpan<byte> buffer)
        {
            return ref UnsafeUtility.As<byte, T>(ref MemoryMarshal.GetReference(buffer));
        }

#if USE_FIXED_STRING_BURST
        [Unity.Burst.BurstCompile]
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref T StringTo<T>(in ReadOnlySpan<char> str, in Span<byte> buffer)
        {
            Encoding.UTF8.GetBytes(str, buffer);
            return ref UnsafeUtility.As<byte, T>(ref MemoryMarshal.GetReference(buffer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNullIndex()
        {
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
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64) >> 58);
                var offsetByte = FixedStringUtils.DeBruijn64_ByteTable[tableIndex];
                return 8 + offsetByte + 1;
            }
            
            if (_1 != 0)
            {
                var x = _1;
                x |= (x >> 1);
                x |= (x >> 2);
                x |= (x >> 4);
                x |= (x >> 8);
                x |= (x >> 16);
                x |= (x >> 32);
                var highest = (ulong)(x - (long)((ulong)x >> 1));
                var tableIndex = (int)((highest * FixedStringUtils.DeBruijn64) >> 58);
                var offsetByte = FixedStringUtils.DeBruijn64_ByteTable[tableIndex];
                return offsetByte + 1;
            }

            return 0;
        }
    }
}
