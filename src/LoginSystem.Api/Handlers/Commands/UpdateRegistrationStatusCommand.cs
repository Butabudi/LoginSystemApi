using AutoMapper;
using LoginSystem.Api.Exceptions;
using LoginSystem.Api.Extensions;
using LoginSystem.Api.Handlers.Responses;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Api.Handlers.Commands;

public record UpdateRegistrationStatusCommand
    (UpdateRegistrationStatusRequest UpdateRegistrationStatusRequest) : IRequest<
        RequestHandlerResponse<UpdateRegistrationStatusResponse>>;

public class UpdateRegistrationStatusCommandHandler(
    LoginSystemDbContext dbContext,
    ILogger<UpdateRegistrationStatusCommand> logger,
    IMapper mapper
) : IRequestHandler<UpdateRegistrationStatusCommand, RequestHandlerResponse<UpdateRegistrationStatusResponse>>
{
    public async Task<RequestHandlerResponse<UpdateRegistrationStatusResponse>> Handle(
        UpdateRegistrationStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            x => x.UserName == request.UpdateRegistrationStatusRequest.UserName,
            cancellationToken
        );

        if (user == null)
        {
            logger.LogError(
                "Requested UserName for {UserName} or email address {EmailAddress} does not exist.",
                user.UserName,
                user.EmailAddress
            );
            return new NotFoundException();
        }

        // mapper is more cleaner
        user.UserStatus = request.UpdateRegistrationStatusRequest.UserStatus;
        user.LastUpdated = DateTime.Now;
        user.UpdatedBy = request.UpdateRegistrationStatusRequest.UpdatedBy;
        user.Password = PasswordHashExtensions.PasswordHash(request.UpdateRegistrationStatusRequest.Password);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Status updated for username {UserName} with user status of {Status}.",
            user.UserName,
            user.UserStatus
        );

        return new SuccessfulHandlerResponse<UpdateRegistrationStatusResponse>(
            mapper.Map<UpdateRegistrationStatusResponse>(user)
        );
    }
}
