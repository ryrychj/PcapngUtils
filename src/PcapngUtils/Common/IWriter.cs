using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haukcode.PcapngUtils.Common
{
    public interface IWriter : IDisposable
    {
        void Close();
        void WritePacket(IPacket packet);

        event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;
    }
}
