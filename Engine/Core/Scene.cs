using Engine.Characters;
using Engine.Native;
using Engine.Render;

namespace Engine.Core {
    public class Scene {
        public List<ICharacter> Characters { get; set; } = [];
        public PaletteInfo[] Palette {
            get; set;
        } = [];
        /// <summary>
        /// Flag used to determine if the simulation loop should continue
        /// running. Setting this to false will terminate the simulation loop.
        /// </summary>
        /// <summary>
        /// <c>AddCharacter</c> adds a character to the scene.
        /// </summary>
        /// <param name="character">
        /// The instance of <c>Character</c> to add to the scene.
        /// </param>
        public virtual void AddCharacter(Character character) {
            Characters.Add(character);
        }
        /// <summary>
        /// <c>Render</c> iterates over the collection of <c>Character</c>
        /// instances and invokes their render method into a final sprite.
        /// </summary>
        /// <param name="framebuffer">
        /// A <c>Sprite</c> reference to render the scene to.
        /// </param>
        public virtual void Render(Sprite framebuffer) {
            for (int i = 0; i < Characters.Count; ++i) {
                Characters[i].Render(framebuffer);
            }
        }
    }
}
