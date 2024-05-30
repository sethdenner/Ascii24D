using Engine.Render;

namespace Engine.Core {
    /// <summary>
    /// 
    /// </summary>
    public interface IApplicationState {
        public long FrameCount {
            get; set;
        }
        public Sprite Framebuffer {
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
