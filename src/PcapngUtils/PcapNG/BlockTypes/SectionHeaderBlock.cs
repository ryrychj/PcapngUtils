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
using System.Reflection;

namespace Haukcode.PcapngUtils.PcapNG.BlockTypes
{
    public sealed class SectionHeaderBlock:AbstractBlock
    {
        #region enum
        public enum MagicNumbers : uint
        {
            Identical = 0x1a2b3c4d,
            Swapped = 0x4d3c2b1a            
        }
        #endregion

        #region Properties
        /// <summary>
        /// The block type
        /// </summary>
        public override BaseBlock.Types BlockType
        {
            get { return BaseBlock.Types.SectionHeader; }
        }

        /// <summary>
        /// Byte-Order Magic: magic number, whose value is the hexadecimal number 0x1A2B3C4D. This number can be used to distinguish 
        /// sections that have been saved on little-endian machines from the ones saved on big-endian machines.
        /// </summary>
        public MagicNumbers MagicNumber 
        {
            get; 
            private set; 
        }

        public string MagicNumberString
        {
            get
            {
                return ((uint)this.MagicNumber).ToString("x");
            }
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public override int? AssociatedInterfaceID
        {
            get { return null; }
        }
        /// <summary>
        /// Major Version: number of the current mayor version of the format. Current value is 1. This value should change if the format 
        /// changes in such a way that tools that can read the new format could not read the old format (i.e., the code would have to check 
        /// the version number to be able to read both formats).
        /// </summary>
        public UInt16 MajorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Minor Version: number of the current minor version of the format. Current value is 0. This value should change if the format 
        /// changes in such a way that tools that can read the new format can still automatically read the new format but code that can only 
        /// read the old format cannot read the new format.
        /// </summary>
        public UInt16 MinorVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Section Length: 64-bit value specifying the length in bytes of the following section, excluding the Section Header Block itself. 
        /// This field can be used to skip the section, for faster navigation inside large files. Section Length equal -1 (0xFFFFFFFFFFFFFFFF) 
        /// means that the size of the section is not specified, and the only way to skip the section is to parse the blocks that it contains. 
        /// Please note that if this field is valid (i.e. not -1), its value is always aligned to 32 bits, as all the blocks are aligned to 32-bit 
        /// boundaries. Also, special care should be taken in accessing this field: since the alignment of all the blocks in the file is 32-bit, 
        /// this field is not guaranteed to be aligned to a 64-bit boundary. This could be a problem on 64-bit workstations.
        /// </summary>
        public Int64 SectionLength
        {
            get;
            set;
        }

        private SectionHeaderOption options;
        /// <summary>
        /// optional fields. Optional fields can be used to insert some information that may be useful when reading data, but that is not 
        /// really needed for packet processing. Therefore, each tool can either read the content of the optional fields (if any), 
        /// or skip some of them or even all at once.
        /// </summary>
        public SectionHeaderOption Options
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

        /// <summary>
        /// determines whether the computer and stream endianness are different (See comment MagicNumber)
        /// Examples:   System Endiannes -> LitleEndian, Stream Endiannes BigEndian -> ReverseByteOrder -> true 
        ///             System Endiannes -> LitleEndian, Stream Endiannes LitleEndian -> ReverseByteOrder -> false 
        /// </summary>
        public bool ReverseByteOrder
        {
            get
            {
                return MagicNumber == MagicNumbers.Swapped;
            }
        }
        #endregion

        #region method
        public static SectionHeaderBlock Parse(BaseBlock baseBlock, Action<Exception> ActionOnException)
        {
            CustomContract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            CustomContract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            CustomContract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.SectionHeader, "Invalid packet type");   

            long positionInStream = baseBlock.PositionInStream;
            using (Stream stream = new MemoryStream(baseBlock.Body))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    uint tempMagicNumber = binaryReader.ReadUInt32().ReverseByteOrder(baseBlock.ReverseByteOrder);

                    if (!Enum.IsDefined(typeof(MagicNumbers), tempMagicNumber))
                        throw new ArgumentException(string.Format("[SectionHeaderBlock.Parse] Unrecognized pcapNG magic number: {0}", tempMagicNumber.ToString("x")));

                    MagicNumbers magicNumber = (MagicNumbers)tempMagicNumber;
                    ushort majorVersion = binaryReader.ReadUInt16().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    ushort minorVersion = binaryReader.ReadUInt16().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    long sectionLength = binaryReader.ReadInt64().ReverseByteOrder(baseBlock.ReverseByteOrder);
                    SectionHeaderOption options = SectionHeaderOption.Parse(binaryReader, baseBlock.ReverseByteOrder, ActionOnException);
                    SectionHeaderBlock headerBlock = new SectionHeaderBlock(magicNumber, majorVersion, minorVersion, sectionLength, options, positionInStream);
                    return headerBlock;
                }
            }
        }

        public static SectionHeaderBlock GetEmptyHeader(bool ReverseByteOrder)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName= assembly.GetName();
            string app = string.Format("{0} {1}", assemblyName.Name, assemblyName.Version.ToString());
            SectionHeaderOption options = new SectionHeaderOption(UserApplication: app);
            return new SectionHeaderBlock(ReverseByteOrder ? MagicNumbers.Swapped : MagicNumbers.Identical, 1, 0, -1, options);
        }
        /// <summary>
        /// The Section Header Block is mandatory. It identifies the beginning of a section of the capture dump file. The Section Header Block 
        /// does not contain data but it rather identifies a list of blocks (interfaces, packets) that are logically correlated.
        /// </summary>
        public SectionHeaderBlock(MagicNumbers MagicNumber, UInt16 MajorVersion, UInt16 MinorVersion, Int64 SectionLength, SectionHeaderOption Options, long PositionInStream = 0)
        {
            CustomContract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");

            this.MagicNumber = MagicNumber;
            this.MajorVersion = MajorVersion;
            this.MinorVersion = MinorVersion;
            this.SectionLength = SectionLength;
            this.options = Options;
            this.PositionInStream = PositionInStream;
        }

        protected override BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            List<byte> body = new List<byte>();
            body.AddRange(BitConverter.GetBytes(((uint)MagicNumber).ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(MajorVersion.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(MinorVersion.ReverseByteOrder(reverseByteOrder)));
            body.AddRange(BitConverter.GetBytes(SectionLength.ReverseByteOrder(reverseByteOrder)));            
            body.AddRange(Options.ConvertToByte(reverseByteOrder, ActionOnException));
            BaseBlock baseBlock = new BaseBlock(this.BlockType, body.ToArray(), reverseByteOrder, 0);
            return baseBlock;
        }
        #endregion
    }
}
