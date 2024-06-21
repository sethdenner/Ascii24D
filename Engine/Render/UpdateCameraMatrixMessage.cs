using Engine.Core;
using System.Numerics;

namespace Engine.Render {
    public class UpdateCameraMatrixMessage(Matrix4x4 camera) : Message {
        public delegate void Delegate(Matrix4x4 camera);
        Matrix4x4 Camera = camera;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(Camera);
        }
    }
}
