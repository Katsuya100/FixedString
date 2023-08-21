using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.PerformanceTesting;

namespace Katuusagi.FixedString.Tests
{
    public class FixedString16BytesPerformanceTest
    {
        private const string Str16              = "1234567890123456";
        private const string StrFirstMismatch   = "0234567890123456";
        private const string StrLastMismatch    = "1234567890123450";
        
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int HashCollisionCheckCount = 10000;
        private static int[] HashCollisionCheckCounts = new int[] { 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        private static int[] HashCollisionPow2 = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
        private static int[] HashCollisionPrimes = new int[] { 31, 61, 127, 257, 509, 1021, 2053, 4093, 8191, 16381 };

        [Test]
        [Performance]
        public void GetHashCode_String()
        {
            string s = Str16;
            Measure.Method(() =>
            {
                s.GetHashCode();
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void GetHashCode_FixedString16Bytes()
        {
            FixedString16Bytes s = Str16;
            Measure.Method(() =>
            {
                s.GetHashCode();
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_SameReference_String()
        {
            string left = Str16;
            Measure.Method(() =>
            {
                left.Equals(left);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_SameReference_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            Measure.Method(() =>
            {
                left.Equals(left);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_ExactMatch_String()
        {
            string left = Str16;
            string right = new string(Str16);
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_ExactMatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = Str16;
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_FirstMismatch_String()
        {
            string left = Str16;
            string right = StrFirstMismatch;
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_FirstMismatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = StrFirstMismatch;
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_LastMismatch_String()
        {
            string left = Str16;
            string right = StrLastMismatch;
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void Equals_LastMismatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = StrLastMismatch;
            Measure.Method(() =>
            {
                left.Equals(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_ExactMatch_String()
        {
            string left = Str16;
            string right = new string(Str16);
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_ExactMatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = Str16;
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_FirstMismatch_String()
        {
            string left = Str16;
            string right = StrFirstMismatch;
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_FirstMismatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = StrFirstMismatch;
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_LastMismatch_String()
        {
            string left = Str16;
            string right = StrLastMismatch;
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void CompareTo_LastMismatch_FixedString16Bytes()
        {
            FixedString16Bytes left = Str16;
            FixedString16Bytes right = StrLastMismatch;
            Measure.Method(() =>
            {
                left.CompareTo(right);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        public void GetHashCode_Collision()
        {
            var stringHash = new HashSet<int>();
            var fixedStringHash = new HashSet<int>();
            var check = new HashSet<string>();
            int sHit = 0;
            int fsHit = 0;
            for (int shift = 5; shift < 14; ++shift)
            for (int i = 0; i < HashCollisionCheckCount; ++i)
            {
                string s = CreateUniqueRandomString(check);
                FixedString16Bytes fs = s;
                if (!stringHash.Add(s.GetHashCode()))
                {
                    ++sHit;
                }

                if (!fixedStringHash.Add(fs.GetHashCode()))
                {
                    ++fsHit;
                }
            }

            TestContext.WriteLine($"¡string");
            TestContext.WriteLine($"CollisionCount: {sHit}");
            TestContext.WriteLine($"CollisionRate: {((float)sHit / HashCollisionCheckCount) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡FixedString16Bytes");
            TestContext.WriteLine($"CollisionCount: {fsHit}");
            TestContext.WriteLine($"CollisionRate: {((float)fsHit / HashCollisionCheckCount) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡Compare");
            var improvementRate = sHit == 0 ? 0 : (float)fsHit / sHit;
            TestContext.WriteLine($"ImprovementRate(less than 1 the better): {improvementRate}");
        }

        [Test]
        public void GetHashCode_Collision_RepeatPrime()
        {
            for (int p = 0; p < HashCollisionCheckCounts.Length; ++p)
            {
                var repeat = HashCollisionPrimes[p];
                var checkCount = HashCollisionCheckCounts[p];
                var stringHash = new HashSet<int>();
                var fixedStringHash = new HashSet<int>();
                var check = new HashSet<string>();
                int sHit = 0;
                int fsHit = 0;
                for (int i = 0; i < checkCount; ++i)
                {
                    string s = CreateUniqueRandomString(check);
                    FixedString16Bytes fs = s;
                    if (!stringHash.Add(s.GetHashCode() % repeat))
                    {
                        ++sHit;
                    }

                    if (!fixedStringHash.Add(fs.GetHashCode() % repeat))
                    {
                        ++fsHit;
                    }
                }

                TestContext.WriteLine($"¡{repeat}");
                TestContext.WriteLine($"    ¡string");
                TestContext.WriteLine($"    CollisionCount: {sHit}");
                TestContext.WriteLine($"    CollisionRate: {((float)sHit / checkCount) * 100}%");
                TestContext.WriteLine();

                TestContext.WriteLine($"    ¡FixedString16Bytes");
                TestContext.WriteLine($"    CollisionCount: {fsHit}");
                TestContext.WriteLine($"    CollisionRate: {((float)fsHit / checkCount) * 100}%");
                TestContext.WriteLine();

                TestContext.WriteLine($"    ¡Compare");
                var improvementRate = sHit == 0 ? 0 : (float)fsHit / sHit;
                TestContext.WriteLine($"    ImprovementRate(less than 1 the better): {improvementRate}");
            }
        }

        [Test]
        public void GetHashCode_Collision_RepeatPow2()
        {
            for (int p = 0; p < HashCollisionCheckCounts.Length; ++p)
            {
                var repeat = HashCollisionPow2[p];
                var checkCount = HashCollisionCheckCounts[p];
                var stringHash = new HashSet<int>();
                var fixedStringHash = new HashSet<int>();
                var check = new HashSet<string>();
                int sHit = 0;
                int fsHit = 0;
                for (int i = 0; i < checkCount; ++i)
                {
                    string s = CreateUniqueRandomString(check);
                    FixedString16Bytes fs = s;
                    if (!stringHash.Add(s.GetHashCode() % repeat))
                    {
                        ++sHit;
                    }

                    if (!fixedStringHash.Add(fs.GetHashCode() % repeat))
                    {
                        ++fsHit;
                    }
                }

                TestContext.WriteLine($"¡{repeat}");
                TestContext.WriteLine($"    ¡string");
                TestContext.WriteLine($"    CollisionCount: {sHit}");
                TestContext.WriteLine($"    CollisionRate: {((float)sHit / checkCount) * 100}%");
                TestContext.WriteLine();

                TestContext.WriteLine($"    ¡FixedString16Bytes");
                TestContext.WriteLine($"    CollisionCount: {fsHit}");
                TestContext.WriteLine($"    CollisionRate: {((float)fsHit / checkCount) * 100}%");
                TestContext.WriteLine();

                TestContext.WriteLine($"    ¡Compare");
                var improvementRate = sHit == 0 ? 0 : (float)fsHit / sHit;
                TestContext.WriteLine($"    ImprovementRate(less than 1 the better): {improvementRate}");
            }
        }

        private string CreateUniqueRandomString(HashSet<string> check)
        {
            while(true)
            {
                var str = CreateRandomString();
                if (check.Add(str))
                {
                    return str;
                }
            }
        }

        private string CreateRandomString()
        {
            return string.Concat(Enumerable.Repeat(Chars, UnityEngine.Random.Range(0, 16)).Select(s => s[UnityEngine.Random.Range(0, 36)]));
        }
    }
}
