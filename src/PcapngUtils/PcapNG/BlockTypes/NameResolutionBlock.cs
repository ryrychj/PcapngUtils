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

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    public sealed class NameResolutionBlock:AbstractBlock
    {
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
        public NameResolutionRecord NameResolutionRecords 
        {
            get
            {
                return nmeResolutionRecords;
            }
            set
            {
                CustomContract.Requires<ArgumentNullException>(value != null, "NameResolutionRecords cannot be null");
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
                CustomContract.Requires<ArgumentNullException>(value != null, "Options cannot be null");
                options = value;
            }
        }
        #endregion

        #region ctor
        public static NameResolutionBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            CustomContract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            CustomContract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.NameResolution, "Invalid packet type");    

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
            CustomContract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");
            CustomContract.Requires<ArgumentNullException>(nameResolutionRecords != null, "NameResolutionRecords cannot be null");
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
