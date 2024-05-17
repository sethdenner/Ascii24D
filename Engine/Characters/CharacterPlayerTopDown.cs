using Engine.Input;
using SharpDX.DirectInput;

namespace Engine.Characters
{
    /// <summary>
    /// <c>CharacterPlayerTopDown</c> is a player controlled character that
    /// behaves in the style of a top-down perspective game.
    /// </summary>
    public class CharacterPlayerTopDown : CharacterPlayer
    {
        public int LastInputSequence { get; set; }

        public CharacterPlayerTopDown() : base()
        {
            LastInputSequence = -1;
        }

        public override void HandleKeyboardMessage(
            InputDevice device,
            KeyboardUpdate update
        )
        {
            // If the input sequence is out of order reject it. We should
            // record when this happens to detect issues. Should not happen.
            if (update.Sequence < LastInputSequence)
            {
                return;
            }

            //Keybinds.
        }
    }
}
