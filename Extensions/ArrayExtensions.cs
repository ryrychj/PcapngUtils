using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils.Extensions
{
    public static class ArrayExtensions
    {
        public static string AsString(this System.Array arr)
        {
            Contract.Requires<ArgumentNullException>(arr != null, "arr cannot be null");
            return string.Format("{0}{1}{2}", "{", string.Join(",", arr.Cast<object>().ToArray()), "}");           
        }   
    }
}
