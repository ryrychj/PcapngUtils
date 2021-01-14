using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;

namespace Haukcode.PcapngUtils.PcapNG.CommonTypes
{
    public sealed class TimestampHelper
    {
        #region fields && properties
        private static readonly DateTime epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public uint TimestampHigh
        {
            get;
            private set;
        }

        public uint TimestampLow
        {
            get;
            private set;
        }
        public uint Seconds
        {
            get;
            private set;
        }

        public uint Microseconds
        {
            get;
            private set;
        }

        #endregion

        #region ctor
        public TimestampHelper(byte[] timestampAsByte, bool reverseByteOrder)
        {
            CustomContract.Requires<ArgumentNullException>(timestampAsByte != null, "timestampAsByte cannot be null");
            CustomContract.Requires<ArgumentException>(timestampAsByte.Length == 8, "timestamp must have length = 8");

            TimestampHigh = (BitConverter.ToUInt32(timestampAsByte.Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);
            TimestampLow = (BitConverter.ToUInt32(timestampAsByte.Skip(4).Take(4).ToArray(), 0)).ReverseByteOrder(reverseByteOrder);

            long timestamp = (TimestampHigh * 4294967296) + TimestampLow;
            Seconds = (uint)(timestamp / 1000000);
            Microseconds = (uint)(timestamp % 1000000);
        }

        public TimestampHelper(uint seconds, uint microseconds)
        {
            Seconds = seconds;
            Microseconds = microseconds;

            long timestamp = seconds * 1000000 + microseconds;
            TimestampHigh = (uint)(timestamp / 4294967296);
            TimestampLow = (uint)(timestamp % 4294967296);
        }
        #endregion

        #region method
        public DateTime ToUtc()
        {
            long ticks = (Microseconds * (TimeSpan.TicksPerMillisecond / 1000)) +
                         (Seconds * TimeSpan.TicksPerSecond);
            return epochDateTime.AddTicks(ticks);
        }

        public byte[] ConvertToByte(bool reverseByteOrder)
        {
            long timestamp = ((long)Seconds * 1000000) + Microseconds;
            uint timestampHigh = (uint)(timestamp / 4294967296);
            uint timestampLow = (uint)(timestamp % 4294967296);

            var ret = new List<byte>(8);
            ret.AddRange(BitConverter.GetBytes(timestampHigh.ReverseByteOrder(reverseByteOrder)));
            ret.AddRange(BitConverter.GetBytes(timestampLow.ReverseByteOrder(reverseByteOrder)));

            return ret.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var p = (TimestampHelper)obj;
            return (Seconds == p.Seconds) && (Microseconds == p.Microseconds);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
