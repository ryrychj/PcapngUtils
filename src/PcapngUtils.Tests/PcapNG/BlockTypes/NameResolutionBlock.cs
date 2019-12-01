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

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class NameResolutionBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void NameResolutionBlock_ConvertToByte_Test(bool reorder)
        {
            byte[] option = { 1, 0, 12, 0, 84, 101, 115, 116, 32, 67, 111, 109, 109, 101, 110, 116, 2, 0, 8, 0, 68, 110, 115, 32, 78, 97, 109, 101, 3, 0, 4, 0, 127, 0, 0, 1, 4, 0, 16, 0, 32, 1, 13, 219, 0, 0, 0, 0, 0, 0, 0, 0, 20, 40, 87, 171, 0, 0, 0, 0 };
            byte[] records = { 1, 0, 13, 0, 127, 0, 0, 1, 108, 111, 99, 97, 108, 104, 111, 115, 116, 0, 0, 0, 2, 0, 25, 0, 32, 1, 13, 219, 0, 0, 0, 0, 0, 0, 0, 0, 20, 40, 87, 171, 116, 101, 115, 116, 32, 97, 100, 100, 114, 0, 0, 0, 0, 0, 0, 0 };

            NameResolutionBlock prePacketBlock, postPacketBlock;
            using (MemoryStream optionStream = new MemoryStream(option))
            {
                using (MemoryStream recordsStream = new MemoryStream(records))
                {
                    using (BinaryReader optionBinaryReader = new BinaryReader(optionStream))
                    {
                        using (BinaryReader recordsBinaryReader = new BinaryReader(recordsStream))
                        {
                            NameResolutionRecord rec = NameResolutionRecord.Parse(recordsBinaryReader, false, null);
                            Assert.IsNotNull(rec);
                            NameResolutionOption opt = NameResolutionOption.Parse(optionBinaryReader, false, null);
                            Assert.IsNotNull(opt);
                            prePacketBlock = new NameResolutionBlock(rec, opt, 0);
                        }
                    }
                }
            }
            Assert.IsNotNull(prePacketBlock);
            byte[] byteblock = prePacketBlock.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.IsNotNull(block);
                    postPacketBlock = block as NameResolutionBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.NameResolutionRecords.Count, postPacketBlock.NameResolutionRecords.Count);
                    for (int i = 0; i < prePacketBlock.NameResolutionRecords.Count; i++)
                    {
                        Assert.AreEqual(prePacketBlock.NameResolutionRecords[i], postPacketBlock.NameResolutionRecords[i]);
                    }
                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.DnsName, postPacketBlock.Options.DnsName);
                    Assert.AreEqual(prePacketBlock.Options.DnsIp4Addr, postPacketBlock.Options.DnsIp4Addr);
                    Assert.AreEqual(prePacketBlock.Options.DnsIp6Addr, postPacketBlock.Options.DnsIp6Addr);
                }
            }
        }
    }
}
