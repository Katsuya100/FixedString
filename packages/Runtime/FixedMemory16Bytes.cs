using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Katuusagi.FixedString
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct FixedMemory16Bytes
    {
        [SerializeField]
        [FieldOffset(0)]
        public byte B1;

        [SerializeField]
        [FieldOffset(1)]
        public byte B2;

        [SerializeField]
        [FieldOffset(2)]
        public byte B3;

        [SerializeField]
        [FieldOffset(3)]
        public byte B4;

        [SerializeField]
        [FieldOffset(4)]
        public byte B5;

        [SerializeField]
        [FieldOffset(5)]
        public byte B6;

        [SerializeField]
        [FieldOffset(6)]
        public byte B7;

        [SerializeField]
        [FieldOffset(7)]
        public byte B8;

        [SerializeField]
        [FieldOffset(8)]
        public byte B9;

        [SerializeField]
        [FieldOffset(9)]
        public byte B10;

        [SerializeField]
        [FieldOffset(10)]
        public byte B11;

        [SerializeField]
        [FieldOffset(11)]
        public byte B12;

        [SerializeField]
        [FieldOffset(12)]
        public byte B13;

        [SerializeField]
        [FieldOffset(13)]
        public byte B14;

        [SerializeField]
        [FieldOffset(14)]
        public byte B15;

        [SerializeField]
        [FieldOffset(15)]
        public byte B16;
    }
}
