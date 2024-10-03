using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace LoginSystem.Api.Handlers.Responses;

public class SuccessfulHandlerResponse<T>(T item) : RequestHandlerResponse<T>(item)
{
}

public class FailedHandlerResponse<T>(Exception exception) : RequestHandlerResponse<T>(exception)
{
}

public abstract class RequestHandlerResponse<T>
{
	protected RequestHandlerResponse(Exception exception)
    {
        IsSuccess = false;
        Exception = exception;
    }

    protected RequestHandlerResponse(T item)
    {
        IsSuccess = true;
        Item = item;
    }

    [MemberNotNullWhen(true, nameof(Item))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool IsSuccess { get; private init; }

    public Exception? Exception { get; private init; }

    public T? Item { get; private init; }

    [Pure]
    public static implicit operator RequestHandlerResponse<T>(T value)
    {
        return new SuccessfulHandlerResponse<T>(value);
    }

    [Pure]
    public static implicit operator RequestHandlerResponse<T>(Exception exception)
    {
        return new FailedHandlerResponse<T>(exception);
    }
}
