using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
namespace PcapngUtils.PcapNG.CommonTypes
{
    [ToString]
    [TestFixture]
    public sealed class TimestampHelper
    {
        #region nUnitTest
        [Test]  
        public static void TimestampHelper_Simple_Test()
        {
            byte[] testData = { 1, 0, 0, 0, 1, 0, 0, 0 };
            uint TimestampHight = BitConverter.ToUInt32(testData, 0);
            TimestampHelper timestamp = new TimestampHelper(testData,false);
            Assert.AreEqual(timestamp.TimestampHight, 1);
            Assert.AreEqual(timestamp.TimestampLow, 1);
            Assert.AreEqual(timestamp.Seconds, 4294);
            Assert.AreEqual(timestamp.Microseconds,967297);   
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
        
        #endregion

        #region fields && properties
        private static readonly System.DateTime epochDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [IgnoreDuringToString]
        public UInt32 TimestampHight
        {
            get;
            private set;
        }

        [IgnoreDuringToString]
        public UInt32 TimestampLow
        {
            get;
            private set;
        }
        public UInt64 Seconds
        {
            get;
            private set;
        }

        public UInt64 Microseconds
        {
            get;
            private set;
        }

        #endregion

        #region ctor
        public TimestampHelper(byte[] timestampAsByte,bool reverseByteOrder)
        {
            Contract.Requires<ArgumentNullException>(timestampAsByte != null, "timestampAsByte cannot be null");
            Contract.Requires<ArgumentException>(timestampAsByte.Length == 8, "timestamp must have length = 8");           

            TimestampHight = (BitConverter.ToUInt32(timestampAsByte.Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);
            TimestampLow = (BitConverter.ToUInt32(timestampAsByte.Skip(4).Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);            

            long timestamp = ((TimestampHight * 4294967296) + TimestampLow);
            this.Seconds = (UInt64)(timestamp / 1000000);
            this.Microseconds = (UInt64)(timestamp % 1000000);
        }

        public TimestampHelper(UInt64 Seconds, UInt64 Microseconds)
        {
            this.Seconds = Seconds;
            this.Microseconds = Microseconds;

            ulong timestamp = Seconds * 1000000 + Microseconds;
            this.TimestampHight = (uint)(timestamp / 4294967296);
            this.TimestampLow = (uint)(timestamp % 4294967296);            
        }
        #endregion

        #region method
        public DateTime ToUtc()
        {
            ulong ticks = (Microseconds * (TimeSpan.TicksPerMillisecond / 1000)) +
                         (Seconds * TimeSpan.TicksPerSecond);
            return epochDateTime.AddTicks((long)ticks);
        }

        public byte[] ConvertToByte(bool reverseByteOrder)
        {
            ulong timestamp = Seconds * 1000000 + Microseconds;
            uint TimestampHight =(uint)(timestamp / 4294967296);
            uint TimestampLow = (uint)(timestamp % 4294967296);           

            List<byte> ret = new List<byte>();
            ret.AddRange(BitConverter.GetBytes(TimestampHight.ReverseByteOrder(reverseByteOrder)));
            ret.AddRange(BitConverter.GetBytes(TimestampLow.ReverseByteOrder(reverseByteOrder)));

            return ret.ToArray();
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            TimestampHelper p = (TimestampHelper)obj;
            return (this.Seconds == p.Seconds) && (this.Microseconds == p.Microseconds);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
