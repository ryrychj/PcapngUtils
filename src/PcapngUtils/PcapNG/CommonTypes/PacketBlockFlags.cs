using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.PcapNG.CommonTypes
{
    public sealed class PacketBlockFlags
    {
        #region enum
        [Flags]
        private enum PacketFlags : uint
        {
            Inbound = 0x00000001,
            Outbound = 0x00000002,
            Unicast = 0x00000004,
            Multicast = 0x00000008,
            Broadcast = 0x0000000C,
            Promiscuous = 0x00000010,
            FCSLength = 0x000001E0,

            CrcError = 0x01000000,
            PacketTooLongError = 0x02000000,
            PacketTooShortError = 0x04000000,
            WrongInterFrameGapError = 0x08000000,
            UnalignedFrameError = 0x10000000,
            StartFrameDelimiterError = 0x20000000,
            PreambleError = 0x40000000,
            SymbolError = 0x80000000
        }
        #endregion

        #region fields && properties
        public uint Flag
        {
            get;
            private set;
        }

        public bool Inbound
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Inbound) == (uint)PacketFlags.Inbound);
            }
        }

        public bool Outbound
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Outbound) == (uint)PacketFlags.Outbound);
            }
        }

        public bool Unicast
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Unicast) == (uint)PacketFlags.Unicast);
            }
        }

        public bool Multicast
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Multicast) == (uint)PacketFlags.Multicast);
            }
        }

        public bool Broadcast
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Broadcast) == (uint)PacketFlags.Broadcast);
            }
        }

        public bool Promiscuous
        {
            get
            {
                return ((Flag & (uint)PacketFlags.Promiscuous) == (uint)PacketFlags.Promiscuous);
            }
        }

        public bool FCSLength
        {
            get
            {
                return ((Flag & (uint)PacketFlags.FCSLength) == (uint)PacketFlags.FCSLength);
            }
        }
        
        public bool CrcError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.CrcError) == (uint)PacketFlags.CrcError);
            }
        }

        public bool PacketTooShortError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.PacketTooShortError) == (uint)PacketFlags.PacketTooShortError);
            }
        }

        public bool PacketTooLongError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.PacketTooLongError) == (uint)PacketFlags.PacketTooLongError);
            }
        }

        public bool WrongInterFrameGapError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.WrongInterFrameGapError) == (uint)PacketFlags.WrongInterFrameGapError);
            }
        }

        public bool UnalignedFrameError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.UnalignedFrameError) == (uint)PacketFlags.UnalignedFrameError);
            }
        }

        public bool StartFrameDelimiterError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.StartFrameDelimiterError) == (uint)PacketFlags.StartFrameDelimiterError);
            }
        }

        public bool PreambleError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.PreambleError) == (uint)PacketFlags.PreambleError);
            }
        }

        public bool SymbolError
        {
            get
            {
                return ((Flag & (uint)PacketFlags.SymbolError) == (uint)PacketFlags.SymbolError);
            }
        }
        #endregion

        #region ctor
        public PacketBlockFlags(uint flag)
        {
            this.Flag = flag;
        }
        #endregion  

        #region method
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            PacketBlockFlags p = (PacketBlockFlags)obj;
            return (this.Flag == p.Flag);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
