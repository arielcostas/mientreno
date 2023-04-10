using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mientreno.Server.Helpers;

public class HttpExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpRequestException e)
        {
            var code = (int)(e.StatusCode ?? HttpStatusCode.InternalServerError);

            context.Result = new ObjectResult(e.Message)
            {
                StatusCode = code,
                Value = new ProblemDetails()
                {
                    Title = e.Message,
                    Status = code
                }
            };

            context.ExceptionHandled = true;
        }
    }
}
