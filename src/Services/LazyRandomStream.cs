namespace Aiursoft.Tracer.Services;

/// <summary>
/// This stream generates read-only random data with a limited size.
/// The data is generated lazily, meaning it is generated only once and only when needed.
/// </summary>
public class LazyRandomStream : Stream
{
    private const int BufferSize = 128 * 1024 * 1024; // 128MB buffer for random data
    private readonly Random _random = new();
    private readonly byte[] _buffer = new byte[BufferSize];
    private readonly long _streamSize;
    private long _bytesRead;

    public LazyRandomStream(long streamSize)
    {
        if (streamSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(streamSize), "Stream size must be non-negative.");
        }

        _streamSize = streamSize;
        _random.NextBytes(_buffer); // Generate random data once
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        if (offset < 0 || count < 0 || offset + count > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer));
        }

        // Determine how many bytes are still available to read
        var remainingBytes = _streamSize - _bytesRead;
        if (remainingBytes <= 0)
        {
            return 0; // No more data to read, return 0 to indicate end of stream
        }

        // Cap the read count by the remaining bytes and buffer size
        var bytesToRead = (int)Math.Min(Math.Min(count, remainingBytes), BufferSize);
        Array.Copy(_buffer, 0, buffer, offset, bytesToRead);

        // Update the total bytes read
        _bytesRead += bytesToRead;

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

    public override long Length => _streamSize; // Return the fixed stream size

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