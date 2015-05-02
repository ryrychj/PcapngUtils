using PcapngUtils.Common;
using PcapngUtils.Extensions;
using PcapngUtils.Pcap;
using PcapngUtils.PcapNG;
using PcapngUtils.PcapNG.BlockTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapngUtils
{
    public sealed class IReaderFactory
    {
        #region fields && properties

        #endregion

        #region methods
        public static IReader GetReader(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");
            
            UInt32 mask = 0;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    if (binaryReader.BaseStream.Length < 12)
                        throw new ArgumentException(string.Format("[IReaderFactory.GetReader] file {0} is too short ", path));

                    mask = binaryReader.ReadUInt32();
                    if (mask == (uint)BaseBlock.Types.SectionHeader)
                    {
                        binaryReader.ReadUInt32();
                        mask = binaryReader.ReadUInt32();
                    }
                }
            }

            switch (mask)
            {       
                case (uint)Pcap.SectionHeader.MagicNumbers.microsecondIdentical:
                case (uint)Pcap.SectionHeader.MagicNumbers.microsecondSwapped:
                case (uint)Pcap.SectionHeader.MagicNumbers.nanosecondSwapped:
                case (uint)Pcap.SectionHeader.MagicNumbers.nanosecondIdentical:
                    {
                        IReader reader = new PcapReader(path);
                        return reader;
                    }
                case (uint)PcapNG.BlockTypes.SectionHeaderBlock.MagicNumbers.Identical:
                    {
                        IReader reader = new PcapNGReader(path, false);
                        return reader;
                    }
                case (uint)PcapNG.BlockTypes.SectionHeaderBlock.MagicNumbers.Swapped:
                    {
                        IReader reader = new PcapNGReader(path, true);
                        return reader;
                    }
                default:
                    throw new ArgumentException(string.Format("[IReaderFactory.GetReader] file {0} is not PCAP/PCAPNG file", path));
            }
        }
   
       
        #endregion
    }
}
