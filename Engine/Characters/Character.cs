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
    public abstract class Character : Entity
    {
        /// <summary>
        /// <c>Character</c> constructor. Initialzies the sprites <c>List</c>.
        /// </summary>
        public Character() : base()
        {
            // Initialize the _sprites List to empty.
            _sprites = new List<Sprite>();
        }
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
            // Calculate the total width and height.
            int width = 0;
            int height = 0;
            for (int i = 0; i < Sprites.Count; ++i)
            {
                Sprite sprite = Sprites[i];
                int newWidth = sprite.Width;
                if (width < newWidth)
                    width = newWidth;

                int newHeight = sprite.Height;
                if (height < newHeight)
                    height = newHeight;
            }
            // Composite all the sprites.
            for (int i = 0; i < Sprites.Count; ++i)
            {
                Sprite sprite = Sprites[i];
                // What the heck am I even doing with EdgeBehavior here?
                // Is this really correct?
                EdgeBehavior pushedBehavior = renderTarget.EdgeBehavior;
                renderTarget.EdgeBehavior = sprite.EdgeBehavior;
                renderTarget.MergeSprite(sprite);
                renderTarget.EdgeBehavior = pushedBehavior;
            }
        }
        /// <summary>
        /// <c>RegisterInputHandlers</c> override this method to register
        /// input handlers. Gets called once per application start after
        /// input devices have been enumerated.
        /// </summary>
        public virtual void RegisterInputHandlers() { }
        /// <summary>
        /// <c>GenerateSprites</c> is an <c>abstract</c> method. The logic for
        /// generating the sprites that comprise of the user interface component.
        /// all <c>Sprite</c> objects should be generated or otherwise retrieved
        /// and added to the <c>List</c> of <c>Sprites</c> as defined in the
        /// <c>Character</c> base class.
        /// </summary>
        public abstract void GenerateSprites();
        /// <summary>
        /// <c>_sprites</c> is a <c>List</c> collection of <c>Sprite</c> objects
        /// that comprise the character.
        /// </summary>
        protected List<Sprite> _sprites;
        /// <summary>
        /// <c>Sprties</c> is a property that publicly exposes the <c>_sprites</c> <c>List</c>.
        /// </summary>
        public  List<Sprite> Sprites { get { return _sprites; } }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position { get; set; }
        /// <summary>
        /// 
        /// </summary>
    }
}
