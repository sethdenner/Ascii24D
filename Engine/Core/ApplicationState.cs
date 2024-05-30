using Engine.Characters;
using Engine.Render;

namespace Engine.Core {
    [Serializable]
    public abstract class ApplicationState(Stage initialStage) : IApplicationState {
        public long FrameCount {
            get; set;
        } = 0;
        public Sprite Framebuffer {
            get; set;
        } = new();
        public List<Stage> LoadedStages {
            get; set;
        } = [];
        public Stage CurrentStage {
            get; set;
        } = initialStage;
        public List<Scene> LoadedScenes {
            get; set;
        } = [];
        public Scene CurrentScene {
            get; set;
        } = initialStage.InitialScene;
    }
}
