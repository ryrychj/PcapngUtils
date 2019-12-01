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
using System.Reflection;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class SectionHeaderBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void SectionHeaderBlock_ConvertToByte_Test(bool reorder)
        {
            SectionHeaderBlock prePacketBlock, postPacketBlock;
            byte[] byteblock = { 10, 13, 13, 10, 136, 0, 0, 0, 77, 60, 43, 26, 1, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 3, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 4, 0, 52, 0, 68, 117, 109, 112, 99, 97, 112, 32, 49, 46, 49, 48, 46, 55, 32, 40, 118, 49, 46, 49, 48, 46, 55, 45, 48, 45, 103, 54, 98, 57, 51, 49, 97, 49, 32, 102, 114, 111, 109, 32, 109, 97, 115, 116, 101, 114, 45, 49, 46, 49, 48, 41, 0, 0, 0, 0, 136, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.IsNotNull(block);
                    prePacketBlock = block as SectionHeaderBlock;
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
                    postPacketBlock = block as SectionHeaderBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.MagicNumber, postPacketBlock.MagicNumber);
                    Assert.AreEqual(prePacketBlock.MajorVersion, postPacketBlock.MajorVersion);
                    Assert.AreEqual(prePacketBlock.MinorVersion, postPacketBlock.MinorVersion);
                    Assert.AreEqual(prePacketBlock.SectionLength, postPacketBlock.SectionLength);
                    Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.Hardware, postPacketBlock.Options.Hardware);
                    Assert.AreEqual(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                    Assert.AreEqual(prePacketBlock.Options.UserApplication, postPacketBlock.Options.UserApplication);
                }
            }
        }
    }
}
