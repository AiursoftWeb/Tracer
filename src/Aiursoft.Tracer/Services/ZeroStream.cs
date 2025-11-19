namespace Aiursoft.Tracer.Services;

/// <summary>
/// This stream generates read-only zero data with a limited size.
/// All data is zero, and the size is specified at construction.
/// </summary>
public class ZeroStream : Stream
{
    private readonly long _streamSize;
    private long _bytesRead;

    public ZeroStream(long streamSize)
    {
        if (streamSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(streamSize), @"Stream size must be non-negative.");
        }

        _streamSize = streamSize;
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

        // Cap the read count by the remaining bytes
        var bytesToRead = (int)Math.Min(count, remainingBytes);

        // Fill the buffer with zeros
        Array.Clear(buffer, offset, bytesToRead);

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
