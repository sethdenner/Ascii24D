namespace Engine.Core.ECS;

public class ComponentArray<T>(int capacity) {
    public Dictionary<int, int> IndexByEntityID = new(capacity);
    public T[] Array = new T[capacity];
    public int[] EntityIDByIndex = new int[capacity];
    public bool[] VacanciesByIndex = new bool[capacity];
    public int Capacity = capacity;
    public int ComponentCount = 0;
    public bool Fragmented = false;

    public Span<T> AsSpan() {
        return Array.AsSpan(0, ComponentCount);
    }
    public T this[int index] {
        get {
            return Array[index];
        }
        set {
            Array[index] = value;
        }
    }
    public void SetComponentByEntityID(int entityID, T component) {
        Array[IndexByEntityID[entityID]] = component;
    }
    public ref T GetComponentByEntityId(int entityID) {
        return ref Array[IndexByEntityID[entityID]];
    }
    public void Add(int entityId, T component) {
        IndexByEntityID.Add(entityId, ComponentCount);
        EntityIDByIndex[ComponentCount] = entityId;
        VacanciesByIndex[ComponentCount] = false;
        Array[ComponentCount] = component;
        ++ComponentCount;
    }
    public void Remove(int entityId) {
        var index = IndexByEntityID[entityId];
        VacanciesByIndex[index] = true;
        if (index != --ComponentCount) {
            Defragment();
        }
    }
    private void Defragment() {
        int emptyIndex = -1;
        int emptyCount = 0;
        int seenComponents = 0;
        for (
            int i = 0;
            i < VacanciesByIndex.Length &&
            seenComponents < ComponentCount;
            ++i
        ) {
            bool vacant = VacanciesByIndex[i];
            if (vacant) {
                if (emptyIndex == -1) {
                    emptyIndex = i;
                }
                // Keep track of how many contiguous empty spots there are.
                ++emptyCount;
            } else {
                // Keep track of how many components we have seen so we can bail
                // early.
                ++seenComponents;
                if (emptyIndex > -1) {
                    // Save the entity ID so we can move it. 
                    int entityId = EntityIDByIndex[i];
                    // Copy the component to the empty index.
                    Span<T> source = Array.AsSpan()[i..(i + 1)];
                    Span<T> destination = Array.AsSpan()[emptyIndex..(emptyIndex + 1)];
                    source.CopyTo(destination);
                    // Remove the entity from the entity to index map.
                    IndexByEntityID.Remove(entityId);
                    // Add the new entity back to the entity to index map with the
                    // new index.
                    IndexByEntityID.Add(entityId, emptyIndex);
                    // Update the index to entity map at the new index with the
                    // entity ID.
                    EntityIDByIndex[emptyIndex] = entityId;
                    // Update the empty index based on the empty count.
                    --emptyCount;
                    if (emptyCount > 0) {
                        // We have another gap to fill.
                        ++emptyIndex;
                    } else {
                        // No empties, defragmented so far.
                        emptyIndex = -1;
                    }
                }
            }
        }
        // Array is defragmented. Set the vacancies array appropriately.
        for (int i = 0; i <  ComponentCount; ++i) {
            VacanciesByIndex[i] = false;
        }
        Fragmented = false;
    }
}
