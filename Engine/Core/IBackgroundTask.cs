using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core {
    public interface IBackgroundTask {
        /// <summary>
        /// Flag that tells the job if it should continue running. Setting
        /// this to false will terminate the job.
        /// </summary>
        public bool IsRunning {
            get; set;
        }
        /// <summary>
        /// Starts the job loop. Overrides should create any resources
        /// required for service operation before calling this method.
        /// </summary>
        public async Task Start() {
            while (IsRunning) {
                await Update();
            }
        }
        /// <summary>
        /// Terminates the job loop. Overrides should clean up any
        /// resources after calling this method.
        /// </summary>
        public void Stop() {
            IsRunning = false;
        }
        /// <summary>
        /// Implement the update method with logic that updates the job
        /// state.
        /// </summary>
        public Task Update();
    }
}
