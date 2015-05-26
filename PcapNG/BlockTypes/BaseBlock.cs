using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PcapngUtils.PcapNG;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using NUnit.Framework;

namespace PcapngUtils.PcapNG.BlockTypes
{             
    public class BaseBlock 
    {
        #region nUnitTest
        [TestFixture]
        private static class BaseBlock_Test
        {
            [TestCase(true)]
            [TestCase(false)]
            public static void BaseBlock_ConvertToByte_Test(bool reorder)
            {
                BaseBlock preBlock, postBlock;
                byte[] byteblock = { 2, 0, 0, 0, 167, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 123, 0, 0, 0, 232, 3, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 151, 143, 0, 243, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 208, 241, 255, 191, 127, 0, 0, 0, 208, 79, 17, 243, 59, 0, 0, 0, 96, 5, 0, 243, 59, 0, 0, 0, 252, 6, 0, 243, 59, 0, 0, 0, 96, 2, 0, 243, 59, 0, 0, 0, 88, 6, 64, 0, 0, 0, 0, 0, 104, 83, 17, 243, 59, 0, 0, 0, 104, 83, 17, 243, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 167, 0, 0, 0 };

                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        preBlock = new BaseBlock(binaryReader, false);
                        Assert.IsNotNull(preBlock);
                        byteblock = preBlock.ConvertToByte(reorder);
                    }
                }
                using (MemoryStream stream = new MemoryStream(byteblock))
                {
                    using (BinaryReader binaryReader = new BinaryReader(stream))
                    {
                        postBlock = new BaseBlock(binaryReader, reorder);
                        Assert.IsNotNull(postBlock);
                    }
                }
                Assert.AreEqual(preBlock.BlockType, postBlock.BlockType);
                Assert.AreEqual(preBlock.Body.Length, postBlock.Body.Length);
                Assert.AreEqual(preBlock.Body, postBlock.Body);
            }
        }
        #endregion

        #region enum
        public enum Types : uint
        {
            SectionHeader = 0x0A0D0D0A,
            InterfaceDescription = 0x00000001,
            Packet = 0x00000002,
            SimplePacket = 0x00000003,
            NameResolution = 0x00000004,
            InterfaceStatistics = 0x00000005,
            EnhancedPacket = 0x00000006
        }
        #endregion    

        #region fields && properties   
        public static readonly Int32 AlignmentBoundary = 4;
        private static readonly uint  minimalTotaLength = 12;

        /// <summary>
        /// Block Type (32 bits): unique value that identifies the block. Values whose Most Significant Bit (MSB) is equal to 1 
        /// are reserved for local use. They allow to save private data to the file and to extend the file format
        /// </summary>
        public Types BlockType 
        { 
            get; 
            protected set;
        }

        /// <summary>
        /// packet position in the stream  (set when reading from the stream. )
        /// </summary>
        public long PositionInStream
        {
            get; 
            protected set; 
        }

        /// <summary>
        /// Block Body: content of the block.
        /// </summary>
        public byte [] Body 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// determines whether the computer and stream endianness are different (See comment MagicNumber)
        /// Examples:   System Endiannes -> LitleEndian, Stream Endiannes BigEndian -> ReverseByteOrder -> true 
        ///             System Endiannes -> LitleEndian, Stream Endiannes LitleEndian -> ReverseByteOrder -> false 
        /// </summary>
        public bool ReverseByteOrder 
        { 
            get;  
            private set;
        }
        #endregion

        #region ctor
        public BaseBlock(BinaryReader binaryReader,bool reverseByteOrder)
        {
            Contract.Requires<ArgumentNullException>(binaryReader != null, "binaryReader cannot be null");

            this.ReverseByteOrder = reverseByteOrder;
            this.PositionInStream = binaryReader.BaseStream.Position;

            uint blockType = binaryReader.ReadUInt32().ReverseByteOrder(ReverseByteOrder);
            if (!Enum.IsDefined(typeof(BaseBlock.Types), blockType))
                throw new ArgumentException(string.Format("[BaseBlock.ctor] invalid blockType: {0}, block begin on position {1} ", blockType.ToString("x"), PositionInStream));
            this.BlockType = (BaseBlock.Types)blockType;

            uint totalLength = binaryReader.ReadUInt32().ReverseByteOrder(ReverseByteOrder); ;
            if (totalLength < minimalTotaLength)
                throw new Exception(string.Format("[BaseBlock.ctor] block begin on position {0} have insufficient total length {1}", PositionInStream, totalLength));
            Body = binaryReader.ReadBytes((int)(totalLength - minimalTotaLength));
            if (Body.Length < totalLength - minimalTotaLength)
                throw new EndOfStreamException("Unable to read beyond the end of the stream");
            int remainderLength = (int)totalLength % AlignmentBoundary;
            if (remainderLength > 0)
            {
                int paddingLength = AlignmentBoundary - remainderLength;
                binaryReader.ReadBytes(paddingLength);
            }

            uint endTotalLength = binaryReader.ReadUInt32().ReverseByteOrder(ReverseByteOrder); ;
            if (totalLength != endTotalLength)
                throw new Exception(string.Format("[BaseBlock.ctor] block begin on position {0} have differens total length fields. Start total fields {1}, and end total fields {2}. Probably this block is corrupted!", PositionInStream, totalLength, endTotalLength));
        }

        public BaseBlock( Types BlockType, byte [] Body,bool reverseByteOrder, long PositionInStream = 0)
        {
            Contract.Requires<ArgumentNullException>(Body != null, "Body cannot be null");

            this.BlockType = BlockType;
            this.Body = Body;
            this.ReverseByteOrder = reverseByteOrder;
            this.PositionInStream = PositionInStream;
        }

        #endregion

        #region Method
        public byte[] ConvertToByte(bool reverseByteOrder)
        {
            Contract.Requires<ArgumentNullException>(Body != null, "BaseBlock.Body cannot be null");

            List<byte> ret = new List<byte>();
            int remainderLength = (AlignmentBoundary - Body.Length % AlignmentBoundary) % AlignmentBoundary ;

            uint totalLength = (uint)Body.Length + minimalTotaLength;

            ret.AddRange(BitConverter.GetBytes(((uint)BlockType).ReverseByteOrder(reverseByteOrder)));
            ret.AddRange(BitConverter.GetBytes(totalLength.ReverseByteOrder(reverseByteOrder)));
            ret.AddRange(Body);
            for (int i = 0; i < remainderLength; i++)
            {
                ret.Add(0);
            }
            ret.AddRange(BitConverter.GetBytes(totalLength.ReverseByteOrder(reverseByteOrder)));
           
            return ret.ToArray();
        }
        #endregion
    }
}
