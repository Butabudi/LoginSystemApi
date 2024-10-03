using AutoMapper;
using LoginSystem.Api.Constants;
using LoginSystem.Api.Exceptions;
using LoginSystem.Api.Handlers.Responses;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain;
using LoginSystem.Domain.Models.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Api.Handlers.Commands;

public record DeleteIndividualUserCommand
    (DeleteIndividualUserRequest DeleteIndividualUserRequest) : IRequest<
        RequestHandlerResponse<DeleteIndividualUserResponse>>;

public class DeleteIndividualUserCommandHandler
    (LoginSystemDbContext dbContext, IMapper mapper, ILogger<DeleteIndividualUserRequest> logger) : IRequestHandler<
        DeleteIndividualUserCommand, RequestHandlerResponse<DeleteIndividualUserResponse>>
{
    public async Task<RequestHandlerResponse<DeleteIndividualUserResponse>> Handle(
        DeleteIndividualUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            x => x.UserId.ToString() == request.DeleteIndividualUserRequest.UserId,
            cancellationToken
        );

        if (user == null)
        {
            logger.LogError(
                "Requested UserId for {UserId} does not exist.",
                request.DeleteIndividualUserRequest.UserId
            );
            return new NotFoundException();
        }

        user.DateClosed = DateTime.Now;
        user.LastUpdated = DateTime.Now;
        user.UserStatus = UserStatus.Closed;
        user.UpdatedBy = ConstantValues.UpdatedBy;

        dbContext.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted individual user with ID {Id}.", user.Id);

        return new SuccessfulHandlerResponse<DeleteIndividualUserResponse>(
            mapper.Map<DeleteIndividualUserResponse>(user)
        );
    }
}
