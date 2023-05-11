using System.Collections.Generic;
using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Tracer.Controllers;

public class PingController : ControllerBase
{
    [AiurNoCache]
    public IActionResult Index()
    {
        return Ok(new List<object>());
    }
}