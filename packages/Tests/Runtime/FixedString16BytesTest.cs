using NUnit.Framework;
using System;
using System.Text;

namespace Katuusagi.FixedString.Tests
{
    public class FixedString16BytesTest
    {
        private const string CStr16      = "1234567890123456";
        private const string CStr15      = "123456789012345";
        private const string CStr10      = "1234567890";
        private const string CStrGreater = "1234567790123456";
        private const string CStrLess    = "1234567990123456";
        private const string CStrAIUEO   = "‚ ‚¢‚¤‚¦‚¨";
        private string Str16      = CStr16;
        private string Str15      = CStr15;
        private string Str10      = CStr10;
        private string StrGreater = CStrGreater;
        private string StrLess    = CStrLess;
        private string StrAIUEO   = CStrAIUEO;

        [Test]
        public void OptimizedCast()
        {
            Assert.AreEqual(((FixedString16Bytes)CStr16).ToString(), CStr16);
            Assert.AreEqual(((FixedString16Bytes)CStr10).ToString(), CStr10);
        }

        [Test]
        public void OptimizedEqualOperator()
        {
            Assert.IsTrue(((FixedString16Bytes)Str16) == CStr16);
            Assert.IsFalse(((FixedString16Bytes)Str16) == CStr15);
            Assert.IsTrue(CStr16 == ((FixedString16Bytes)Str16));
            Assert.IsFalse(CStr15 == ((FixedString16Bytes)Str16));
        }

        [Test]
        public void OptimizedInequalOperator()
        {
            Assert.IsTrue(((FixedString16Bytes)Str16) != CStr15);
            Assert.IsFalse(((FixedString16Bytes)Str16) != CStr16);
            Assert.IsTrue(CStr15 != ((FixedString16Bytes)Str16));
            Assert.IsFalse(CStr16 != ((FixedString16Bytes)Str16));
        }

        [Test]
        public void Cast()
        {
            Assert.AreEqual(((FixedString16Bytes)Str16).ToString(), Str16);
            Assert.AreEqual(((FixedString16Bytes)Str10).ToString(), Str10);

            Assert.AreEqual(((FixedString16Bytes)Str16.AsSpan()).ToString(), Str16);
            Assert.AreEqual(((FixedString16Bytes)Str10.AsSpan()).ToString(), Str10);

            Span<char> chars16 = stackalloc char[Str16.Length];
            Str16.AsSpan().CopyTo(chars16);
            Assert.AreEqual(((FixedString16Bytes)chars16).ToString(), Str16);

            Span<char> chars15 = stackalloc char[Str15.Length];
            Str15.AsSpan().CopyTo(chars15);
            Assert.AreEqual(((FixedString16Bytes)chars15).ToString(), Str15);

            Span<byte> bytes16 = stackalloc byte[16];
            Encoding.UTF8.GetBytes(Str16, bytes16);
            Assert.AreEqual(((FixedString16Bytes)bytes16).ToString(), Str16);
            Assert.AreEqual(((FixedString16Bytes)(ReadOnlySpan<byte>)bytes16).ToString(), Str16);

            Span<byte> bytes15 = stackalloc byte[15];
            Encoding.UTF8.GetBytes(Str15, bytes15);
            Assert.AreEqual(((FixedString16Bytes)bytes15).ToString(), Str15);
            Assert.AreEqual(((FixedString16Bytes)(ReadOnlySpan<byte>)bytes15).ToString(), Str15);

            Assert.AreEqual(((FixedString16Bytes)Str16.ToUTF8()).ToString(), Str16);
            Assert.AreEqual(((FixedString16Bytes)Str10.ToUTF8()).ToString(), Str10);
        }

        [Test]
        public void ToString_()
        {
            Assert.AreEqual(((FixedString16Bytes)Str10).ToString(), Str10);
            Assert.AreEqual(((FixedString16Bytes)Str16).ToString(), Str16);
        }

        [Test]
        public void EqualOperator()
        {
            Assert.IsTrue(((FixedString16Bytes)Str16) == ((FixedString16Bytes)Str16));
            Assert.IsTrue(((FixedString16Bytes)Str16) == Str16);
            Assert.IsTrue(Str16 == ((FixedString16Bytes)Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16) == ((FixedString16Bytes)Str15));
            Assert.IsFalse(((FixedString16Bytes)Str16) == Str15);
            Assert.IsFalse(Str15 == ((FixedString16Bytes)Str16));
        }

        [Test]
        public void InequalOperator()
        {
            Assert.IsFalse(((FixedString16Bytes)Str16) != ((FixedString16Bytes)Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16) != Str16);
            Assert.IsFalse(Str16 != ((FixedString16Bytes)Str16));

            Assert.IsTrue(((FixedString16Bytes)Str16) != ((FixedString16Bytes)Str15));
            Assert.IsTrue(((FixedString16Bytes)Str16) != Str15);
            Assert.IsTrue(Str15 != ((FixedString16Bytes)Str16));
        }

        [Test]
        public void Equals()
        {
            Assert.IsTrue(((FixedString16Bytes)Str16).Equals((FixedString16Bytes)Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16).Equals((FixedString16Bytes)Str15));

            Assert.IsTrue(((FixedString16Bytes)Str16).Equals(Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16).Equals(Str15));

            Assert.IsTrue(((FixedString16Bytes)Str16).Equals((object)(FixedString16Bytes)Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16).Equals((object)(FixedString16Bytes)Str15));

            Assert.IsTrue(((FixedString16Bytes)Str16).Equals((object)Str16));
            Assert.IsFalse(((FixedString16Bytes)Str16).Equals((object)Str15));

            Assert.IsFalse(((FixedString16Bytes)Str16).Equals(10));
        }

        [Test]
        public void CompareTo()
        {
            Assert.AreEqual(((FixedString16Bytes)Str16).CompareTo((FixedString16Bytes)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((FixedString16Bytes)Str10), 0);
            Assert.Less(((FixedString16Bytes)Str10).CompareTo((FixedString16Bytes)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((FixedString16Bytes)StrGreater), 0);
            Assert.Less(((FixedString16Bytes)Str16).CompareTo((FixedString16Bytes)StrLess), 0);

            Assert.AreEqual(((FixedString16Bytes)Str16).CompareTo(Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo(Str10), 0);
            Assert.Less(((FixedString16Bytes)Str10).CompareTo(Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo(StrGreater), 0);
            Assert.Less(((FixedString16Bytes)Str16).CompareTo(StrLess), 0);

            Assert.AreEqual(((FixedString16Bytes)Str16).CompareTo((object)(FixedString16Bytes)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((object)(FixedString16Bytes)Str10), 0);
            Assert.Less(((FixedString16Bytes)Str10).CompareTo((object)(FixedString16Bytes)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((object)(FixedString16Bytes)StrGreater), 0);
            Assert.Less(((FixedString16Bytes)Str16).CompareTo((object)(FixedString16Bytes)StrLess), 0);

            Assert.AreEqual(((FixedString16Bytes)Str16).CompareTo((object)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((object)Str10), 0);
            Assert.Less(((FixedString16Bytes)Str10).CompareTo((object)Str16), 0);
            Assert.Greater(((FixedString16Bytes)Str16).CompareTo((object)StrGreater), 0);
            Assert.Less(((FixedString16Bytes)Str16).CompareTo((object)StrLess), 0);

            Assert.Less(((FixedString16Bytes)Str16).CompareTo(10), 0);
        }

        [Test]
        public void Length()
        {
            Assert.AreEqual(new FixedString16Bytes(Str16).Length, 16);
            Assert.AreEqual(new FixedString16Bytes(Str10).Length, 10);
            Assert.AreEqual(new FixedString16Bytes(StrAIUEO).Length, 5);
        }

        [Test]
        public void Constructor()
        {
            Assert.IsTrue(new FixedString16Bytes(Str16) == Str16);
            Assert.IsTrue(new FixedString16Bytes(Str15) == Str15);

            Assert.IsTrue(new FixedString16Bytes((FixedString16Bytes)Str16) == Str16);
            Assert.IsTrue(new FixedString16Bytes((FixedString16Bytes)Str15) == Str15);

            Assert.IsTrue(new FixedString16Bytes(Str16.AsSpan()) == Str16);
            Assert.IsTrue(new FixedString16Bytes(Str15.AsSpan()) == Str15);

            Span<byte> bytes1 = stackalloc byte[15];
            Encoding.UTF8.GetBytes(Str15, bytes1);
            Assert.IsTrue(new FixedString16Bytes(bytes1) == Str15);

            Span<byte> bytes2 = stackalloc byte[16];
            Encoding.UTF8.GetBytes(Str16, bytes2);
            Assert.IsTrue(new FixedString16Bytes(bytes2) == Str16);

            Assert.IsTrue(new FixedString16Bytes(Str16.ToUTF8()) == Str16);
            Assert.IsTrue(new FixedString16Bytes(Str15.ToUTF8()) == Str15);
        }

        [Test]
        public void IsConvertableTo()
        {
            Assert.IsTrue(((FixedString16Bytes)Str16).IsConvertableTo(stackalloc byte[16]));
            Assert.IsFalse(((FixedString16Bytes)Str16).IsConvertableTo(stackalloc byte[15]));

            Assert.IsTrue(((FixedString16Bytes)Str16).IsConvertableTo(stackalloc char[16]));
            Assert.IsFalse(((FixedString16Bytes)Str16).IsConvertableTo(stackalloc char[15]));
        }

        [Test]
        public void GetEnumerator()
        {
            string checkStr = "";
            foreach (var c in ((FixedString16Bytes)Str10))
            {
                checkStr += c;
            }
            Assert.IsTrue(Str10 == checkStr);
        }
    }
}
