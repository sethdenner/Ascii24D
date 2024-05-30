using Engine.Characters;
using Engine.Render;

namespace Engine.Core
{
    /// <summary>
    /// The <c>Stage</c> abstract class organizes services, simulations and
    /// scenes and coordinates their operation.
    /// </summary>
    [Serializable]
    public class Stage(List<Scene> scenes, Scene initialScene) {
        public List<Scene> Scenes {
            get; set;
        } = scenes;
        /// <summary>
        /// A <c>List</c> of instances implementing the <c>IService</c>
        /// interface. Services run asyncronously during the entire lifetime of
        /// the application.
        /// </summary>
        public List<IService> Services { get; set; } = [];
        /// <summary>
        /// A <c>List</c> of instances implementing the <c>ISimulation</c>
        /// interface. These simulations will run syncronously and in the order
        /// that they are defined.
        /// </summary>
        public List<ISimulation> Simulations { get; set; } = [];
        /// <summary>
        /// A <c>List</c> of instances implementing the <c>ISimulation</c>
        /// interface. These simulations will run asyncronously so they should
        /// not usually have side effects that modify other simulations or have
        /// their state depended on by other simulations.
        /// </summary>
        public List<ISimulation> SimulationsAsync { get; set; } = [];
        public Scene InitialScene {
            get;
            set;
        } = initialScene;

        /// <summary>
        /// <c>AddScene</c> adds a scene to the stage.
        /// </summary>
        /// <param name="scene">
        /// The instance implementing <c>IScene</c> to add to the stage.
        /// </param>
        public virtual void AddScene(Scene scene)
        {
            Scenes.Add(scene);
        }
    }
}
