namespace Engine.Core.ECS {
    public abstract class Entity() {
        private static class EntityIDCounter {
            private static int NextEntityID = 0;
            internal static int Next() {
                return NextEntityID++;
            }
        }
        private const int _default_capacity = 128;
        public int EntityID = EntityIDCounter.Next();
        public void AddComponent<T>(
            T component,
            int capacity = _default_capacity
        ) {
            ComponentArray<T> array = ArrayManager.GetArray<T>(capacity);
            array.Add(EntityID, component);
        }

        public T GetComponent<T>(int capacity = _default_capacity) {
            ComponentArray<T> array = ArrayManager.GetArray<T>(capacity);
            return array.GetComponentByEntityId(EntityID);
        }

        public void SetComponent<T>(T component) {
            ComponentArray<T> array = ArrayManager.GetArray<T>();
            array.SetComponentByEntityID(EntityID, component);
        }
    }
}
