using AutoMapper;
using LoginSystem.Api.Constants;
using LoginSystem.Api.Exceptions;
using LoginSystem.Api.Extensions;
using LoginSystem.Api.Handlers.Responses;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain;
using LoginSystem.Domain.Models;
using LoginSystem.Domain.Models.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Api.Handlers.Commands;

public record CreateUserCommand
    (CreateUserRequestDto CreateUserRequestDto) : IRequest<RequestHandlerResponse<CreateUserResponse>>;

public class CreateUserCommandHandler(
    LoginSystemDbContext dbContext,
    ILogger<CreateUserCommandHandler> logger,
    IMapper mapper
) : IRequestHandler<CreateUserCommand, RequestHandlerResponse<CreateUserResponse>>
{
    public async Task<RequestHandlerResponse<CreateUserResponse>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        if (dbContext.Users.Any(x => x.EmailAddress == request.CreateUserRequestDto.Email).Equals(true))
        {
            return new DuplicatedValueException("Duplicate email address found.")
            {
                ErrorCode = DuplicatedValue.EmailAddress
            };
        }

        if (dbContext.Users.Any(x => x.UserName == request.CreateUserRequestDto.UserName).Equals(true))
        {
            return new DuplicatedValueException("Duplicate username found.")
            {
                ErrorCode = DuplicatedValue.UserName
            };
        }

        if (dbContext.Users.Any(x => x.MobileNumber == request.CreateUserRequestDto.MobileNumber).Equals(true))
        {
            return new DuplicatedValueException("Duplicate mobile number found.")
            {
                ErrorCode = DuplicatedValue.MobileNumber
            };
        }

        var user = mapper.Map<User>(request.CreateUserRequestDto);
        var initialNumber = await dbContext.Users.MaxAsync(x => (int?)x.UserId, cancellationToken) ?? 0;
        var userNumber = Math.Max(initialNumber, 0001000) + 1;

        var newUser = new User
        {
            UserId = userNumber,
            UserName = user.UserName,
            Password = PasswordHashExtensions.PasswordHash(user.Password),
            EmailAddress = user.EmailAddress,
            MobileNumber = user.MobileNumber,
            DateCreated = DateTime.Now,
            UserStatus = UserStatus.Pending, // TODO: because it needs to activate in mobile and/or email address
            UpdatedBy = ConstantValues.UpdatedBy
        };

        await dbContext.Users.AddAsync(newUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SuccessfulHandlerResponse<CreateUserResponse>(mapper.Map<CreateUserResponse>(newUser));
    }
}
