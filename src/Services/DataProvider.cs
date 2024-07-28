using Aiursoft.Scanner.Abstractions;

namespace Aiursoft.Tracer.Services;

public class DataProvider : ISingletonDependency
{
    private const int Length = 1024 * 1024 * 1024; // 1G
    private static readonly byte[] Data;

    // Static constructor to ensure data is initialized only once.
    static DataProvider()
    {
        Data = new byte[Length];
        var random = new Random();
        random.NextBytes(Data);
    }

    public byte[] GetData()
    {
        return Data;
    }

    public MemoryStream GetStreamData()
    {
        return new MemoryStream(Data, writable: false);
    }
}
