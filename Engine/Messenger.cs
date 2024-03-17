namespace Engine
{
    public static class Messenger<T> where T : System.Delegate
    {
        private static T? _handle;

        public static void Register(T callback)
        {
            _handle = System.Delegate.Combine(
                _handle,
                callback
            ) as T;
        }

        public static void Unregister(T callback)
        {
            _handle = System.Delegate.Remove(
                _handle,
                callback
            ) as T;
        }

        public static T? Trigger
        {
            get { return _handle; }
        }
    }
}
