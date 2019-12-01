using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using System.IO;
using System.Diagnostics.Contracts;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    public sealed class InterfaceStatisticsOption : AbstractOption
    {
        #region enum
        public enum InterfaceStatisticsOptionCode : ushort
        {
            EndOfOptionsCode = 0,
            CommentCode = 1,
            StartTimeCode = 2,
            EndTimeCode = 3,
            InterfaceReceivedCode = 4,
            InterfaceDropCode = 5,
            FilterAcceptCode = 6,
            SystemDropCode = 7,
            DeliveredToUserCode = 8
        };
        #endregion

        #region fields & properies
        /// <summary>
        /// A UTF-8 string containing a comment that is associated to the current block.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Time in which the capture started; time will be stored in two blocks of four bytes each. 
        /// Timestamp (High) and Timestamp (Low): high and low 32-bits of a 64-bit quantity representing the timestamp. The timestamp is a 
        /// single 64-bit unsigned integer representing the number of units since 1/1/1970. The way to interpret this field is specified by the 
        /// 'if_tsresol' option (see Figure 9) of the Interface Description block referenced by this packet. Please note that differently 
        /// from the libpcap file format, timestamps are not saved as two 32-bit values accounting for the seconds and microseconds since 
        /// 1/1/1970. They are saved as a single 64-bit quantity saved as two 32-bit words.
        /// </summary>
        public TimestampHelper StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Time in which the capture ended; ; time will be stored in two blocks of four bytes each
        /// Timestamp (High) and Timestamp (Low): high and low 32-bits of a 64-bit quantity representing the timestamp. The timestamp is a 
        /// single 64-bit unsigned integer representing the number of units since 1/1/1970. The way to interpret this field is specified by the 
        /// 'if_tsresol' option (see Figure 9) of the Interface Description block referenced by this packet. Please note that differently 
        /// from the libpcap file format, timestamps are not saved as two 32-bit values accounting for the seconds and microseconds since 
        /// 1/1/1970. They are saved as a single 64-bit quantity saved as two 32-bit words.
        /// </summary>
        public TimestampHelper EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Number of packets received from the physical interface starting from the beginning of the capture.
        /// </summary>
        public long? InterfaceReceived
        {
            get;
            set;
        }

        /// <summary>
        /// Number of packets dropped by the interface due to lack of resources starting from the beginning of the capture.
        /// </summary>
        public long? InterfaceDrop
        {
            get;
            set;
        }

        /// <summary>
        /// Number of packets accepted by filter starting from the beginning of the capture.
        /// </summary>
        public long? FilterAccept
        {
            get;
            set;
        }

        /// <summary>
        /// Number of packets dropped by the operating system starting from the beginning of the capture.
        /// </summary>
        public long? SystemDrop
        {
            get;
            set;
        }
          
        /// <summary>
        /// Number of packets delivered to the user starting from the beginning of the capture. The value contained in this field can be different
        /// from the value 'isb_filteraccept - isb_osdrop' because some packets could still lay in the OS buffers when the capture ended.
        /// </summary>
        public long? DeliveredToUser
        {
            get;
            set;
        }
        #endregion

        #region ctor
        public InterfaceStatisticsOption(string Comment = null, TimestampHelper StartTime = null, TimestampHelper EndTime = null, long? InterfaceReceived = null,
            long? InterfaceDrop = null, long? FilterAccept = null, long? SystemDrop = null, long? DeliveredToUser =null) 
        {
            this.Comment = Comment;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.InterfaceReceived = InterfaceReceived;
            this.InterfaceDrop = InterfaceDrop;
            this.FilterAccept = FilterAccept;
            this.SystemDrop = SystemDrop;
            this.DeliveredToUser = DeliveredToUser;
        }
        #endregion

        #region method
        public static InterfaceStatisticsOption Parse(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");

            InterfaceStatisticsOption option = new InterfaceStatisticsOption();
            List<KeyValuePair<ushort, byte[]>> optionsList = EkstractOptions(binaryReader, reverseByteOrder, ActionOnException);
           
            if (optionsList.Any())
            {
                foreach (var item in optionsList)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case (ushort)InterfaceStatisticsOptionCode.CommentCode:
                                option.Comment = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.StartTimeCode:
                                if (item.Value.Length == 8)
                                    option.StartTime = new TimestampHelper(item.Value, reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] StartTimeCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.EndTimeCode:
                                if (item.Value.Length == 8)
                                    option.EndTime = new TimestampHelper(item.Value, reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] EndTimeCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.InterfaceReceivedCode:                                
                                if (item.Value.Length == 8)
                                     option.InterfaceReceived = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] InterfaceReceivedCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.InterfaceDropCode:                               
                                if (item.Value.Length == 8)
                                     option.InterfaceDrop = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] InterfaceDropCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.FilterAcceptCode:                               
                                if (item.Value.Length == 8)
                                     option.FilterAccept = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] FilterAcceptCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.SystemDropCode:                               
                                if (item.Value.Length == 8)
                                     option.SystemDrop = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] SystemDropCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.DeliveredToUserCode:                                
                                if (item.Value.Length == 8)
                                     option.DeliveredToUser = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceStatisticsOption.Parse] DeliveredToUserCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceStatisticsOptionCode.EndOfOptionsCode:
                            default:
                                break;
                        }
                    }
                    catch (Exception exc)
                    {
                        if (ActionOnException != null)
                            ActionOnException(exc);
                    }
                }
            }
            
            return option;
        }

        public override byte[] ConvertToByte(bool reverseByteOrder, Action<Exception> ActionOnException)
        {    
            List<byte> ret = new List<byte>();

            if (Comment != null)
            {
                byte[] comentValue = UTF8Encoding.UTF8.GetBytes(Comment);
                if (comentValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.CommentCode, comentValue, reverseByteOrder, ActionOnException));
            }

            if (StartTime != null)
            { 
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.StartTimeCode, StartTime.ConvertToByte(reverseByteOrder), reverseByteOrder, ActionOnException));
            }

            if (EndTime != null)
            {
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.EndTimeCode, EndTime.ConvertToByte(reverseByteOrder), reverseByteOrder, ActionOnException));
            }

            if (InterfaceReceived.HasValue)
            {
                byte[] receivedCountValue = BitConverter.GetBytes(InterfaceReceived.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.InterfaceReceivedCode, receivedCountValue, reverseByteOrder, ActionOnException));
            }

            if (InterfaceDrop.HasValue)
            {
                byte[] dropCountValue = BitConverter.GetBytes(InterfaceDrop.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.InterfaceDropCode, dropCountValue, reverseByteOrder, ActionOnException));
            }

            if (FilterAccept.HasValue)
            {
                byte[] filterValue = BitConverter.GetBytes(FilterAccept.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.FilterAcceptCode, filterValue, reverseByteOrder, ActionOnException));
            }

            if (SystemDrop.HasValue)
            {
                byte[] systemDropValue = BitConverter.GetBytes(SystemDrop.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.SystemDropCode, systemDropValue, reverseByteOrder, ActionOnException));
            }

            if (DeliveredToUser.HasValue)
            {
                byte[] deliverValue = BitConverter.GetBytes(DeliveredToUser.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.DeliveredToUserCode, deliverValue, reverseByteOrder, ActionOnException));
            }

            ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceStatisticsOptionCode.EndOfOptionsCode, new byte[0], reverseByteOrder, ActionOnException));
            return ret.ToArray();
        }
        #endregion
    }
}
