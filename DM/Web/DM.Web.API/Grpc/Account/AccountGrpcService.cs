using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.Users.Reading;
using DM.Services.Community.BusinessProcesses.Users.Updating;
using DM.Web.Api.Grpc.Account;
using DM.Web.API.Services.Common;
using DM.Web.API.Services.Users;
using DM.Web.Core.Authentication.Credentials;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace DM.Web.API.Grpc.Account;

/// <summary>
/// Account GRPC service
/// </summary>
public sealed class AccountGrpcService : AccountService.AccountServiceBase
{
    private readonly ILoginApiService loginService;
    private readonly ICredentialsStorage credentialsStorage;
    private readonly IAuthenticationService authenticationService;
    private readonly IRegistrationApiService registrationService;
    private readonly IUserReadingService userService;
    private readonly IActivationApiService activationService;
    private readonly IEmailChangeApiService emailChangeService;
    private readonly IPasswordResetApiService passwordService;
    private readonly IUserProfileUpdateService userProfileUpdateService;
    private readonly IMapper mapper;

    /// <summary>
    /// Account GRPC service constructor
    /// </summary>
    public AccountGrpcService(
        ILoginApiService loginService,
        ICredentialsStorage credentialsStorage,
        IAuthenticationService authenticationService,
        IRegistrationApiService registrationService,
        IUserReadingService userService,
        IActivationApiService activationService,
        IEmailChangeApiService emailChangeService,
        IPasswordResetApiService passwordService,
        IUserProfileUpdateService userProfileUpdateService,
        IMapper mapper)
    {
        this.loginService = loginService;
        this.credentialsStorage = credentialsStorage;
        this.authenticationService = authenticationService;
        this.registrationService = registrationService;
        this.userService = userService;
        this.activationService = activationService;
        this.emailChangeService = emailChangeService;
        this.passwordService = passwordService;
        this.userProfileUpdateService = userProfileUpdateService;
        this.mapper = mapper;
    }

    /// <summary>
    /// Login
    /// </summary>
    public override async Task<LoginResponse> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        var user = await loginService.Login(mapper.Map<LoginCredentials>(request), httpContext);
        var credentials = await credentialsStorage.ExtractTokenFromResponse(httpContext);
        return new LoginResponse
        {
            User = mapper.Map<UserEnvelope>(user),
            Token = credentials.Token,
        };
    }

    /// <summary>
    /// Logout
    /// </summary>
    public override async Task<Empty> Logout(
        LogoutRequest request,
        ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        await authenticationService.Authenticate(request.Token, httpContext);
        await loginService.Logout(httpContext);
        return new Empty();
    }

    /// <summary>
    /// Logout elsewhere
    /// </summary>
    public override async Task<Empty> LogoutAll(
        LogoutAllRequest request,
        ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        await authenticationService.Authenticate(request.Token, httpContext);
        await loginService.LogoutAll(httpContext);
        return new Empty();
    }

    /// <summary>
    /// Get current account
    /// </summary>
    public override async Task<GetCurrentAccountResponse> GetCurrentAccount(
        GetCurrentAccountRequest request,
        ServerCallContext context)
    {
        await authenticationService.Authenticate(request.Token, context.GetHttpContext());
        var currentAccount = await loginService.GetCurrent();
        return new GetCurrentAccountResponse
        {
            User = mapper.Map<UserDetailsEnvelope>(currentAccount)
        };
    }

    /// <summary>
    /// Get accounts (with pagination)
    /// </summary>
    public override async Task<GetAccountsResponse> GetAccounts(
        GetAccountsRequest request,
        ServerCallContext context)
    {
        var paging = mapper.Map<DM.Services.Core.Dto.PagingQuery>(request.Paging);

        var result = await userService.Get(
            paging ?? DM.Services.Core.Dto.PagingQuery.Default,
            request.WithInactive);

        return new GetAccountsResponse
        {
            Paging = mapper.Map<PagingResult>(result.paging),
            Accounts =
            {
                mapper.Map<IEnumerable<User>>(result.users)
            }
        };
    }

    /// <summary>
    /// Get all accounts from server stream
    /// </summary>
    public override async Task GetAccountsServerStream(
        Empty request,
        IServerStreamWriter<User> responseStream,
        ServerCallContext context)
    {
        await foreach (var user in userService.GetUsersAsyncEnumerable())
        {
            await responseStream.WriteAsync(mapper.Map<User>(user));
        }
    }

    /// <summary>
    /// Get users by logins duplex stream
    /// </summary>
    public override async Task GetAccountsByLoginDuplexStream(
        IAsyncStreamReader<GetAccountsByLoginRequest> requestStream,
        IServerStreamWriter<GetAccountsByLoginResponse> responseStream,
        ServerCallContext context)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            try
            {
                var user = await userService.Get(request.Login);
                await responseStream.WriteAsync(new GetAccountsByLoginResponse
                {
                    Login = request.Login,
                    User = mapper.Map<User>(user),
                });
            }
            catch (Exception exception)
            {
                await responseStream.WriteAsync(new GetAccountsByLoginResponse
                {
                    Login = request.Login,
                    Error = exception.Message
                });
            }
        }
    }

    /// <summary>
    /// Register account
    /// </summary>
    public override async Task<RegisterAccountResponse> RegisterAccount(
        RegisterAccountRequest request,
        ServerCallContext context)
    {
        var registration = mapper.Map<Dto.Users.Registration>(request);
        var userId = await registrationService.Register(registration);

        return new RegisterAccountResponse
        {
            Id = userId.ToString(),
            Login = registration.Login,
        };
    }

    /// <summary>
    /// Register accounts from client stream
    /// </summary>
    public override async Task<RegisterAccountClientStreamResponse> RegisterAccountClientStream(
        IAsyncStreamReader<RegisterAccountRequest> requestStream,
        ServerCallContext context)
    {
        var results = new List<RegisterAccountClientStreamResponse.Types.Result>();
        await foreach (var request in requestStream.ReadAllAsync())
        {
            var registration = mapper.Map<Dto.Users.Registration>(request);
            try
            {
                var userId = await registrationService.Register(registration);
                results.Add(new RegisterAccountClientStreamResponse.Types.Result
                {
                    Login = registration.Login,
                    Id = userId.ToString(),
                });
            }
            catch (Exception exception)
            {
                results.Add(new RegisterAccountClientStreamResponse.Types.Result
                {
                    Login = registration.Login,
                    Error = exception.Message,
                });
            }
        }

        return new RegisterAccountClientStreamResponse
        {
            Results = { results }
        };
    }

    /// <summary>
    /// Activate account
    /// </summary>
    public override async Task<ActivateAccountResponse> ActivateAccount(
        ActivateAccountRequest request,
        ServerCallContext context)
    {
        var user = await activationService.Activate(GetGuid(request.ActivationToken));

        return new ActivateAccountResponse
        {
            User = mapper.Map<UserEnvelope>(user)
        };
    }

    /// <summary>
    /// Change account email
    /// </summary>
    public override async Task<ChangeAccountEmailResponse> ChangeAccountEmail(
        ChangeAccountEmailRequest request,
        ServerCallContext context)
    {
        var changeEmail = mapper.Map<Dto.Users.ChangeEmail>(request);

        var user = await emailChangeService.Change(changeEmail);

        return new ChangeAccountEmailResponse
        {
            User = mapper.Map<UserEnvelope>(user)
        };
    }

    /// <summary>
    /// Reset account password
    /// </summary>
    public override async Task<ResetAccountPasswordResponse> ResetAccountPassword(
        ResetAccountPasswordRequest request,
        ServerCallContext context)
    {
        var resetPassword = mapper.Map<Dto.Users.ResetPassword>(request);

        var user = await passwordService.Reset(resetPassword);

        return new ResetAccountPasswordResponse
        {
            User = mapper.Map<UserEnvelope>(user)
        };
    }

    /// <summary>
    /// Change account password
    /// </summary>
    public override async Task<ChangeAccountPasswordResponse> ChangeAccountPassword(
        ChangeAccountPasswordRequest request,
        ServerCallContext context)
    {
        var changePassword = mapper.Map<Dto.Users.ChangePassword>(request);

        var user = await passwordService.Change(changePassword);

        return new ChangeAccountPasswordResponse
        {
            User = mapper.Map<UserEnvelope>(user)
        };
    }

    /// <summary>
    /// Update account
    /// </summary>
    public override async Task<UpdateAccountResponse> UpdateAccount(
        UpdateAccountRequest request,
        ServerCallContext context)
    {
        var update = mapper.Map<UpdateUser>(request.UserData);
        var currentUser = await authenticationService.Authenticate(request.Token, context.GetHttpContext());
        var user = await userProfileUpdateService.UpdateUserProfileAsync(currentUser.User.Login, update);
        return new UpdateAccountResponse
        {
            User = mapper.Map<UserDetailsEnvelope>(user)
        };
    }

    private static Guid GetGuid(string input)
    {
        if (Guid.TryParse(input, out var guid) is false)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid guid input"));
        }

        return guid;
    }
}
