using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input
{
    public class SimulationDirectInput : Simulation {
        /// <summary>
        /// 
        /// </summary>
        public DirectInput DirectInput;
        /// <summary>
        /// 
        /// </summary>
        public SharpDXDirectInputWrapper SharpDXDirectInputWrapper;
        /// <summary>
        /// 
        /// </summary>
        public Input Input;
        /// <summary>
        /// 
        /// </summary>
        public MessageFrame[] BufferedInputFrames = [
            new(),
            new()
        ];
        /// <summary>
        /// 
        /// </summary>
        public MessageFrame ReadInputFrame;
        /// <summary>
        /// 
        /// </summary>
        public MessageFrame WriteInputFrame;
        public SimulationDirectInput() {
            DirectInput = new();
            SharpDXDirectInputWrapper = new(DirectInput);
            Input = new(SharpDXDirectInputWrapper);

            ReadInputFrame = BufferedInputFrames[0];
            WriteInputFrame = BufferedInputFrames[1];
        }
        public override void Cleanup(IApplicationState state) {
        }

        public override void Setup(IApplicationState state) {
            Input.EnumerateDevices();
        }

        public override void Simulate(IApplicationState state, long step, bool headless = false) {
            Input.Update();
        }

        public override Task SimulateAsync(IApplicationState state, long step, bool headless = false) {
            throw new NotImplementedException();
        }
    }
}
