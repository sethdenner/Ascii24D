using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Engine.Core {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameTime"></param>
    public delegate void FrameEndMessage(long frameTime);
    /// <summary>
    /// <c>Application</c> is the highest level class of the engine in the
    /// sense that it encapsulates all services and simulations and manages
    /// their startup execution and teardown.
    /// </summary>
    /// <param name="tickRate">
    /// The tick rate is how often a simulation step should be executed
    /// measured in 100s of nanoseconds (10,000 ticks = 1ms).
    /// </param>
    /// <param name="simulationStep">
    /// The simulation step is the fixed time step to use for each simulation
    /// step measured in 100s of nanoseconds (10,000 ticks = 1ms). For
    /// "real-time" execution <c>simulationStep</c> should equal
    /// <c>tickRate</c>. You can make the simulation more precise by lowering
    /// the simulation step but the simulation will update in slow-motion if
    /// <c>simulationStep</c> is less than <c>tickRate</c>. Conversely if the
    /// simulation step is larger than the tick rate the simulation will be less
    /// accurate but will execute in fast-forward.
    /// </param>
    /// <param name="applicationState">
    /// Instance of <c>IApplicationState</c> that implements interfaces
    /// required by all simulations and services.
    /// </param>
    public abstract class Application(
        long tickRate,
        long simulationStep,
        Stage initialStage,
        IApplicationState state
    ) {
        /// <summary>
        /// The tick rate is how often a simulation step should be executed
        /// measured in 100s of nanoseconds (10,000 ticks = 1ms).
        /// </summary>
        public long TickRate = tickRate;
        /// <summary>
        /// The simulation step is the fixed time step to use for each simulation
        /// step measured in 100s of nanoseconds (10,000 ticks = 1ms). For
        /// "real-time" execution <c>SimulationStep</c> should equal
        /// <c>TickRate</c>. You can make the simulation more precise by lowering
        /// the simulation step but the simulation will update in slow-motion if
        /// <c>SimulationStep</c> is less than <c>TickRate</c>. Conversely if the
        /// simulation step is larger than the tick rate the simulation will be less
        /// accurate but will execute in fast-forward.
        /// </summary>
        public long SimulationStep = simulationStep;
        /// <summary>
        /// 
        /// </summary>
        readonly IApplicationState State = state;
        /// <summary>
        /// 
        /// </summary>
        public List<Stage> LoadedStages = [];
        /// <summary>
        /// 
        /// </summary>
        public Stage CurrentStage = initialStage;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning = false;
#pragma warning disable SYSLIB0011 // We know what we're doing. Never
                                   // deserialize network data with this.
        BinaryFormatter Formatter = new();
#pragma warning restore SYSLIB0011
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [RequiresDynamicCode("Uses BinaryFormatter.")]
        [RequiresUnreferencedCode("Uses BinaryFormatter.")]
        public async Task LoadStage(string filename) {
            long fileSize = new FileInfo(filename).Length;
            int chunkSize = int.MaxValue;
            int numChunks;
            if (fileSize > chunkSize) {
                numChunks = (int)(fileSize / (long)int.MaxValue);
                if (fileSize % (long)int.MaxValue > 0) {
                    ++numChunks;
                }
            } else {
                numChunks = 1;
                chunkSize = (int)fileSize;
            }
            MemoryStream fileBytes = new();
            for (int i = 0; i < numChunks; i++) {
                var buffer = new byte[chunkSize];
                var stream = new FileStream(
                    filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None,
                    chunkSize,
                    FileOptions.Asynchronous
                );
                stream.Seek(i * chunkSize, SeekOrigin.Begin);
                int bytesRead = await stream.ReadAsync(
                    buffer.AsMemory(
                        i * chunkSize, chunkSize
                    )
                );
                await fileBytes.WriteAsync(
                    buffer.AsMemory(
                        0, bytesRead
                    )
                );
            }
            using (fileBytes) {
                Stage? stage = Formatter.Deserialize(fileBytes) as Stage;
            }
        }
        /// <summary>
        /// Starts the application. Starts all services asyncronously, calls the
        /// <c>Setup</c> method for all simulations and starts the simulation
        /// loop. Runs asyncronously and will not complete until the simulation
        /// loop completes and all service tasks have completed. Call
        /// <c>Shutdown</c> to cleanup and close the application.
        /// </summary>
        public async Task Startup() {
            List<Task> serviceTasks = [];
            for (int i = 0; i < CurrentStage.Services.Count; ++i) {
                var service = CurrentStage.Services[i];
                service.Setup();
                serviceTasks.Add(
                    service.Start()
                );
            }

            for (int i = 0; i < CurrentStage.Simulations.Count; ++i) {
                var simulation = CurrentStage.Simulations[i];
                simulation.Setup(State);
            }

            for (int i = 0; i < CurrentStage.SimulationsAsync.Count; ++i) {
                var simulation = CurrentStage.SimulationsAsync[i];
                simulation.Setup(State);
            }

            await StartSimulationLoop();
            await Task.WhenAll(serviceTasks);
        }
        /// <summary>
        /// Cleans up and terminates execution of all services and simulations.
        /// </summary>
        public void Shutdown() {
            IsRunning = false;

            for (int i = CurrentStage.SimulationsAsync.Count; i >= 0; --i) {
                var simulation = CurrentStage.SimulationsAsync[i];
                simulation.Cleanup(State);
            }

            for (int i = CurrentStage.Simulations.Count; i >= 0; --i) {
                var simulation = CurrentStage.SimulationsAsync[i];
                simulation.Cleanup(State);
            }

            for (int i = CurrentStage.Services.Count - 1; i >= 0; --i) {
                var service = CurrentStage.Services[i];
                service.Stop();
                service.Cleanup();
            }
        }
        /// <summary>
        /// Starts running all simulations in a loop that will advance all
        /// simulations one <c>SimulationStep</c> forward with the frequency
        /// defined by <c>TickRate</c>.
        /// </summary>
        public async Task StartSimulationLoop() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IsRunning = true;

            long prevTime = 0;
            long accumulator = 0;
            while (IsRunning) {
                long time = stopwatch.ElapsedTicks;
                long timeDelta = time - prevTime;
                accumulator += timeDelta;
                while (accumulator > TickRate) {
                    await UpdateSimulations(SimulationStep);
                    accumulator -= TickRate;
                }

                Messenger<FrameEndMessage>.Trigger?.Invoke(
                    stopwatch.ElapsedTicks - time
                );
                prevTime = time;
            }
        }
        /// <summary>
        /// Updates all simulations. First asyncronous simulation tasks are
        /// started and then the rest of the simulations are updated
        /// syncronously.
        /// </summary>
        /// <param name="step">
        /// The time step to advance the simulation in ticks. Ticks are 100s of
        /// nanoseconds (10,000 ticks = 1ms).
        /// </param>
        /// <param name="headless">
        /// If true indicates that the simulation will not be intended to be
        /// displayed so the step can be optimized by excluding any elements of
        /// the simulation that are uncessary for determining the state of
        /// future steps.
        /// </param>
        public async Task UpdateSimulations(long step, bool headless=false) {
            Task[] simulationTasks = new Task[
                CurrentStage.SimulationsAsync.Count
            ];
            for (int i = 0; i < CurrentStage.SimulationsAsync.Count; ++i) {
                var simulation = CurrentStage.SimulationsAsync[i];
                simulationTasks[i] = simulation.SimulateAsync(
                    State,
                    step,
                    headless
                );
            }
            for (int i = 0; i < CurrentStage.Simulations.Count; ++i) {
                var simulation = CurrentStage.Simulations[i];
                simulation.Simulate(State, step, headless);
            }

            await Task.WhenAll(simulationTasks);
        }
    }
}
