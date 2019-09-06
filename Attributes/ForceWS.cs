﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Tracer.Attributes
{
    public class ForceWS : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.HttpContext.WebSockets.IsWebSocketRequest)
            {
                throw new Exception();
            }
        }
    }
}
