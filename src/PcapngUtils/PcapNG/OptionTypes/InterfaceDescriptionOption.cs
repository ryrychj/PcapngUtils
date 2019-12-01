using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.NetworkInformation;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    public sealed class InterfaceDescriptionOption : AbstractOption
    {
        #region internal class
        public sealed class IPAddress_v4
        {
            #region properties
            private readonly byte[] address;
            public string Address
            {
                get { return string.Join(".", address ); }
            }

            private readonly byte[] mask = null;  

            public string Mask
            {
                get { return string.Join(".", mask); }
            }
            #endregion

            #region ctor
            public IPAddress_v4(byte [] data)
            {
                CustomContract.Requires<ArgumentNullException>(data != null, "Data cannot be null");
                CustomContract.Requires<ArgumentException>(data.Length == 8,"Invalid data length. (Data length must be = 8)") ;
                address = data.Take(4).ToArray();
                mask = data.Skip(4).Take(4).ToArray();
            }
            #endregion

            #region method
            public override string ToString()
            {
                return string.Format("{0} {1}", Address, Mask);
            }

            public IPAddress ConvertAddressToIPAddress()
            {
                return new IPAddress(address);
            }

            public byte[] ConvertToByte()
            {
                return address.Concat(mask).ToArray();
            }

            public override bool Equals(Object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                IPAddress_v4 p = (IPAddress_v4)obj;
                return (this.ToString().CompareTo(p.ToString()) ==0);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }   
        
        public sealed class IPAddress_v6
        {
            #region properties  
            private readonly byte[] address;
            public string Address
            {
                get 
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 1; i < address.Length; i = i + 2)
                    {
                        if (sb.Length > 0)
                            sb.Append(":");
                        ushort item = (ushort)(address[i] + (address[i - 1] << 8));
                        sb.Append(item.ToString("x").PadLeft(4, '0'));
                    }
                    return sb.ToString();
                }
            } 

            public byte PrefixLength
            {
                get;
                private set;
            }
            #endregion

            #region ctor
            public IPAddress_v6(byte[] data)
            {
                CustomContract.Requires<ArgumentNullException>(data != null, "Data cannot be null");
                CustomContract.Requires<ArgumentException>(data.Length == 17, "Invalid data length. (Data length must be = 17)");
                address = data.Take(16).ToArray();
                PrefixLength = data[16];
            }
            #endregion

            #region method
            public override string ToString()
            {
                return string.Format("{0}/{1}", Address, PrefixLength);
            }

            public IPAddress ConvertAddressToIPAddress()
            {
                return new IPAddress(address);
            }

            public byte[] ConvertToByte()
            {
                return address.Concat(new List<byte>(){PrefixLength}).ToArray();
            }

            public override bool Equals(Object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                IPAddress_v6 p = (IPAddress_v6)obj;
                return (this.ToString().CompareTo(p.ToString()) == 0);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            #endregion
        }
        #endregion              

        #region enum
        public enum InterfaceDescriptionOptionCode : ushort
        {
            EndOfOptionsCode = 0,
            CommentCode = 1,
            NameCode = 2,
            DescriptionCode = 3,
            IPv4AddressCode = 4,
            IPv6AddressCode = 5,
            MacAddressCode = 6,
            EuiAddressCode = 7,
            SpeedCode = 8,
            TimestampResolutionCode = 9,
            TimeZoneCode = 10,
            FilterCode = 11,
            OperatingSystemCode = 12,
            FrameCheckSequenceCode = 13,
            TimeOffsetSecondsCode = 14
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
        /// A UTF-8 string containing the name of the device used to capture data.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// A UTF-8 string containing the description of the device used to capture data.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Interface network address and netmask. This option can be repeated multiple times within the same Interface Description Block 
        /// when multiple IPv4 addresses are assigned to the interface.
        /// </summary>
        public IPAddress_v4 IPv4Address
        {
            get;
            set;
        }

        /// <summary>
        /// Interface network address and prefix length (stored in the last byte). This option can be repeated multiple times within the 
        /// same Interface Description Block when multiple IPv6 addresses are assigned to the interface.
        /// </summary>
        public IPAddress_v6 IPv6Address
        {
            get;
            set;
        }

        /// <summary>
        /// Interface Hardware MAC address
        /// </summary>
        public PhysicalAddress MacAddress
        {
            get;
            set;
        }

        private byte[] euiAddress;
        /// <summary>
        /// Interface Hardware EUI address (64 bits), if available.
        /// </summary>
        public byte [] EuiAddress
        {
            get
            {
                return euiAddress;
            }
            set
            {
                CustomContract.Requires<ArgumentException>(value == null || value.Length == 8, "Invalid EuiAddress length. (Should be 8 bytes)");
                euiAddress = value;
            }
        }

        /// <summary>
        /// Interface speed (in bps).
        /// </summary>
        public long? Speed
        {
            get;
            set;
        }

        /// <summary>
        /// Resolution of timestamps. If the Most Significant Bit is equal to zero, the remaining bits indicates the resolution of the timestamp 
        /// as as a negative power of 10 (e.g. 6 means microsecond resolution, timestamps are the number of microseconds since 1/1/1970). 
        /// If the Most Significant Bit is equal to one, the remaining bits indicates the resolution as as negative power of 2 (e.g. 10 means 
        /// 1/1024 of second). If this option is not present, a resolution of 10^-6 is assumed (i.e. timestamps have the same resolution 
        /// of the standard 'libpcap' timestamps).
        /// </summary>
        public byte? TimestampResolution
        {
            get;
            set;
        }

        /// <summary>
        /// Time zone for GMT support
        /// </summary>
        public int? TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// The filter (e.g. "capture only TCP traffic") used to capture traffic. The first byte of the Option Data keeps a code of the 
        /// filter used (e.g. if this is a libpcap string, or BPF bytecode, and more)
        /// </summary>
        public byte[] Filter
        {
            get;
            set;
        }

        /// <summary>
        /// A UTF-8 string containing the name of the operating system of the machine in which this interface is installed. This can be 
        /// different from the same information that can be contained by the Section Header Block (Section 3.1) because the capture can have 
        /// been done on a remote machine.
        /// </summary>
        public string OperatingSystem
        {
            get;
            set;
        }

        /// <summary>
        /// An integer value that specified the length of the Frame Check Sequence (in bits) for this interface. For link layers whose FCS 
        /// length can change during time, the Packet Block Flags Word can be use
        /// </summary>
        public byte? FrameCheckSequence
        {
            get;
            set;
        }

        /// <summary>
        /// A 64 bits integer value that specifies an offset (in seconds) that must be added to the timestamp of each packet to obtain 
        /// the absolute timestamp of a packet. If the option is missing, the timestamps stored in the packet must be considered absolute 
        /// timestamps. The time zone of the offset can be specified with the option if_tzone. TODO: won't a if_tsoffset_low for fractional 
        /// second offsets be useful for highly syncronized capture systems?
        /// </summary>
        public long? TimeOffsetSeconds
        {
            get;
            set;
        }
        #endregion

        #region ctor
        public InterfaceDescriptionOption(string Comment = null, string Name = null, string Description = null, IPAddress IPv4Address_v4 = null,
            IPAddress_v6 IPv6Address = null, PhysicalAddress MacAddress = null, byte[] EuiAddress = null, long? Speed = null, byte? TimestampResolution = 6,
            int? TimeZone = null, byte[] Filter = null, string OperatingSystem = null, byte? FrameCheckSequence = null, long? TimeOffsetSeconds = null) 
        {
            CustomContract.Requires<ArgumentException>(EuiAddress == null || EuiAddress.Length == 8, "Invalid EuiAddress length. (Should be 8 bytes)");               
            this.Comment = Comment;
            this.Name = Name;
            this.Description = Description;
            this.IPv4Address = IPv4Address;
            this.IPv6Address = IPv6Address;
            this.MacAddress = MacAddress;
            this.euiAddress = EuiAddress;
            this.Speed = Speed;
            this.TimestampResolution = TimestampResolution;
            this.TimeZone = TimeZone;
            this.Filter = Filter;
            this.OperatingSystem = OperatingSystem;
            this.FrameCheckSequence = FrameCheckSequence;
            this.TimeOffsetSeconds = TimeOffsetSeconds;    
        }
        #endregion

        #region method
        public static InterfaceDescriptionOption Parse(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");
            InterfaceDescriptionOption option = new InterfaceDescriptionOption();
            List<KeyValuePair<ushort, byte[]>> optionsList = EkstractOptions(binaryReader, reverseByteOrder, ActionOnException);
            
            if (optionsList.Any())
            {                          
                foreach (var item in optionsList)
                {
                    try
                    {
                        switch (item.Key)
                        {
                            case (ushort)InterfaceDescriptionOptionCode.CommentCode:
                                option.Comment = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.NameCode:
                                option.Name = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.DescriptionCode:
                                option.Description = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.IPv4AddressCode:                                 
                                if (item.Value.Length == 8)
                                {
                                    option.IPv4Address = new IPAddress_v4(item.Value);
                                }
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] IPv4AddressCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.IPv6AddressCode:                                
                                if (item.Value.Length == 17)
                                {
                                    option.IPv6Address = new IPAddress_v6(item.Value);
                                }
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] IPv6AddressCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 17));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.MacAddressCode:                                
                                if (item.Value.Length == 6)
                                    option.MacAddress = new PhysicalAddress(item.Value);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] MacAddressCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 6));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.EuiAddressCode:                                 
                                if (item.Value.Length == 8)
                                    option.EuiAddress = item.Value;
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] EuiAddressCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.SpeedCode:                               
                                if (item.Value.Length == 8)
                                    option.Speed = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] SpeedCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.TimestampResolutionCode:                               
                                if (item.Value.Length == 1)
                                    option.TimestampResolution = item.Value[0];
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] TimestampResolutionCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 1));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.TimeZoneCode:                                						
                                if (item.Value.Length == 4)
                                    option.TimeZone = (BitConverter.ToInt32(item.Value, 0)).ReverseByteOrder(reverseByteOrder);	// GMT offset
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] TimeZoneCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 4));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.FilterCode:
                                option.Filter = item.Value;	
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.OperatingSystemCode:
                                option.OperatingSystem = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.FrameCheckSequenceCode:                                
                                 if (item.Value.Length == 1)
                                     option.FrameCheckSequence = item.Value[0];
                                else
                                     throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] FrameCheckSequenceCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 1));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.TimeOffsetSecondsCode:                               
                                if (item.Value.Length == 8)
                                    option.TimeOffsetSeconds = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[InterfaceDescriptio.Parse] TimeOffsetSecondsCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)InterfaceDescriptionOptionCode.EndOfOptionsCode:
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
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.CommentCode, comentValue, reverseByteOrder, ActionOnException));
            }

            if (Name != null)
            {
                byte[] nameValue = UTF8Encoding.UTF8.GetBytes(Name);
                if (nameValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.NameCode, nameValue, reverseByteOrder, ActionOnException));
            }

            if (Description != null)
            {
                byte[] descValue = UTF8Encoding.UTF8.GetBytes(Description);
                if (descValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.DescriptionCode, descValue, reverseByteOrder, ActionOnException));
            }

            if (IPv4Address != null)
            {
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.IPv4AddressCode, IPv4Address.ConvertToByte(), reverseByteOrder, ActionOnException));
            }

            if (IPv6Address != null)
            {
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.IPv6AddressCode, IPv6Address.ConvertToByte(), reverseByteOrder, ActionOnException));
            }

            if (MacAddress != null)
            {
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.MacAddressCode, MacAddress.GetAddressBytes(), reverseByteOrder, ActionOnException));
            }

            if (EuiAddress != null)
            {
                if (EuiAddress.Length <= UInt16.MaxValue)
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.EuiAddressCode, EuiAddress, reverseByteOrder, ActionOnException));
            } 

            if (Speed.HasValue)
            {
                byte[] speedValue = BitConverter.GetBytes(Speed.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.SpeedCode, speedValue, reverseByteOrder, ActionOnException));
            }

            if (TimestampResolution.HasValue)
            {
                byte[] timestampValue = { TimestampResolution.Value};
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.TimestampResolutionCode, timestampValue, reverseByteOrder, ActionOnException));
            }

            if (TimeZone.HasValue)
            {
                byte[] timeZoneValue = BitConverter.GetBytes(TimeZone.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.TimeZoneCode, timeZoneValue, reverseByteOrder, ActionOnException));
            }

            if (Filter != null)
            {
                if (Filter.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.FilterCode, Filter, reverseByteOrder, ActionOnException));
            }

            if (OperatingSystem != null)
            {
                byte[] systemValue = UTF8Encoding.UTF8.GetBytes(OperatingSystem);
                if (systemValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.OperatingSystemCode, systemValue, reverseByteOrder, ActionOnException));
            }

            if (FrameCheckSequence.HasValue)
            {
                byte[] fcValue = { FrameCheckSequence.Value };
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.FrameCheckSequenceCode, fcValue, reverseByteOrder, ActionOnException));
            }

            if (TimeOffsetSeconds.HasValue)
            {
                byte[] timeOffsetValue = BitConverter.GetBytes(TimeOffsetSeconds.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.TimeOffsetSecondsCode, timeOffsetValue, reverseByteOrder, ActionOnException));
            }

            ret.AddRange(ConvertOptionFieldToByte((ushort)InterfaceDescriptionOptionCode.EndOfOptionsCode, new byte[0], reverseByteOrder, ActionOnException));
            return ret.ToArray();
        } 
        #endregion
    }
}
