using Engine.Core;
using Engine.Core.ECS;
using System.Numerics;

namespace Engine.Characters {
    public class CharacterSystem : System<CharacterComponent> {
        public override void Cleanup() {
            throw new NotImplementedException();
        }

        public override void SetupComponent(
            ref CharacterComponent component
        ) { }

        public override void UpdateComponent(
            ref CharacterComponent component,
            long step,
            bool headless = false
        ) {
            for (int i = 0; i < component.SpriteEntityIDs.Length; ++i) {
                Matrix4x4 world = Matrix4x4.CreateTranslation(
                    component.Position
                );
                MessageOutbox.Add(new WorldTransformMessage(
                    component.SpriteEntityIDs[i],
                    world
                ));
            }
        }
    }
}
