
using System.Text.Json.Serialization;

namespace Engine.Core {
    [Serializable]
    public abstract class Service : IService {
        public abstract void Cleanup();
        public abstract void Setup();
        public abstract Task Start();
        public abstract void Stop();
    }
}
