using NUnit.Framework;
using PcapngUtils.Common;
using PcapngUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils.PcapNG.BlockTypes
{          
    public static class AbstractBlockFactory
    {
        #region nUnitTest
        [TestFixture]
        private static class AbstractBlockFactory_Test
        {
            [Test]
            public static void AbstractBlockFactory_ConvertTo_SectionHeaderBlock_Test()
            {
                byte[] byteblock = { 10, 13, 13, 10, 136, 0, 0, 0, 77, 60, 43, 26, 1, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 3, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 4, 0, 52, 0, 68, 117, 109, 112, 99, 97, 112, 32, 49, 46, 49, 48, 46, 55, 32, 40, 118, 49, 46, 49, 48, 46, 55, 45, 48, 45, 103, 54, 98, 57, 51, 49, 97, 49, 32, 102, 114, 111, 109, 32, 109, 97, 115, 116, 101, 114, 45, 49, 46, 49, 48, 41, 0, 0, 0, 0, 136, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        SectionHeaderBlock headerBlock = block as SectionHeaderBlock;
                        Assert.IsNotNull(headerBlock);
                        Assert.AreEqual(headerBlock.MagicNumber, SectionHeaderBlock.MagicNumbers.Identical);
                        Assert.AreEqual(headerBlock.SectionLength, -1);
                        Assert.AreEqual(headerBlock.MajorVersion, 1);
                        Assert.AreEqual(headerBlock.MinorVersion, 0);
                        Assert.AreEqual(headerBlock.Options.OperatingSystem, "64-bit Windows 7 Service Pack 1, build 7601");
                        Assert.AreEqual(headerBlock.Options.UserApplication, "Dumpcap 1.10.7 (v1.10.7-0-g6b931a1 from master-1.10)");
                    }
                }
            }

            [Test]
            public static void AbstractBlockFactory_ConvertTo_InterfaceDescriptionBlock_Test()
            {
                byte[] byteblock = { 1, 0, 0, 0, 136, 0, 0, 0, 1, 0, 0, 0, 255, 255, 0, 0, 2, 0, 50, 0, 92, 68, 101, 118, 105, 99, 101, 92, 78, 80, 70, 95, 123, 68, 65, 51, 70, 56, 65, 55, 54, 45, 55, 49, 55, 69, 45, 52, 69, 65, 55, 45, 57, 69, 68, 53, 45, 48, 51, 57, 56, 68, 68, 69, 57, 67, 49, 55, 69, 125, 0, 0, 9, 0, 1, 0, 6, 0, 0, 0, 12, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 0, 0, 0, 0, 136, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        InterfaceDescriptionBlock interfaceBlock = block as InterfaceDescriptionBlock;
                        Assert.IsNotNull(interfaceBlock);
                        Assert.AreEqual(interfaceBlock.LinkType, LinkTypes.Ethernet);
                        Assert.AreEqual(interfaceBlock.SnapLength, 65535);
                        Assert.AreEqual(interfaceBlock.Options.OperatingSystem, "64-bit Windows 7 Service Pack 1, build 7601");
                        Assert.AreEqual(interfaceBlock.Options.Name, @"\Device\NPF_{DA3F8A76-717E-4EA7-9ED5-0398DDE9C17E}");
                        Assert.AreEqual(interfaceBlock.Options.TimestampResolution, 6);
                    }
                }
            }

            [Test]
            public static void AbstractBlockFactory_ConvertTo_EnchantedPacketBlock_Test()
            {
                byte[] byteblock = { 6, 0, 0, 0, 132, 0, 0, 0, 0, 0, 0, 0, 12, 191, 4, 0, 118, 176, 176, 8, 98, 0, 0, 0, 98, 0, 0, 0, 0, 0, 94, 0, 1, 177, 0, 33, 40, 5, 41, 186, 8, 0, 69, 0, 0, 84, 48, 167, 0, 0, 255, 1, 3, 72, 192, 168, 177, 160, 10, 64, 11, 49, 8, 0, 10, 251, 67, 168, 0, 0, 79, 161, 27, 41, 0, 2, 83, 141, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 0, 0, 132, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        EnchantedPacketBlock packetBlock = block as EnchantedPacketBlock;
                        Assert.IsNotNull(packetBlock);
                        Assert.AreEqual(packetBlock.InterfaceID, 0);
                        Assert.AreEqual(packetBlock.Seconds, 1335958313);
                        Assert.AreEqual(packetBlock.Microseconds, 152630);
                        Assert.IsTrue(true);
                    }
                }
            }

            [Test]
            public static void AbstractBlockFactory_ConvertTo_PacketBlock_Test()
            {
                byte[] byteblock = { 2, 0, 0, 0, 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 123, 0, 0, 0, 232, 3, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 167, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        PacketBlock packetBlock = block as PacketBlock;
                        Assert.IsNotNull(packetBlock);
                        Assert.AreEqual(packetBlock.PacketLength, 1000);
                        Assert.AreEqual(packetBlock.CapturedLength, 123);
                        Assert.AreEqual(packetBlock.InterfaceID, 0);
                        Assert.AreEqual(packetBlock.Seconds, 0);
                        Assert.AreEqual(packetBlock.Microseconds, 0);
                    }
                }
            }

            [Test]
            public static void AbstractBlockFactory_ConvertTo_SimplePacketBlock_Test()
            {
                byte[] byteblock = { 3, 0, 0, 0, 139, 0, 0, 0, 123, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 139, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        SimplePacketBlock packetBlock = block as SimplePacketBlock;
                        Assert.IsNotNull(packetBlock);
                        Assert.AreEqual(packetBlock.PacketLength, 123);
                    }
                }
            }

            [Test]
            public static void AbstractBlockFactory_ConvertTo_InterfaceStatisticBlock_Test()
            {
                byte[] byteblock = { 5, 0, 0, 0, 108, 0, 0, 0, 1, 0, 0, 0, 34, 18, 5, 0, 87, 234, 56, 202, 1, 0, 28, 0, 67, 111, 117, 110, 116, 101, 114, 115, 32, 112, 114, 111, 118, 105, 100, 101, 100, 32, 98, 121, 32, 100, 117, 109, 112, 99, 97, 112, 2, 0, 8, 0, 34, 18, 5, 0, 36, 137, 18, 202, 3, 0, 8, 0, 34, 18, 5, 0, 87, 234, 56, 202, 4, 0, 8, 0, 56, 0, 0, 0, 0, 0, 0, 0, 5, 0, 8, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 108, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        InterfaceStatisticsBlock interfaceBlock = block as InterfaceStatisticsBlock;
                        Assert.IsNotNull(interfaceBlock);
                        Assert.AreEqual(interfaceBlock.InterfaceID, 1);
                        Assert.AreEqual(interfaceBlock.Timestamp.Seconds, 1427315514);
                        Assert.AreEqual(interfaceBlock.Timestamp.Microseconds, 468951);
                        Assert.AreEqual(interfaceBlock.Options.Comment, "Counters provided by dumpcap");
                        Assert.AreEqual(interfaceBlock.Options.InterfaceDrop, 1);
                        Assert.AreEqual(interfaceBlock.Options.InterfaceReceived, 56);
                        Assert.AreEqual(interfaceBlock.Options.StartTime.Seconds, 1427315511);
                        Assert.AreEqual(interfaceBlock.Options.StartTime.Microseconds, 953700);
                        Assert.AreEqual(interfaceBlock.Options.EndTime.Seconds, 1427315514);
                        Assert.AreEqual(interfaceBlock.Options.EndTime.Microseconds, 468951);
                    }
                }
            }
        }
        #endregion

        #region method
        public static AbstractBlock ReadNextBlock(BinaryReader binaryReader, bool bytesReorder, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");
            try
            {
                BaseBlock baseblock = new BaseBlock(binaryReader, bytesReorder);
                AbstractBlock block = null; ;
                switch (baseblock.BlockType)
                {
                    case BaseBlock.Types.SectionHeader:
                        block = SectionHeaderBlock.Parse(baseblock, ActionOnException);  
                        break;
                    case BaseBlock.Types.InterfaceDescription:
                        block = InterfaceDescriptionBlock.Parse(baseblock, ActionOnException);                        
                        break;
                    case BaseBlock.Types.Packet:
                        block = PacketBlock.Parse(baseblock, ActionOnException);
                        break;
                    case BaseBlock.Types.SimplePacket:                             
                        block = SimplePacketBlock.Parse(baseblock, ActionOnException);   
                        break;
                    case BaseBlock.Types.NameResolution:
                        block = NameResolutionBlock.Parse(baseblock, ActionOnException);                         
                        break;
                    case BaseBlock.Types.InterfaceStatistics:
                        block = InterfaceStatisticsBlock.Parse(baseblock, ActionOnException);
                        break;
                    case BaseBlock.Types.EnhancedPacket:
                        block = EnchantedPacketBlock.Parse(baseblock, ActionOnException);
                        break;
                    default:                             
                        break;
                }
                return block;
            }
            catch(Exception exc)
            {
                ActionOnException(exc);
                return null;
            }

        }
        #endregion
    }
}
