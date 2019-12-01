using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.Extensions
{
    public static class ReverseByteOrderExtension
    {
        [TestFixture]
        private static class ReverseByteOrderExtension_Test
        {
            [Test]
            public static void ReverseByteOrderExtension_UInt16_Test()
            {
                UInt16 origin = 0xABCD;
                UInt16 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0xCDAB);
            }

            [Test]
            public static void ReverseByteOrderExtension_UInt32_Test()
            {
                UInt32 origin = 0xABCDEF01;
                UInt32 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0x01EFCDAB);
            }

            [Test]
            public static void ReverseByteOrderExtension_UInt64_Test()
            {
                UInt64 origin = 0x0123456789ABCDEF;
                UInt64 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0xEFCDAB8967452301);
            }

            [Test]
            public static void ReverseByteOrderExtension_Int16_Test()
            {
                Int16 origin = 0x3210;
                Int16 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0x1032);
            }

            [Test]
            public static void ReverseByteOrderExtension_Int32_Test()
            {
                Int32 origin = 0x76543210;
                Int32 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0x10325476);
            }

            [Test]
            public static void ReverseByteOrderExtension_Int64_Test()
            {
                Int64 origin = 0x7654321076543210;
                Int64 test = origin.ReverseByteOrder(false);
                Assert.AreEqual(test, origin);
                test = origin.ReverseByteOrder(true);
                Assert.AreEqual(test, 0x1032547610325476);
            }
        }
    }
}
