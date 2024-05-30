using Xunit;
using SharpDX.DirectInput;
using Moq;
using Engine.Core;
using Engine.Input;
using Engine.Network;

namespace EngineTests
{
    public class InputTests
    {
        /// <summary>
        /// Helper method to create a mock of the DirectInput wrapper object.
        /// </summary>
        /// <returns>
        /// The generated mock representing a DirectInput wrapper object.
        /// </returns>
        public Mock<Engine.Input.IDirectInput> CreateDirectInputMock()
        {
            // Define a mock device instance for each device type.
            var mockMouseDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            var mockKeyboardDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            var mockJoystickDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            // Define a mock InputDevice instance of the appropriate
            // derived class for each supported device type.
            var mockMouseInputDevice = new Mock<Engine.Input.InputDeviceMouse>();
            var mockKeyboardInputDevice = new Mock<Engine.Input.InputDeviceKeyboard>();
            var mockJoystickInputDevice = new Mock<Engine.Input.InputDeviceJoystick>();
            // Define a mock DirectInput wrapper instance.
            var mockDirectInput = new Mock<Engine.Input.IDirectInput>();
            // Startup GetDeviceType call for each DeviceInstance.
            mockMouseDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Mouse);
            mockKeyboardDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Keyboard);
            mockJoystickDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Joystick);
            // Startup DetDeviceType call for each InputDevice
            mockMouseInputDevice.Setup(
               deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Mouse);
            mockKeyboardInputDevice.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Keyboard);
            mockJoystickInputDevice.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Joystick);
            // Startup GetUpdates call for each InputDevice.
            mockMouseInputDevice.Setup(
                inputDevice => inputDevice.GetUpdates()
            ).Returns([
                new MouseUpdate() { },
                new MouseUpdate() { }
            ]);
            mockKeyboardInputDevice.Setup(
                inputDevice => inputDevice.GetUpdates()
            ).Returns([
                new KeyboardUpdate() { },
                new KeyboardUpdate() { }
            ]);
            mockJoystickInputDevice.Setup(
                inputDevice => inputDevice.GetUpdates()
            ).Returns([
                new JoystickUpdate() { },
                new JoystickUpdate() { }]
            );
            // Startup GetDevices call for the DirectInput wrapper.
            mockDirectInput.Setup(
                directInput => directInput.GetDevices()
            ).Returns(new List<IDeviceInstance>([
                mockMouseDeviceInstance.Object,
                mockKeyboardDeviceInstance.Object,
                mockJoystickDeviceInstance.Object
            ]));
            // Startup CreateInputDevice call for each DeviceInstance type.
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockMouseDeviceInstance.Object)
            ).Returns(mockMouseInputDevice.Object);
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockKeyboardDeviceInstance.Object)
            ).Returns(mockKeyboardInputDevice.Object);
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockJoystickDeviceInstance.Object)
            ).Returns(mockJoystickInputDevice.Object);
            // Return the final DirectInput mock.
            return mockDirectInput;
        }
        /// <summary>
        /// Test the <c>IDirectInput</c> wrapper <c>EnumerateDevices</c> method.
        /// </summary>
        [Fact]
        public void TestInputEnumerateDevices()
        {
            Mock<Engine.Input.IDirectInput> mockDirectInput = CreateDirectInputMock();
            Input input = new Input(mockDirectInput.Object);

            input.EnumerateDevices();
            
            mockDirectInput.Verify(directInput => directInput.GetDevices(), Times.Once());
            Assert.Equal(3, input.Devices.Count);
        }
        /// <summary>
        /// Test the <c>IDirectInput</c> wrapper <c>Update</c> method.
        /// </summary>
        [Fact]
        public void TestInputUpdate()
        {
            // Define int variables to keep track of how many event
            // messages of each type we have recieved.
            int joystickMessageRecieved = 0;
            int keyboardMessageRecieved = 0;
            int mouseMessageRecieved = 0;
            // Startup handlers to check if input messages are sent properly.
            Messenger<JoystickMessage>.Register((device, update) => {
                ++joystickMessageRecieved;
            });
            Messenger<KeyboardMessage>.Register((device, update) => {
                ++keyboardMessageRecieved;
            });
            Messenger<MouseMessage>.Register((device, update) => {
                ++mouseMessageRecieved;
            });
            Mock<Engine.Input.IDirectInput> mockDirectInput = CreateDirectInputMock();
            Input input = new Input(mockDirectInput.Object);

            input.EnumerateDevices();
            input.Update();

            Assert.Equal(2, joystickMessageRecieved);
            Assert.Equal(2, keyboardMessageRecieved);
            Assert.Equal(2, mouseMessageRecieved);
        }
    }
}
