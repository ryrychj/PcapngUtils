using System;
using System.Collections.Generic;
using System.IO;
using Haukcode.PcapngUtils.PcapNG.BlockTypes;
using System.Diagnostics.Contracts;
using Haukcode.PcapngUtils;
using System.Linq;
using Haukcode.PcapngUtils.Common;
using System.Runtime.ExceptionServices;
using Haukcode.PcapngUtils.Extensions;

namespace Haukcode.PcapngUtils.PcapNG
{
   
    public sealed class PcapNGReader : Disposable, IReader
    {
        #region event & delegate  
        public event CommonDelegates.ExceptionEventDelegate OnExceptionEvent;

        private void OnException(Exception exception)
        {
            CustomContract.Requires<ArgumentNullException>(exception != null, "exception cannot be null or empty");
            CommonDelegates.ExceptionEventDelegate handler = OnExceptionEvent;
            if (handler != null)
                handler(this, exception);
            else
                ExceptionDispatchInfo.Capture(exception).Throw();
        }

        public event CommonDelegates.ReadPacketEventDelegate OnReadPacketEvent;
        private void OnReadPacket(IPacket packet)
        {
            CustomContract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
            CommonDelegates.ReadPacketEventDelegate handler = OnReadPacketEvent;
            if (handler != null)
                handler(this.headersWithInterface.Last(), packet);
        }

        
        #endregion

        #region fields && properties
        private BinaryReader binaryReader;
        private Stream stream;
        private long basePosition = 0;
        private bool ReverseByteOrder = false;

        private List<HeaderWithInterfacesDescriptions> headersWithInterface = new List<HeaderWithInterfacesDescriptions>();
        public IList<HeaderWithInterfacesDescriptions> HeadersWithInterfaceDescriptions
        {
            get { return headersWithInterface.AsReadOnly(); }
        }

        private object syncRoot = new object();
        #endregion

        #region ctor
        public PcapNGReader(string path, bool swapBytes)             
        {
            CustomContract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            CustomContract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            Initialize(new FileStream(path, FileMode.Open), swapBytes);
        }

        public PcapNGReader(Stream stream, bool reverseByteOrder)             
        {
            CustomContract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");

            Initialize(stream, reverseByteOrder);
        }

        private void Initialize(Stream stream, bool reverseByteOrder)
        {
            CustomContract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
            CustomContract.Requires<Exception>(stream.CanRead == true, "cannot read stream");
            Action<Exception> ReThrowException = (exc) => 
            { 
                ExceptionDispatchInfo.Capture(exc).Throw(); 
            };
            this.ReverseByteOrder = reverseByteOrder;
            this.stream = stream;   
            binaryReader = new BinaryReader(stream);
            List<KeyValuePair<SectionHeaderBlock, List<InterfaceDescriptionBlock>>> preHeadersWithInterface = new List<KeyValuePair<SectionHeaderBlock, List<InterfaceDescriptionBlock>>>(); 
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && basePosition == 0)
            {
                AbstractBlock block = AbstractBlockFactory.ReadNextBlock(binaryReader, this.ReverseByteOrder, ReThrowException);
                if (block == null )
                    break;

                switch (block.BlockType)
                {
                    case BaseBlock.Types.SectionHeader:
                        if (block is SectionHeaderBlock)
                        {
                            SectionHeaderBlock headerBlock = block as SectionHeaderBlock;
                            preHeadersWithInterface.Add(new KeyValuePair<SectionHeaderBlock,List<InterfaceDescriptionBlock>>(headerBlock,new List<InterfaceDescriptionBlock>()));
                        }
                        break;
                    case BaseBlock.Types.InterfaceDescription:
                        if (block is InterfaceDescriptionBlock)
                        {
                            InterfaceDescriptionBlock interfaceBlock = block as InterfaceDescriptionBlock;
                            if (preHeadersWithInterface.Any())
                            {
                                preHeadersWithInterface.Last().Value.Add(interfaceBlock);
                            }
                            else
                            {
                                throw new Exception(string.Format("[PcapNgReader.Initialize] stream must contains SectionHeaderBlock before any InterfaceDescriptionBlock"));
                            }
                        }
                        break;
                    default:
                        basePosition = block.PositionInStream;
                        break;
                }     
            }
            if (basePosition <= 0)
                basePosition = binaryReader.BaseStream.Position;

            if(!preHeadersWithInterface.Any() )
                throw new ArgumentException(string.Format("[PcapNgReader.Initialize] Stream don't contains any SectionHeaderBlock"));

            if(!(from item in preHeadersWithInterface where (item.Value.Any()) select item).Any())
                throw new ArgumentException(string.Format("[PcapNgReader.Initialize] Stream don't contains any InterfaceDescriptionBlock"));               

            headersWithInterface = (from item in preHeadersWithInterface 
                                                where (item.Value.Any()) 
                                                select item)
                                                .Select(x => new HeaderWithInterfacesDescriptions(x.Key, x.Value))
                                                .ToList(); 
   
            Rewind();
        }
        #endregion

        #region methods
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// rewind to the beginning of the stream 
        /// </summary>
        private void Rewind()
        {
            lock (syncRoot)
            {
                binaryReader.BaseStream.Position = basePosition;
            }
        }

        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(binaryReader != null)
                binaryReader.Close();
            if (stream != null)
                stream.Close();
        } 

        public void ReadPackets(System.Threading.CancellationToken cancellationToken)
        {
            AbstractBlock block;
            long prevPosition = 0;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && !cancellationToken.IsCancellationRequested)
            {                   
                try
                {
                    lock (syncRoot)
                    {
                        prevPosition = binaryReader.BaseStream.Position;
                        block = AbstractBlockFactory.ReadNextBlock(binaryReader, this.ReverseByteOrder, OnException);
                    }

                    if (block == null)
                    {
                        throw new Exception(string.Format("[ReadPackets] AbstractBlockFactory cannot read packet on position {0}", prevPosition));
                       
                    }

                    switch(block.BlockType)
                    {
                        case BaseBlock.Types.EnhancedPacket:
                            {
                                EnchantedPacketBlock enchantedBlock = block as EnchantedPacketBlock;
                                if (enchantedBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to EnchantedPacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(enchantedBlock);
                            }
                            break;
                        case BaseBlock.Types.Packet:
                            {
                                PacketBlock packetBlock = block as PacketBlock;
                                if (packetBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to PacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(packetBlock);
                            }
                            break;
                        case BaseBlock.Types.SimplePacket:
                            {
                                SimplePacketBlock simpleBlock = block as SimplePacketBlock;
                                if (simpleBlock == null)
                                    throw new Exception(string.Format("[ReadPackets] system cannot cast block to SimplePacketBlock. Block start on position: {0}.", prevPosition));
                                else
                                    OnReadPacket(simpleBlock);
                            }
                            break;
                        default:
                            break;
                    } 
                }
                catch (Exception exc)
                {
                    OnException(exc);
                    lock (syncRoot)
                    {
                        if (prevPosition == binaryReader.BaseStream.Position)
                            break;
                    }
                    continue;
                }
            }
        }
       
        #endregion
    }
}
