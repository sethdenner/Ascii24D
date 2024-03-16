using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

namespace Engine.Characters.UI
{
    /// <summary>
    /// <c>UI</c> is a special type of <c>Character</c> used for
    /// representing and generating graphics for UI windows and 
    /// components.
    /// </summary>
    public abstract class UI : Character
    {
        /// <summary>
        /// <c>UI</c> default constructor.
        /// </summary>
        public UI() : base() { }
    }
}
