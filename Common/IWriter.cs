using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils.Common
{
    public interface IWriter 
    {
        void Close();
        void WritePacket(IPacket packet);

        event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;
    }
}
