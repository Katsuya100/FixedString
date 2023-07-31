using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.PerformanceTesting;
using UnityEngine.Profiling;

namespace Katuusagi.FixedString.Tests
{
    public class FixedString16BytesPerformanceTest
    {
        private const string Str16              = "1234567890123456";
        private const string StrFirstMismatch   = "0234567890123456";
        private const string StrLastMismatch    = "1234567890123450";
        
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int HashCollisionCheckCount = 100000;
        private static int[] Primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223, };

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
            float sRateSum = 0.0f;
            float fsRateSum = 0.0f;
            foreach (var p in Primes)
            {
                var count = Math.Min(p, HashCollisionCheckCount);
                var stringHash = new HashSet<int>();
                var fixedStringHash = new HashSet<int>();
                var check = new HashSet<string>();
                int sHit = 0;
                int fsHit = 0;
                for (int i = 0; i < count; ++i)
                {
                    string s = CreateUniqueRandomString(check);
                    FixedString16Bytes fs = s;
                    if (!stringHash.Add(s.GetHashCode() % p))
                    {
                        ++sHit;
                    }

                    if (!fixedStringHash.Add(fs.GetHashCode() % p))
                    {
                        ++fsHit;
                    }
                }

                sRateSum += (float)sHit / count;
                fsRateSum += (float)fsHit / count;
            }

            TestContext.WriteLine($"¡string");
            TestContext.WriteLine($"CollisionRate: {(sRateSum / Primes.Length) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡FixedString16Bytes");
            TestContext.WriteLine($"CollisionRate: {(fsRateSum / Primes.Length) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡Compare");
            var improvementRate = sRateSum == 0 ? 0 : (float)fsRateSum / sRateSum;
            TestContext.WriteLine($"ImprovementRate(less than 1 the better): {improvementRate}");
        }

        [Test]
        public void GetHashCode_Collision_RepeatPow2()
        {

            float sRateSum = 0.0f;
            float fsRateSum = 0.0f;
            for (int shift = 1; shift < 31; ++shift)
            {
                var p = 1 << shift;
                var count = Math.Min(p, HashCollisionCheckCount);
                var stringHash = new HashSet<int>();
                var fixedStringHash = new HashSet<int>();
                var check = new HashSet<string>();
                int sHit = 0;
                int fsHit = 0;
                for (int i = 0; i < count; ++i)
                {
                    string s = CreateUniqueRandomString(check);
                    FixedString16Bytes fs = s;
                    if (!stringHash.Add(s.GetHashCode() % p))
                    {
                        ++sHit;
                    }

                    if (!fixedStringHash.Add(fs.GetHashCode() % p))
                    {
                        ++fsHit;
                    }
                }

                sRateSum += (float)sHit / count;
                fsRateSum += (float)fsHit / count;
            }

            TestContext.WriteLine($"¡string");
            TestContext.WriteLine($"CollisionRate: {(sRateSum / 30) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡FixedString16Bytes");
            TestContext.WriteLine($"CollisionRate: {(fsRateSum / 30) * 100}%");
            TestContext.WriteLine();

            TestContext.WriteLine($"¡Compare");
            var improvementRate = sRateSum == 0 ? 0 : (float)fsRateSum / sRateSum;
            TestContext.WriteLine($"ImprovementRate(less than 1 the better): {improvementRate}");
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
