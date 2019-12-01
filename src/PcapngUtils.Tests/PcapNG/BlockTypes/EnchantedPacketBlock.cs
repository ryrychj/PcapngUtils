using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using NUnit.Framework;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class EnchantedPacketBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void EnchantedPacketBlock_ConvertToByte_Test(bool reorder)
        {
            EnchantedPacketBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 6, 0, 0, 0, 132, 0, 0, 0, 0, 0, 0, 0, 12, 191, 4, 0, 118, 176, 176, 8, 98, 0, 0, 0, 98, 0, 0, 0, 0, 0, 94, 0, 1, 177, 0, 33, 40, 5, 41, 186, 8, 0, 69, 0, 0, 84, 48, 167, 0, 0, 255, 1, 3, 72, 192, 168, 177, 160, 10, 64, 11, 49, 8, 0, 10, 251, 67, 168, 0, 0, 79, 161, 27, 41, 0, 2, 83, 141, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 0, 0, 132, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.IsNotNull(block);
                    prePacketBlock = block as EnchantedPacketBlock;
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
                    postPacketBlock = block as EnchantedPacketBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.Data, postPacketBlock.Data);
                    Assert.AreEqual(prePacketBlock.InterfaceID, postPacketBlock.InterfaceID);
                    Assert.AreEqual(prePacketBlock.Microseconds, postPacketBlock.Microseconds);
                    Assert.AreEqual(prePacketBlock.PacketLength, postPacketBlock.PacketLength);
                    Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.AreEqual(prePacketBlock.Seconds, postPacketBlock.Seconds);
                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.DropCount, postPacketBlock.Options.DropCount);
                    Assert.AreEqual(prePacketBlock.Options.Hash, postPacketBlock.Options.Hash);
                    Assert.AreEqual(prePacketBlock.Options.PacketFlag, postPacketBlock.Options.PacketFlag);
                }
            }
        }
    }
}
