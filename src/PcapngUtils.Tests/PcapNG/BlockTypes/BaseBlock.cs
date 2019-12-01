using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    [TestFixture]
    public static class BaseBlock_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        public static void BaseBlock_ConvertToByte_Test(bool reorder)
        {
            BaseBlock preBlock, postBlock;
            byte[] byteblock = { 2, 0, 0, 0, 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 123, 0, 0, 0, 232, 3, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 167, 0, 0, 0 };

            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    preBlock = new BaseBlock(binaryReader, false);
                    Assert.IsNotNull(preBlock);
                    byteblock = preBlock.ConvertToByte(reorder);
                }
            }
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postBlock = new BaseBlock(binaryReader, reorder);
                    Assert.IsNotNull(postBlock);
                }
            }
            Assert.AreEqual(preBlock.BlockType, postBlock.BlockType);
            Assert.AreEqual(preBlock.Body.Length, postBlock.Body.Length);
            Assert.AreEqual(preBlock.Body, postBlock.Body);
        }
    }
}
