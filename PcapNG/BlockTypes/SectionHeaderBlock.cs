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
using System.Reflection;
namespace PcapngUtils.PcapNG.BlockTypes
{
    [ToString]    
    public sealed class SectionHeaderBlock:AbstractBlock
    {
        #region nUnitTest
        [TestFixture]
        private static class SectionHeaderBlock_Test
        {
            [TestCase(true)]
            [TestCase(false)]
            public static void SectionHeaderBlock_ConvertToByte_Test(bool reorder)
            {
                SectionHeaderBlock prePacketBlock, postPacketBlock;
                byte[] byteblock = { 10, 13, 13, 10, 136, 0, 0, 0, 77, 60, 43, 26, 1, 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255, 3, 0, 43, 0, 54, 52, 45, 98, 105, 116, 32, 87, 105, 110, 100, 111, 119, 115, 32, 55, 32, 83, 101, 114, 118, 105, 99, 101, 32, 80, 97, 99, 107, 32, 49, 44, 32, 98, 117, 105, 108, 100, 32, 55, 54, 48, 49, 0, 4, 0, 52, 0, 68, 117, 109, 112, 99, 97, 112, 32, 49, 46, 49, 48, 46, 55, 32, 40, 118, 49, 46, 49, 48, 46, 55, 45, 48, 45, 103, 54, 98, 57, 51, 49, 97, 49, 32, 102, 114, 111, 109, 32, 109, 97, 115, 116, 101, 114, 45, 49, 46, 49, 48, 41, 0, 0, 0, 0, 136, 0, 0, 0 };
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, false, null);
                        Assert.IsNotNull(block);
                        prePacketBlock = block as SectionHeaderBlock;
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
                        postPacketBlock = block as SectionHeaderBlock;
                        Assert.IsNotNull(postPacketBlock);

                        Assert.AreEqual(prePacketBlock.BlockType, postPacketBlock.BlockType);
                        Assert.AreEqual(prePacketBlock.MagicNumber, postPacketBlock.MagicNumber);
                        Assert.AreEqual(prePacketBlock.MajorVersion, postPacketBlock.MajorVersion);
                        Assert.AreEqual(prePacketBlock.MinorVersion, postPacketBlock.MinorVersion);
                        Assert.AreEqual(prePacketBlock.SectionLength, postPacketBlock.SectionLength);
                        Assert.AreEqual(prePacketBlock.PositionInStream, postPacketBlock.PositionInStream);
                        Assert.AreEqual(prePacketBlock.Options.Comment, postPacketBlock.Options.Comment);
                        Assert.AreEqual(prePacketBlock.Options.Hardware, postPacketBlock.Options.Hardware);
                        Assert.AreEqual(prePacketBlock.Options.OperatingSystem, postPacketBlock.Options.OperatingSystem);
                        Assert.AreEqual(prePacketBlock.Options.UserApplication, postPacketBlock.Options.UserApplication);
                    }
                }
            }
        }
        #endregion

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
        [IgnoreDuringToString]
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
                Contract.Requires<ArgumentNullException>(value != null, "Options cannot be null");
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
            Contract.Requires<ArgumentNullException>(baseBlock != null, "BaseBlock cannot be null");
            Contract.Requires<ArgumentNullException>(baseBlock.Body != null, "BaseBlock.Body cannot be null");
            Contract.Requires<ArgumentException>(baseBlock.BlockType == BaseBlock.Types.SectionHeader, "Invalid packet type");   

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
            Contract.Requires<ArgumentNullException>(Options != null, "Options cannot be null");

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
