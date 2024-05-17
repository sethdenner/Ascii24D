using Engine.Core;

namespace Engine.Input
{
    /// <summary>
    /// <c>InputBinds</c> manages emitting user defined messages on user
    /// configured input messages.
    /// </summary>
    public class InputBinds : Entity
    {
        /// <summary>
        /// <c>InputBinds</c> default constructor.
        /// </summary>
        /// <param name="bindArraySize">
        /// The total number of binds that can be supported by this instance.
        /// </param>
        InputBinds(int bindArraySize=1024)
        {
            _binds = new Action[bindArraySize];
        }
        /// <summary>
        /// An array of <c>Actions</c> representing bound 
        /// </summary>
        private Action[] _binds;
    }
}
