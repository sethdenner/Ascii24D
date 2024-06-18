using System.Data;

namespace Engine.Core.ECS {
    public abstract class System<T> : ISystem {
        public List<IMessage> MessageOutbox = [];
        public abstract void SetupComponent(ref T component);
        public void Setup() {
            var componentArray = ArrayManager.GetArray<T>(100);
            var componentSpan = componentArray.AsSpan();
            for (int i = 0; i < componentArray.ComponentCount; ++i) {
                SetupComponent(ref componentSpan[i]);
            }
            FlushOutbox();
        }
        public abstract void Cleanup();
        public abstract void UpdateComponent(
            ref T component,
            long step,
            bool headless = false
        );
        public void Update(long step, bool headless = false) {
            BeforeUpdates(step, headless);
            var componentArray = ArrayManager.GetArray<T>(100);
            var componentSpan = componentArray.AsSpan();
            for (int i = 0; i < componentArray.ComponentCount; ++i) {
                UpdateComponent(
                    ref componentSpan[i],
                    step,
                    headless
                );
            }
            AfterUpdates(step, headless);
            FlushOutbox();
        }
        public async Task UpdateAsync(long step, bool headless = false) {
            BeforeUpdates(step, headless);
            var componentArray = ArrayManager.GetArray<T>(100);
            for (int i = 0; i < componentArray.ComponentCount; ++i) {
                await Task.Run(() => UpdateComponent(
                    ref componentArray.AsSpan()[i],
                    step,
                    headless
                ));
            }
            AfterUpdates(step, headless);
            FlushOutbox();
        }
        public virtual void AfterUpdates(long step, bool headless = false) { }
        public virtual void BeforeUpdates(long step, bool headless = false) { }
        public void FlushOutbox() {
            for (int i = 0; i < MessageOutbox.Count; ++i) {
                MessageOutbox[i].Send();
            }
            MessageOutbox.Clear();
        }
    }
}
