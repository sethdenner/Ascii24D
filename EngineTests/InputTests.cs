using Engine.Input;
using Moq;
using SharpDX.DirectInput;

namespace EngineTests
{
    public class InputTests
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestInputEnumerateDevices()
        {
            var mockMouseDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            var mockKeyboardDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            var mockJoystickDeviceInstance = new Mock<Engine.Input.IDeviceInstance>();
            var mockMouseInputDevice = new Mock<Engine.Input.InputDeviceMouse>();
            var mockKeyboardInputDevice = new Mock<Engine.Input.InputDeviceKeyboard>();
            var mockJoystickInputDevice = new Mock<Engine.Input.InputDeviceJoystick>();
            var mockDirectInput = new Mock<Engine.Input.IDirectInput>();
            mockMouseDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Mouse);
            mockKeyboardDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Keyboard);
            mockJoystickDeviceInstance.Setup(
                deviceInstance => deviceInstance.GetDeviceType()
            ).Returns(DeviceType.Joystick);
            mockDirectInput.Setup(
                directInput => directInput.GetDevices()
            ).Returns(new List<IDeviceInstance>([
                mockMouseDeviceInstance.Object,
                mockKeyboardDeviceInstance.Object,
                mockJoystickDeviceInstance.Object
            ]));
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockMouseDeviceInstance.Object)
            ).Returns(mockMouseInputDevice.Object);
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockKeyboardDeviceInstance.Object)
            ).Returns(mockKeyboardInputDevice.Object);
            mockDirectInput.Setup(
                directInput => directInput.CreateInputDevice(mockJoystickDeviceInstance.Object)
            ).Returns(mockJoystickInputDevice.Object);
            Input input = new Input(mockDirectInput.Object);

            input.EnumerateDevices();
            
            mockDirectInput.Verify(directInput => directInput.GetDevices(), Times.Once());
            Assert.Equal(3, input.Devices.Count);

        }

        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestInputCreateDevice()
        {
        }
    }
}
