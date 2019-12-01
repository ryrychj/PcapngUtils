using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.Pcap
{
    [TestFixture]
    public static class SectionHeader_Test
    {
        [Test]
        public static void SectionHeader_ConvertToByte_Test()
        {
            SectionHeader pre = SectionHeader.CreateEmptyHeader(false, false);
            using (MemoryStream stream = new MemoryStream(pre.ConvertToByte()))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    SectionHeader post = SectionHeader.Parse(br);
                    Assert.AreEqual(pre.MagicNumber, post.MagicNumber);
                    Assert.AreEqual(pre.ReverseByteOrder, post.ReverseByteOrder);
                    Assert.AreEqual(pre.MajorVersion, post.MajorVersion);
                    Assert.AreEqual(pre.MinorVersion, post.MinorVersion);
                    Assert.AreEqual(pre.LinkType, post.LinkType);
                    Assert.AreEqual(pre.MaximumCaptureLength, post.MaximumCaptureLength);
                    Assert.AreEqual(pre.NanoSecondResolution, post.NanoSecondResolution);
                    Assert.AreEqual(pre.SignificantFigures, post.SignificantFigures);
                    Assert.AreEqual(pre.TimezoneOffset, post.TimezoneOffset);
                }
            }

        }
    }
}
