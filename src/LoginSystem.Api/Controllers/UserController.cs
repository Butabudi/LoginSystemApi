using AutoMapper;
using FluentValidation;
using LoginSystem.Api.Constants;
using LoginSystem.Api.Controllers.Base;
using LoginSystem.Api.Exceptions;
using LoginSystem.Api.Extensions;
using LoginSystem.Api.Handlers.Commands;
using LoginSystem.Api.Handlers.Queries;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoginSystem.Api.Controllers;

[Route("[controller]")]
[AllowAnonymous]
[ApiController]
public class UserController(IMediator mediator, ILogger<UserController> logger, IMapper mapper) : CompanyControllerBase
{
    /// <summary>
    ///     Create new user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="validator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateUserResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Post(
        [FromBody] CreateUserRequestDto request,
        [FromServices] IValidator<CreateUserRequestDto> validator,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return ValidationError(validationResult);
        }

        var result = await mediator.Send(new CreateUserCommand(request), cancellationToken);

        return result.ToActionResult(
            item => Created(nameof(Post), item),
            exception => exception switch
            {
                DuplicatedValueException duplicatedValueException => Problem(
                    result.Exception?.Message,
                    errorCode: MapErrorCodeException.MapErrorCode(duplicatedValueException.ErrorCode),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                NotFoundException => Problem(
                    result.Exception?.Message,
                    errorCode: ErrorCodes.NoSuchOrganisationError,
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(result.Exception?.Message)
            }
        );
    }

    /// <summary>
    ///     Retrieve user by user id
    /// </summary>
    /// <returns></returns>
    [HttpGet("Search")]
    public async Task<IActionResult> Search(
        [FromQuery] SearchUserRequestDto request,
        [FromServices] IValidator<SearchUserRequestDto> validator
    )
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationError(validationResult);
        }

        var result = await mediator.Send(new SearchUserQuery(request));

        return result.ToActionResult(
            Ok,
            exception => exception switch
            {
                NotFoundException => NotFound(),
                _ => Problem(exception.Message)
            }
        );
    }

    /// <summary>
    ///     Retrieve all users
    /// </summary>
    /// <param name="userNumber"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> Get()
    {
        var result = await mediator.Send(new ListUserQuery());

        return result.ToActionResult(
            Ok,
            exception => exception switch
            {
                NotFoundException => NotFound(),
                _ => Problem(exception.Message)
            }
        );
    }

    /// <summary>
    ///     Delete a user
    /// </summary>
    /// <returns></returns>
    // TODO : still need some work
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteIndividualUserRequest request,
        [FromServices] IValidator<DeleteIndividualUserRequest> validator
    )
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationError(validationResult);
        }

        var result = await mediator.Send(new DeleteIndividualUserCommand(request));

        return result.ToActionResult(
            Ok,
            exception => exception switch
            {
                NotFoundException => Problem(
                    result.Exception?.Message,
                    errorCode: ErrorCodes.NotFoundError,
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(result.Exception?.Message)
            }
        );
    }


    /// <summary>
    ///     Delete pending users that stalled beyond the expected time frame.
    /// </summary>
    /// <returns></returns>
    // TODO : still need some work
    [HttpDelete("DeleteOverdueApplication")]
    public async Task<IActionResult> DeletePendingUser()
    {
        return null;
    }

    // TODO : still need some work

    /// <summary>
    ///     Update password of user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="validator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateRegistrationStatusRequest request,
        [FromServices] IValidator<UpdateRegistrationStatusRequest> validator,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return ValidationError(validationResult);
        }

        var result = await mediator.Send(new UpdateRegistrationStatusCommand(request));

        return result.ToActionResult(
            Ok,
            exception => exception switch
            {
                DuplicatedValueException duplicatedValueException => Problem(
                    result.Exception?.Message,
                    errorCode: MapErrorCodeException.MapErrorCode(duplicatedValueException.ErrorCode),
                    statusCode: StatusCodes.Status400BadRequest
                ),
                NotFoundException => Problem(
                    result.Exception?.Message,
                    errorCode: ErrorCodes.NotFoundError,
                    statusCode: StatusCodes.Status400BadRequest
                ),
                _ => Problem(result.Exception?.Message)
            }
        );
    }
}
