using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Whisper.Hubs;
using Whisper.Models.Calls.Infrastructure;
using Whisper.Services.Call.Infrastructure;
using Whisper.Services.Notifications;
using Whisper.Storage;

namespace Whisper.Tests.Extensions;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    private readonly Mock<IHubContext<SignalV1Hub>> _hubContextMock = new(MockBehavior.Strict);
    private readonly Mock<INotificationProvider> _notificationProviderMock = new(MockBehavior.Strict);
    private readonly Mock<ISignalRDataStorage> _signalRDataStorageMock = new(MockBehavior.Strict);
    private readonly Mock<ISubscriptionStorage> _subscriptionStorageMock = new(MockBehavior.Strict);

    [TestMethod]
    public void AllConcreteCallRequests_HaveSingleProcessorRegistered()
    {
        // Arrange: build full host and override external dependencies
        var host = Program.CreateHostBuilder([])
            .ConfigureServices((_, services) =>
            {
                services
                    .RemoveAll<ISubscriptionStorage>()
                    .RemoveAll<ISignalRDataStorage>()
                    .RemoveAll<IHubContext<SignalV1Hub>>()
                    .AddSingleton(typeof(ISubscriptionStorage), _ => _subscriptionStorageMock.Object)
                    .AddSingleton(typeof(ISignalRDataStorage), _ => _signalRDataStorageMock.Object)
                    .AddSingleton(typeof(INotificationProvider), _ => _notificationProviderMock.Object)
                    .AddSingleton(typeof(IHubContext<SignalV1Hub>), _ => _hubContextMock.Object);
            })
            .Build();

        // Act: resolve all processors
        var processors = host.Services.CreateScope().ServiceProvider.GetServices<ICallRequestProcessor>().ToList();

        // Get all concrete ICallRequest types
        var allRequestTypes = typeof(ICallRequest).Assembly.GetTypes()
            .Where(t => typeof(ICallRequest).IsAssignableFrom(t) && t is
            {
                IsInterface: false,
                IsAbstract: false
            })
            .ToList();

        foreach (var requestType in allRequestTypes)
        {
            var registeredProcessors = processors.Where(p => p.CallRequestType == requestType).ToList();
            Assert.IsTrue(
                registeredProcessors.Count != 0,
                $"Processor for type {requestType.Name} is not registered.");
            Assert.IsTrue(
                registeredProcessors.Count == 1,
                $"There are more than one processor for type {requestType} is registered ({string.Join(", ", registeredProcessors.Select(x => x.GetType().Name))}).");
        }
    }
}