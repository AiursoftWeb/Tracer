using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Tracer.Controllers
{
    public class PingController : ControllerBase
    {
        [AiurNoCache]
        public IActionResult Index()
        {
            return Ok(new List<object>());
        }
    }
}
