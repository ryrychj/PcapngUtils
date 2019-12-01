using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using System.IO;
using Haukcode.PcapngUtils.Extensions;
using NUnit.Framework;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class SectionHeaderOption_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void SectionHeaderOption_ConvertToByte_Test(bool reorder)
        {
            SectionHeaderOption preOption = new SectionHeaderOption();
            SectionHeaderOption postOption;
            preOption.Comment = "Test Comment";
            preOption.Hardware = "x86 Personal Computer";
            preOption.OperatingSystem = "Windows 7";
            preOption.UserApplication = "PcapngUtils";
            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = SectionHeaderOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.Hardware, postOption.Hardware);
            Assert.AreEqual(preOption.OperatingSystem, postOption.OperatingSystem);
            Assert.AreEqual(preOption.UserApplication, postOption.UserApplication);
        }
    }
}
