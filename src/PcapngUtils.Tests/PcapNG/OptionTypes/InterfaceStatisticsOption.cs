using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using System.IO;
using System.Diagnostics.Contracts;
using NUnit.Framework;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class InterfaceStatisticsOption_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void InterfaceStatisticsOption_ConvertToByte_Test(bool reorder)
        {
            InterfaceStatisticsOption preOption = new InterfaceStatisticsOption();
            InterfaceStatisticsOption postOption;
            preOption.Comment = "Test Comment";
            preOption.DeliveredToUser = 25;
            preOption.EndTime = new TimestampHelper(new byte[] { 1, 0, 0, 0, 2, 0, 0, 0 }, false);
            preOption.StartTime = new TimestampHelper(new byte[] { 1, 0, 0, 3, 2, 0, 0, 4 }, false);
            preOption.FilterAccept = 30;
            preOption.InterfaceDrop = 35;
            preOption.InterfaceReceived = 40;
            preOption.SystemDrop = 45;

            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = InterfaceStatisticsOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.DeliveredToUser, postOption.DeliveredToUser);
            Assert.AreEqual(preOption.EndTime.Seconds, postOption.EndTime.Seconds);
            Assert.AreEqual(preOption.EndTime.Microseconds, postOption.EndTime.Microseconds);
            Assert.AreEqual(preOption.StartTime.Seconds, postOption.StartTime.Seconds);
            Assert.AreEqual(preOption.StartTime.Microseconds, postOption.StartTime.Microseconds);
            Assert.AreEqual(preOption.FilterAccept, postOption.FilterAccept);
            Assert.AreEqual(preOption.InterfaceDrop, postOption.InterfaceDrop);
            Assert.AreEqual(preOption.InterfaceReceived, postOption.InterfaceReceived);
            Assert.AreEqual(preOption.SystemDrop, postOption.SystemDrop);
        }
    }
}
