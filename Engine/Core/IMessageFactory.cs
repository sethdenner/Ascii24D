namespace Engine.Core {
    public interface IMessageFactory<T> {
        public T CreateInstance();
    }
}
