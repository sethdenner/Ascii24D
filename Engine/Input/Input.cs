using System;
using System.Collections.Generic;
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
    public class Input
    {
        /// <summary>
        /// <c>Input</c> constuctor. Intializes standard collection members to
        /// empty collections. Instantiates the <c>DirectInput</c> wrapper
        /// instance.
        /// </summary>
        public Input(IDirectInput directInputWrapper)
        {
            _inputDevices = new List<InputDevice>();
            _directInput = directInputWrapper;
        }
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
                _inputDevices.Add(inputDevice);
            }
        }
        /// <summary>
        /// The <c>Update</c> method iterates over all enumerated devices, polls
        /// DirectInput for all queued state updates and saves them into the
        /// corresponding <c>InputState</c> manager that matches each device
        /// <c>Guid</c>.
        /// </summary>
        public void Update()
        { 
            foreach (var device in _inputDevices)
            {
                var deviceType = device.GetDeviceType();
                if (DeviceType.Mouse == deviceType)
                {
                    InputDeviceMouse mouseDevice = (InputDeviceMouse)device;
                    MouseUpdate[] updates = mouseDevice.GetUpdates();
                    foreach (var update in updates)
                    {
                        MouseMessage? handler = Messenger<MouseMessage>.Trigger;
                        if (null != handler)
                            handler(device, update);
                    }
                }
                else if (DeviceType.Keyboard == deviceType)
                {
                    InputDeviceKeyboard keyboardDevice =
                        (InputDeviceKeyboard)device;
                    KeyboardUpdate[] updates = keyboardDevice.GetUpdates();
                    foreach (var update in updates)
                    {
                        KeyboardMessage? handler =
                            Messenger<KeyboardMessage>.Trigger;
                        if (null != handler)
                            handler(device, update);
                    }
                }
                else if (
                    DeviceType.Joystick == deviceType ||
                    DeviceType.Gamepad == deviceType
                )
                {
                    InputDeviceJoystick joystickDevice =
                        (InputDeviceJoystick)device;
                    JoystickUpdate[] updates = joystickDevice.GetUpdates();
                    foreach (var update in updates)
                    {
                        JoystickMessage? handler =
                            Messenger<JoystickMessage>.Trigger;
                        if (null != handler)
                            handler(device, update);
                    }
                }
            }
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
            return _inputDevices[0];
        }
        /// <summary>
        /// <c>_inputDevices</c> is a <c>List</c> of <c>InputDevice</c>
        /// instances representing all supported and enumarated devices
        /// connected to the system.
        /// </summary>
        private List<InputDevice> _inputDevices;
        /// <summary>
        /// <c>_directInput</c> is a reference to an instance of a class that
        /// implementes <c>IDirectInput</c> interface.
        /// wrapper class.
        /// </summary>
        private IDirectInput _directInput;
        /// <summary>
        /// <c>Devices</c> is a property providing public access to the
        /// <c>_inputDevices</c> list.
        /// </summary>
        public List<InputDevice> Devices { get { return _inputDevices; } }
    }
}
