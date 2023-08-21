using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

namespace Katuusagi.FixedString.Tests
{
    public class FixedStringUtilsTest
    {
        [Test]
        public void IsConvertableToFixedString16Bytes()
        {
            Assert.IsTrue("123456789012345".IsConvertableToFixedString16Bytes());
            Assert.IsTrue("1234567890123456".IsConvertableToFixedString16Bytes());
            Assert.IsFalse("12345678901234567".IsConvertableToFixedString16Bytes());

            Assert.IsTrue((stackalloc char[15]).IsConvertableToFixedString16Bytes());
            Assert.IsTrue((stackalloc char[16]).IsConvertableToFixedString16Bytes());
            Assert.IsFalse((stackalloc char[17]).IsConvertableToFixedString16Bytes());

            Assert.IsTrue(((ReadOnlySpan<char>)stackalloc char[15]).IsConvertableToFixedString16Bytes());
            Assert.IsTrue(((ReadOnlySpan<char>)stackalloc char[16]).IsConvertableToFixedString16Bytes());
            Assert.IsFalse(((ReadOnlySpan<char>)stackalloc char[17]).IsConvertableToFixedString16Bytes());

            Assert.IsTrue((stackalloc byte[15]).IsConvertableToFixedString16Bytes());
            Assert.IsTrue((stackalloc byte[16]).IsConvertableToFixedString16Bytes());
            Assert.IsFalse((stackalloc byte[17]).IsConvertableToFixedString16Bytes());

            Assert.IsTrue(((ReadOnlySpan<byte>)stackalloc byte[15]).IsConvertableToFixedString16Bytes());
            Assert.IsTrue(((ReadOnlySpan<byte>)stackalloc byte[16]).IsConvertableToFixedString16Bytes());
            Assert.IsFalse(((ReadOnlySpan<byte>)stackalloc byte[17]).IsConvertableToFixedString16Bytes());
        }

        [Test]
        public void ToUTF8()
        {
            Assert.IsTrue("1234".ToUTF8().SequenceEqual(Encoding.UTF8.GetBytes("1234")));
        }
    }
}
