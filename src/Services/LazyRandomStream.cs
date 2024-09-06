namespace Aiursoft.Tracer.Services;

/// <summary>
/// This stream is infinite read-only random data.
///
/// This stream is generated lazily, meaning that the data is generated only once and only when it is needed.
/// </summary>
public class LazyRandomStream : Stream
{
    private const int BufferSize = 128 * 1024 * 1024; // 128MB
    private readonly Random _random = new();
    private readonly byte[] _buffer = new byte[BufferSize];
    
    public LazyRandomStream()
    {
        _random.NextBytes(_buffer);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }
        var bytesToRead = Math.Min(count, BufferSize);
        Array.Copy(_buffer, 0, buffer, offset, bytesToRead);
        return bytesToRead;
    }
    
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;

    #region Not Supported
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override long Length => throw new NotSupportedException();
    
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
    #endregion
}