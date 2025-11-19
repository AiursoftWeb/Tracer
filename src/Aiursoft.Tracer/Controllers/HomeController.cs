using Aiursoft.AiurObserver.WebSocket.Server;
using Aiursoft.Tracer.Models.HomeViewModels;
using Aiursoft.Tracer.Services;
using Aiursoft.UiStack.Navigation;
using Aiursoft.WebTools;
using Aiursoft.WebTools.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Aiursoft.Tracer.Controllers;

[LimitPerMin]
public class HomeController : Controller
{
    [RenderInNavBar(
        NavGroupName = "Features",
        NavGroupOrder = 1,
        CascadedLinksGroupName = "Home",
        CascadedLinksIcon = "home",
        CascadedLinksOrder = 1,
        LinkText = "Network Quality Tester",
        LinkOrder = 1)]
    public IActionResult Index()
    {
        return this.StackView(new IndexViewModel());
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
}
