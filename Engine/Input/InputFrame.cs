using Engine.Core;

namespace Engine.Input
{
    /// <summary>
    /// Represents all player input for a single application frame.
    /// </summary>
    public class InputFrame : IHistorySerializable {
        /// <summary>
        /// Serializes all mutable fields for use with application history.
        /// </summary>
        /// <returns>
        /// Binary serialized data for all mutable fields.
        /// </returns>
        public Memory<byte> SerializeHistoryFields() {
            Memory<byte> serialized = new();
            return serialized;
        }
        /// <summary>
        /// Deserializes and stores all mutable fields in the provided data
        /// buffer.
        /// </summary>
        /// <param name="data">
        /// Binary serialized data that was serialized with
        /// <c>SerializeHistoryFields</c>.
        /// </param>
        public void DeserializeHistoryFields(Memory<byte> data) { }

        internal void AddMessage(IMessage message) {
            throw new NotImplementedException();
        }
    }
}
