using Engine.Core;
using Engine.Core.ECS;
using SharpDX.DirectInput;

namespace Engine.Input
{
    public class DirectInputSystem : System<DirectInputComponent> {
        public DirectInputSystem() {
        }
        public override void Cleanup() { }

        public override void SetupComponent(ref DirectInputComponent component) {
            component.DirectInput = new();
            component.SharpDXDirectInputWrapper = new(component.DirectInput);
            component.Input = new(component.SharpDXDirectInputWrapper);
            component.Input.EnumerateDevices();
        }

        public override void UpdateComponent(
            ref DirectInputComponent component,
            long step,
            bool headless = false
        ) {
            component.Input.Update();
        }
    }
}
