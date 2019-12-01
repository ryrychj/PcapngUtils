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

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class InterfaceStatisticsBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void InterfaceStatisticsBlock_ConvertToByte_Test(bool reorder)
        {
            InterfaceStatisticsBlock preStatisticBlock, postStatisticBlock;
            byte[] byteblock = { 5, 0, 0, 0, 108, 0, 0, 0, 1, 0, 0, 0, 34, 18, 5, 0, 87, 234, 56, 202, 1, 0, 28, 0, 67, 111, 117, 110, 116, 101, 114, 115, 32, 112, 114, 111, 118, 105, 100, 101, 100, 32, 98, 121, 32, 100, 117, 109, 112, 99, 97, 112, 2, 0, 8, 0, 34, 18, 5, 0, 36, 137, 18, 202, 3, 0, 8, 0, 34, 18, 5, 0, 87, 234, 56, 202, 4, 0, 8, 0, 56, 0, 0, 0, 0, 0, 0, 0, 5, 0, 8, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 108, 0, 0, 0 };
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                    Assert.IsNotNull(block);
                    preStatisticBlock = block as InterfaceStatisticsBlock;
                    Assert.IsNotNull(preStatisticBlock);
                    byteblock = preStatisticBlock.ConvertToByte(reorder, null);
                }
            }
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.IsNotNull(block);
                    postStatisticBlock = block as InterfaceStatisticsBlock;
                    Assert.IsNotNull(postStatisticBlock);

                    Assert.AreEqual(preStatisticBlock.BlockType, postStatisticBlock.BlockType);
                    Assert.AreEqual(preStatisticBlock.InterfaceID, postStatisticBlock.InterfaceID);
                    Assert.AreEqual(preStatisticBlock.Timestamp, postStatisticBlock.Timestamp);
                    Assert.AreEqual(preStatisticBlock.Options.Comment, postStatisticBlock.Options.Comment);
                    Assert.AreEqual(preStatisticBlock.Options.DeliveredToUser, postStatisticBlock.Options.DeliveredToUser);
                    Assert.AreEqual(preStatisticBlock.Options.EndTime, postStatisticBlock.Options.EndTime);
                    Assert.AreEqual(preStatisticBlock.Options.StartTime, postStatisticBlock.Options.StartTime);
                    Assert.AreEqual(preStatisticBlock.Options.FilterAccept, postStatisticBlock.Options.FilterAccept);
                    Assert.AreEqual(preStatisticBlock.Options.InterfaceDrop, postStatisticBlock.Options.InterfaceDrop);
                    Assert.AreEqual(preStatisticBlock.Options.InterfaceReceived, postStatisticBlock.Options.InterfaceReceived);
                    Assert.AreEqual(preStatisticBlock.Options.SystemDrop, postStatisticBlock.Options.SystemDrop);
                }
            }
        }
    }
}
