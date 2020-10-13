# PcapngUtils  [![NuGet Version](http://img.shields.io/nuget/v/Haukcode.PcapngUtils.svg?style=flat)](https://www.nuget.org/packages/Haukcode.PcapngUtils/)

C# full managed implementation Pcap/PcapNG file format
<h2><a id="user-content-the-nuget-package--" class="anchor" href="#the-nuget-package--" aria-hidden="true"><span class="octicon octicon-link"></span></a>The nuget package  <a href="https://www.nuget.org/packages/PcapngUtils/"><img src="https://dl.dropboxusercontent.com/u/75969946/Download/PcapNGUtils/v1.0.7.svg" alt="NuGet Status"  style="max-width:100%;"></a></h2>

<pre><code>PM&gt; Install-Package PcapngUtils 
</code></pre>

<h2>Description</h2>
Pcap and PcapNG are file formats used to store dumps of network traffic. There formats are described in:
* Pcap: https://wiki.wireshark.org/Development/LibpcapFileFormat
* Pcap Next Generation: https://www.winpcap.org/ntar/draft/PCAP-DumpFileFormat.html


The implementation of these formats is made by wrapping unmanaged WinPcap library.
I added the implementation of both formats in a fully managed C #.

<h2>Usage</h2>
<h4>Open Pcap file</h4>
<pre><code>
public void OpenPcapFile(string filename,CancellationToken token)
{
  using (var reader = new PcapReader(filename))
  {
    reader.OnReadPacketEvent += reader_OnReadPacketEvent;
    reader.ReadPackets(token);
    reader.OnReadPacketEvent -= reader_OnReadPacketEvent;
  }
}  

void reader_OnReadPacketEvent(object context, IPacket packet)
{
  Console.WriteLine(string.Format("Packet received {0}.{1}",packet.Seconds, packet.Microseconds ));
}
</code></pre>
<h4>Open PcapNG file</h4>
<pre><code>
public void OpenPcapNGFile(string filename,bool swapBytes,CancellationToken token)
{
  using (var reader = new PcapNGReader("test.pcap",swapBytes))
  {
    reader.OnReadPacketEvent += reader_OnReadPacketEvent;
    reader.ReadPackets(token);
    reader.OnReadPacketEvent -= reader_OnReadPacketEvent;
  }
}  

void reader_OnReadPacketEvent(object context, IPacket packet)
{
  Console.WriteLine(string.Format("Packet received {0}.{1}",packet.Seconds, packet.Microseconds ));
}
</code></pre>
<h4>Open Pcap/PcapNG file</h4>
Better solutions, library can recognize the file format,
<pre><code>
public void OpenPcapORPcapNFFile(string filename,CancellationToken token)
{
  using (var reader = IReaderFactory.GetReader(filename))
  {
    reader.OnReadPacketEvent += reader_OnReadPacketEvent;
    reader.ReadPackets(token);
    reader.OnReadPacketEvent -= reader_OnReadPacketEvent;
  }
}  

void reader_OnReadPacketEvent(object context, IPacket packet)
{
  Console.WriteLine(string.Format("Packet received {0}.{1}",packet.Seconds, packet.Microseconds ));
}
</code></pre>
<h4>Read packages and save to Pcap file</h4>
<pre><code>
public void CloneFile(string inputFileName, string outputFileName, CancellationToken token)
{
  using (var reader = IReaderFactory.GetReader(inputFileName))
  {
    using (var writer = new PcapWriter(outputFileName))
    {
      CommonDelegates.ReadPacketEventDelegate handler = (obj, packet) =>
      {
        writer.WritePacket(packet);
      };
      reader.OnReadPacketEvent += handler;
      reader.ReadPackets(token);
      reader.OnReadPacketEvent -= handler; 
    }                
  }
}
</code></pre>
<h4>Read packages and save to PcapNG file</h4>
<pre><code>
public void CloneFile(string inputFileName, string outputFileName, CancellationToken token)
{
  using (var reader = IReaderFactory.GetReader(inputFileName))
  {
    using (var writer = new PcapNGWriter(outputFileName))
    {
      CommonDelegates.ReadPacketEventDelegate handler = (obj, packet) =>
      {
        writer.WritePacket(packet);
      };
      reader.OnReadPacketEvent += handler;
      reader.ReadPackets(token);
      reader.OnReadPacketEvent -= handler; 
    }                
  }
}
</code></pre>
