using Engine.Input;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Characters
{
    /// <summary>
    /// <c>CharacterMouseCursor</c> a mouse controled character intended to
    /// represent a mouse cursor.
    /// </summary>
    public class CharacterMouseCursor : Character
    {
        /// <summary>
        /// <c>CharacterMouseCursor</c> default constuctor.
        /// </summary>
        public CharacterMouseCursor() : base() {}

        /// <summary>
        /// <c>GenerateSprites</c> override. Generate 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void GenerateSprites()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// <c>HandleMouseMessage</c> Callback function for <c>MouseMessage</c>.
        /// Defines default behavior for updating mouse position. The default
        /// behavior is as you would expect. Moving the mouse in the X-axis updates
        /// the X position and moving the mouse in the Y-axis updates the Y position. 
        /// </summary>
        /// <param name="device">
        /// A reference to an <c>IInputDevice</c> implementing instance representing
        /// the device that generated the message.
        /// </param>
        /// <param name="update">
        /// A reference to a <c>SharpDX.DirectInput.MouseUpdate</c> instance containing
        /// information about the mouse device state change.
        /// </param>
        public virtual void HandleMouseMessage(IInputDevice device, MouseUpdate update)
        {
            if (update.Offset == MouseOffset.X)
            {
                MousePosition = new Vector2(
                    update.Value * MouseMovementScale.X,
                    MousePosition.Y
                );
            }
            if (update.Offset == MouseOffset.Y)
            {
                MousePosition = new Vector2(
                    MousePosition.X,
                    update.Value * MouseMovementScale.Y
                );
            }
        }
        /// <summary>
        /// <c>RegisterInputHandlers</c> registers the HandleMouseMessage callback to <c>MouseMessage</c>
        /// </summary>
        public override void RegisterInputHandlers()
        {
            Messenger<Input.Input.MouseMessage>.Register(HandleMouseMessage);
        }
        /// <summary>
        /// <c>CenterMousePosition</c> sets <c>MousePosition</c> to the center of the screen
        /// according to the current <c>ScreenDimensions</c>.
        /// </summary>
        public virtual void CenterMousePosition()
        {
            MousePosition = new Vector2(
                ScreenDimensions.X / 2.0f,
                ScreenDimensions.Y / 2.0f
            );
        }
        /// <summary>
        /// <c>MousePositon</c> property stores and retrieves the position
        /// of the mouse in screen coordinates in a <c>Vector2</c>
        /// </summary>
        public Vector2 MousePosition { get; set; }
        /// <summary>
        /// <c>MouseMovementScale</c> property stores and retrieves the scale
        /// mouse movement should be multiplied by. Could also be called "sensitivity".
        /// </summary>
        public Vector2 MouseMovementScale { get; set; }
        /// <summary>
        /// <c>ScreenDimensions</c> property stores and retrieves the current dimensions
        /// of the application screen. This must be set by the consuming application.
        /// </summary>
        public Vector2 ScreenDimensions { get; set; }
    }
}
