using LoginSystem.Api.Handlers.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LoginSystem.Api.Extensions;

public static class RequestHandlerResponseExtensions
{
    public static IActionResult ToActionResult<T>(this RequestHandlerResponse<T> response,
        Func<T, IActionResult> success, Func<Exception, IActionResult> failure)
    {
        return response.IsSuccess ? success(response.Item) : failure(response.Exception);
    }
}