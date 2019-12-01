using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using System.IO;
using NUnit.Framework;
using System.Net;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class NameResolutionOption_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void NameResolutionOption_ConvertToByte_Test(bool reorder)
        {
            NameResolutionOption preOption = new NameResolutionOption();
            NameResolutionOption postOption;
            preOption.Comment = "Test Comment";
            preOption.DnsName = "Dns Name";
            preOption.DnsIp4Addr = new IPAddress(new byte[] { 127, 0, 0, 1 });
            preOption.DnsIp6Addr = new IPAddress(new byte[] { 0x20, 0x01, 0x0d, 0x0db, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x14, 0x28, 0x57, 0xab });

            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);

            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = NameResolutionOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.DnsName, postOption.DnsName);
            Assert.AreEqual(preOption.DnsIp4Addr, postOption.DnsIp4Addr);
            Assert.AreEqual(preOption.DnsIp6Addr, postOption.DnsIp6Addr);

        }
    }
}
