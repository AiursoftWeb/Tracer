using Aiursoft.AiurObserver.WebSocket.Server;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.WebTools.Attributes;

namespace Aiursoft.Tracer.Controllers;

public class HomeController : Controller
{
    private const int Length = 1024 * 1024 * 1024; // 1G
    private static byte[]? _data;

    private static byte[] GetData()
    {
        if (_data != null) return _data;
        var random = new Random();

        _data = new byte[Length];
        random.NextBytes(_data);

        return _data;
    }

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
    public IActionResult Download()
    {
        HttpContext.Response.Headers.Append("Content-Length", Length.ToString());
        return new FileContentResult(GetData(), "application/octet-stream");
    }
}