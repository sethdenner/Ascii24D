namespace Engine.Core {
    /// <summary>
    /// Declares the methods and properties that are required to implement for
    /// opperating a simulation.
    /// </summary>
    public interface ISimulation {
        /// <summary>
        /// The <c>Setup</c> method implementation is where all state
        /// prerequsite to run the simulation should be genareated.
        /// </summary>
        /// <param name="state">
        /// State object shared between simulations.
        /// </param>
        public void Setup(IApplicationState state);
        /// <summary>
        /// The <c>Cleanup</c> method implementation is where all resources
        /// should be released.
        /// </summary>
        /// <param name="state">
        /// State object shared between simulations.
        /// </param>
        public void Cleanup(IApplicationState state);
        /// <summary>
        /// Advance the simulation one step forward.
        /// </summary>
        /// <param name="state">
        /// The application state to simulate forward.
        /// </param>
        /// <param name="step">
        /// The time step to advance the simulation in ticks. One tick is 100
        /// nanoseconds (10,000 ticks = 1ms)
        /// </param>
        /// <param name="headless">
        /// A flag that the simulation can use to optimize steps that are not
        /// intended to be displayed to a user. Visual elements that future
        /// simulation steps do not rely on might not need to be simulated.
        /// </param>
        public void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        );
        /// <summary>
        /// Asyncronous method that advances the simulation one step forward.
        /// </summary>
        /// <param name="state">
        /// The application state to simulate forward.
        /// </param>
        /// <param name="step">
        /// The time step to advance the simulation in ticks. One tick is 100
        /// nanoseconds (10,000 ticks = 1ms)
        /// </param>
        /// <param name="headless">
        /// A flag that the simulation can use to optimize steps that are not
        /// intended to be displayed to a user. Visual elements that future
        /// simulation steps do not rely on might not need to be simulated.
        /// </param>
        /// <returns>
        /// An asyncronous Task object.
        /// </returns>
        public Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        );
    }
}
