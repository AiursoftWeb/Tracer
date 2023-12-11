using Microsoft.AspNetCore.Mvc;
using Aiursoft.WebTools.Services;
using Aiursoft.WebTools.Attributes;

namespace Aiursoft.Tracer.Controllers;

public class HomeController : Controller
{
    private const int Length = 1024 * 1024 * 1024; // 1G
    private static byte[]? _data;

    private static byte[] GetData()
    {
        if (_data == null)
        {
            var random = new Random();

            _data = new byte[Length];
            random.NextBytes(_data);
        }

        return _data;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AiurNoCache]
    public async Task Pushing()
    {
        var pusher = await HttpContext.AcceptWebSocketClient();
        for (var i = 0; pusher.Connected; i++)
        {
            _ = Task.Run(() => pusher.Send(DateTime.UtcNow.ToString() + $"|{i + 1}"));
            await Task.Delay(100);
        }
    }

    [AiurNoCache]
    public IActionResult Download()
    {
        HttpContext.Response.Headers.Add("Content-Length", Length.ToString());
        return new FileContentResult(GetData(), "application/octet-stream");
    }
}