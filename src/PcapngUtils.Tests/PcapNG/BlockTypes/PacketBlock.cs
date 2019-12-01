using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class PacketBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void PacketBlock_ConvertToByte_Test(bool reorder)
        {
            PacketBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 2, 0, 0, 0, 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 123, 0, 0, 0, 232, 3, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 167, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.IsNotNull(block);
                    prePacketBlock = block as PacketBlock;
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
                    postPacketBlock = block as PacketBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.Data, postPacketBlock.Data);
                    Assert.AreEqual(prePacketBlock.InterfaceID, postPacketBlock.InterfaceID);
                    Assert.AreEqual(prePacketBlock.DropCount, postPacketBlock.DropCount);
                    Assert.AreEqual(prePacketBlock.Microseconds, postPacketBlock.Microseconds);
                    Assert.AreEqual(prePacketBlock.PacketLength, postPacketBlock.PacketLength);
                    Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.AreEqual(prePacketBlock.Seconds, postPacketBlock.Seconds);
                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.Hash, postPacketBlock.Options.Hash);
                    Assert.AreEqual(prePacketBlock.Options.PacketFlag, postPacketBlock.Options.PacketFlag);
                }
            }
        }
    }
}
