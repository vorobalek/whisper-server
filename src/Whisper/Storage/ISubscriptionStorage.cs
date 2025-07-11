using Whisper.Data;
using Whisper.Storage.Infrastructure;

namespace Whisper.Storage;

public interface ISubscriptionStorage : IStorage<string, Subscription>;