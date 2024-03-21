using Engine.Characters;
using Engine.Render;

namespace Engine.Core
{
    /// <summary>
    /// The <c>Map</c> class organizes <c>Characters</c> into a scene that is
    /// intendnded to be rendered in whole or in part to the framebuffer.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// <c>Map</c> default constructor.
        /// </summary>
        public Map()
        {
            Characters = [];
        }
        /// <summary>
        /// <c>AddCharacter</c> adds a character to the scene.
        /// </summary>
        /// <param name="character">
        /// The instance of <c>Character</c> to add to the scene.
        /// </param>
        public void AddCharacter(Character character)
        {
            Characters.Add(character);
        }
        /// <summary>
        /// <c>Render</c> iterates over the collection of <c>Character</c>
        /// instances and invokes their render method into a final sprite.
        /// </summary>
        /// <param name="framebuffer">
        /// A <c>Sprite</c> reference to render the scene to.
        /// </param>
        public void Render(Sprite framebuffer)
        {
            foreach (var character in Characters)
            {
                framebuffer.MergeSprite(character.Render());
            }
        }
        /// <summary>
        /// A property that manages a collection of <c>Character</c> instances
        /// that belong to the scene.
        /// </summary>
        public List<Character> Characters { get; set; }
    }
}
