namespace Engine.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Messenger<T> where T : Delegate
    {
        /// <summary>
        /// 
        /// </summary>
        private static T? _handle;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public static void Register(T callback)
        {
            _handle = Delegate.Combine(
                _handle,
                callback
            ) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public static void Unregister(T callback)
        {
            _handle = Delegate.Remove(
                _handle,
                callback
            ) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        public static T? Trigger
        {
            get
            {
                return _handle;
            }
        }
    }
}
