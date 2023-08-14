using System;
using System.Text.Json;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.Users.Updating;
using DM.Services.Core.Dto;
using DM.Web.Api.Grpc.Account;
using DM.Web.Core.Authentication.Credentials;
using Google.Protobuf.WellKnownTypes;
using PagingQuery = DM.Web.Api.Grpc.Account.PagingQuery;
using PagingResult = DM.Web.Api.Grpc.Account.PagingResult;

namespace DM.Web.API.Grpc.Account;

/// <summary>
/// Account gRPC profile
/// </summary>
public class AccountGrpcProfile : Profile
{
    /// <summary>
    /// Account gRPC profile constructor
    /// </summary>
    public AccountGrpcProfile()
    {
        CreateMap<PagingQuery, DM.Services.Core.Dto.PagingQuery>();
        CreateMap<DM.Services.Core.Dto.PagingResult, PagingResult>();

        CreateMap<LoginRequest, LoginCredentials>();
        CreateMap<ChangeAccountEmailRequest, Dto.Users.ChangeEmail>();
        CreateMap<ResetAccountPasswordRequest, Dto.Users.ResetPassword>();
        CreateMap<ChangeAccountPasswordRequest, Dto.Users.ChangePassword>();

        CreateMap<DateTimeOffset?, TimestampValue>()
            .ConvertUsing(src => src.HasValue
                ? new TimestampValue { Value = Timestamp.FromDateTimeOffset(src.Value) }
                : null);

        CreateMap<RegisterAccountRequest, Dto.Users.Registration>();
        CreateMap<Dto.Users.Rating, UserRating>();
        CreateMap<Dto.Users.PagingSettings, PagingSettings>();
        CreateMap<Dto.Users.UserSettings, UserSettings>()
            .ForMember(dest => dest.PagingSettings, opts => opts.MapFrom(src => src.Paging));

        CreateMap<UserUpdate, UpdateUser>()
            .ForMember(dest => dest.Login, opts => opts.Ignore())
            .ForMember(dest => dest.Settings, opts => opts.Ignore());

        CreateMap<Dto.Users.User, User>();

        CreateMap<Dto.Users.UserDetails, UserDetails>()
            .ForMember(dest => dest.Info, opts => opts.MapFrom(src => src.Info.Value));

        CreateMap<GeneralUser, UserRating>()
            .ForMember(dest => dest.Enabled, opts => opts.MapFrom(src => !src.RatingDisabled))
            .ForMember(dest => dest.Quality, opts => opts.MapFrom(src => src.QualityRating))
            .ForMember(dest => dest.Quantity, opts => opts.MapFrom(src => src.QuantityRating));

        CreateMap<GeneralUser, User>()
            .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => new [] { src.Role }))
            .ForMember(dest => dest.Online, opts => opts.MapFrom(src => src.LastVisitDate))
            .ForMember(dest => dest.Registration, opts => opts.Ignore())
            .ForMember(dest => dest.Rating, opts => opts.MapFrom(src => src));

        CreateMap<Dto.Contracts.Envelope<Dto.Users.User>, UserEnvelope>()
            .ForMember(
                dest => dest.Metadata,
                opts => opts.MapFrom(src => src.Metadata == null
                    ? null
                    : JsonSerializer.Serialize(src.Metadata, (JsonSerializerOptions) null)));

        CreateMap<Dto.Contracts.Envelope<Dto.Users.UserDetails>, UserDetailsEnvelope>()
            .ForMember(
                dest => dest.Metadata,
                opts => opts.MapFrom(src => src.Metadata == null
                    ? null
                    : JsonSerializer.Serialize(src.Metadata, (JsonSerializerOptions) null)));
    }
}
