using Engine.Input;
using static Engine.Input.Input;
using Engine.Core;
using SharpDX.DirectInput;

namespace Engine.Characters
{
    /// <summary>
    /// <c>CharacterPlayer</c> is an abstract class intended to provide basic
    /// behavior and properties that are useful for rendering a player
    /// controlled character.
    /// </summary>
    public abstract class CharacterPlayer : Character
    {
        /// <summary>
        /// <c>CharacterPlayer</c> default constructor. Provides default values
        /// to initalize the instance.
        /// </summary>
        public CharacterPlayer() : base()
        {
            RegisterMessageHandlers();

            InputDevices = new List<InputDevice>();
            PlayerID = 0;
        }
        /// <summary>
        /// <c>CharacterPlayer</c> constructor that accepts parameters used to
        /// initialize the instance.
        /// </summary>
        public CharacterPlayer(int playerID) : base()
        {
            RegisterMessageHandlers();

            InputDevices = new List<InputDevice>();
            PlayerID = playerID;
        }
        /// <summary>
        /// <c>RegisterMessageHandlers</c> registers handlers for each type of
        /// input message. Override the corresponding handler method to provide
        /// character behavior based on these messages.
        /// </summary>
        public virtual void RegisterMessageHandlers()
        {
            Messenger<KeyboardMessage>.Register(
                HandleKeyboardMessage
            );
            Messenger<JoystickMessage>.Register(
                HandleJoystickMessage
            );
            Messenger<MouseMessage>.Register(
                HandleMouseMessage
            );
            Messenger<NewDeviceMessage>.Register(
                HandleNewDeviceMessage
            );
            Messenger<LostDeviceMessage>.Register(
                HandleLostDeviceMessage
            );
        }
        /// <summary>
        /// <c>HandleLostDeviceMessage</c> handles <c>LostDeviceMessage</c>
        /// delegate calls.
        /// </summary>
        /// <param name="deviceGuid">
        /// An instance of <c>Guid</c> representing the ID of the lost input
        /// device.
        /// </param>
        /// <param name="deviceType">
        /// The type of the device that has been lost.
        /// </param>
        public virtual void HandleLostDeviceMessage(
            Guid deviceGuid,
            DeviceType deviceType
        ) { }
        /// <summary>
        /// <c>HandleNewDeviceMessage</c> handles <c>NewDeviceMessage</c>
        /// delegate calls.
        /// </summary>
        /// <param name="deviceGuid">
        /// An instance of <c>Guid</c> representing the ID of the new input
        /// device.
        /// </param>
        /// <param name="deviceType">
        /// The type of the device that has been connected.
        /// </param>
        public virtual void HandleNewDeviceMessage(
            Guid deviceGuid,
            DeviceType deviceType
        ) { }
        /// <summary>
        /// <c>HandleMouseMessage</c> handles <c>MouseMessage</c>
        /// delegate calls.
        /// </summary>
        /// <param name="device">
        /// An <c>InputDevice></c> implementing reference representing the
        /// device that triggered the input message.
        /// </param>
        /// <param name="update">
        /// A <c>MouseUpdate</c> instance containing properties describing the
        /// input state.
        /// </param>
        public virtual void HandleMouseMessage(
            InputDevice device,
            MouseUpdate update
        ) { }
        /// <summary>
        /// <c>HandleJoystickMessage</c> handles <c>JoystickMessage</c>
        /// delegate calls.
        /// </summary>
        /// <param name="device">
        /// An <c>InputDevice></c> implementing reference representing the
        /// device that triggered the input message.
        /// </param>
        /// <param name="update">
        /// A <c>JoystickUpdate</c> instance containing properties describing
        /// the input state.
        /// </param>
        public virtual void HandleJoystickMessage(
            InputDevice device,
            JoystickUpdate update
        ) { }
        /// <summary>
        /// <c>HandleKeyboardMessage</c> handles <c>KeyboardMessage</c>
        /// delegate calls.
        /// </summary>
        /// <param name="device">
        /// An <c>InputDevice></c> implementing reference representing the
        /// device that triggered the input message.
        /// </param>
        /// <param name="update">
        /// A <c>KeyboardUpdate</c> instance containing properties describing
        /// the input state.
        /// </param>
        public virtual void HandleKeyboardMessage(
            InputDevice device,
            KeyboardUpdate update
        ) { }
        /// <summary>
        /// We don't do generative art. Maybe this should be an interface?
        /// or just not abstract maybe.
        /// </summary>
        public override void GenerateSprites()
        { }
        /// <summary>
        /// The numeric ID of the player.
        /// </summary>
        public int PlayerID { get; set; }
        /// <summary>
        /// A list of input device IDs corresponding to the input devices that
        /// have control over the character.
        /// </summary>
        public List<InputDevice> InputDevices { get; set; }
    }
}
