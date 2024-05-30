using Engine.Render;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.Characters {
    /// <summary>
    /// <c>ICharacter</c> interface. Characters have visual components
    /// in the form of sprites and behavor defined in methods that manipulate
    /// those sprites.
    /// </summary>
    public interface ICharacter {
        public Vector3 Position {
            get; set;
        }
        public List<Sprite> Sprites {
            get; set;
        }
        public List<ICharacter> Children {
            get; set; 
        }
        /// <summary>
        /// The <c>Render</c> method compiles all of the elements
        /// returned by the <c>Sprites</c> property. 
        /// </summary>
        /// <returns>
        /// Retuns an instance of <c>Sprite</c> representing the
        /// final compiled sprite.
        /// </returns>
        public void Render(Sprite renderTarget);
        public void RegisterInputHandlers();
        public void GenerateSprites();
    }
}
