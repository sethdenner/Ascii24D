using Engine.Core.ECS;

namespace Engine.Input {
    public class DirectInputEntity : Entity {
        public DirectInputEntity() {
            AddComponent<DirectInputComponent>(new DirectInputComponent());
        }
    }
}
