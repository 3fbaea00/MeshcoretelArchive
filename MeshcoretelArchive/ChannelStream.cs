using System.Text;

public class ChannelStream : IDisposable
{
    public ChannelStream(string channelFilePath)
    {
        stream = File.Open(channelFilePath, FileMode.Open, FileAccess.ReadWrite);
        stream.Seek(0, SeekOrigin.End);
        writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
    }

    FileStream stream;
    StreamWriter writer;

    public void WriteLine(string line)
    {
        writer.WriteLine(line);
    }

    public void Dispose()
    {
        writer.Flush();
        writer.Dispose();
        stream.Dispose();
    }
}