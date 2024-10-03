using LoginSystem.Api.Models.Request;
using LoginSystem.Domain;

namespace LoginSystem.Api.Helpers;

public class CheckUserDuplicateResultHelper
{
    // TODO: Implement this on a later stage
    public CheckUserDuplicate CheckUserDuplicateResult(CreateUserRequestDto request, LoginSystemDbContext dbContext)
    {
        var result = dbContext.Users.Any(
            x => x.EmailAddress == request.Email || x.UserName == request.UserName ||
                 x.MobileNumber == request.MobileNumber
        );

        return new CheckUserDuplicate
        {
            IsDuplicate = result,
            ErrorCode = result ? "Duplicate" : null
        };
    }
}
