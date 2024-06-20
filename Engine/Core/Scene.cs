using Engine.Characters;
using Engine.Core.ECS;
using Engine.Native;
using Engine.Render;
using System.Numerics;

namespace Engine.Core {
    public class Scene {
        public List<Entity> Entities { get; set; } = [];
        public PaletteInfo[] Palette {
            get; set;
        } = [];
        /// <summary>
        /// <c>AddEntity</c> adds a entity to the scene.
        /// </summary>
        /// <param name="entity">
        /// The instance of <c>Entity</c> to add to the scene.
        /// </param>
        public virtual void AddEntity(Entity entity) {
            Entities.Add(entity);
        }
    }
}
