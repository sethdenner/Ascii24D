using System.Numerics;
using Engine.Core;
using Engine.Native;
using Engine.Render;

namespace Engine.Characters
{
    /// <summary>
    /// <c>Character</c> abstract class. Characters have visual components
    /// in the form of sprites and behavor defined in methods that manipulate
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
        /// <c>Sprties</c> is a property that publicly exposes the <c>Sprites</c> <c>List</c>.
        /// </summary>
        public List<Sprite> Sprites { get; set; } = [];
        /// <summary>
        /// The <c>Render</c> method compiles all of the elements
        /// returned by the <c>Sprites</c> property. 
        /// </summary>
        /// <returns>
        /// Retuns an instance of <c>Sprite</c> representing the
        /// final compiled sprite.
        /// </returns>
        public virtual void Render(Sprite renderTarget)
        {
            // Composite child UI elements.
            for (int i = 0; i < Children.Count; ++i)
            {
                Character child = (Character)Children[i];
                child.Render(renderTarget);
            }
            GenerateSprites();
            // Composite all the sprites.
            for (int i = 0; i < Sprites.Count; ++i)
            {
                Sprite sprite = Sprites[i];
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
        /// <c>RegisterInputHandlers</c> override this method to register
        /// input handlers. Gets called once per application start after
        /// input devices have been enumerated.
        /// </summary>
        public abstract void RegisterInputHandlers();
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
