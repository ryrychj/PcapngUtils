using System;
using PcapngUtils.PcapNG.BlockTypes;
namespace PcapngUtils.PcapNG.BlockTypes
{
    public abstract class AbstractBlock
    {
        #region fields && properties
        public abstract BaseBlock.Types BlockType 
        {
            get; 
        }

        /// <summary>
        /// packet position in the stream  (set when reading from the stream. )
        /// </summary>
        public long PositionInStream 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// contains information about relations between packet and interface  on which it was captured 
        /// </summary>
        public abstract int? AssociatedInterfaceID
        {
            get;
        }
        #endregion

        #region method
        protected abstract BaseBlock ConvertToBaseBlock(bool reverseByteOrder, Action<Exception> ActionOnException);

        public byte[] ConvertToByte(bool reverseByteOrder, Action<Exception> ActionOnException)
        {
            BaseBlock baseBlock = ConvertToBaseBlock(reverseByteOrder, ActionOnException);
            return baseBlock.ConvertToByte(reverseByteOrder);
        }
        #endregion
    }
}
