using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;

namespace PcapngUtils.PcapNG.OptionTypes
{
    public abstract class AbstractOption
    {
        #region fields && properties
        public static readonly Int32 AlignmentBoundary = 4;
        private const Int16 EndOfOption = 0;
        #endregion

        #region methods
        protected static List<KeyValuePair<ushort, byte[]>> EkstractOptions(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");
            UInt16 optionCode;
            byte[] value = null;
            UInt16 valueLength;
            int remainderLength;

            List<KeyValuePair<ushort, byte[]>> ret = new List<KeyValuePair<ushort, byte[]>>();

            if (binaryReader.BaseStream.Position + 4 >= binaryReader.BaseStream.Length)
                return ret;
                
            try
            {
                while (binaryReader.BaseStream.Position + 4 <= binaryReader.BaseStream.Length)
                {
                    optionCode = binaryReader.ReadUInt16().ReverseByteOrder(reverseByteOrder);
                    valueLength = binaryReader.ReadUInt16().ReverseByteOrder(reverseByteOrder);
                    if (valueLength > 0)
                    {
                        value = binaryReader.ReadBytes(valueLength);
                        if (value.Length < valueLength)
                            throw new EndOfStreamException("Unable to read beyond the end of the stream");
                        remainderLength = (int)valueLength % AlignmentBoundary;
                        if (remainderLength > 0)
                            binaryReader.ReadBytes(AlignmentBoundary - remainderLength);
                    }
                    else
                        break;

                    ret.Add(new KeyValuePair<ushort, byte[]>(optionCode, value));
                    if (optionCode == EndOfOption)
                        break;
                }
            }
            catch  (Exception exc)
            {
                if (ActionOnException != null)
                    ActionOnException(exc);
            }
            return ret;
        }

        protected static byte[] ConvertOptionFieldToByte(ushort optionType, byte[] value, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value cannot be null");
            Contract.Requires<ArgumentException>(value.Length > 0 || optionType == 0, "Only for optionType == 0 value.Lenght can be == 0");
            Contract.Requires<IndexOutOfRangeException>(value.Length <= UInt16.MaxValue, "value.Lenght > UInt16.MaxValue");
            
            List<byte> ret = new List<byte>();

            try
            {
                int remainderLength = (AlignmentBoundary - value.Length % AlignmentBoundary) % AlignmentBoundary;
                ret.AddRange(BitConverter.GetBytes(optionType.ReverseByteOrder(reverseByteOrder)));
                ret.AddRange(BitConverter.GetBytes(((UInt16)value.Length).ReverseByteOrder(reverseByteOrder)));
                if (value.Length > 0)
                {
                    ret.AddRange(value);
                    for (int i = 0; i < remainderLength; i++)
                    {
                        ret.Add(0);
                    }
                }
            }
            catch (Exception exc)
            {
                if (ActionOnException != null)
                    ActionOnException(exc);
            }

            return ret.ToArray();
        }
        public abstract byte[] ConvertToByte(bool reverseByteOrder, Action<Exception> ActionOnException);
         
        #endregion
    }
}
