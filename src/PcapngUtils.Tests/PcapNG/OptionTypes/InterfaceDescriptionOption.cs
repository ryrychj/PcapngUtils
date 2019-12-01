using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using System.Net;
using System.Net.NetworkInformation;
using static Haukcode.PcapngUtils.PcapNG.OptionTypes.InterfaceDescriptionOption;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    [TestFixture]
    public static class InterfaceDescriptionOption_Test
    {
        [TestCase(true)]
        [TestCase(false)]
        [ContractVerification(false)]
        public static void InterfaceDescriptionOption_ConvertToByte_Test(bool reorder)
        {
            InterfaceDescriptionOption preOption = new InterfaceDescriptionOption();
            InterfaceDescriptionOption postOption;
            preOption.Comment = "Test Comment";
            preOption.Description = "Test Description";
            preOption.EuiAddress = new byte[] { 0x00, 0x0A, 0xE6, 0xFF, 0xFE, 0x3E, 0xFD, 0xE1 };
            preOption.Filter = new byte[] { 5, 6, 7, 8 };
            preOption.FrameCheckSequence = 255;
            preOption.IPv4Address = new IPAddress_v4(new byte[] { 127, 0, 0, 1, 255, 255, 255, 0 });
            preOption.IPv6Address = new IPAddress_v6(new byte[] { 0x20, 0x01, 0x0d, 0x0db, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x14, 0x28, 0x57, 0xab, 0x40 });
            preOption.MacAddress = new PhysicalAddress(new byte[] { 0x00, 0x0A, 0xE6, 0x3E, 0xFD, 0xE1 });
            preOption.Name = "Test Name";
            preOption.OperatingSystem = "Test OperatingSystem";
            preOption.Speed = 12345678;
            preOption.TimeOffsetSeconds = 1234;
            preOption.TimestampResolution = 6;
            preOption.TimeZone = 1;

            byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(preOptionByte))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    postOption = InterfaceDescriptionOption.Parse(binaryReader, reorder, null);
                }
            }

            Assert.IsNotNull(postOption);
            Assert.AreEqual(preOption.Comment, postOption.Comment);
            Assert.AreEqual(preOption.Description, postOption.Description);
            Assert.AreEqual(preOption.EuiAddress, postOption.EuiAddress);
            Assert.AreEqual(preOption.Filter, postOption.Filter);
            Assert.AreEqual(preOption.FrameCheckSequence, postOption.FrameCheckSequence);
            Assert.AreEqual(preOption.IPv4Address, postOption.IPv4Address);
            Assert.AreEqual(preOption.IPv6Address, postOption.IPv6Address);
            Assert.AreEqual(preOption.MacAddress, postOption.MacAddress);
            Assert.AreEqual(preOption.Name, postOption.Name);
            Assert.AreEqual(preOption.OperatingSystem, postOption.OperatingSystem);
            Assert.AreEqual(preOption.Speed, postOption.Speed);
            Assert.AreEqual(preOption.TimeOffsetSeconds, postOption.TimeOffsetSeconds);
            Assert.AreEqual(preOption.TimestampResolution, postOption.TimestampResolution);
            Assert.AreEqual(preOption.TimeZone, postOption.TimeZone);

        }
    }

    [TestFixture]
    public static class IPAddress_v4_Test
    {
        [Test]
        public static void InterfaceDescriptionOption_IPAddress_v4_Test()
        {
            byte[] preTab = new byte[] { 192, 168, 0, 1, 255, 255, 255, 0 };
            IPAddress_v4 address = new IPAddress_v4(preTab);
            Assert.IsNotNull(address);
            Assert.AreEqual(address.Address, "192.168.0.1");
            Assert.AreEqual(address.Mask, "255.255.255.0");
            Assert.AreEqual(address.ToString(), "192.168.0.1 255.255.255.0");
            byte[] postTab = address.ConvertToByte();
            Assert.AreEqual(preTab, postTab);
        }
    }

    [TestFixture]
    public static class IPAddress_v6_Test
    {
        [Test]
        public static void InterfaceDescriptionOption_IPAddress_v6_Test()
        {
            byte[] preTab = new byte[] { 0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x08, 0xd3, 0x13, 0x19, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x44, 0x40 };
            IPAddress_v6 address = new IPAddress_v6(preTab);
            Assert.IsNotNull(address);
            Assert.AreEqual(address.Address, "2001:0db8:85a3:08d3:1319:8a2e:0370:7344");
            Assert.AreEqual(address.PrefixLength, 64);
            Assert.AreEqual(address.ToString(), "2001:0db8:85a3:08d3:1319:8a2e:0370:7344/64");
            byte[] postTab = address.ConvertToByte();
            Assert.AreEqual(preTab, postTab);
        }
    }
}
