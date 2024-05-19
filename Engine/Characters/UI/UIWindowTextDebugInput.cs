using System.Numerics;
using SharpDX.DirectInput;
using Engine.Input;
using Engine.Core;

namespace Engine.Characters.UI
{
    /// <summary>
    /// UIWindowTextDebugInput is a UIWindowText character that knows
    /// how to retrieve input debug information and how to render
    /// that information to text for display.
    /// </summary>
    public class UIWindowTextDebugInput : UIWindowText
    {
        /// <summary>
        /// <c>JoystickState</c> is a struct that represents a row
        /// of joystick state data to be rendered in the debug window.
        /// </summary>
        private struct JoystickState
        {
            /// <summary>
            /// 
            /// </summary>
            public int Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Timestamp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Sequence { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public JoystickOffset Offset { get; set; }
        }
        /// <summary>
        /// <c>KeyboardState</c> is a struct that represents a row
        /// of keyboard state data to be rendered in the debug window.
        /// </summary>
        private struct KeyboardState
        {
            /// <summary>
            /// 
            /// </summary>
            public int Value { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Timestamp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Sequence { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Key Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool IsPressed { get; set; }
        }
        /// <summary>
        /// <c>MouseState</c> is a struct that represents a row
        /// of mouse state data to be rendered in the debug window.
        /// </summary>
        private struct MouseState 
        {
            public int Value { get; set; }
            public int Timestamp { get; set; }
            public int Sequence { get; set; }
            public MouseOffset Offset { get; set; }
            public bool IsButton { get; set; }

        }
        /// <summary>
        /// UIWindowTextDebugInput constructor. Store the 
        /// reference to an instance of the Input class so we
        /// can output the input state data.
        /// </summary>
        /// <param name="input">
        ///     Input object with enumerated input devices 
        ///     (EnumerateDevices has been called)
        /// </param>
        public UIWindowTextDebugInput(Input.Input input) : base(
            Native.Native.WindowWidth - 2,
            Native.Native.WindowHeight - 2,
            new Vector3(1, 1, 10001),
            new Native.ConsolePixel() {
                ForegroundColorIndex = 0,
                BackgroundColorIndex = 0,
                CharacterCode = (byte)' '
            }, new Native.ConsolePixel() {
                ForegroundColorIndex = 0,
                BackgroundColorIndex = 3,
                CharacterCode = (byte)'#'
            },
            2,
            0
        )
        {
            _input = input;

            _joystickStates = [];
            _keyboardStates = [];
            _mouseStates = [];

            Messenger<JoystickMessage>.Register(HandleJoystickMessage);
            Messenger<KeyboardMessage>.Register(HandleKeyboardMessage);
            Messenger<MouseMessage>.Register(HandleMouseMessage);
        }
        /// <summary>
        /// Update method. Populates the window text with details about the current state
        /// of connected input devices.
        /// </summary>
        /// <param name="elapsedSeconds">The total application execution time in seconds.</param>
        public override void Update(float elapsedSeconds)
        {
            // store all the text for the text window in debugText variable.
            List<string> debugText = new List<string>();
            foreach (InputDevice device in _input.Devices)
            {
                Guid deviceGuid = device.DeviceGuid;
                DeviceType deviceType = device.GetDeviceType();
                if (DeviceType.Keyboard == deviceType)
                {
                    debugText.Add($"Keyboard: {deviceGuid}");
                    if (!_keyboardStates.ContainsKey(deviceGuid)) continue;

                    KeyboardState[] inputState = _keyboardStates[deviceGuid];
                    foreach (var state in inputState)
                    {
                        if (0 == state.Timestamp) continue;
                        debugText.Add(string.Join(", ", [
                            $"Value: {state.Value}",
                            $"Sequence: {state.Sequence}",
                            $"Key: {state.Key}",
                            $"IsPressed: {state.IsPressed}",
                            $"Timestamp: {state.Timestamp}"
                        ]));
                    }
                }
                if (DeviceType.Joystick == deviceType || DeviceType.Gamepad == deviceType)
                {
                    debugText.Add($"Joystick: {deviceGuid}");
                    if (!_joystickStates.ContainsKey(deviceGuid)) continue;

                    JoystickState[] inputState = _joystickStates[deviceGuid];
                    foreach (var state in inputState)
                    {
                        if (0 == state.Timestamp) continue;
                        debugText.Add(
                            $"Value: {state.Value}, " +
                            $"Sequence: {state.Sequence}, " +
                            $"Offset: {state.Offset}," +
                            $" Timestamp: {state.Timestamp}"
                        );
                    }
                }
                if (DeviceType.Mouse == deviceType)
                {
                    debugText.Add($"Mouse: {deviceGuid}");
                    if (!_mouseStates.ContainsKey(deviceGuid)) continue;
                    MouseState[] inputState = _mouseStates[deviceGuid];
                    foreach (var state in inputState)
                    {
                        if (0 == state.Timestamp) continue;
                        debugText.Add(
                            $"Value: {state.Value}, " +
                            $"Sequence: {state.Sequence}, " +
                            $"Offset: {state.Offset}, " +
                            $"IsButton: {state.IsButton} " +
                            $"Timestamp: {state.Timestamp}"
                        );
                    }
                }
            }
            // WTH? Should the position and what not be updated every frame?
            Width = Native.Native.WindowWidth - 2;
            Height = Native.Native.WindowHeight - 2;
            Position = new Vector3() {
                X = 1,
                Y = 1,
                Z = 10002
            };
            Text = string.Join("\n", debugText.ToArray());
        }
        /// <summary>
        /// Updates the joystick state with thed provided update
        /// message data.
        /// </summary>
        /// <param name="device">
        /// A reference to an <c>InputDevice</c> implementing instance
        /// representing the device that triggered the message.
        /// </param>
        /// <param name="update">
        /// A reference to an instance of <c>JoysticUpdate</c> containing data
        /// about the updated state of the joystick.
        /// </param>
        public void HandleJoystickMessage(InputDevice device, JoystickUpdate update)
        {
            if (!_joystickStates.ContainsKey(device.DeviceGuid))
                _joystickStates.Add(
                    device.DeviceGuid,
                    new JoystickState[(int)(Enum.GetValues<JoystickOffset>().Max())]
                );

            _joystickStates[device.DeviceGuid][(int)update.Offset] = new JoystickState() 
            {
                Offset = update.Offset,
                Value = update.Value,
                Sequence = update.Sequence,
                Timestamp = update.Timestamp
            }; 
        }
        /// <summary>
        /// Updates the keyboard state with thed provided update
        /// message data.
        /// </summary>
        /// <param name="device">
        /// A reference to an <c>InputDevice</c> implementing instance
        /// representing the device that triggered the message.
        /// </param>
        /// <param name="update">
        /// A reference to an instance of <c>KeyboardUpdate</c> containing data
        /// about the updated state of the keyboard.
        /// </param>
        public void HandleKeyboardMessage(InputDevice device, KeyboardUpdate update)
        {
            if (!_keyboardStates.ContainsKey(device.DeviceGuid))
                _keyboardStates.Add(
                    device.DeviceGuid,
                    new KeyboardState[(int)(Enum.GetValues<Key>().Max())]
                );

            _keyboardStates[device.DeviceGuid][(int)update.Key] = new KeyboardState()
            {
                Key = update.Key,
                Value = update.Value,
                IsPressed = update.IsPressed,
                Sequence = update.Sequence,
                Timestamp = update.Timestamp
            };
        }
        /// <summary>
        /// Updates the mouse state with thed provided update
        /// message data.
        /// </summary>
        /// <param name="device">
        /// A reference to an <c>InputDevice</c> implementing instance
        /// representing the device that triggered the message.
        /// </param>
        /// <param name="update">
        /// A reference to an instance of <c>MouseUpdate</c> containing data
        /// about the updated state of the mouse.
        /// </param>
        public void HandleMouseMessage(InputDevice device, MouseUpdate update)
        {
            if (!_mouseStates.ContainsKey(device.DeviceGuid))
                _mouseStates.Add(
                    device.DeviceGuid,
                    new MouseState[(int)(Enum.GetValues<MouseOffset>().Max())]
                );

            _mouseStates[device.DeviceGuid][(int)update.Offset] = new MouseState()
            {
                Offset = update.Offset,
                Value = update.Value,
                IsButton = update.IsButton,
                Sequence = update.Sequence,
                Timestamp = update.Timestamp
            };
        }
        /// <summary>
        /// <c>_input</c> is a private reference to a Input.Input manager object.
        /// </summary>
        private Input.Input _input;
        /// <summary>
        /// <c>_joystickStates</c> keeps track of the current state of connected
        /// joysticks and gamepads. The dictionary key is the device guid.
        /// </summary>
        private Dictionary<Guid, JoystickState[]> _joystickStates;
        /// <summary>
        /// <c>_mouseStates</c> keeps track of the current state of connected
        /// mouse devices. The dictionary key is the device guid.
        /// </summary>
        private Dictionary<Guid, MouseState[]> _mouseStates;
        /// <summary>
        /// <c>_keyboardStates</c> keeps track of the current state of connected
        /// keyboards. The dictionary key is the device guid.
        /// </summary>
        private Dictionary<Guid, KeyboardState[]> _keyboardStates;
    }
}
