using Engine.Render;
using System.Numerics;

namespace Engine.Core {
    /// <summary>
    /// 
    /// </summary>
    public interface IApplicationState {
        public long FrameCount {
            get; set;
        }
        public int FramebufferWidth {
            get; set;
        }
        public int FramebufferHeight {
            get; set;
        }
        public Matrix4x4 ViewMatrix {
            get; set;
        }
        public List<Stage> LoadedStages {
            get; set;
        }
        public Stage CurrentStage {
            get; set;
        }
        public List<Scene> LoadedScenes {
            get; set;
        }
        public Scene CurrentScene {
            get; set;
        }
    }
}
