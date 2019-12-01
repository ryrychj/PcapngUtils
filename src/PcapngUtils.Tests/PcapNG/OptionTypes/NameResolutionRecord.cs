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
using System.Net.Sockets;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class NameResolutionRecord_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void NameResolutionRecord_ConvertToByte_Test(bool reorder)
        {
            NameResolutionRecord postNameResolution;
            NameResolutionRecord preNameResolution = new NameResolutionRecord(new List<NameResolutionRecordEntry>());
            preNameResolution.Add(new NameResolutionRecordEntry(new IPAddress(new byte[] { 127, 0, 0, 1 }), "localhost"));
            preNameResolution.Add(new NameResolutionRecordEntry(new IPAddress(new byte[] { 0x20, 0x01, 0x0d, 0x0db, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x14, 0x28, 0x57, 0xab }), "test addr"));

            byte[] preNameResolutionRecord = preNameResolution.ConvertToByte(reorder, null);

            using (MemoryStream stream = new MemoryStream(preNameResolutionRecord))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postNameResolution = NameResolutionRecord.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postNameResolution);
            Assert.AreEqual(preNameResolution.Count, postNameResolution.Count);
            for (int i = 0; i < preNameResolution.Count; i++)
            {
                Assert.AreEqual(preNameResolution[i].IpAddr, postNameResolution[i].IpAddr);
                Assert.AreEqual(preNameResolution[i].Description, postNameResolution[i].Description);
            }
        }
    }
}
