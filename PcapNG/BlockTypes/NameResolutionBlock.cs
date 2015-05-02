using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;
using PcapngUtils.Extensions;
using PcapngUtils.PcapNG.OptionTypes;
using System.Diagnostics.Contracts;
using NUnit.Framework;
namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]
    [TestFixture]
    public sealed class NameResolutionBlock:AbstractBlock
    {
        #region nUnitTest
        [TestCase(true)]
        [TestCase(false)]
        public static void NameResolutionBlock_ConvertToByte_Test(bool reorder)
        {
            byte[] option = { 1, 0, 12, 0, 84, 101, 115, 116, 32, 67, 111, 109, 109, 101, 110, 116, 2, 0, 8, 0, 68, 110, 115, 32, 78, 97, 109, 101, 3, 0, 4, 0, 127, 0, 0, 1, 4, 0, 16, 0, 32, 1, 13, 219, 0, 0, 0, 0, 0, 0, 0, 0, 20, 40, 87, 171, 0, 0, 0, 0 };
            byte[] records = { 1, 0, 13, 0, 127, 0, 0, 1, 108, 111, 99, 97, 108, 104, 111, 115, 116, 0, 0, 0, 2, 0, 25, 0, 32, 1, 13, 219, 0, 0, 0, 0, 0, 0, 0, 0, 20, 40, 87, 171, 116, 101, 115, 116, 32, 97, 100, 100, 114, 0, 0, 0, 0, 0, 0, 0 };

            NameResolutionBlock prePacketBlock, postPacketBlock;
            using (MemoryStream optionStream = new MemoryStream(option))
            {
                using (MemoryStream recordsStream = new MemoryStream(records))
                {
                    using (BinaryReader optionBinaryReader = new BinaryReader(optionStream))
                    {
                        using (BinaryReader recordsBinaryReader = new BinaryReader(recordsStream))
                        {
                            NameResolutionRecord rec = NameResolutionRecord.Parse(recordsBinaryReader, false, null);
                            Assert.IsNotNull(rec);
                            NameResolutionOption opt = NameResolutionOption.Parse(optionBinaryReader, false, null);
                            Assert.IsNotNull(opt);
                            prePacketBlock = new NameResolutionBlock(rec, opt, 0);
                        }
                    }
                }
            }
            Assert.IsNotNull(prePacketBlock);            
            byte[] byteblock = prePacketBlock.ConvertToByte(reorder, null);
            using (MemoryStream stream = new MemoryStream(byteblock))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                    Assert.IsNotNull(block);
                    postPacketBlock = block as NameResolutionBlock;
                    Assert.IsNotNull(postPacketBlock);

                    Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                    Assert.AreEqual(prePacketBlock.NameResolutionRecords.Count, postPacketBlock.NameResolutionRecords.Count);
                    for (int i = 0; i < prePacketBlock.NameResolutionRecords.Count; i++)
                    {
                        Assert.AreEqual(prePacketBlock.NameResolutionRecords[i], postPacketBlock.NameResolutionRecords[i]);
                    }
                    Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                    Assert.AreEqual(prePacketBlock.Options.DnsName, postPacketBlock.Options.DnsName);
                    Assert.AreEqual(prePacketBlock.Options.DnsIp4Addr, postPacketBlock.Options.DnsIp4Addr);
                    Assert.AreEqual(prePacketBlock.Options.DnsIp6Addr, postPacketBlock.Options.DnsIp6Addr);
                }
            }   
        }
        #endregion

        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.NameResolution; }
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceID
        {
            get { return null; }
        }

        private NameResolutionRecord nmeResolutionRecords;
        [IgnoreDuringToString]
        public NameResolutionRecord NameResolutionRecords 
        {
            get
            {
                return nmeResolutionRecords;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "NameResolutionRecords cannot be null");
                nmeResolutionRecords = value;
            }
        }

        private NameResolutionOption options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public NameResolutionOption Options
        {
            get
            {
                return options;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Options cannot be null");
                options = value;
            }
        }
        #endregion

        #region ctor
        public static NameResolutionBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.NameResolution, "Invalid packet type");    

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    NameResolutionRecord nameResolutionRecords = NameResolutionRecord.Parse(binaryReader, baseBlock.ReverseByteOrder, ActionOnException);
                    NameResolutionOption options = NameResolutionOption.Parse(binaryReader, baseBlock.ReverseByteOrder, ActionOnException);
                    NameResolutionBlock nameResolutionBlock = new NameResolutionBlock(nameResolutionRecords,options, positionInStream);
                    return nameResolutionBlock;
                }
            }
        }

        /// <summary>
        /// The Name Resolution Block is used to support the correlation of numeric addresses (present in the captured packets) and their 
        /// corresponding canonical names and it is optional. Having the literal names saved in the file, this prevents the need of a name 
        /// resolution in a delayed time, when the association between names and addresses can be different from the one in use at capture time. 
        /// Moreover, the Name Resolution Block avoids the need of issuing a lot of DNS requests every time the trace capture is opened, 
        /// and allows to have name resolution also when reading the capture with a machine not connected to the network.
        /// A Name Resolution Block is normally placed at the beginning of the file, but no assumptions can be taken about its position. 
        /// Name Resolution Blocks can be added in a second time by tools that process the file, like network analyzers.
        /// </summary>
        public NameResolutionBlock(NameResolutionRecord nameResolutionRecords, NameResolutionOption Options, long PositionInStream = 0)
        {              
            Contract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");
            Contract.Requires<ArgumentNullException>(nameResolutionRecords != null, "NameResolutionRecords cannot be null");
            this.NameResolutionRecords = nameResolutionRecords;            
            this.options = Options;
            this.PositionInStream = PositionInStream;
        }
        #endregion

        #region method
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(NameResolutionRecords.ConvertToByte(reverseByteOrder, ActionOnException));
            body.AddRange(Options.ConvertToByte(reverseByteOrder, ActionOnException));
            BaseBlock baseBlock = new BaseBlock(this.BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }
        #endregion
    }
}
