using System.Numerics;
using Engine.Core;
using Engine.Native;
using Engine.Render;

namespace Engine.Characters
{
    /// <summary>
    /// <c>Character</c> abstract class. Entities have visual components
    /// in the form of sprites and behavior defined in methods that manipulate
    /// those sprites.
    /// </summary>
    [Serializable]
    public abstract class Character : ICharacter
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ICharacter> Children {
            get; set;
        } = [];
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// <c>Sprites</c> is a property that publicly exposes the
        /// <c>Sprites</c> <c>List</c>.
        /// </summary>
        public bool UseTransform {
            get; set;
        } = true;
        public List<Sprite> Sprites { get; set; } = [];
        /// <summary>
        /// The <c>Render</c> method compiles all of the elements
        /// returned by the <c>Sprites</c> property and transforms them with the
        /// injected translate vector.
        /// </summary>
        /// <param name="renderTarget">
        /// A <c>Sprite</c> representing a canvas to draw other sprites to.
        /// </param>
        /// <param name="translate">
        /// A <c>Vector3</c> representing the translation that should be
        /// preformed before rendering sprites.
        /// </param>
        public virtual void Render(
            Sprite renderTarget,
            Matrix4x4 worldCameraViewport
        ) {
            // Composite child UI elements.
            for (int i = 0; i < Children.Count; ++i)
            {
                Character child = (Character)Children[i];
                child.Render(renderTarget, worldCameraViewport);
            }
            GenerateSprites();
            // Composite all the sprites.
            for (int i = 0; i < Sprites.Count; ++i)
            {
                // Transform the sprite offset by adding the position vector.
                // Should sprite offset also be a vector?
                Vector3 modelPosition = new(
                    Sprites[i].OffsetX,
                    Sprites[i].OffsetY,
                    0
                );


                Vector3 screenPosition;
                if (UseTransform) {
                    screenPosition = Vector3.Transform(
                        modelPosition,
                        worldCameraViewport
                    );
                } else {
                    screenPosition = modelPosition;
                }
                Sprite sprite = new(
                    Sprites[i].Width,
                    Sprites[i].Height,
                    (int)Math.Ceiling(screenPosition.X),
                    (int)Math.Ceiling(screenPosition.Y)
                );
                Sprites[i].BufferPixels.AsSpan().CopyTo(
                    sprite.BufferPixels.AsSpan()
                );
                renderTarget.MergeSprite(sprite);
            }
        }
        /// <summary>
        /// <c>AddChild</c> is a method that adds a user interface
        /// component to the <c>List</c> of children user interface
        /// components.
        /// </summary>
        /// <param name="child"></param>
        public virtual void AddChild(ICharacter child) {
            Children.Add(child);
        }
        /// <summary>
        /// <c>GenerateSprites</c> is an <c>abstract</c> method. The logic for
        /// generating the sprites that comprise of the user interface component.
        /// all <c>Sprite</c> objects should be generated or otherwise retrieved
        /// and added to the <c>List</c> of <c>Sprites</c> as defined in the
        /// <c>Character</c> base class.
        /// </summary>
        public abstract void GenerateSprites();
    }
}
