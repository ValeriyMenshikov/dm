using HotChocolate;

namespace DM.Web.API.GraphQL;

/// <summary>
/// GraphQL error filter
/// </summary>
public sealed class ExceptionFilter : IErrorFilter
{
    /// <inheritdoc />
    public IError OnError(IError error)
    {
        return error.WithMessage(error.Exception?.Message ?? "Unknown error");
    }
}
