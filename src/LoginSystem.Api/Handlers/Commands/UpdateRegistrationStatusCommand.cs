using AutoMapper;
using LoginSystem.Api.Exceptions;
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
            x => x.UserId == request.UpdateRegistrationStatusRequest.UserId,
            cancellationToken
        );

        if (user == null)
        {
            logger.LogError("Requested UserId for {UserId} does not exist.", user.UserId);
            return new NotFoundException();
        }

        // mapper is more cleaner
        user.UserStatus = request.UpdateRegistrationStatusRequest.UserStatus;
        user.LastUpdated = DateTime.Now;
        user.UpdatedBy = request.UpdateRegistrationStatusRequest.UpdatedBy;

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Status updated for user id {UserId} with user status of {Status}.",
            user.UserId,
            user.UserStatus
        );

        return new SuccessfulHandlerResponse<UpdateRegistrationStatusResponse>(
            mapper.Map<UpdateRegistrationStatusResponse>(user)
        );
    }
}
