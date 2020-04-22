using Aiursoft.SDK.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Tracer.Controllers
{
    public class PingController : Controller
    {
        [AiurNoCache]
        public JsonResult Index()
        {
            return Json(new List<object>());
        }
    }
}
