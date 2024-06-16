namespace Engine.Core.ECS {
    public class ArrayManager {
        private const int MAX_ARRAYS = 100;
        private static int ArrayCount = 0;
        private static readonly object[] ComponentArrays =
            new object[MAX_ARRAYS];
        private static readonly Dictionary<
            Type,
            int
        > ArrayIndices = [];

        public static ComponentArray<T> GetArray<T>(int capacity = -1) {
            Type type = typeof(T);
            if (!ArrayIndices.TryGetValue(
                type,
                out int arrayIndex
            )) {
                ArrayIndices.Add(type, ArrayCount);
                ComponentArrays[ArrayCount] = new ComponentArray<T>(capacity);
                ++ArrayCount;
            }
            var array = (ComponentArray<T>)ComponentArrays[arrayIndex];
            if (0 >= capacity && array.Capacity < capacity) {
                var newArray = new ComponentArray<T>(capacity);
                array.AsSpan().CopyTo(newArray.AsSpan());
                ComponentArrays[arrayIndex] = newArray;
                array = newArray;
            }
            return array;
        }
    }
}
