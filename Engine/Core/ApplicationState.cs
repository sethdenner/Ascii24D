using Engine.Characters;
using Engine.Render;
using System.Numerics;

namespace Engine.Core {
    [Serializable]
    public abstract class ApplicationState(
        Stage initialStage,
        int framebufferWidth,
        int framebufferHeight
    ) : IApplicationState {
        public long FrameCount {
            get; set;
        } = 0;
        public int FramebufferWidth {
            get; set;
        } = framebufferWidth;
        public int FramebufferHeight {
            get; set;
        } = framebufferHeight;
        public Matrix4x4 ViewMatrix {
            get; set;
        } = Matrix4x4.Identity;
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
