namespace LoginSystem.Api.Exceptions;

public class DuplicatedValueException : InvalidOperationException
{
    public DuplicatedValue ErrorCode;

    public DuplicatedValueException()
    {
    }

    public DuplicatedValueException(string message) : base(message)
    {
    }

    public DuplicatedValueException(string message, Exception inner) : base(message, inner)
    {
    }
}

public enum DuplicatedValue
{
    UserNumber,
    UserName,
    EmailAddress,
    MobileNumber
}
