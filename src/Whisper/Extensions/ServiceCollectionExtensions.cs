using FluentValidation;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using Whisper.Data;
using Whisper.Env;
using Whisper.Models.Calls.Answer;
using Whisper.Models.Calls.Close;
using Whisper.Models.Calls.Dial;
using Whisper.Models.Calls.Ice;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Models.Calls.Offer;
using Whisper.Models.Calls.Update;
using Whisper.Services.Call;
using Whisper.Services.Call.Infrastructure;
using Whisper.Services.Cryptography;
using Whisper.Services.Notifications;
using Whisper.Services.Serializers;
using Whisper.Services.Serializers.Infrastructure;
using Whisper.Storage;
using Whisper.Storage.Mongo;
using Whisper.Storage.Mongo.Infrastructure;
using Whisper.Storage.Redis;
using Whisper.Validators.Calls.Answer;
using Whisper.Validators.Calls.Close;
using Whisper.Validators.Calls.Dial;
using Whisper.Validators.Calls.Ice;
using Whisper.Validators.Calls.Infrastructure;
using Whisper.Validators.Calls.Offer;
using Whisper.Validators.Calls.Update;

namespace Whisper.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCryptography(this IServiceCollection services)
    {
        return services
            .AddScoped<ICrypto, Crypto>();
    }

    internal static IServiceCollection AddStorage(this IServiceCollection services)
    {
        return services
            .AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(AppVariables.RedisConnectionString))
            .AddScoped<ISignalRDataStorage, SignalRDataRedisStorage>()
            .AddSingleton(_ =>
            {
                var database = new MongoClient(AppVariables.MongoConnectionString).GetDatabase("whisper");
                var indexKeysDefinition = Builders<MongoDocument>.IndexKeys.Ascending(doc => doc.ExpireAt);

                const string indexName = "expire_At";
                var indexOptions = new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero,
                    Name = indexName
                };

                var subscriptionCollection = database.GetCollection<MongoDocument>(MongoCollectionNames.SubscriptionData);
                var subscriptionIndexes = subscriptionCollection.Indexes.List().ToList();
                if (subscriptionIndexes.Any(idx => idx["name"].AsString == indexName))
                {
                    subscriptionCollection.Indexes.DropOne(indexName);
                }

                subscriptionCollection.Indexes.CreateOne(new CreateIndexModel<MongoDocument>(indexKeysDefinition, indexOptions));

                return database;
            })
            .AddScoped<ISubscriptionStorage, SubscriptionMongoStorage>();
    }

    internal static IServiceCollection AddCallProcessing(this IServiceCollection services)
    {
        return services
            .AddScoped<ICallProcessor, CallProcessor>()
            .AddScoped<ICallRequestProcessorFactory, CallRequestProcessorFactory>()
            .AddCallRequestProcessor<CallRequestBase, CallRequestBaseProcessor>()
            .AddCallRequestProcessor<UpdateCallRequest, UpdateCallRequestProcessor>()
            .AddCallRequestProcessor<DialCallRequest, DialCallRequestProcessor>()
            .AddDefaultTransmittableCallRequestProcessor<OfferCallRequest, OfferCallData>()
            .AddDefaultTransmittableCallRequestProcessor<AnswerCallRequest, AnswerCallData>()
            .AddDefaultTransmittableCallRequestProcessor<IceCallRequest, IceCallData>()
            .AddDefaultTransmittableCallRequestProcessor<CloseCallRequest, CloseCallData>();
    }

    private static IServiceCollection AddCallRequestProcessor<TRequest, TProcessor>(this IServiceCollection services)
        where TRequest : ICallRequest
        where TProcessor : class, ICallRequestProcessor<TRequest>
    {
        return services
            .AddScoped<ICallRequestProcessor, TProcessor>();
    }

    private static IServiceCollection AddDefaultTransmittableCallRequestProcessor<TRequest, TData>(this IServiceCollection services)
        where TRequest : ICallRequest<TData>
        where TData : ITransmittableCallData
    {
        return services
            .AddCallRequestProcessor<TRequest, DefaultTransmittableCallRequestProcessor<TRequest, TData>>();
    }

    internal static IServiceCollection AddPushServices(this IServiceCollection services)
    {
        return services
            .AddMemoryCache()
            .AddMemoryVapidTokenCache()
            .AddPushServiceClient(options =>
            {
                options.Subject = NotificationVariables.Subject;
                options.PublicKey = NotificationVariables.PublicKey;
                options.PrivateKey = NotificationVariables.PrivateKey;
                options.AutoRetryAfter = true;
                options.MaxRetriesAfter = -1;
            })
            .AddScoped<IPushServiceClient, DefaultPushServiceClient>()
            .AddScoped<INotificationProvider, NotificationProvider>();
    }

    internal static IServiceCollection AddSerializers(this IServiceCollection services)
    {
        return services
            .AddSingleton(_ => new JsonSerializerSettings())
            .AddScoped(typeof(IJsonSerializer<>), typeof(JsonSerializer<>))
            .AddJsonSerializer<SignalRData, SignalRDataJsonSerializer>()
            .AddJsonSerializer<ICallRequest, CallRequestJsonSerializer>();
    }

    private static IServiceCollection AddJsonSerializer<T, TSerializer>(this IServiceCollection services)
        where TSerializer : class, IJsonSerializer<T>
    {
        return services
            .AddScoped<IJsonSerializer<T>, TSerializer>();
    }

    internal static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
            .AddTransient<IValidator<ICallRequest>, CallRequestValidator>()
            .AddTransient<IValidator<UpdateCallRequest>, CallRequestValidator<UpdateCallRequest, UpdateCallData>>()
            .AddTransient<IValidator<UpdateCallData>, UpdateCallDataValidator>()
            .AddTransient<IValidator<DialCallRequest>, CallRequestValidator<DialCallRequest, DialCallData>>()
            .AddTransient<IValidator<DialCallData>, DialCallDataValidator>()
            .AddTransient<IValidator<OfferCallRequest>, CallRequestValidator<OfferCallRequest, OfferCallData>>()
            .AddTransient<IValidator<OfferCallData>, OfferCallDataValidator>()
            .AddTransient<IValidator<AnswerCallRequest>, CallRequestValidator<AnswerCallRequest, AnswerCallData>>()
            .AddTransient<IValidator<AnswerCallData>, AnswerCallDataValidator>()
            .AddTransient<IValidator<IceCallRequest>, CallRequestValidator<IceCallRequest, IceCallData>>()
            .AddTransient<IValidator<IceCallData>, IceCallDataValidator>()
            .AddTransient<IValidator<CloseCallRequest>, CallRequestValidator<CloseCallRequest, CloseCallData>>()
            .AddTransient<IValidator<CloseCallData>, CloseCallDataValidator>();
    }
}