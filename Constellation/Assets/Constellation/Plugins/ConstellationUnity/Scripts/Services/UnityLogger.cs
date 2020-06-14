using UnityEngine;

namespace Constellation.Services {
    public class UnityLogger : ILogger
    {
        public void Log(object _object)
        {
			Debug.Log(_object);
        }

        public void LogError(object _object)
        {
			Debug.LogError(_object);
        }

        public void LogWarning(object _object)
        {
			Debug.LogWarning(_object);
        }
    }
}