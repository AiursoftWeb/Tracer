using Aiursoft.AiurObserver.WebSocket.Server;
using Aiursoft.WebTools;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.WebTools.Attributes;
using Microsoft.Net.Http.Headers;
using Aiursoft.Tracer.Services;

namespace Aiursoft.Tracer.Controllers;

public class HomeController(DataProvider data) : Controller
{
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
    [Route("download.dat")]
    public IActionResult Download()
    {
        var stream = data.GetStreamData();
        var response = HttpContext.Response;
        response.Headers[HeaderNames.ContentDisposition] = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = "download.dat"
        }.ToString();
        response.Headers[HeaderNames.ContentType] = "application/octet-stream";
        response.Headers[HeaderNames.ContentLength] = stream.Length.ToString();
        response.Headers[HeaderNames.AcceptRanges] = "bytes";
        response.StatusCode = StatusCodes.Status200OK;

        return new FileStreamResult(stream, "application/octet-stream")
        {
            EnableRangeProcessing = true,
            FileDownloadName = "download.dat"
        };
    }
}