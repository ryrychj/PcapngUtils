using System;
using System.Collections.Generic;
using System.IO;
using PcapngUtils.Extensions;
using System.Diagnostics.Contracts;
using PcapngUtils.Common;
using System.Linq;
using System.Runtime.ExceptionServices;
namespace PcapngUtils.Pcap
{
    public sealed class PcapWriter : Disposable, IWriter
    {
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
        #endregion

        #region fields & properties
        private Stream stream;
        private BinaryWriter binaryWriter;
        private SectionHeader header = null;
        private object syncRoot = new object();
        #endregion

        #region ctor
        public PcapWriter(string path, bool nanoseconds = false, bool reverseByteOrder = false)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");
            SectionHeader sh = SectionHeader.CreateEmptyHeader(nanoseconds, reverseByteOrder);
            Initialize(new FileStream(path, FileMode.Create),sh);
        }

        public PcapWriter(string path, SectionHeader header)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(path), "path cannot be null or empty");
            Contract.Requires<ArgumentException>(!File.Exists(path), "file exists");
            Contract.Requires<ArgumentNullException>(header!=null, "SectionHeader cannot be null");
            
            Initialize(new FileStream(path, FileMode.Create),header);
        }

         private void Initialize(Stream stream, SectionHeader header)
         {                     
             Contract.Requires<ArgumentNullException>(stream != null, "stream cannot be null");
             Contract.Requires<Exception>(stream.CanWrite == true, "Cannot write to stream");
             Contract.Requires<ArgumentNullException>(header != null, "header cannot be null");
             this.header = header;              
             this.stream = stream;
             binaryWriter = new BinaryWriter(stream);
             binaryWriter.Write(header.ConvertToByte());            
         }
        #endregion

         /// <summary>
         /// Close stream, dispose members
         /// </summary>
        public void Close()
        {
            Dispose();
        }

        public void WritePacket(IPacket packet)
        {
            try
            {
                uint secs = (uint)packet.Seconds;
                uint usecs = (uint)packet.Microseconds;
                if (header.NanoSecondResolution)
                    usecs = usecs * 1000;
                uint caplen = (uint)packet.Data.Length;
                uint len = (uint)packet.Data.Length;
                byte[] data = packet.Data;

                List<byte> ret = new List<byte>();

                ret.AddRange(BitConverter.GetBytes(secs.ReverseByteOrder(header.ReverseByteOrder)));
                ret.AddRange(BitConverter.GetBytes(usecs.ReverseByteOrder(header.ReverseByteOrder)));
                ret.AddRange(BitConverter.GetBytes(caplen.ReverseByteOrder(header.ReverseByteOrder)));
                ret.AddRange(BitConverter.GetBytes(len.ReverseByteOrder(header.ReverseByteOrder)));
                ret.AddRange(data);
                if (ret.Count > header.MaximumCaptureLength)
                    throw new ArgumentOutOfRangeException(string.Format("[PcapWriter.WritePacket] packet length: {0} is greater than MaximumCaptureLength: {1}", ret.Count, header.MaximumCaptureLength));
                lock (syncRoot)
                {
                    binaryWriter.Write(ret.ToArray());
                }
            }
            catch (Exception exc)
            {
                OnException(exc);
            }
        }         


        #region IDisposable Members
        /// <summary>
        /// Close stream, dispose members
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (binaryWriter != null)
                binaryWriter.Close();
            if (stream != null)
                stream.Close();
        }

        #endregion      
    }  
}
