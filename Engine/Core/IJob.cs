namespace Engine.Core {
    /// <summary>
    /// Declares the methods and properties required to implement a job.
    /// Jobs are asyncronous functions that run for the lifetime of the
    /// application at a set frequency..
    /// </summary>
    public interface IJob : IBackgroundTask{
        /// <summary>
        /// The period of time that the job should wait before updating
        /// again in milliseconds.
        /// </summary>
        public int Period {
            get; set;
        }
        /// <summary>
        /// Starts the job loop. Overrides should create any resources
        /// required for service operation before calling this method. The loop
        /// pauses after each iteration for the length of time defined by
        /// <c>Period</c> in milliseconds.
        /// </summary>
        public new async Task Start() {
            while (IsRunning) {
                await Update();
                Thread.Sleep(Period);
            }
        }
    }
}
