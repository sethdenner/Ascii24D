using Engine.Core;
using Engine.Core.ECS;
using Engine.Native;
using System.ComponentModel;
using System.Numerics;

namespace Engine.Render {
    public class SpriteSystem : System<SpriteComponent> {
        /// <summary>
        /// 
        /// </summary>
        public ConsolePixel[] Framebuffer;
        /// <summary>
        /// 
        /// </summary>
        public int ConsoleRenderEntityID;
        /// <summary>
        /// 
        /// </summary>
        public int FramebufferWidth;
        /// <summary>
        /// 
        /// </summary>
        public int FramebufferHeight;
        /// <summary>
        /// 
        /// </summary>
        public Matrix4x4 CameraViewportMatrix;
        /// <summary>
        /// 
        /// </summary>
        public Matrix4x4 CameraMatrix;
        /// <summary>
        /// 
        /// </summary>
        public Matrix4x4 ViewportMatrix;

        public SpriteSystem(
            int consoleRenderEntityID,
            int framebufferWidth,
            int framebufferHeight
        ) {
            Framebuffer = new Native.ConsolePixel[
                framebufferWidth * framebufferHeight
            ];
            ConsoleRenderEntityID = consoleRenderEntityID;
            FramebufferWidth = framebufferWidth;
            FramebufferHeight = framebufferHeight;
            CameraMatrix = Matrix4x4.Identity;
            ViewportMatrix = Matrix4x4.CreateViewport(
                0, 0,
                framebufferWidth, framebufferHeight,
                -1, 1
            );
            CameraViewportMatrix = Matrix4x4.Identity;

            Message.Register<ApplicationWindowResizedMessage.Delegate>(
                ResizeFramebuffer
            );
            Message.Register<UpdateCameraMatrixMessage.Delegate>(
                UpdateCameraMatrix
            );
        }

        public void UpdateCameraMatrix(Matrix4x4 camera) {
            CameraMatrix = camera;
        }
        public override void Cleanup() { }

        public override void SetupComponent(ref SpriteComponent component) { }

        public override void UpdateComponent(
            ref SpriteComponent component,
            long step,
            bool headless = false
        ) {
            if (headless) { return; }
            Vector3 screenPosition;
            if (component.TransformToScreenSpace) {
                screenPosition = Vector3.Transform(
                    component.WorldPosition,
                    CameraViewportMatrix
                );
            } else {
                screenPosition = component.WorldPosition;
            }
            Vector2 screenCoordinates = new(
                screenPosition.X,
                screenPosition.Y
            );

            // Hack to not update screen coordinates if moving diagonally unless
            // movement in both direction occurs in order to smooth movement.
            double screenCoordinateX = Math.Floor(Math.Abs(screenCoordinates.X));
            double screenCoordinateY = Math.Floor(Math.Abs(screenCoordinates.Y));
            double prevScreenCoordinateX = Math.Floor(
                Math.Abs(component.ScreenCoordinates.X)
            );
            double prevScreenCoordinateY = Math.Floor(
                Math.Abs(component.ScreenCoordinates.Y)
            );
            double screenDeltaX = Math.Abs(screenCoordinateX - prevScreenCoordinateX);
            double screenDeltaY = Math.Abs(screenCoordinateY - prevScreenCoordinateY);
            if (
                screenDeltaX == 0.0 ||
                screenDeltaY == 0.0 ||
                (screenDeltaX >= 1.0) &&
                (screenDeltaY >= 1.0)
            ) {
                component.ScreenCoordinates = screenCoordinates;
            }

            // End hack
            Helper.Methods.BlendTextures(
                Framebuffer,
                FramebufferWidth,
                FramebufferHeight,
                component.BufferPixels,
                component.Width,
                component.Height,
                component.ScreenCoordinates
            );
        }

        public override void BeforeUpdates(long step,  bool headless = false) {
            if (headless) { return; }

            CameraViewportMatrix = CameraMatrix * ViewportMatrix;
            Framebuffer = new ConsolePixel[Framebuffer.Length];
        }

        public override void AfterUpdates(long step, bool headless = false) {
            if (headless) { return; }

            new UpdateFramebufferMessage(
                ConsoleRenderEntityID,
                Framebuffer
            ).Send();
        }

        public void ResizeFramebuffer(SMALL_RECT newSizeRect) {
            int width = newSizeRect.Right - newSizeRect.Left;
            int height = newSizeRect.Bottom - newSizeRect.Top;
            Framebuffer = new ConsolePixel[width * height];
            FramebufferWidth = width;
            FramebufferHeight = height;
        }

    }
}
