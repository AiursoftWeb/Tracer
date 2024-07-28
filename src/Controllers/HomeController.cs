using Aiursoft.AiurObserver.WebSocket.Server;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.WebTools.Attributes;

namespace Aiursoft.Tracer.Controllers;

public class HomeController(IConfiguration configuration) : Controller
{
    private const int Length = 1024 * 1024 * 1024; // 1G
    private static byte[]? _data;
    private readonly string _workspaceFolder = configuration["Storage:Path"]!;
    
    private async Task<string> GetTempFileAsync()
    {
        var tempFile = Path.Combine(_workspaceFolder, "temp.dat");
        if (!Directory.Exists(_workspaceFolder))
        {
            Directory.CreateDirectory(_workspaceFolder);
        }
        
        if (!System.IO.File.Exists(tempFile))
        {
            await System.IO.File.WriteAllBytesAsync(tempFile, GetData());
        }

        return tempFile;
    }
    
    private static byte[] GetData()
    {
        if (_data != null) return _data;
        var random = new Random();

        _data = new byte[Length];
        random.NextBytes(_data);

        return _data;
    }

    [LimitPerMin(10)]
    public IActionResult Index()
    {
        return View();
    }

    [AiurNoCache]
    [EnforceWebSocket]
    public async Task Pushing()
    {
        var pusher = await HttpContext.AcceptWebSocketClient();
        while (pusher.Connected)
        {
            _ = Task.Run(() => pusher.Send(DateTime.UtcNow.ToHtmlDateTime()));
            await Task.Delay(100);
        }
    }

    [AiurNoCache]
    public async Task<IActionResult> Download()
    {
        return PhysicalFile(
            physicalPath: await GetTempFileAsync(),
            contentType: "application/octet-stream",
            fileDownloadName: "temp.dat",
            enableRangeProcessing: true
        );
    }
}