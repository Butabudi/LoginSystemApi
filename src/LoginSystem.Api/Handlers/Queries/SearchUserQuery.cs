using AutoMapper;
using LoginSystem.Api.Exceptions;
using LoginSystem.Api.Handlers.Commands;
using LoginSystem.Api.Handlers.Responses;
using LoginSystem.Api.Models.Request;
using LoginSystem.Api.Models.Response;
using LoginSystem.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoginSystem.Api.Handlers.Queries;

public record SearchUserQuery
    (SearchUserRequestDto SearchUserRequestDto) : IRequest<RequestHandlerResponse<SearchUserResponse>>;

public class SearchUserQueryHandler(
    LoginSystemDbContext dbContext,
    ILogger<CreateUserCommandHandler> logger,
    IMapper mapper
) : IRequestHandler<SearchUserQuery, RequestHandlerResponse<SearchUserResponse>>
{
    public async Task<RequestHandlerResponse<SearchUserResponse>> Handle(
        SearchUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = int.Parse(request.SearchUserRequestDto.UserId);

        var result = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (result == null)
        {
            return new NotFoundException();
        }

        var response = mapper.Map<SearchUserResponse>(result);

        return response;
    }
}
