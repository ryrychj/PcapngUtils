using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.Common
{
    public interface IPacket
    {
        UInt64 Seconds {get;}
        UInt64 Microseconds{get;}
        byte[] Data { get; }
    }
}
