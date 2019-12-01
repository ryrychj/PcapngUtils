using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Haukcode.PcapngUtils.PcapNG.OptionTypes
{
    public sealed class NameResolutionRecordEntry 
    {
        #region enum
        public enum NameResolutionRecordCode : ushort
        {
            EndOfRecord = 0,
            Ip4Record = 1,
            Ip6Record = 2,
        };
        #endregion

        #region fields & properies

        private IPAddress ipAddr;
         /// <summary>
        /// The IPv4 or IPv6 address of the DNS server.
         /// </summary>
        public IPAddress IpAddr
        {
            get
            {
                return ipAddr;
            }
            set
            {
                CustomContract.Requires<ArgumentNullException>(value != null, "IpAddr cannot be null");
                CustomContract.Requires<ArgumentNullException>(value.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || value.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6, "Invalid ip addres family");
                this.ipAddr = value;
            }
        }

        private string description;
        /// <summary>
        /// A UTF-8 string containing the name of the machine (DNS server) used to perform the name resolution.
        /// </summary>        
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                CustomContract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(value), "Description cannot be null, empty or whitespace");
                this.description = value;
            }
        }
        #endregion

        #region ctor
        public NameResolutionRecordEntry(IPAddress IpAddr, string Description)
        {
            CustomContract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(Description), "Description cannot be null, empty or whitespace");
            CustomContract.Requires<ArgumentNullException>(IpAddr != null, "IpAddr cannot be null");
            CustomContract.Requires<ArgumentNullException>(IpAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || IpAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6, "Invalid ip addres family");
            this.ipAddr = IpAddr;
            this.description = Description;
        }
        #endregion

        #region method
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            NameResolutionRecordEntry p = (NameResolutionRecordEntry)obj;
          
           return Enumerable.SequenceEqual(this.IpAddr.GetAddressBytes(), p.IpAddr.GetAddressBytes()) && (this.Description.CompareTo(p.Description) == 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }

    public sealed class NameResolutionRecord : AbstractOption, IList<NameResolutionRecordEntry>
    {
        #region fields & properies
        private readonly List<NameResolutionRecordEntry> listRecords = new List<NameResolutionRecordEntry>();

        public int IndexOf(NameResolutionRecordEntry item)
        {
            return listRecords.IndexOf(item);
        }

        public void Insert(int index, NameResolutionRecordEntry item)
        {
            listRecords.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            listRecords.RemoveAt(index);
        }

        public NameResolutionRecordEntry this[int index]
        {
            get
            {
                return listRecords[index];
            }
            set
            {
                listRecords[index] = value;
            }
        }

        public void Add(NameResolutionRecordEntry item)
        {
            listRecords.Add(item);
        }

        public void Clear()
        {
            listRecords.Clear();
        }

        public bool Contains(NameResolutionRecordEntry item)
        {
            return listRecords.Contains(item);
        }

        public void CopyTo(NameResolutionRecordEntry[] array, int arrayIndex)
        {
            listRecords.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return listRecords.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(NameResolutionRecordEntry item)
        {
            return listRecords.Remove(item);
        }

        public IEnumerator<NameResolutionRecordEntry> GetEnumerator()
        {
            return listRecords.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return listRecords.GetEnumerator();
        }
        #endregion

        #region ctor
        public NameResolutionRecord(List<NameResolutionRecordEntry> listRecords)
        {
            CustomContract.Requires<ArgumentNullException>(listRecords != null, "binaryReader cannot be null");
            this.listRecords = listRecords; 
        }
        #endregion

        #region method
        public static NameResolutionRecord Parse(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");

            List<NameResolutionRecordEntry> listRecords = new List<NameResolutionRecordEntry>();
            List<KeyValuePair<ushort, byte[]>> keyValueList = EkstractOptions(binaryReader, reverseByteOrder, ActionOnException);

            foreach (var item in keyValueList)
            {
                try
                {
                    switch (item.Key)
                    {
                        case (ushort)NameResolutionRecordEntry.NameResolutionRecordCode.Ip4Record:
                            {
                                if (item.Value.Length >= 4)
                                {
                                    byte[] addrTemp = item.Value.Take(4).ToArray();
                                    byte[] descTemp = item.Value.Skip(4).Take(item.Value.Length - 4).ToArray();
                                    IPAddress addr = new IPAddress(addrTemp);
                                    string desc = UTF8Encoding.UTF8.GetString(descTemp);
                                    NameResolutionRecordEntry record = new NameResolutionRecordEntry(addr, desc);
                                    listRecords.Add(record);
                                }
                                break;
                            }
                        case (ushort)NameResolutionRecordEntry.NameResolutionRecordCode.Ip6Record:
                            {
                                if (item.Value.Length >= 16)
                                {
                                    byte[] addrTemp = item.Value.Take(16).ToArray();
                                    byte[] descTemp = item.Value.Skip(16).Take(item.Value.Length - 16).ToArray();
                                    IPAddress addr = new IPAddress(addrTemp);
                                    string desc = UTF8Encoding.UTF8.GetString(descTemp);
                                    NameResolutionRecordEntry record = new NameResolutionRecordEntry(addr, desc);
                                    listRecords.Add(record);
                                }
                                break;
                            }
                        case (ushort)NameResolutionRecordEntry.NameResolutionRecordCode.EndOfRecord:
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
            return new NameResolutionRecord(listRecords);
        }

        public override byte[] ConvertToByte(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            List<byte> ret = new List<byte>();
            foreach (var record in listRecords)
            {
                switch(record.IpAddr.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        {
                            List<byte> temp = new List<byte>();
                            temp.AddRange(record.IpAddr.GetAddressBytes());
                            temp.AddRange( UTF8Encoding.UTF8.GetBytes(record.Description));
                            if (temp.Count <= UInt16.MaxValue)
                                ret.AddRange(ConvertOptionFieldToByte((ushort)NameResolutionRecordEntry.NameResolutionRecordCode.Ip4Record, temp.ToArray(), reverseByteOrder, ActionOnException));
                        }
                        break;
                    case AddressFamily.InterNetworkV6:
                        {
                            List<byte> temp = new List<byte>();
                            temp.AddRange(record.IpAddr.GetAddressBytes());
                            temp.AddRange(UTF8Encoding.UTF8.GetBytes(record.Description));
                            if (temp.Count <= UInt16.MaxValue)
                                ret.AddRange(ConvertOptionFieldToByte((ushort)NameResolutionRecordEntry.NameResolutionRecordCode.Ip6Record, temp.ToArray(), reverseByteOrder, ActionOnException));
                        }
                        break;
                    default:
                        break;
                }                
            }

            ret.AddRange(ConvertOptionFieldToByte((ushort)NameResolutionRecordEntry.NameResolutionRecordCode.EndOfRecord, new byte[0], reverseByteOrder, ActionOnException));
            return ret.ToArray();
        }

        
        #endregion

       
    }
}
