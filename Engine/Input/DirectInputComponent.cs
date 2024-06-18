using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Input
{
    public struct DirectInputComponent() {
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
        public MessageFrame[] BufferedMessageFrames = [
            new(),
            new()
        ];
        /// <summary>
        /// 
        /// </summary>
        public MessageFrame ReadMessageFrame;
        /// <summary>
        /// 
        /// </summary>
        public MessageFrame WriteMessageFrame;
    }
}
