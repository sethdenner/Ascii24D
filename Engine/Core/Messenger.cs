namespace Engine.Core
{
    public static class Messenger<T> where T : Delegate
    {
        private static T? _handle;

        public static void Register(T callback)
        {
            _handle = Delegate.Combine(
                _handle,
                callback
            ) as T;
        }

        public static void Unregister(T callback)
        {
            _handle = Delegate.Remove(
                _handle,
                callback
            ) as T;
        }

        public static T Trigger
        {
            get
            {
                if (null == _handle)
                    throw new InvalidOperationException(
                        nameof(_handle) + " is null."
                    );
                return _handle;
            }
        }
    }
}
