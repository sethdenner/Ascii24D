using System.Text.Json.Serialization;

namespace Engine.Core {
    [Serializable]
    public abstract class Simulation() : ISimulation {
        public abstract void Cleanup(IApplicationState state);
        public abstract void Setup(IApplicationState state);
        public abstract void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        );
        public abstract Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        );
    }
}
