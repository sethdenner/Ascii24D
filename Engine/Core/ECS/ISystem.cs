namespace Engine.Core.ECS {
    public interface ISystem {
        public void Setup();
        public void Cleanup();
        public void Update(long step, bool headless = false);
        public Task UpdateAsync(long step, bool headless = false);

        public void BeforeUpdates(long step, bool headless = false);
        public void AfterUpdates(long step, bool headless = false);
    }
}
