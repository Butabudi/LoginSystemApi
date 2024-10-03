using AutoMapper;
using LoginSystem.Api.Handlers.Commands;
using LoginSystem.Api.Handlers.Responses;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Api.Handlers.Queries;

public record ListUserQuery : IRequest<RequestHandlerResponse<IEnumerable<SearchUserResponse>>>;

public class ListUsersQueryHandler(
    LoginSystemDbContext dbContext,
    ILogger<CreateUserCommandHandler> logger,
    IMapper mapper
) : IRequestHandler<ListUserQuery, RequestHandlerResponse<IEnumerable<SearchUserResponse>>>
{
    public async Task<RequestHandlerResponse<IEnumerable<SearchUserResponse>>> Handle(
        ListUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await dbContext.Users.ToListAsync(cancellationToken);

        return result.Select(mapper.Map<SearchUserResponse>).OrderBy(x => x.UserId).ToList();
    }
}
