using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Prometheus;
using StackExchange.Redis;
using Whisper.Env;
using Whisper.Extensions;
using Whisper.Hubs;
using Whisper.Services.DbDateTime;
using Whisper.Services.GlobalCancellationToken;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage.Redis.Infrastructure;

namespace Whisper;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Metrics.SuppressDefaultMetrics();
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder
                .ConfigureServices((_, services) =>
                {
                    services
                        .AddCors(options => options
                            .AddPolicy("AllowCorsOrigins", corsPolicyBuilder => corsPolicyBuilder
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .SetIsOriginAllowed(origin =>
                                    AppVariables.CorsOrigins.Length == 0 || AppVariables.CorsOrigins.Contains(origin))))
                        .AddControllers()
                        .AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.AddConverters([JsonConverters.CallRequest]);
                        });

                    services
                        .AddSignalR()
                        .AddNewtonsoftJsonProtocol(options =>
                        {
                            options.PayloadSerializerSettings.AddConverters([JsonConverters.CallRequest]);
                        })
                        .AddStackExchangeRedis(options =>
                        {
                            options.ConnectionFactory = async _ => await ConnectionMultiplexer.ConnectAsync(AppVariables.RedisConnectionString);
                            options.Configuration.DefaultDatabase = RedisDatabaseIds.SignalRCache;
                            options.Configuration.ClientName = "Whisper";
                            options.Configuration.ChannelPrefix = RedisChannel.Literal("SignalR.");
                        });

                    services
                        .AddCryptography()
                        .AddSerializers()
                        .AddValidators()
                        .AddStorage()
                        .AddCallProcessing()
                        .AddPushServices()
                        .AddScoped<IDbDateTimeProvider, DbDateTimeProvider>()
                        .AddSingleton<IGlobalCancellationTokenSource, GlobalCancellationTokenSource>()
                        .AddHealthChecks();
                })
                .Configure(app =>
                {
                    if (!string.IsNullOrWhiteSpace(AppVariables.PathBase))
                        app.UsePathBase(AppVariables.PathBase);
                    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
                        {
                            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                            if (exceptionHandlerPathFeature?.Error is ValidationException validationException)
                            {
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsJsonAsync(new
                                {
                                    errors = validationException.Errors.Select(e => e.ErrorMessage)
                                });
                            }
                        }))
                        .UseRouting()
                        .UseCors("AllowCorsOrigins")
                        .UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<SignalV1Hub>("/signal/v1");
                            endpoints.MapControllers();
                            endpoints.MapMetrics();
                        })
                        .UseHealthChecks("/health");
                })
                .UseUrls($"http://+:{AppVariables.Port}"));
    }
}