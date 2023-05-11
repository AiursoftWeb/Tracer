using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tracer.Services;

namespace Tracer.Models;

public class WebSocketPusher : IPusher
{
    private WebSocket? _ws;
    public bool Connected => _ws?.State == WebSocketState.Open;

    public async Task Accept(HttpContext context)
    {
        _ws = await context.WebSockets.AcceptWebSocketAsync();
    }

    public async Task SendMessage(string message)
    {
        if (_ws == null) throw new Exception("WebSocket not connected!");
        await _ws.SendMessage(message);
    }
}