namespace Engine.Core {
    /// <summary>
    /// Declares the interface required to enable history state serialization of
    /// an object.
    /// </summary>
    public interface IHistorySerializable {
        /// <summary>
        /// Implementation of this method should serialzie only the required
        /// fields to restore the state of the game. Immutable fields need not
        /// be saved.
        /// </summary>
        /// <returns>
        /// Binary serialized memory buffer containing the mutable state of the
        /// object.
        /// </returns>
        public Memory<byte> SerializeHistoryFields();
        /// <summary>
        /// Implementation of this method should mirror the implementation of
        /// <c>SerializeHistoryFields</c>. This is the reverse opperation where
        /// we are taking data previously serialized by that method are now
        /// being deserialized and restored in this method.
        /// </summary>
        /// <param name="data">
        /// A memory buffer of data that was previously serialized by this
        /// object using <c>SerialzieHistoryFields</c>
        /// </param>
        public void DeserializeHistoryFields(Memory<byte> data);
    }
}
