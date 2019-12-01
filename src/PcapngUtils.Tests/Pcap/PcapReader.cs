using System;
using System.Collections.Generic;
using System.IO;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils.Common;
using NUnit.Framework;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Haukcode.PcapngUtils.Pcap
{
    [TestFixture]
    public static class PcapReader_Test
    {
        [TestCase(20)]
        [TestCase(170)]
        [TestCase(200)]
        public static void PcapReader_IncompletedFileStream_Test(int maxLength)
        {
            byte[] data = { 212, 195, 178, 161, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 1, 0, 0, 0, 89, 92, 28, 85, 58, 246, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0, 154, 76, 44, 8, 0, 69, 0, 0, 110, 100, 55, 0, 0, 117, 17, 76, 144, 37, 157, 173, 13, 192, 168, 1, 101, 130, 165, 130, 165, 0, 90, 107, 107, 0, 25, 137, 153, 119, 253, 219, 183, 207, 74, 89, 213, 110, 239, 3, 75, 110, 227, 57, 128, 86, 105, 94, 91, 40, 2, 126, 2, 227, 250, 106, 221, 113, 98, 211, 229, 10, 134, 44, 193, 245, 77, 75, 238, 69, 78, 16, 195, 254, 113, 224, 43, 130, 205, 115, 131, 90, 245, 238, 164, 68, 27, 45, 26, 73, 234, 87, 155, 38, 207, 55, 185, 252, 116, 214, 9, 21, 191, 90, 47, 72, 237, 89, 92, 28, 85, 238, 252, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 192, 74, 0, 154, 76, 44, 68, 109, 87, 125, 40, 18, 8, 0, 69, 0, 0, 110, 86, 139 };
            data = data.Take(maxLength).ToArray();
            using (var stream = new MemoryStream(data))
            {
                Assert.That(() =>
                {
                    using (var reader = new PcapReader(stream))
                    {
                        reader.OnReadPacketEvent += (context, packet) =>
                            {
                                IPacket ipacket = packet;
                            };
                        reader.OnExceptionEvent += (sender, exc) =>
                            {
                                ExceptionDispatchInfo.Capture(exc).Throw();
                            };
                        reader.ReadPackets(new System.Threading.CancellationToken());
                        var a = reader.Header;
                    }
                }, Throws.TypeOf<EndOfStreamException>());
            }
        }
    }
}
