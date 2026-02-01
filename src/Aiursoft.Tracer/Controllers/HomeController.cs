using Aiursoft.AiurObserver.WebSocket.Server;
using Aiursoft.Tracer.Models.HomeViewModels;
using Aiursoft.Tracer.Services;
using Aiursoft.UiStack.Navigation;
using Aiursoft.WebTools;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Aiursoft.Tracer.Controllers;

public class HomeController(IpGeolocationService ipGeolocationService) : Controller
{
    [LimitPerMin]
    [RenderInNavBar(
        NavGroupName = "Features",
        NavGroupOrder = 1,
        CascadedLinksGroupName = "Home",
        CascadedLinksIcon = "home",
        CascadedLinksOrder = 1,
        LinkText = "Network Quality Tester",
        LinkOrder = 1)]
    public async Task<IActionResult> Index()
    {
        var model = new IndexViewModel();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrWhiteSpace(ip))
        {
            var location = await ipGeolocationService.GetLocationAsync(ip);
            if (location != null)
            {
                model.CountryName = location.Value.CountryName;
                model.CountryCode = location.Value.CountryCode;
            }
        }
        return this.StackView(model);
    }

    [LimitPerMin]
    [RenderInNavBar(
        NavGroupName = "Features",
        NavGroupOrder = 1,
        CascadedLinksGroupName = "Home",
        CascadedLinksIcon = "home",
        CascadedLinksOrder = 1,
        LinkText = "Self host your own server",
        LinkOrder = 1)]
    public IActionResult Selfhost()
    {
        return this.StackView(new SelfhostViewModel());
    }

    [LimitPerMin]
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
    [Route("ip")]
    public IActionResult Ip()
    {
        return Content(HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty, "text/plain");
    }

    [AiurNoCache]
    [Route("download.dat")]
    public IActionResult Download()
    {
        var streamSize = 0x100000000; // 4GB
        var stream = new ZeroStream(streamSize);
        var response = HttpContext.Response;
        response.Headers[HeaderNames.ContentDisposition] = new ContentDispositionHeaderValue("attachment")
        {
            FileNameStar = "download.dat"
        }.ToString();
        response.Headers[HeaderNames.ContentType] = "application/octet-stream";
        response.Headers[HeaderNames.ContentLength] = streamSize.ToString();
        response.Headers[HeaderNames.AcceptRanges] = "bytes";

        return new FileStreamResult(stream, "application/octet-stream")
        {
            EnableRangeProcessing = true,
            FileDownloadName = "download.dat"
        };
    }

    [AiurNoCache]
    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> Upload()
    {
        await Request.Body.CopyToAsync(Stream.Null);
        return Ok();
    }
}
