using SharpDX.DirectInput;

namespace Engine.Input
{
    /// <summary>
    /// <c>IDirectInput</c> declares the interface required
    /// to interact with <c>SharpDX.DirectInput</c>. Useful
    /// for allowing unit tests to mock out the third party
    /// code.
    /// </summary>
    public interface IDirectInput
    {
        public IList<IDeviceInstance> GetDevices();
        public InputDevice? CreateInputDevice(IDeviceInstance deviceInstance);
        public DirectInput? SharpDXDirectInput { get; set; }
    }
    /// <summary>
    /// <c>SharpDXDirectInput<c> wrapper class. Gets IDirectInput
    /// implementation from <c>SharpDX.DirectInput.DirectInput</c>.
    /// this wrapper has the primary function of allowing mocks to
    /// be created for this third party library making unit tests
    /// easier.
    /// </summary>
    public class SharpDXDirectInputWrapper : IDirectInput
    {
        /// <summary>
        /// <c>SharpDXDirectInput</c> default constructor.
        /// </summary>
        public SharpDXDirectInputWrapper()
        {
            SharpDXDirectInput = null;
        }
        /// <summary>
        /// <c>SharpDXDirectInput</c> constructor saves the
        /// SharpDX.DirectInput.DirectInput object reference.
        /// </summary>
        /// <param name="input"></param>
        public SharpDXDirectInputWrapper(DirectInput input)
        {
            SharpDXDirectInput = input;
        }
        /// <summary>
        /// <c>GetDevices</c> retrieves the DirectInput device
        /// objects and converts them to a list of 
        /// <c>SharpDXDeviceInstanceWrapper</c> objects.
        /// </summary>
        /// <returns></returns>
        public virtual IList<IDeviceInstance> GetDevices()
        {
            if (null == SharpDXDirectInput)
                throw new NullReferenceException(nameof(SharpDXDirectInput));

            List<IDeviceInstance> deviceInstances = new List<IDeviceInstance>();
            SharpDXDirectInput.GetDevices().ToList<DeviceInstance>().ForEach(
                instance => deviceInstances.Add(new SharpDXDeviceInstanceWrapper(instance))
            );
            return deviceInstances;
        }
        public virtual InputDevice? CreateInputDevice(IDeviceInstance deviceInstance)
        {
            // Determine the type of the device and set inputDevice
            // to an instance of the corresponding input device type.
            // add the new instance to the _inputDevices List.
            InputDevice? inputDevice = null;
            DeviceType deviceType = deviceInstance.GetDeviceType();
            if (DeviceType.Mouse == deviceType)
            {
                inputDevice = new InputDeviceMouse(this, deviceInstance);
            }
            else if (DeviceType.Keyboard == deviceType)
            {
                inputDevice = new InputDeviceKeyboard(this, deviceInstance);
            }
            else if ( // There may be more types that can be supported by InputDeviceJoystick.
                DeviceType.Joystick == deviceType ||
                DeviceType.Gamepad == deviceType ||
                DeviceType.FirstPerson == deviceType
            )
            {
                inputDevice = new InputDeviceJoystick(this, deviceInstance);
            }
            return inputDevice;
        }
        public DirectInput? SharpDXDirectInput { get; set; }
    }
}
