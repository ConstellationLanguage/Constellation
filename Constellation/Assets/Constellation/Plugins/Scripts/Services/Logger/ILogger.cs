namespace Constellation.Services {
    public interface ILogger {
        void Log (object _object);
        void LogWarning (object _object);
        void LogError (object _object);
    }
}