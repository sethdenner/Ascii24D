using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Core;
using SharpDX.DirectInput;


namespace Engine.Input
{
    /// <summary>
    /// <c>KeyboardMessage</c> delegate. Register a callback function matching
    /// this signature to be notified when keyboard state changes.
    /// </summary>
    /// <param name="device">
    /// <paramref name="device"/> is a reference to a <c>IInputDevice</c> 
    /// instance representing the device that triggered the message.
    /// </param>
    /// <param name="update">
    /// <paramref name="update"/> is the
    /// <c>SharpDX.DirectInput.KeyboardUpdate</c> object containing event data
    /// for the keyboard state change.
    /// </param>
    public delegate void KeyboardMessage(
        InputDevice device,
        KeyboardUpdate update
    );
    /// <summary>
    /// <c>MouseMessage</c> delegate. Register a callback function matching
    /// this signature to be notified when mouse state changes.
    /// </summary>
    /// <param name="device">
    /// <paramref name="device"/> is a reference to a <c>IInputDevice</c>
    /// instance representing the device that triggered the message.
    /// </param>
    /// <param name="update">
    /// <paramref name="update"/> is the <c>SharpDX.DirectInput.MouseUpdate</c>
    /// object containing event data for the mouse state change.
    /// </param>
    public delegate void MouseMessage(InputDevice device, MouseUpdate update);
    /// <summary>
    /// <c>JoystickMessage</c> delegate. Register a callback function matching
    /// this signature to be notified when joystick state changes.
    /// </summary>
    /// <param name="device">
    /// <paramref name="device"/> is a reference to a <c>IInputDevice</c> 
    /// instance representing the device that triggered the message.
    /// </param>
    /// <param name="update">
    /// <paramref name="update"/> is the
    /// <c>SharpDX.DirectInput.JoystickUpdate</c> object containing event data
    /// for the Joystick state change.
    /// </param>
    public delegate void JoystickMessage(
        InputDevice device,
        JoystickUpdate update
    );
    /// <summary>
    /// <c>NewDeviceMessage</c> delegate. Register a callback function matching
    /// this method signature to be notified when a new device is connected.
    /// </summary>
    /// <param name="deviceGuid">
    /// <paramref name="deviceGuid"/> the <c>Guid</c> representing the newly
    /// connected device.
    /// </param>
    /// <param name="deviceType">
    /// The <c>SharpDX.DirectInput.DeviceType</c> member that corresponds to the
    /// type of the device that has been connected.
    /// </param>
    public delegate void NewDeviceMessage(
        Guid deviceGuid,
        DeviceType deviceType
    );
    /// <summary>
    /// <c>LostDeviceMessage</c> delegate. Register a callback function matching
    /// this method signature to be notified when connection to a device is
    /// lost.
    /// </summary>
    /// <param name="deviceGuid">
    /// <paramref name="deviceGuid"/> the <c>Guid</c> representing the lost
    /// device.
    /// </param>
    /// <param name="deviceType">
    /// The <c>SharpDX.DirectInput.DeviceType</c> member that corresponds to the
    /// type of the device that has been disconnected..
    /// </param>
    public delegate void LostDeviceMessage(
        Guid deviceGuid,
        DeviceType deviceType
    );
    /// <summary>
    /// Input class wraps input functionality including instantiating the
    /// SharpDX.DirectInput wrapper and providing functionality to
    /// enumerate devices, signal device state updates and maintaining the
    /// current input state.
    /// </summary>
    /// <remarks>
    /// <c>Input</c> constuctor. Intializes standard collection members to
    /// empty collections. Instantiates the <c>DirectInput</c> wrapper
    /// instance.
    /// </remarks>
    public class Input(IDirectInput directInputWrapper) {
        /// <summary>
        /// <c>_inputDevices</c> is a <c>List</c> of <c>InputDevice</c>
        /// instances representing all supported and enumarated devices
        /// connected to the system.
        /// </summary>
        public List<InputDevice> Devices = [];
        /// <summary>
        /// <c>_directInput</c> is a reference to an instance of a class that
        /// implementes <c>IDirectInput</c> interface.
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
            // Iterate over the collection of devices.
            foreach (var device in inputDevices)
            {
                var inputDevice = _directInput.CreateInputDevice(device);
                if (null == inputDevice) continue;
                Devices.Add(inputDevice);
                Messenger<NewDeviceMessage>.Trigger?.Invoke(
                    inputDevice.DeviceGuid,
                    inputDevice.GetDeviceType()
                );
            }
        }
        /// <summary>
        /// The <c>Update</c> method iterates over all enumerated devices, polls
        /// DirectInput for all queued state updates and saves them into the
        /// corresponding <c>InputState</c> manager that matches each device
        /// <c>Guid</c>.
        /// </summary>
        /// <returns>
        /// An instance of InputFrame populated with the bind triggers
        /// associated with user input.
        /// </returns>
        public InputFrame Update() {
            InputFrame frame = new();
                 
            for (int i = 0; i < Devices.Count; ++i) {
                var device = Devices[i];
                var deviceType = device.GetDeviceType();
                if (DeviceType.Mouse == deviceType) {
                    InputDeviceMouse mouseDevice = (InputDeviceMouse)device;
                    MouseUpdate[] updates = mouseDevice.GetUpdates();
                    for (int j = 0; j < updates.Length; ++j) {
                        var update = updates[j];
                        Messenger<MouseMessage>.Trigger?.Invoke(device, update);
                        if (Binds.IsMouseOffsetBound(update.Offset)) {
                            IMessage message = Binds.CreateMessageFromMouse(
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
                        Messenger<KeyboardMessage>.Trigger?.Invoke(device, update);
                        if (Binds.IsKeyboardKeyBound(update.Key)) {
                            IMessage message = Binds.CreateMessageFromKeyboard(
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
                        Messenger<JoystickMessage>.Trigger?.Invoke(device, update);
                        if (Binds.IsJoystickOffsetBound(update.Offset)) {
                            IMessage message = Binds.CreateMessageFromJoystick(
                                device,
                                update
                            );
                            frame.AddMessage(message);
                        }
                    }
                }
            }

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
