using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils.PcapNG.CommonTypes
{
    [ToString]       
    public sealed class PacketBlockFlags
    {
        #region nUnitTest
        [TestFixture]
        private static class PacketBlockFlags_Test
        {
            [Test]
            public static void PacketBlockFlags_Inbound_Test()
            {
                PacketBlockFlags packetFlagInbound = new PacketBlockFlags(511);
                Assert.IsTrue(packetFlagInbound.Inbound);

                PacketBlockFlags packetFlagNoInbound = new PacketBlockFlags(512);
                Assert.IsFalse(packetFlagNoInbound.Inbound);
            }

            [Test]
            public static void PacketBlockFlags_Outbound_Test()
            {
                PacketBlockFlags packetFlagOutbound = new PacketBlockFlags(254);
                Assert.IsTrue(packetFlagOutbound.Outbound);

                PacketBlockFlags packetFlagNoOutbound = new PacketBlockFlags(253);
                Assert.IsFalse(packetFlagNoOutbound.Outbound);
            }

            [Test]
            public static void PacketBlockFlags_Unicast_Test()
            {
                PacketBlockFlags packetFlagUnicast = new PacketBlockFlags(255);
                Assert.IsTrue(packetFlagUnicast.Unicast);

                PacketBlockFlags packetFlagNoUnicast = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoUnicast.Unicast);
            }

            [Test]
            public static void PacketBlockFlags_Multicast_Test()
            {
                PacketBlockFlags packetFlagMulticast = new PacketBlockFlags(255);
                Assert.IsTrue(packetFlagMulticast.Multicast);

                PacketBlockFlags packetFlagNoMulticast = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoMulticast.Multicast);
            }

            [Test]
            public static void PacketBlockFlags_Broadcast_Test()
            {
                PacketBlockFlags packetFlagBroadcast = new PacketBlockFlags(255);
                Assert.IsTrue(packetFlagBroadcast.Broadcast);

                PacketBlockFlags packetFlagNoBroadcast = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoBroadcast.Broadcast);
            }

            [Test]
            public static void PacketBlockFlags_Promisious_Test()
            {
                PacketBlockFlags packetFlagPromisious = new PacketBlockFlags(255);
                Assert.IsTrue(packetFlagPromisious.Promiscuous);

                PacketBlockFlags packetFlagNoPromisious = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoPromisious.Promiscuous);
            }

            [Test]
            public static void PacketBlockFlags_FCSLength_Test()
            {
                PacketBlockFlags packetFlagFCSLength = new PacketBlockFlags(480);
                Assert.IsTrue(packetFlagFCSLength.FCSLength);

                PacketBlockFlags packetFlagNoFCSLength = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoFCSLength.FCSLength);
            }

            [Test]
            public static void PacketBlockFlags_CrcError_Test()
            {
                PacketBlockFlags packetCrcError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetCrcError.CrcError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.CrcError);
            }

            [Test]
            public static void PacketBlockFlags_PacketTooLongError_Test()
            {
                PacketBlockFlags packetTooLongError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetTooLongError.PacketTooLongError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.PacketTooLongError);
            }

            [Test]
            public static void PacketBlockFlags_PacketTooShortError_Test()
            {
                PacketBlockFlags packetTooShortError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetTooShortError.PacketTooShortError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.PacketTooShortError);
            }

            [Test]
            public static void PacketBlockFlags_WrongInterFrameGapError_Test()
            {
                PacketBlockFlags packetWrongInterFrameGapError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetWrongInterFrameGapError.WrongInterFrameGapError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.WrongInterFrameGapError);
            }

            [Test]
            public static void PacketBlockFlags_UnalignedFrameError_Test()
            {
                PacketBlockFlags packetUnalignedFrameError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetUnalignedFrameError.UnalignedFrameError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.UnalignedFrameError);
            }

            [Test]
            public static void PacketBlockFlags_tartFrameDelimiterError_Test()
            {
                PacketBlockFlags packetStartFrameDelimiterError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetStartFrameDelimiterError.StartFrameDelimiterError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.StartFrameDelimiterError);
            }

            [Test]
            public static void PacketBlockFlags_PreambleError_Test()
            {
                PacketBlockFlags packetPreambleError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetPreambleError.PreambleError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.PreambleError);
            }

            [Test]
            public static void PacketBlockFlags_SymbolError_Test()
            {
                PacketBlockFlags packetSymbolError = new PacketBlockFlags(0xFF000000);
                Assert.IsTrue(packetSymbolError.SymbolError);

                PacketBlockFlags packetFlagNoError = new PacketBlockFlags(128);
                Assert.IsFalse(packetFlagNoError.SymbolError);
            }
        }
        #endregion

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
