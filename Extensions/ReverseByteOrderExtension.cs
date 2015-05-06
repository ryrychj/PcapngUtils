using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils.Extensions
{
    [TestFixture]
    public static class ReverseByteOrderExtension
    {
        /*
        #region nUnitTest
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
        #endregion
         * */
        #region Extenstion method
        public static UInt32 ReverseByteOrder(this UInt32 value,bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value); 
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
        }

        public static Int32 ReverseByteOrder(this Int32 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        public static UInt16 ReverseByteOrder(this UInt16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes, 0);
            }
        }

        public static Int16 ReverseByteOrder(this Int16 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)                
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes, 0);
            }
        }

        public static UInt64 ReverseByteOrder(this UInt64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToUInt64(bytes, 0);
            }
        }

        public static Int64 ReverseByteOrder(this Int64 value, bool reverseByteOrder)
        {
            if (!reverseByteOrder)
                return value;
            else
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Array.Reverse(bytes);
                return BitConverter.ToInt64(bytes, 0);
            }
        }
        #endregion
    }
}
