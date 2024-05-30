using System.Text.Json.Serialization;

namespace Engine.Core {
    /// <summary>
    /// Declares the methods and properties required to implement a service.
    /// Services are a combination of a method that continuously updates the
    /// service state and message listeners that respond to messages which
    /// the service responds to.
    /// </summary>
    public interface IService {
        /// <summary>
        /// 
        /// </summary>
        public void Setup();
        /// <summary>
        /// 
        /// </summary>
        public void Cleanup();
        /// <summary>
        /// 
        /// </summary>
        public Task Start();
        /// <summary>
        /// 
        /// </summary>
        public void Stop();
    }
}
