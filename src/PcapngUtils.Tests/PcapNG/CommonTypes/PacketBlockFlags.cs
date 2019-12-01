using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.PcapNG.CommonTypes
{
    [TestFixture]
    public static class PacketBlockFlags_Test
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
}
