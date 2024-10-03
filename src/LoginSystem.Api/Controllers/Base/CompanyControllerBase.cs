using System.Diagnostics.Contracts;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace LoginSystem.Api.Controllers.Base;

public class CompanyControllerBase : ControllerBase
{
    /// <summary>
    /// Creates an <see cref="ObjectResult"/> that produces a <see cref="ProblemDetails"/> response.
    /// </summary>
    /// <param name="statusCode">The value for <see cref="ProblemDetails.Status" />.</param>
    /// <param name="detail">The value for <see cref="ProblemDetails.Detail" />.</param>
    /// <param name="instance">The value for <see cref="ProblemDetails.Instance" />.</param>
    /// <param name="title">The value for <see cref="ProblemDetails.Title" />.</param>
    /// <param name="type">The value for <see cref="ProblemDetails.Type" />.</param>
    /// <param name="errorCode">A code which can be used by the client to determine what error has occurred.</param>
    /// <param name="extensions">A dictionary of other extensions to pass back to the client.</param>
    /// <returns>The created <see cref="ObjectResult"/> for the response.</returns>
    /// <remarks>Note that if both the <paramref name="errorCode"/> parameter and a <c>errorCode</c> additional extension are provided, the parameter will win.</remarks>
    [NonAction]
    protected ObjectResult Problem(
        string? detail = null,
        string? instance = null,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? errorCode = null,
        IDictionary<string, object?>? extensions = null)
    {
        var problemDetails = CreateProblemDetails(detail, instance, statusCode, title, type);

        if (errorCode is not null)
        {
            problemDetails.Extensions.Add("errorCode", errorCode);
        }
        
        if (extensions is not null)
        {
            foreach (var extension in extensions)
            {
                problemDetails.Extensions.TryAdd(extension.Key, extension.Value);
            }
        }

        return new ObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json", "application/problem+xml" },
            StatusCode = problemDetails.Status
        };
    }

    /// <summary>
    /// Creates an <see cref="ObjectResult"/> that produces a <see cref="ProblemDetails"/> response for a validation error.
    /// </summary>
    /// <param name="validation">The validation result to use to build the response.</param>
    /// <returns>The created <see cref="ObjectResult"/> for the response.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="validation"/> is valid.</exception>
    [NonAction]
    protected ObjectResult ValidationError(ValidationResult validation)
    {
        const string errorCode = "VALIDATION_ERROR";
        const string errorTitle = "One or more validation errors occurred.";
        const string errorExtensionKey = "errors";

        if (validation.IsValid)
        {
            throw new ArgumentException("Validation result must be invalid to create an error response", nameof(validation));
        }

        var ext = new Dictionary<string, object?>
        {
            { errorExtensionKey, BuildErrorDictionary(validation) }
        };

        return Problem(title: errorTitle, errorCode: errorCode, extensions: ext,
            statusCode: StatusCodes.Status400BadRequest);
    }
    
    /// <summary>
    /// Helper method to build a dictionary of errors from a <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="validation">The validation result to build the dictionary from</param>
    /// <returns>An aggregated dictionary of all property names, with an array of validation error messages</returns>
    [NonAction]
    [Pure]
    private static IDictionary<string, ICollection<string>> BuildErrorDictionary(ValidationResult validation)
    {
        var errors = new Dictionary<string, ICollection<string>>();

        foreach (var error in validation.Errors)
        {
            var propName = error.PropertyName;
            if (errors.ContainsKey(propName))
            {
                errors[propName].Add(error.ErrorMessage);
            }
            else
            {
                errors[propName] = new List<string> { error.ErrorMessage };
            }
        }

        return errors;
    }

    /// <summary>
    /// Creates a <see cref="ProblemDetails" /> instance that configures defaults based on values specified
    /// </summary>
    /// <param name="detail"></param>
    /// <param name="instance"></param>
    /// <param name="statusCode"></param>
    /// <param name="title"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private ProblemDetails CreateProblemDetails(string? detail, string? instance, int? statusCode, string? title, string? type) =>
        // ProblemDetailsFactory may be null in unit testing scenarios. Improvise to make this more testable.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        ProblemDetailsFactory is null
            ? new ProblemDetails
            {
                Detail = detail,
                Instance = instance,
                Status = statusCode ?? StatusCodes.Status500InternalServerError,
                Title = title,
                Type = type,
            }
            : ProblemDetailsFactory.CreateProblemDetails(
                HttpContext,
                statusCode: statusCode ?? StatusCodes.Status500InternalServerError,
                title: title,
                type: type,
                detail: detail,
                instance: instance
            );
}
