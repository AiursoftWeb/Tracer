using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Tracer.Models;

namespace Aiursoft.Tracer.Controllers;

public class HomeController : Controller
{
    private const int Length = 1024 * 1024 * 1024; // 1G
    private static byte[]? _data;

    private readonly IPusher _pusher;

    public HomeController()
    {
        _pusher = new WebSocketPusher();
    }

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
    public async Task<IActionResult> Pushing()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest) return Json(new { });
        await _pusher.Accept(HttpContext);
        for (var i = 0; i < 36000 && _pusher.Connected; i++)
            try
            {
                _pusher.SendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff") + $"|{i + 1}").GetAwaiter();
                await Task.Delay(100);
            }
            catch
            {
                break;
            }

        return Json(new { });
    }

    [AiurNoCache]
    public IActionResult Download()
    {
        HttpContext.Response.Headers.Add("Content-Length", Length.ToString());
        return new FileContentResult(GetData(), "application/octet-stream");
    }
}