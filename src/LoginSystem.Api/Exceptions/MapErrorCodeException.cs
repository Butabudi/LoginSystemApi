using LoginSystem.Api.Constants;

namespace LoginSystem.Api.Exceptions;

public class MapErrorCodeException
{
    public static string MapErrorCode(DuplicatedValue errorCode)
    {
        return errorCode switch
        {
            DuplicatedValue.UserNumber => ErrorCodes.UserNumberInUse,
            DuplicatedValue.EmailAddress => ErrorCodes.EmailAddressInUse,
            DuplicatedValue.MobileNumber => ErrorCodes.MobileNumberInUse,
            DuplicatedValue.UserName => ErrorCodes.UserNameInUse,
            _ => throw new ArgumentException($"Unexpected Duplicated Value {errorCode}", nameof(errorCode))
        };
    }
}




