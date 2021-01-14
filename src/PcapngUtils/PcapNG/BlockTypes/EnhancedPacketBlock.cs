using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Haukcode.PcapngUtils.PcapNG;
using Haukcode.PcapngUtils.PcapNG.CommonTypes;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using Haukcode.PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils.PcapNG.OptionTypes;
using Haukcode.PcapngUtils.Common;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    public sealed class EnhancedPacketBlock : AbstractBlock, IPacket
    {
        #region IPacket
        public uint Seconds
        {
            get { return Timestamp.Seconds; }
        }

        public uint Microseconds
        {
            get { return Timestamp.Microseconds; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.EnhancedPacket; }
        }

        /// <summary>
        /// Interface ID: it specifies the interface this packet comes from; the correct interface will be the one whose Interface Description 
        /// Block (within the current Section of the file) is identified by the same number (see Section 3.2) of this field.
        /// </summary>
        public int InterfaceID
        {
            get;
            set;
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceID
        {
            get { return InterfaceID; }
        }

        /// <summary>
        /// Timestamp (High) and Timestamp (Low): high and low 32-bits of a 64-bit quantity representing the timestamp. The timestamp is a 
        /// single 64-bit unsigned integer representing the number of units since 1/1/1970. The way to interpret this field is specified by the 
        /// 'if_tsresol' option (see Figure 9) of the Interface Description block referenced by this packet. Please note that differently 
        /// from the libpcap file format, timestamps are not saved as two 32-bit values accounting for the seconds and microseconds since 
        /// 1/1/1970. They are saved as a single 64-bit quantity saved as two 32-bit words.
        /// </summary>
        public TimestampHelper Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Packet Len: actual length of the packet when it was transmitted on the network. It can be different from Captured Len if the user 
        /// wants only a snapshot of the packet.
        /// </summary>
        public int PacketLength
        {
            get;
            set;
        }

        /// <summary>
        /// Captured Len: number of bytes captured from the packet (i.e. the length of the Packet Data field). It will be the minimum value  
        /// among the actual Packet Length and the snapshot length (defined in Figure 9). The value of this field does not include the padding 
        /// bytes added at the end of the Packet Data field to align the Packet Data Field to a 32-bit boundary
        /// </summary>
        public int CaptureLength
        {
            get { return data != null ? data.Length : 0; }
        }

        private byte[] data;
        /// <summary>
        /// Packet Data: the data coming from the network, including link-layer headers. The actual length of this field is Captured Len. 
        /// The format of the link-layer headers depends on the LinkType field specified in the Interface Description Block
        /// </summary>
        public byte[] Data
        {
            get
            {
                return data;
            }
            set
            {
                CustomContract.Requires<ArgumentNullException>(value != null, "Data cannot be null");
                data = value;
            }
        }

        private EnhancedPacketOption options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public EnhancedPacketOption Options
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
        public static EnhancedPacketBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            CustomContract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            CustomContract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.EnhancedPacket, "Invalid packet type");

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    int interfaceID = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    byte[] timestamp = binaryReader.ReadBytes(8);
                    var timestampHelper = new TimestampHelper(timestamp, baseBlock.ReverseByteOrder);
                    int capturedLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    int packetLength = binaryReader.ReadInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    byte[] data = binaryReader.ReadBytes(capturedLength);
                    if (data.Length < capturedLength)
                        throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    int remainderLength = (int)capturedLength % BaseBlock.AlignmentBoundary;
                    if (remainderLength > 0)
                    {
                        int paddingLength = BaseBlock.AlignmentBoundary - remainderLength;
                        binaryReader.ReadBytes(paddingLength);
                    }
                    var option = EnhancedPacketOption.Parse(binaryReader, baseBlock.ReverseByteOrder, ActionOnException);
                    var enhancedBlock = new EnhancedPacketBlock(interfaceID, timestampHelper, packetLength, data, option, positionInStream);

                    return enhancedBlock;
                }
            }
        }

        public static EnhancedPacketBlock CreateEnhancedPacketFromIPacket(IPacket packet, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
            CustomContract.Requires<ArgumentNullException>(packet.Data != null, "packet.Data cannot be null");
            var timestampHelper = new TimestampHelper(packet.Seconds, packet.Microseconds);

            var enhancedBlock = new EnhancedPacketBlock(0, timestampHelper, packet.Data.Length, packet.Data, new EnhancedPacketOption(), 0);

            return enhancedBlock;
        }

        /// <summary>
        /// An Enhanced Packet Block is the standard container for storing the packets coming from the network. The Enhanced Packet Block 
        /// is optional because packets can be stored either by means of this block or the Simple Packet Block, which can be used to speed 
        /// up dump generation. 
        /// The Enhanced Packet Block is an improvement over the original Packet Block: 
        /// it stores the Interface Identifier as a 32bit integer value. This is a requirement when a capture stores packets coming from 
        /// a large number of interfaces differently from the Packet Block, the number of packets dropped by the capture system between 
        /// this packet and the previous one is not stored in the header, but rather in an option of the block itself.
        /// </summary>        
        public EnhancedPacketBlock(int InterfaceID, TimestampHelper Timestamp, int PacketLength, byte[] Data, EnhancedPacketOption Options, long PositionInStream = 0)
        {
            CustomContract.Requires<ArgumentNullException>(Timestamp != null, "Timestamp cannot be null");
            CustomContract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");
            CustomContract.Requires<ArgumentNullException>(Data != null, "Data cannot be null");

            this.InterfaceID = InterfaceID;
            this.Timestamp = Timestamp;
            this.PacketLength = PacketLength;
            this.data = Data;
            this.options = Options;
            this.PositionInStream = PositionInStream;
        }
        #endregion        

        #region method
        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(InterfaceID.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Timestamp.ConvertToByte(reverseByteOrder));
            body.AddRange(BitConverter.GetBytes(Data.Length.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(PacketLength.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(Data);
            int remainderLength = (BaseBlock.AlignmentBoundary - Data.Length % BaseBlock.AlignmentBoundary) % BaseBlock.AlignmentBoundary;
            for (int i = 0; i < remainderLength; i++)
            {
                body.Add(0);
            }
            body.AddRange(Options.ConvertToByte(reverseByteOrder, ActionOnException));
            BaseBlock baseBlock = new BaseBlock(this.BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }
        #endregion
    }
}
