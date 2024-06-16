using System.Data;

namespace Engine.Core.ECS {
    public abstract class System<T> : ISystem {
        public abstract void Setup();
        public abstract void Cleanup();
        public abstract void UpdateComponent(
            ref T component,
            long step,
            bool headless = false
        );
        public void Update(long step, bool headless = false) {
            var componentArray = ArrayManager.GetArray<T>(100);
            var componentSpan = componentArray.AsSpan();
            for (int i = 0; i < componentArray.ComponentCount; ++i) {
                UpdateComponent(
                    ref componentSpan[i],
                    step,
                    headless
                );
            }
        }
        public async Task UpdateAsync(long step, bool headless = false) {
            var componentArray = ArrayManager.GetArray<T>(100);
            for (int i = 0; i < componentArray.ComponentCount; ++i) {
                await Task.Run(() => UpdateComponent(
                    ref componentArray.AsSpan()[i],
                    step,
                    headless
                ));
            }
        }
    }
}
