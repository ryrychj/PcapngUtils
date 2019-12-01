using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Haukcode.PcapngUtils.Extensions
{
    public static class CustomContract
    {
        public static void Requires<TException>(bool Predicate, string Message)
            where TException : Exception, new()
        {
            if (!Predicate)
            {
                Debug.WriteLine(Message);

                throw new TException();
            }
        }
    }
}
