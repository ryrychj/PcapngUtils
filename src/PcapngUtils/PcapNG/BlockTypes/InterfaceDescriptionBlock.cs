using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    public sealed class InterfaceDescriptionBlock:AbstractBlock
    {
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
                CustomContract.Requires<ArgumentNullException>(value != null, "Options cannot be null");
                options = value;
            }
        }
        #endregion

        #region ctor
        public static InterfaceDescriptionBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            CustomContract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            CustomContract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.InterfaceDescription, "Invalid packet type");

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
            CustomContract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");

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
