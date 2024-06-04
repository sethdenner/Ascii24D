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
        /// returned by the <c>Sprites</c> property and transforms them with the
        /// injected translate vector.
        /// </summary>
        /// <param name="renderTarget">
        /// A <c>Sprite</c> representing a canvas to draw other sprites to.
        /// </param>
        /// <param name="worldCameraViewport">
        /// A <c>Matrix4x4</c> representing the combined Model -> World,
        /// World -> Camera and Camera -> Viewport matrices.
        /// </param>
        public void Render(Sprite renderTarget, Matrix4x4 worldCameraViewport);
        public void GenerateSprites();
    }
}
