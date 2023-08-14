using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DM.Services.Core.Extensions;

/// <summary>
/// Расширения построителя веб-приложения
/// </summary>
public static class WebBuilderExtensions
{
    private const int DefaultPort = 5000;
    private const int DefaultGrpcPort = 5001;

    private const string PortEnv = "PORT";
    private const string GrpcPortEnv = "GRPC_PORT";

    private static int ExtractPort(string envName, int defaultPort)
    {
        // For heroku deployment, where only available port is defined in runtime by the environment variable
        var predefinedPort = Environment.GetEnvironmentVariable(envName);
        return string.IsNullOrEmpty(predefinedPort) || int.TryParse(predefinedPort, out var port) is false
            ? defaultPort
            : port;
    }

    private static IWebHostBuilder UseCustomPort(this IWebHostBuilder builder) => builder
        .UseUrls($"http://+:{ExtractPort(PortEnv, DefaultPort)}");

    private static IWebHostBuilder UseCustomGrpcPort(this IWebHostBuilder builder) => builder
        .UseKestrel(options =>
        {
            options.AllowSynchronousIO = true;
            options.ListenAnyIP(ExtractPort(GrpcPortEnv, DefaultGrpcPort), cfg => cfg.Protocols = HttpProtocols.Http1);
        });

    private static IWebHostBuilder UseCustomWebApiAndGrpcPort(this IWebHostBuilder builder) => builder
        .UseKestrel(options =>
        {
            options.AllowSynchronousIO = true;
            options.ListenAnyIP(ExtractPort(PortEnv, DefaultPort), cfg => cfg.Protocols = HttpProtocols.Http1);
            options.ListenAnyIP(ExtractPort(GrpcPortEnv, DefaultGrpcPort), cfg => cfg.Protocols = HttpProtocols.Http2);
        });

    /// <summary>
    /// Настроить grpc-сервер по-умолчанию
    /// </summary>
    public static IWebHostBuilder UseDefaultGrpc<TStartup>(this IWebHostBuilder builder)
        where TStartup : class => builder
            .UseStartup<TStartup>()
            .UseCustomGrpcPort();

    /// <summary>
    /// Настроить веб-сервер по-умолчанию
    /// </summary>
    public static IWebHostBuilder UseDefault<TStartup>(this IWebHostBuilder builder)
        where TStartup : class => builder
            .UseStartup<TStartup>()
            .UseCustomPort();

    /// <summary>
    /// Настроить Web API + gRPC
    /// </summary>
    public static IWebHostBuilder UseWebApiAndGrpc<TStartup>(this IWebHostBuilder builder)
        where TStartup : class => builder
            .UseStartup<TStartup>()
            .UseCustomWebApiAndGrpcPort();
}
