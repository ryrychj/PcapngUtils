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
using PcapngUtils.Common;
namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]      
    public sealed class InterfaceDescriptionBlock:AbstractBlock
    {
        #region nUnitTest
        [TestFixture]
        private static class InterfaceDescriptionBlock_Test
        {                               
            [TestCase(true)]
            [TestCase(false)]
            public static void InterfaceDescriptionBlock_ConvertToByte_Test(bool reorder)
            {
                InterfaceDescriptionBlock prePacketBlock, postPacketBlock;
                byte[] byteblock = { 1, 0, 0, 0, 136, 0, 0, 0, 1, 0, 0, 0, 255, 255, 0, 0, 2, 0, 50, 0, 92, 68, 101, 118, 105, 99, 101, 92, 78, 80, 70, 95, 123, 68, 65, 51, 70, 56, 65, 55, 54, 45, 55, 49, 55, 69, 45, 52, 69, 65, 55, 45, 57, 69, 68, 53, 45, 48, 51, 57, 56, 68, 68, 69, 57, 67, 49, 55, 69, 125, 0, 0, 9, 0, 1, 0, 6, 0, 0, 0, 12, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 0, 0, 0, 0, 136, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        prePacketBlock = block as InterfaceDescriptionBlock;
                        Assert.IsNotNull(prePacketBlock);
                        byteblock = prePacketBlock.ConvertToByte(reorder, null);
                    }
                }
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, reorder, null);
                        Assert.IsNotNull(block);
                        postPacketBlock = block as InterfaceDescriptionBlock;
                        Assert.IsNotNull(postPacketBlock);

                        Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                        Assert.AreEqual(prePacketBlock.LinkType, postPacketBlock.LinkType);
                        Assert.AreEqual(prePacketBlock.SnapLength, postPacketBlock.SnapLength);
                        Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);

                        Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                        Assert.AreEqual(prePacketBlock.Options.Description, postPacketBlock.Options.Description);
                        Assert.AreEqual(prePacketBlock.Options.EuiAddress, postPacketBlock.Options.EuiAddress);
                        Assert.AreEqual(prePacketBlock.Options.Filter, postPacketBlock.Options.Filter);
                        Assert.AreEqual(prePacketBlock.Options.FrameCheckSequence, postPacketBlock.Options.FrameCheckSequence);
                        Assert.AreEqual(prePacketBlock.Options.IPv4Address, postPacketBlock.Options.IPv4Address);
                        Assert.AreEqual(prePacketBlock.Options.IPv6Address, postPacketBlock.Options.IPv6Address);
                        Assert.AreEqual(prePacketBlock.Options.MacAddress, postPacketBlock.Options.MacAddress);
                        Assert.AreEqual(prePacketBlock.Options.Name, postPacketBlock.Options.Name);
                        Assert.AreEqual(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                        Assert.AreEqual(prePacketBlock.Options.Speed, postPacketBlock.Options.Speed);
                        Assert.AreEqual(prePacketBlock.Options.TimeOffsetSeconds, postPacketBlock.Options.TimeOffsetSeconds);
                        Assert.AreEqual(prePacketBlock.Options.TimestampResolution, postPacketBlock.Options.TimestampResolution);
                        Assert.AreEqual(prePacketBlock.Options.TimeZone, postPacketBlock.Options.TimeZone);
                    }
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
            get { return BaseBlock.Types.InterfaceDescription; }
        }

        /// <summary>
        /// LinkType: a value that defines the link layer type of this interface
        /// </summary>
        public LinkTypes LinkType
        {
            get;
            set;
        }

        /// <summary>
        /// SnapLen: maximum number of bytes dumped from each packet. The portion of each packet that exceeds this value will not be stored in the file. 
        /// </summary>
        public int SnapLength
        {
            get;
            set;
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceID
        {
            get { return null; }
        }

        private InterfaceDescriptionOption options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public InterfaceDescriptionOption Options
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
        public static InterfaceDescriptionBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.InterfaceDescription, "Invalid packet type");

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    UInt16 linktype = binaryReader.ReadUInt16().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    if (!Enum.IsDefined(typeof(LinkTypes), linktype))
                        throw new ArgumentException(string.Format("[InterfaceDescriptionBlock.ctor] invalid LinkTypes: {0}, block begin on position {1} ", linktype, positionInStream));
                    LinkTypes linkType = (LinkTypes)linktype;
                    binaryReader.ReadUInt16();	// Reserved field.
                    int snapLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    InterfaceDescriptionOption Options = InterfaceDescriptionOption.Parse(binaryReader, baseBlock.ReverseByteOrder, ActionOnException);
                    InterfaceDescriptionBlock interfaceBlock = new InterfaceDescriptionBlock(linkType, snapLength, Options, positionInStream);
                    return interfaceBlock;
                }
            }
        }

        /// <summary>
        /// The Interface Description Block is mandatory. This block is needed to specify the characteristics of the network interface 
        /// on which the capture has been made. In order to properly associate the captured data to the corresponding interface, the Interface 
        /// Description Block must be defined before any other block that uses it; therefore, this block is usually placed immediately after 
        /// the Section Header Block.
        /// </summary>          
        public InterfaceDescriptionBlock(LinkTypes LinkType, int SnapLength, InterfaceDescriptionOption Options, long PositionInStream = 0)
        {
            Contract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");

            this.LinkType = LinkType;
            this.SnapLength = SnapLength;
            this.options = Options;
            this.PositionInStream = PositionInStream;
        }

        public static InterfaceDescriptionBlock GetEmptyInterfaceDescription(bool reverseBytesOrder)
        {
            InterfaceDescriptionOption Options = new InterfaceDescriptionOption();
            return new InterfaceDescriptionBlock(LinkTypes.Ethernet, 65535, Options);
        }
        #endregion

        #region method
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(((UInt16)LinkType).ReverseByteOrder(reverseByteOrder)));
            body.Add(0);
            body.Add(0);
            body.AddRange(BitConverter.GetBytes(SnapLength.ReverseByteOrder(reverseByteOrder)));             
            body.AddRange(Options.ConvertToByte(reverseByteOrder, ActionOnException));
            BaseBlock baseBlock = new BaseBlock(this.BlockType, body.ToArray(), reverseByteOrder, 0);

            return baseBlock;
        }   
        #endregion              
    }
}
