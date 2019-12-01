using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;

namespace Haukcode.PcapngUtils.Extensions
{
    public static class ArrayExtensions
    {
        public static string AsString(this System.Array arr)
        {
            CustomContract.Requires<ArgumentNullException>(arr != null, "arr cannot be null");
            return string.Format("{0}{1}{2}", "{", string.Join(",", arr.Cast<object>().ToArray()), "}");           
        }   
    }
}
