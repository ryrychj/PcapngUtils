using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.Common
{
    public class CommonDelegates
    {
        public delegate void ExceptionEventDelegate(object sender, Exception exception);

        public delegate void ReadPacketEventDelegate(object context, IPacket packet);
    }
}
