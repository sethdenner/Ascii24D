namespace Engine.Core {
    /// <summary>
    /// Stores a single frame of serialized state and input data for use in
    /// state history.
    /// </summary>
    public class HistoryFrame {
        /// <summary>
        /// The number of the frame that the state and input data belong to.
        /// </summary>
        public int FrameNumber {
            get; set;
        }
        /// <summary>
        /// Binary serialized state data for a single application frame.
        /// </summary>
        public Memory<byte> State {
            get; set; 
        }
        /// <summary>
        /// Binary serialized input data for a single application frame.
        /// </summary>
        public Memory<byte> Input {
            get; set;
        }
    }
}
