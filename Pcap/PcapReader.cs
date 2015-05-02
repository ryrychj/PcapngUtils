using System;
using System.Collections.Generic;
using System.IO;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using PcapngUtils.Common;
using NUnit.Framework;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace PcapngUtils.Pcap
{
    [TestFixture]
    public sealed class PcapReader : Disposable, IReader
    {
        #region nUnitTest
        [TestCase(20)]
        [TestCase(170)]
        [TestCase(200)]
        [ExpectedException(typeof(EndOfStreamException))]
        public static void PcapReader_IncopletedFileStream_Test(int maxLength)
        {
            byte[] data = { 212, 195, 178, 161, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 0, 0, 1, 0, 0, 0, 89, 92, 28, 85, 58, 246, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 68, 109, 87, 125, 40, 18, 192, 74, 0, 154, 76, 44, 8, 0, 69, 0, 0, 110, 100, 55, 0, 0, 117, 17, 76, 144, 37, 157, 173, 13, 192, 168, 1, 101, 130, 165, 130, 165, 0, 90, 107, 107, 0, 25, 137, 153, 119, 253, 219, 183, 207, 74, 89, 213, 110, 239, 3, 75, 110, 227, 57, 128, 86, 105, 94, 91, 40, 2, 126, 2, 227, 250, 106, 221, 113, 98, 211, 229, 10, 134, 44, 193, 245, 77, 75, 238, 69, 78, 16, 195, 254, 113, 224, 43, 130, 205, 115, 131, 90, 245, 238, 164, 68, 27, 45, 26, 73, 234, 87, 155, 38, 207, 55, 185, 252, 116, 214, 9, 21, 191, 90, 47, 72, 237, 89, 92, 28, 85, 238, 252, 7, 0, 124, 0, 0, 0, 124, 0, 0, 0, 192, 74, 0, 154, 76, 44, 68, 109, 87, 125, 40, 18, 8, 0, 69, 0, 0, 110, 86, 139 };
            data = data.Take(maxLength).ToArray();
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (PcapReader reader = new PcapReader(stream))
                {
                    reader.OnReadPacketEvent += (context, packet) =>
                        {
                            IPacket ipacket = packet;
                        };
                    reader.OnExceptionEvent += (sender, exc) =>
                        {
                            ExceptionDispatchInfo.Capture(exc).Throw();
                        };
                    reader.ReadPackets(new System.Threading.CancellationToken());
                    var a = reader.Header;
                }
                
            }            
        }     
        
        #endregion

        #region event & delegate
        public event CommonDelegates.ExceptionEventDelegate OnExceptionEvent; 
        private void OnException(Exception exception)
        {
            Contract.Requires<ArgumentNullException>(exception != null, "exception cannot be null or empty");
            CommonDelegates.ExceptionEventDelegate handler = OnExceptionEvent;
            if (handler != null)
                handler(this, exception); 
            else
                ExceptionDispatchInfo.Capture(exception).Throw();
        }

        public event CommonDelegates.ReadPacketEventDelegate OnReadPacketEvent;
        private void OnReadPacket(IPacket packet)
        {
            Contract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
            Contract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
            CommonDelegates.ReadPacketEventDelegate handler = OnReadPacketEvent;
            if (handler != null)
                handler(Header, packet);
        }
        #endregion

        #region fields & properties
        private Stream stream;
        private BinaryReader binaryReader;
        public SectionHeader Header { get; private set; }
        private object syncRoot = new object();
        private long BasePosition = 0;
        #endregion

        #region ctor
        public PcapReader(string path)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            Initialize(new FileStream(path, FileMode.Open));
        }

        public PcapReader(Stream s)
        {
            Contract.Requires<ArgumentNullException>(s != null, "stream cannot be null");

            Initialize(s);
        }

        private void Initialize(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
            Contract.Requires<Exception>(stream.CanRead == true, "cannot read stream");

            this.stream = stream;
            binaryReader = new BinaryReader(stream);
            Header = SectionHeader.Parse(binaryReader);
            BasePosition = binaryReader.BaseStream.Position;
            Rewind();
        }
        #endregion

        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Read all packet from a stream. After read any packet event OnReadPacketEvent is called.
        /// Function is NOT asynchronous! (blocking thread). If you want abort it, use CancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void ReadPackets(System.Threading.CancellationToken cancellationToken)
        {
            uint secs, usecs,caplen,len;
            long position = 0;
            byte[] data;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    lock (syncRoot)
                    {
                        position = binaryReader.BaseStream.Position;
                        secs = binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
                        usecs = binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
                        if (Header.NanoSecondResolution)
                            usecs = usecs / 1000;
                        caplen = binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);
                        len = binaryReader.ReadUInt32().ReverseByteOrder(Header.ReverseByteOrder);

                        data = binaryReader.ReadBytes((int)caplen);
                        if (data.Length < caplen)
                            throw new EndOfStreamException("Unable to read beyond the end of the stream");
                    }
                    PcapPacket packet = new PcapPacket((UInt64)secs, (UInt64)usecs, data,position);
                    OnReadPacket(packet);
                }
                catch(Exception exc)
                {
                    OnException(exc);
                }
            }
        }

        /// <summary>
        /// rewind to the beginning of the stream 
        /// </summary>
        private void Rewind()
        {
            Contract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
            lock (syncRoot)
            {
                binaryReader.BaseStream.Position = this.BasePosition;
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (binaryReader != null)
                binaryReader.Close();
            if (stream != null)
                stream.Close();
        }

        #endregion

    }

       
}
