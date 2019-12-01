using System;
using System.Collections.Generic;
using System.IO;
using Haukcode.PcapngUtils.Extensions;
using Haukcode.PcapngUtils.Common;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Haukcode.PcapngUtils.Pcap
{
    public sealed class PcapReader : Disposable, IReader
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
            CustomContract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
            CustomContract.Requires<ArgumentNullException>(packet != null, "packet cannot be null");
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
            CustomContract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            CustomContract.Requires<ArgumentException>(File.Exists(path), "file must exists");

            Initialize(new FileStream(path, FileMode.Open));
        }

        public PcapReader(Stream s)
        {
            CustomContract.Requires<ArgumentNullException>(s != null, "stream cannot be null");

            Initialize(s);
        }

        private void Initialize(Stream stream)
        {
            CustomContract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
            CustomContract.Requires<Exception>(stream.CanRead == true, "cannot read stream");

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
            uint secs, usecs, caplen, len;
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
                    PcapPacket packet = new PcapPacket((UInt64)secs, (UInt64)usecs, data, position);
                    OnReadPacket(packet);
                }
                catch (Exception exc)
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
            CustomContract.Requires<ArgumentNullException>(Header != null, "Header cannot be null");
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
