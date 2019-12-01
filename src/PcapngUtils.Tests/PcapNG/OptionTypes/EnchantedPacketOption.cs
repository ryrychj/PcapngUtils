using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using System.Security.Cryptography;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class EnchantedPacketOption_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void EnchantedPacketOption_ConvertToByte_Test(bool reorder)
        {
            EnchantedPacketOption preOption = new EnchantedPacketOption();
            EnchantedPacketOption postOption;
            preOption.Comment = "Test Comment";
            preOption.DropCount = 25;
            byte[] md5Hash = { 3, 87, 248, 225, 163, 56, 121, 102, 219, 226, 164, 68, 165, 51, 9, 177, 59 };
            preOption.Hash = new HashBlock(md5Hash);
            preOption.PacketFlag = new PacketBlockFlags(0xFF000000);
            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = EnchantedPacketOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.DropCount, postOption.DropCount);
            Assert.AreEqual(preOption.Hash.Algorithm, postOption.Hash.Algorithm);
            Assert.AreEqual(preOption.Hash.Value, postOption.Hash.Value);
            Assert.AreEqual(preOption.PacketFlag.Flag, postOption.PacketFlag.Flag);
        }
    }
}
