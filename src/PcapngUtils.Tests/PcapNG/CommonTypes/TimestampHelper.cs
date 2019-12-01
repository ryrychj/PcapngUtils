using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;

namespace Haukcode.PcapngUtils.PcapNG.CommonTypes
{
    [TestFixture]
    public static class TimestampHelper_Test
    {
        [Test]
        public static void TimestampHelper_Simple_Test()
        {
            byte[] testData = { 1, 0, 0, 0, 1, 0, 0, 0 };
            uint TimestampHight = BitConverter.ToUInt32(testData, 0);
            TimestampHelper timestamp = new TimestampHelper(testData, false);
            Assert.AreEqual(timestamp.TimestampHight, 1);
            Assert.AreEqual(timestamp.TimestampLow, 1);
            Assert.AreEqual(timestamp.Seconds, 4294);
            Assert.AreEqual(timestamp.Microseconds, 967297);
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void TimestampHelper_ConvertToByte_Test(bool reorder)
        {
            byte[] preData = { 1, 0, 0, 0, 1, 0, 0, 0 };

            TimestampHelper preTimestamp = new TimestampHelper(preData, false);
            byte[] postData = preTimestamp.ConvertToByte(reorder);
            TimestampHelper postTimestamp = new TimestampHelper(postData, reorder);
            Assert.AreEqual(preTimestamp, postTimestamp);
        }
    }
}
