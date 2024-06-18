using Engine.Core;
using SharpDX.DirectInput;


namespace Engine.Input
{
    /// <summary>
    /// Input class wraps input functionality including instantiating the
    /// SharpDX.DirectInput wrapper and providing functionality to
    /// enumerate devices, signal device state updates and maintaining the
    /// current input state.
    /// </summary>
    /// <remarks>
    /// <c>Input</c> constructor. Initializes standard collection members to
    /// empty collections. Instantiates the <c>DirectInput</c> wrapper
    /// instance.
    /// </remarks>
    public class Input(IDirectInput directInputWrapper) {
        /// <summary>
        /// <c>_inputDevices</c> is a <c>List</c> of <c>InputDevice</c>
        /// instances representing all supported and enumerated devices
        /// connected to the system.
        /// </summary>
        public List<InputDevice> Devices = [];
        /// <summary>
        /// <c>_directInput</c> is a reference to an instance of a class that
        /// implements <c>IDirectInput</c> interface.
        /// wrapper class.
        /// </summary>
        private readonly IDirectInput _directInput = directInputWrapper;
        /// <summary>
        /// 
        /// </summary>
        public DirectInputBinds Binds = new();
        /// <summary>
        /// <c>EnumerateDevices</c> uses the <c>DirectInput</c> wrapper to
        /// iterate through all of the devices that can be seen by
        /// <c>SharpDX.DirectInput</c>. This method only enumerates mouse,
        /// keyboard and joystick devices. For joysticks
        /// <c>SharpDX.DirectInput.DeviceType.Joystick</c>,
        /// <c>SharpDX.DirectInput.DeviceType.Gamepad</c> and
        /// <c>SharpDX.DirectInput.DeviceType.FirstPerson</c> are all supported.
        /// To support other device types extend this method.
        /// </summary>
        public void EnumerateDevices()
        {
            var inputDevices = _directInput.GetDevices();
            MessageFrame frame = new();
            // Iterate over the collection of devices.
            foreach (var device in inputDevices)
            {
                var inputDevice = _directInput.CreateInputDevice(device);
                if (null == inputDevice) continue;
                Devices.Add(inputDevice);
                frame.AddMessage(new FoundDeviceMessage(
                    inputDevice.DeviceGuid,
                    inputDevice.GetDeviceType()
                ));
            }
            frame.PlayMessages();
        }
        /// <summary>
        /// The <c>Update</c> method iterates over all enumerated devices, polls
        /// DirectInput for all queued state updates and saves them into the
        /// corresponding <c>InputState</c> manager that matches each device
        /// <c>Guid</c>.
        /// </summary>
        /// <returns>
        /// An instance of MessageFrame populated with the bind triggers
        /// associated with user input.
        /// </returns>
        public MessageFrame Update() {
            MessageFrame frame = new();
                 
            for (int i = 0; i < Devices.Count; ++i) {
                var device = Devices[i];
                var deviceType = device.GetDeviceType();
                if (DeviceType.Mouse == deviceType) {
                    InputDeviceMouse mouseDevice = (InputDeviceMouse)device;
                    MouseUpdate[] updates = mouseDevice.GetUpdates();
                    for (int j = 0; j < updates.Length; ++j) {
                        var update = updates[j];
                        frame.AddMessage(new MouseMessage(device, update));
                        if (Binds.IsMouseOffsetBound(update.Offset)) {
                            IMessage message = Binds.CreateMouseMessage(
                                device,
                                update
                            );
                            frame.AddMessage(message);
                        }
                    }
                }
                else if (DeviceType.Keyboard == deviceType) {
                    InputDeviceKeyboard keyboardDevice =
                        (InputDeviceKeyboard)device;
                    KeyboardUpdate[] updates = keyboardDevice.GetUpdates();
                    for (int j = 0; j < updates.Length; ++j) {
                        var update = updates[j];
                        frame.AddMessage(new KeyboardMessage(device, update));
                        if (Binds.IsKeyboardKeyBound(update.Key)) {
                            IMessage message = Binds.CreateKeyboardMessage(
                                device,
                                update
                            );
                            frame.AddMessage(message);
                        }
                    }
                } else if (
                    DeviceType.Joystick == deviceType ||
                    DeviceType.Gamepad == deviceType
                ) {
                    InputDeviceJoystick joystickDevice =
                        (InputDeviceJoystick)device;
                    JoystickUpdate[] updates = joystickDevice.GetUpdates();
                    for (int j = 0; j < updates.Length; ++j) {
                        var update = updates[j];
                        frame.AddMessage(new JoystickMessage(device, update));
                        if (Binds.IsJoystickOffsetBound(update.Offset)) {
                            IMessage message = Binds.CreateJoystickMessage(
                                device,
                                update
                            );
                            frame.AddMessage(message);
                        }
                    }
                }
            }

            frame.PlayMessages();
            return frame;
        }
        /// <summary>
        /// <c>SelectSuitableDevice</c> is a method that automatically selects
        /// an enumerated device of the type specified for use in the
        /// application.
        /// </summary>
        /// <param name="deviceType">
        /// The <c>SharpDX.DirectInput.DeviceType</c> corresponding to the
        /// device type desired.
        /// </param>
        /// <returns>
        /// An reference to an instance of InputDevice representing a suitable
        /// input device to use for <c>Character</c> control.
        /// </returns>
        public InputDevice SelectSuitableDevice(DeviceType deviceType)
        {
            // TODO: Make this intellegently select a device.
            return Devices[0];
        }
    }
}
