using Whisper.Data;
using Whisper.Storage.Infrastructure;

namespace Whisper.Storage;

public interface ISignalRDataStorage : IStorage<string, SignalRData>;