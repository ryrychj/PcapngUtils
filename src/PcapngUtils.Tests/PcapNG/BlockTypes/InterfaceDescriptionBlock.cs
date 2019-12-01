using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class InterfaceDescriptionBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void InterfaceDescriptionBlock_ConvertToByte_Test(bool reorder)
        {
            InterfaceDescriptionBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 1, 0, 0, 0, 136, 0, 0, 0, 1, 0, 0, 0, 255, 255, 0, 0, 2, 0, 50, 0, 92, 68, 101, 118, 105, 99, 101, 92, 78, 80, 70, 95, 123, 68, 65, 51, 70, 56, 65, 55, 54, 45, 55, 49, 55, 69, 45, 52, 69, 65, 55, 45, 57, 69, 68, 53, 45, 48, 51, 57, 56, 68, 68, 69, 57, 67, 49, 55, 69, 125, 0, 0, 9, 0, 1, 0, 6, 0, 0, 0, 12, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 0, 0, 0, 0, 136, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.IsNotNull(block);
                    prePacketBlock = block as InterfaceDescriptionBlock;
                    Assert.IsNotNull(prePacketBlock);
                    byteblock = prePacketBlock.ConvertToByte(reorder, null);
                }
            }
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.IsNotNull(block);
                    postPacketBlock = block as InterfaceDescriptionBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.LinkType, postPacketBlock.LinkType);
                    Assert.AreEqual(prePacketBlock.SnapLength, postPacketBlock.SnapLength);
                    Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);

                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.Description, postPacketBlock.Options.Description);
                    Assert.AreEqual(prePacketBlock.Options.EuiAddress, postPacketBlock.Options.EuiAddress);
                    Assert.AreEqual(prePacketBlock.Options.Filter, postPacketBlock.Options.Filter);
                    Assert.AreEqual(prePacketBlock.Options.FrameCheckSequence, postPacketBlock.Options.FrameCheckSequence);
                    Assert.AreEqual(prePacketBlock.Options.IPv4Address, postPacketBlock.Options.IPv4Address);
                    Assert.AreEqual(prePacketBlock.Options.IPv6Address, postPacketBlock.Options.IPv6Address);
                    Assert.AreEqual(prePacketBlock.Options.MacAddress, postPacketBlock.Options.MacAddress);
                    Assert.AreEqual(prePacketBlock.Options.Name, postPacketBlock.Options.Name);
                    Assert.AreEqual(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                    Assert.AreEqual(prePacketBlock.Options.Speed, postPacketBlock.Options.Speed);
                    Assert.AreEqual(prePacketBlock.Options.TimeOffsetSeconds, postPacketBlock.Options.TimeOffsetSeconds);
                    Assert.AreEqual(prePacketBlock.Options.TimestampResolution, postPacketBlock.Options.TimestampResolution);
                    Assert.AreEqual(prePacketBlock.Options.TimeZone, postPacketBlock.Options.TimeZone);
                }
            }
        }
    }
}
