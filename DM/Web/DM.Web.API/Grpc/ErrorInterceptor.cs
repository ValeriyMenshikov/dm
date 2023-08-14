using System;
using System.Net;
using System.Threading.Tasks;
using DM.Services.Core.Exceptions;
using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace DM.Web.API.Grpc;

/// <summary>
/// gRPC error interceptor
/// </summary>
public sealed class ErrorInterceptor : Interceptor
{
    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await base.UnaryServerHandler(request, context, continuation);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, exception.Message));
        }
        catch (ValidationException exception)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, exception.Message));
        }
        catch (HttpException exception)
        {
            var code = exception.StatusCode switch
            {
                HttpStatusCode.BadRequest => StatusCode.FailedPrecondition,
                HttpStatusCode.Unauthorized => StatusCode.Unauthenticated,
                HttpStatusCode.PaymentRequired => StatusCode.FailedPrecondition,
                HttpStatusCode.Forbidden => StatusCode.Unauthenticated,
                HttpStatusCode.NotFound => StatusCode.NotFound,
                HttpStatusCode.MethodNotAllowed => StatusCode.FailedPrecondition,
                HttpStatusCode.NotAcceptable => StatusCode.FailedPrecondition,
                HttpStatusCode.ProxyAuthenticationRequired => StatusCode.Unauthenticated,
                HttpStatusCode.RequestTimeout => StatusCode.DeadlineExceeded,
                HttpStatusCode.PreconditionFailed => StatusCode.FailedPrecondition,
                _ => StatusCode.Internal
            };
            throw new RpcException(new Status(code, exception.Message));
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, exception.Message));
        }
    }
}
