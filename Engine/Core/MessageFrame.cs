namespace Engine.Core
{
    /// <summary>
    /// Maintains an array of IMessages objects that can saved to send at a
    /// later time.
    /// </summary>
    public struct MessageFrame() : IHistorySerializable
    {
        public const int INITIAL_MESSAGE_CAPACITY = 50;
        public const int MESSAGE_GROW_SIZE = 10;
        public IMessage[] Messages = new IMessage[INITIAL_MESSAGE_CAPACITY];
        public int MessageCount = 0;
        /// <summary>
        /// Serializes all mutable fields for use with application history.
        /// </summary>
        /// <returns>
        /// Binary serialized data for all mutable fields.
        /// </returns>
        public readonly Memory<byte> SerializeHistoryFields() {
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
        public readonly void DeserializeHistoryFields(Memory<byte> data) { }

        public void AddMessage(IMessage message)
        {
            if (MessageCount >= Messages.Length)
            {
                IMessage[] newMessageBuffer = new IMessage[
                    Messages.Length + MESSAGE_GROW_SIZE
                ];
                Messages.AsSpan().CopyTo(
                    newMessageBuffer.AsSpan(0, Messages.Length)
                );
                Messages = newMessageBuffer;
            }
            Messages[MessageCount++] = message;
        }

        public readonly void PlayMessages() {
            for (int i = 0; i < MessageCount; ++i) {
                Messages[i].Send();
            }
        }
    }
}
