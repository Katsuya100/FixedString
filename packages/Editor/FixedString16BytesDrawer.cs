using System;
using UnityEditor;
using UnityEngine;

namespace Katuusagi.FixedString.Editor
{
    [CustomPropertyDrawer(typeof(FixedString16Bytes))]
    public class FixedString16BytesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Span<byte> buffer = stackalloc byte[16];
            var b1 = property.FindPropertyRelative("_memory.B1");
            var b2 = property.FindPropertyRelative("_memory.B2");
            var b3 = property.FindPropertyRelative("_memory.B3");
            var b4 = property.FindPropertyRelative("_memory.B4");
            var b5 = property.FindPropertyRelative("_memory.B5");
            var b6 = property.FindPropertyRelative("_memory.B6");
            var b7 = property.FindPropertyRelative("_memory.B7");
            var b8 = property.FindPropertyRelative("_memory.B8");
            var b9 = property.FindPropertyRelative("_memory.B9");
            var b10 = property.FindPropertyRelative("_memory.B10");
            var b11 = property.FindPropertyRelative("_memory.B11");
            var b12 = property.FindPropertyRelative("_memory.B12");
            var b13 = property.FindPropertyRelative("_memory.B13");
            var b14 = property.FindPropertyRelative("_memory.B14");
            var b15 = property.FindPropertyRelative("_memory.B15");
            var b16 = property.FindPropertyRelative("_memory.B16");

            buffer[0] = (byte)b1.intValue;
            buffer[1] = (byte)b2.intValue;
            buffer[2] = (byte)b3.intValue;
            buffer[3] = (byte)b4.intValue;
            buffer[4] = (byte)b5.intValue;
            buffer[5] = (byte)b6.intValue;
            buffer[6] = (byte)b7.intValue;
            buffer[7] = (byte)b8.intValue;
            buffer[8] = (byte)b9.intValue;
            buffer[9] = (byte)b10.intValue;
            buffer[10] = (byte)b11.intValue;
            buffer[11] = (byte)b12.intValue;
            buffer[12] = (byte)b13.intValue;
            buffer[13] = (byte)b14.intValue;
            buffer[14] = (byte)b15.intValue;
            buffer[15] = (byte)b16.intValue;

            var fs = (FixedString16Bytes)buffer;
            var s = EditorGUI.TextField(position, label, fs);
            if (!s.IsConvertableToFixedString16Bytes())
            {
                return;
            }
            fs = s;
            fs.CopyTo(buffer);

            b1.intValue = buffer[0];
            b2.intValue = buffer[1];
            b3.intValue = buffer[2];
            b4.intValue = buffer[3];
            b5.intValue = buffer[4];
            b6.intValue = buffer[5];
            b7.intValue = buffer[6];
            b8.intValue = buffer[7];
            b9.intValue = buffer[8];
            b10.intValue = buffer[9];
            b11.intValue = buffer[10];
            b12.intValue = buffer[11];
            b13.intValue = buffer[12];
            b14.intValue = buffer[13];
            b15.intValue = buffer[14];
            b16.intValue = buffer[15];
        }
    }
}
