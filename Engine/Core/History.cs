namespace Engine.Core {
    /// <summary>
    /// <c>History</c> class maintains a historical record of game state and
    /// player input where each frame is stored an a ring buffer of user defined
    /// size.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="historyLength">
    /// Indicates the number of history frames that should be remembered. Used
    /// for the size of history ring buffers.
    /// </param>
    public class History(int historyLength = 7) {
        /// <summary>
        /// The number of history frames to maintain.
        /// </summary>
        public int HistoryLength {
            get; set;
        } = historyLength;
        /// <summary>
        /// The current frame number we are on.
        /// </summary>
        public int FrameCount {
            get; set;
        } = 0;
        /// <summary>
        /// Array of memory buffers containing the binary serialized state data
        /// for remembered history frames.
        /// </summary>
        public Memory<byte>[] StateHistory {
            get; set;
        } = new Memory<byte>[historyLength];
        /// <summary>
        /// Array of memory buffers containing the binary serialized local and
        /// networked input state for remembered history frames.
        /// </summary>
        public Memory<byte>[] InputHistory {
            get; set;
        } = new Memory<byte>[historyLength];
        /// <summary>
        /// Adds binary serialized state and input data to a new history frame.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="input"></param>
        public void AddFrame(Memory<byte> state, Memory<byte> input) {
            ++FrameCount;
            StateHistory[FrameCount % HistoryLength] = state;
            InputHistory[FrameCount % HistoryLength] = input;
        }
    }
}
