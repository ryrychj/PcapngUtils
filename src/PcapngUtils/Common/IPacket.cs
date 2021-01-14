using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.Common
{
    public interface IPacket
    {
        uint Seconds { get; }

        uint Microseconds { get; }

        byte[] Data { get; }
    }
}
