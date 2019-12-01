using System;
using System.Collections.Generic;
using System.IO;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils;
using System.Linq;
using Haukcode.PcapngUtils.Common;
using NUnit.Framework;
using System.Runtime.ExceptionServices;

namespace Haukcode.PcapngUtils.PcapNG
{
    [TestFixture]
    public static class PcapNGReader_Test
    {
        [TestCase(50)]
        [TestCase(500)]
        public static void PcapNgReader_IncompletedFileStream_Test(int maxLength)
        {
            byte[] data = { 10, 13, 13, 10, 28, 0, 0, 0, 77, 60, 43, 26, 1, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 28, 0, 0, 0, 1, 0, 0, 0, 32, 0, 0, 0, 1, 0, 0, 0, 255, 255, 0, 0, 9, 0, 1, 0, 6, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 6, 0, 0, 0, 156, 0, 0, 0, 0, 0, 0, 0, 176, 18, 5, 0, 122, 254, 36, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0, 154, 76, 44, 8, 0, 69, 0, 0, 110, 100, 55, 0, 0, 117, 17, 76, 144, 37, 157, 173, 13, 192, 168, 1, 101, 130, 165, 130, 165, 0, 90, 107, 107, 0, 25, 137, 153, 119, 253, 219, 183, 207, 74, 89, 213, 110, 239, 3, 75, 110, 227, 57, 128, 86, 105, 94, 91, 40, 2, 126, 2, 227, 250, 106, 221, 113, 98, 211, 229, 10, 134, 44, 193, 245, 77, 75, 238, 69, 78, 16, 195, 254, 113, 224, 43, 130, 205, 115, 131, 90, 245, 238, 164, 68, 27, 45, 26, 73, 234, 87, 155, 38, 207, 55, 185, 252, 116, 214, 9, 21, 191, 90, 47, 72, 237, 156, 0, 0, 0, 6, 0, 0, 0, 156, 0, 0, 0, 0, 0, 0, 0, 176, 18, 5, 0, 46, 5, 37, 0, 124, 0, 0, 0, 124, 0, 0, 0, 192, 74, 0, 154, 76, 44, 68, 109, 87, 125, 40, 18, 8, 0, 69, 0, 0, 110, 86, 139, 0, 0, 128, 17, 79, 60, 192, 168, 1, 101, 37, 157, 173, 13, 130, 165, 130, 165, 0, 90, 231, 47, 1, 58, 24, 184, 214, 196, 94, 75, 77, 220, 157, 176, 83, 89, 123, 27, 227, 4, 191, 49, 212, 210, 159, 242, 76, 107, 220, 255, 224, 49, 210, 91, 60, 123, 25, 25, 177, 182, 26, 207, 101, 44, 139, 21, 36, 187, 192, 158, 161, 12, 197, 7, 14, 227, 100, 74, 127, 93, 217, 215, 125, 71, 63, 0, 53, 68, 127, 44, 168, 214, 168, 23, 226, 50, 204, 25, 152, 57, 240, 212, 94, 223, 156, 0, 0, 0, 6, 0, 0, 0, 156, 0, 0, 0, 0, 0, 0, 0, 176, 18, 5, 0, 43, 45, 40, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0, 154, 76, 44, 8, 0, 69, 0, 0, 110, 48, 136, 0, 0, 120, 17, 134, 122, 93, 193, 107, 174, 192, 168, 1, 101, 130, 165, 130, 165, 0, 90, 105, 33, 0, 200, 108, 212, 239, 124, 52, 18, 91, 157, 116, 129, 208, 179, 149, 94, 224, 221, 174, 167, 233, 167, 231, 45, 177, 240, 114, 56, 218, 205, 246, 228, 40, 64, 239, 25, 130, 125, 47, 206, 242, 0, 130, 81, 95, 174, 138, 87, 250, 242, 190, 183, 131, 163, 164, 85, 183, 158 };
            data = data.Take(maxLength).ToArray();
            using (var stream = new MemoryStream(data))
            {
                Assert.That(() =>
                {
                    using (var reader = new PcapNGReader(stream, false))
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
                        var a = reader.HeadersWithInterfaceDescriptions.Last();
                    }
                }, Throws.TypeOf<EndOfStreamException>());
            }
        }
    }
}
