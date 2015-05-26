using PcapngUtils.PcapNG.CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;
using System.Security.Cryptography;

namespace PcapngUtils.PcapNG.OptionTypes
{
    [ToString]      
    public sealed class EnchantedPacketOption : AbstractOption
    {
        #region nUnitTest
        [TestFixture]
        private static class EnchantedPacketOption_Test
        {
            [TestCase(true)]
            [TestCase(false)]
            [ContractVerification(false)]
            public static void EnchantedPacketOption_ConvertToByte_Test(bool reorder)
            {
                EnchantedPacketOption preOption = new EnchantedPacketOption();
                EnchantedPacketOption postOption;
                preOption.Comment = "Test Comment";
                preOption.DropCount = 25;
                byte[] md5Hash = { 3, 87, 248, 225, 163, 56, 121, 102, 219, 226, 164, 68, 165, 51, 9, 177, 59 };
                preOption.Hash = new HashBlock(md5Hash);
                preOption.PacketFlag = new PacketBlockFlags(0xFF000000);
                byte[] preOptionByte = preOption.ConvertToByte(reorder, null);
                using (MemoryStream stream = new MemoryStream(preOptionByte))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        postOption = EnchantedPacketOption.Parse(binaryReader, reorder, null);
                    }
                }

                Assert.IsNotNull(postOption);
                Assert.AreEqual(preOption.Comment, postOption.Comment);
                Assert.AreEqual(preOption.DropCount, postOption.DropCount);
                Assert.AreEqual(preOption.Hash.Algorithm, postOption.Hash.Algorithm);
                Assert.AreEqual(preOption.Hash.Value, postOption.Hash.Value);
                Assert.AreEqual(preOption.PacketFlag.Flag, postOption.PacketFlag.Flag);
            }
        }
        #endregion

        #region enum
        public enum EnchantedPacketOptionCode : ushort
        {
            EndOfOptionsCode = 0,
            CommentCode = 1,
            PacketFlagCode = 2,
            HashCode = 3,
            DropCountCode = 4
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
        /// A flags word containing link-layer information. 
        /// </summary>
        public PacketBlockFlags PacketFlag
        {
            get;
            set;
        }

        /// <summary>
        /// This option contains a hash of the packet. The first byte specifies the hashing algorithm, while the following bytes contain 
        /// the actual hash, whose size depends on the hashing algorithm, and hence from the value in the first bit. The hashing algorithm 
        /// can be: 2s complement (algorithm byte = 0, size=XXX), XOR (algorithm byte = 1, size=XXX), CRC32 (algorithm byte = 2, size = 4), 
        /// MD-5 (algorithm byte = 3, size=XXX), SHA-1 (algorithm byte = 4, size=XXX). The hash covers only the packet, not the header added 
        /// by the capture driver: this gives the possibility to calculate it inside the network card. The hash allows easier comparison/merging 
        /// of different capture files, and reliable data transfer between the data acquisition system and the capture library. (TODO: the text 
        /// above uses "first bit", but shouldn't this be "first byte"?!?)
        /// </summary>
        public HashBlock Hash
        {
            get;
            set;
        }

        /// <summary>
        /// A 64bit integer value specifying the number of packets lost (by the interface and the operating system) between this packet and the preceding one.
        /// </summary>
        public long? DropCount
        {
            get;
            set;
        }
        #endregion

        #region ctor
        public EnchantedPacketOption(string Comment = null, PacketBlockFlags PacketFlag = null, long? DropCount = null, HashBlock Hash = null) 
        {
            this.Comment = Comment;
            this.PacketFlag = PacketFlag;
            this.DropCount = DropCount;
            this.Hash = Hash;
        }
        #endregion

        #region methods
        public static EnchantedPacketOption Parse(BinaryReader binaryReader, bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");
            EnchantedPacketOption option = new EnchantedPacketOption();
            List<KeyValuePair<ushort, byte[]>> optionsList = EkstractOptions(binaryReader, reverseByteOrder, ActionOnException);
            
            if (optionsList.Any())
            {
                foreach (var item in optionsList)
                {   
                    try
                    {
                        switch (item.Key)
                        {
                            case (ushort)EnchantedPacketOptionCode.CommentCode:
                                option.Comment = UTF8Encoding.UTF8.GetString(item.Value);
                                break;
                            case (ushort)EnchantedPacketOptionCode.PacketFlagCode:
                                if (item.Value.Length == 4)
                                {
                                    uint packetFlag = (BitConverter.ToUInt32(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                    option.PacketFlag = new PacketBlockFlags(packetFlag);
                                }
                                else
                                    throw new ArgumentException(string.Format("[EnchantedPacketOptio.Parse] PacketFlagCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 4));
                                break;
                            case (ushort)EnchantedPacketOptionCode.HashCode:
                                option.Hash = new HashBlock(item.Value);
                                break;
                            case (ushort)EnchantedPacketOptionCode.DropCountCode:
                                if (item.Value.Length == 8)
                                    option.DropCount = (BitConverter.ToInt64(item.Value, 0)).ReverseByteOrder(reverseByteOrder);
                                else
                                    throw new ArgumentException(string.Format("[EnchantedPacketOptio.Parse] HashCode contains invalid length. Received: {0} bytes, expected: {1}", item.Value.Length, 8));
                                break;
                            case (ushort)EnchantedPacketOptionCode.EndOfOptionsCode:
                            default:
                                break;
                        }
                    }
                    catch(Exception exc)
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
                    ret.AddRange(ConvertOptionFieldToByte((ushort)EnchantedPacketOptionCode.CommentCode, comentValue, reverseByteOrder, ActionOnException));
            }

            if (PacketFlag != null)
            {
                byte[] packetFlagValue = BitConverter.GetBytes(PacketFlag.Flag.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)EnchantedPacketOptionCode.PacketFlagCode, packetFlagValue, reverseByteOrder, ActionOnException));
            }

            if (Hash != null)
            {
                byte[] hashValue = Hash.ConvertToByte();
                if (hashValue.Length <= UInt16.MaxValue)
                    ret.AddRange(ConvertOptionFieldToByte((ushort)EnchantedPacketOptionCode.HashCode, hashValue, reverseByteOrder, ActionOnException));
            }

            if (DropCount.HasValue)
            {
                byte[] dropCountValue = BitConverter.GetBytes(DropCount.Value.ReverseByteOrder(reverseByteOrder));
                ret.AddRange(ConvertOptionFieldToByte((ushort)EnchantedPacketOptionCode.DropCountCode, dropCountValue, reverseByteOrder, ActionOnException));
            }

            ret.AddRange(ConvertOptionFieldToByte((ushort)EnchantedPacketOptionCode.EndOfOptionsCode, new byte[0], reverseByteOrder, ActionOnException));
            return ret.ToArray();
        } 
        #endregion

        
    }
}
