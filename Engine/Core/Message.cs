namespace Engine.Core {
    public abstract class Message : IMessage {
        public abstract void Send();
        public static void Register<T> (T handler) where T : Delegate {
            Messenger<T>.Register(handler);
        }
    }
}
