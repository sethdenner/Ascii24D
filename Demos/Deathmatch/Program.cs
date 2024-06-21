using Engine.Characters;
using Engine.Characters.UI;
using Engine.Core;
using Engine.Core.ECS;
using Engine.Input;
using Engine.Native;
using Engine.Render;
using SharpDX.DirectInput;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Deathmatch {
    internal class DeathmatchApplication(
        long tickRate,
        long simulationStep,
        Stage initialStage
    ) : Application(
        tickRate,
        simulationStep,
        initialStage
    ) {
    }
    internal struct FontDebuggerComponent(int spriteEntityID) {
        public int SpriteEntityID = spriteEntityID;
        public int PageDelta = 0;
        public int CurrentPage;
        public int TotalPages;
        public long UpdateEveryTicks;
        public long TicksSinceLastUpdate;
    }
    internal class FontDebuggerEntity : Entity {
        public FontDebuggerEntity() {
            Messenger<KeyboardMessage.Delegate>.Register((device, update) => {
                var component = GetComponent<FontDebuggerComponent>();
                if (
                    update.IsPressed &&
                    update.Key == SharpDX.DirectInput.Key.Left
                ) {
                    component.PageDelta -= 1;
                } else if (
                    update.IsPressed &&
                    update.Key == SharpDX.DirectInput.Key.Right
                ) {
                    component.PageDelta += 1;
                }
                SetComponent(component);
            });
        }
    }
    internal class FontDebuggerSystem : System<FontDebuggerComponent> {
        public override void Cleanup() { }

        public override void SetupComponent(
            ref FontDebuggerComponent component
        ) {
            int width = 16;
            int height = 17;
            ConsolePixel[] bufferPixels = new ConsolePixel[width * height];
            for (int i = 0; i < 256; ++i) {
                bufferPixels[i] = new() {
                    ForegroundColorIndex = 8,
                    BackgroundColorIndex = 9,
                    CharacterCode = (ushort)(i + (component.CurrentPage * 256))
                };
            }
            var pageString =
                component.CurrentPage.ToString() +
                "/" +
                component.TotalPages.ToString();
            ;
            for (int i = 0; i < pageString.Length; ++i) {
                bufferPixels[i + 256] = new() {
                    ForegroundColorIndex = 8,
                    BackgroundColorIndex = 9,
                    CharacterCode = pageString[i]
                };
            }

            MessageOutbox.Add(new UpdateSpritePixelsMessage(
                component.SpriteEntityID,
                width,
                height,
                bufferPixels
            ));
        }

        public override void UpdateComponent(
            ref FontDebuggerComponent component,
            long step,
            bool headless = false
        ) {
            component.CurrentPage += component.PageDelta;
            if (component.CurrentPage < 0) {
                component.CurrentPage =
                    component.TotalPages + component.CurrentPage + 1;
            } else if (component.CurrentPage > component.TotalPages) {
                component.CurrentPage -= component.TotalPages + 1;
            }
            component.PageDelta = 0;
            /* 
            character.TicksSinceLastUpdate += step;
            if (character.TicksSinceLastUpdate > character.UpdateEveryTicks) {
                character.TicksSinceLastUpdate -= character.UpdateEveryTicks;
                ++character.CurrentPage;
                if (character.CurrentPage >= character.TotalPages) {
                    character.CurrentPage = 0;
                }
            }
            */
        }
    }
    internal struct PlayerControlComponent(
        int playerEntityID
    ) {
        public int PlayerEntityID = playerEntityID;
        public Vector3 Position = Vector3.Zero;
        public int MoveDeltaX = 0;
        public int MoveDeltaY = 0;
        public float MoveSpeed = 0.01f;
        public Vector3 MoveVector = new();
        public Guid DeviceID = Guid.Empty;
        public Matrix4x4 WorldMatrix = Matrix4x4.Identity;
    }
    internal class PlayerControlEntity : Entity {
        public PlayerControlEntity(int playerEntityID) {
            AddComponent(new PlayerControlComponent(
                playerEntityID
            ));
            Messenger<KeyboardMessage.Delegate>.Register((device, update) => {
                var component = GetComponent<PlayerControlComponent>();
                if (Guid.Empty == component.DeviceID) {
                    component.DeviceID = device.DeviceGuid;
                }
                if (component.DeviceID != device.DeviceGuid) {
                    return;
                }
                component.MoveDeltaX = 0;
                component.MoveDeltaY = 0;
                if (
                    update.Key == SharpDX.DirectInput.Key.Left ||
                    update.Key == SharpDX.DirectInput.Key.A
                ) {
                    if (update.IsPressed) {
                        component.MoveDeltaX -= 1;
                    } else {
                        component.MoveDeltaX += 1;
                    }
                }
                if (
                    update.Key == SharpDX.DirectInput.Key.Right ||
                    update.Key == SharpDX.DirectInput.Key.D
                ) {
                    if (update.IsPressed) {
                        component.MoveDeltaX += 1;
                    } else {
                        component.MoveDeltaX -= 1;
                    }
                }
                if (
                    update.Key == SharpDX.DirectInput.Key.Up ||
                    update.Key == SharpDX.DirectInput.Key.W
                ) {
                    if (update.IsPressed) {
                        component.MoveDeltaY += 1;
                    } else {
                        component.MoveDeltaY -= 1;
                    }
                }
                if (
                    update.Key == SharpDX.DirectInput.Key.Down ||
                    update.Key == SharpDX.DirectInput.Key.S
                ) {
                    if (update.IsPressed) {
                        component.MoveDeltaY -= 1;
                    } else {
                        component.MoveDeltaY += 1;
                    }
                }
                component.MoveVector = new Vector3(
                    component.MoveVector.X + component.MoveDeltaX,
                    component.MoveVector.Y + component.MoveDeltaY,
                    0
                );
                SetComponent(component);
            });
        }
    }
    internal class UpdateWorldMatrixMessage(
        int entityID,
        Matrix4x4 world
    ) : Message {
        public delegate void Delegate(
            int entityID,
            Matrix4x4 world
        );

        public int EntityID = entityID;
        public Matrix4x4 World = world;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                EntityID,
                World
            );
        }
    }
    internal class PlayerControlSystem : System<PlayerControlComponent> {
        public override void Cleanup() {
        }

        public override void SetupComponent(
            ref PlayerControlComponent component
        ) {
        }
        public override void UpdateComponent(
            ref PlayerControlComponent component,
            long step,
            bool headless = false
        ) {
            if (Vector3.Zero != component.MoveVector) {
                component.Position += Vector3.Normalize(
                    component.MoveVector
                ) * component.MoveSpeed;
                component.WorldMatrix = Matrix4x4.CreateTranslation(
                    component.Position
                );
                MessageOutbox.Add(new UpdateWorldMatrixMessage(
                   component.PlayerEntityID,
                   component.WorldMatrix
               ));
            }
        }
    }
    internal struct ArenaComponent(
        int spriteEntityID,
        int width,
        int height
    ) {
        public int SpriteEntityID = spriteEntityID;
        public int Width = width;
        public int Height = height;
    }
    internal class ArenaEntity : Entity {
        public ArenaEntity(int spriteEntityID, int width, int height) {
            AddComponent(new ArenaComponent(
                spriteEntityID,
                width,
                height
            ));
        }
    }
    internal class ArenaSystem : System<ArenaComponent> {
        public override void Cleanup() { }
        public override void SetupComponent(ref ArenaComponent component) {
            ConsolePixel[] bufferPixels = new ConsolePixel[
                component.Width * component.Height
            ]; 
            for (int j = 0; j < bufferPixels.Length; ++j) {
                bufferPixels[j] = new Engine.Native.ConsolePixel() {
                    ForegroundColorIndex = 7,
                    BackgroundColorIndex = 8,
                    CharacterCode = 0 == j % 3 || 0 == j % 8 ? '+' : ' '
                };
            }

            MessageOutbox.Add(new UpdateSpritePixelsMessage(
                component.SpriteEntityID,
                component.Width,
                component.Height,
                bufferPixels
            ));
        }

        public override void UpdateComponent(
            ref ArenaComponent component,
            long step,
            bool headless = false
        ) { }
    }
    internal struct CameraComponent(
        Vector3 position,
        Vector3 focusPosition,
        bool active = true
    ) {
        public Vector3 Position = position;
        public Vector3 FocusPosition = focusPosition;
        public bool Active = active;
    }
    internal class UpdateCameraPositionMessage(
        int cameraEntityID,
        Vector3 position
    ) : Message {
        public delegate void Delegate(
            int cameraEntityID,
            Vector3 position
        );
        public int CameraEntityID = cameraEntityID;
        public Vector3 Position = position;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                CameraEntityID,
                Position
            );
        }
    }
    internal class UpdateCameraFocusPositionMessage(
        int cameraEntityID,
        Vector3 focusPosition
    ) : Message {
        public delegate void Delegate(
            int cameraEntityID,
            Vector3 focusPosition
        );
        public int CameraEntityID = cameraEntityID;
        public Vector3 FocusPosition = focusPosition;
        public override void Send() {
            Messenger<Delegate>.Trigger?.Invoke(
                CameraEntityID,
                FocusPosition
            );
        }
    }
    internal class CameraEntity : Entity {
        public CameraEntity(
            Vector3 position,
            Vector3 focusPosition
        ) {
            AddComponent(new CameraComponent(
                position,
                focusPosition
            ));

            Message.Register<UpdateCameraPositionMessage.Delegate>(
                (cameraEntityID, position) => {
                    if (cameraEntityID == EntityID) {
                        var component = GetComponent<CameraComponent>();
                        component.Position = position;
                        SetComponent(component);
                    }
                }
            );
            Message.Register<UpdateCameraFocusPositionMessage.Delegate>(
                (cameraEntityID, focusPosition) => {
                    if (cameraEntityID == EntityID) {
                        var component = GetComponent<CameraComponent>();
                        component.FocusPosition = focusPosition;
                        SetComponent(component);
                    }
                }
            );
        }
    }
    internal class CameraSystem : System<CameraComponent> {
        public Matrix4x4 CameraMatrix = Matrix4x4.Identity;
        public override void Cleanup() { }
        public override void SetupComponent(ref CameraComponent component) { }
        public override void BeforeUpdates(long step, bool headless = false) {
            CameraMatrix = Matrix4x4.Identity;
        }
        public override void UpdateComponent(
            ref CameraComponent component,
            long step,
            bool headless = false
        ) {
            if (component.Active) {
                CameraMatrix = View(component);
            }
        }

        public override void AfterUpdates(long step, bool headless = false) {
            MessageOutbox.Add(new UpdateCameraMatrixMessage(
                CameraMatrix
            ));
        }
        public Matrix4x4 View(CameraComponent component) {
            Vector3 forward = Vector3.Normalize(
                component.Position - component.FocusPosition
            );
            Vector3 right = Vector3.Normalize(Vector3.Cross(
                new(0, 1, 0), forward
            ));
            Vector3 up = Vector3.Normalize(Vector3.Cross(
                forward, right
            ));

            float translationX = Vector3.Dot(component.Position, right);
            float translationY = Vector3.Dot(component.Position, up);
            float translationZ = Vector3.Dot(component.Position, forward);

            return new Matrix4x4(
                right.X, up.X, forward.X, 0,
                right.Y, up.Y, forward.Y, 0,
                right.Z, up.Z, forward.Z, 0,
                -translationX, -translationY, -translationZ, 1
            );
        }
    }
    internal struct PlayerComponent(
        int spriteEntityID,
        int playerCameraEntityID
    ) {
        public int SpriteEntityID = spriteEntityID;
        public int PlayerCameraEntityID = playerCameraEntityID;
        public Matrix4x4 WorldMatrix = Matrix4x4.Identity;
    }
    internal class PlayerEntity : Entity {
        public PlayerEntity(
            int spriteEntityID,
            int playerCameraEntityID
        ) {
            AddComponent(new PlayerComponent(
                spriteEntityID,
                playerCameraEntityID
            ));

            Message.Register<UpdateWorldMatrixMessage.Delegate>((
                playerEntityID,
                world
            ) => {
                if (playerEntityID == EntityID) {
                    var component = GetComponent<PlayerComponent>();
                    component.WorldMatrix = world;
                    SetComponent(component);

                    Vector3 worldPosition = component.WorldMatrix.Translation;
                    new UpdateSpritePositionMessage(
                        component.SpriteEntityID,
                        worldPosition
                    ).Send();
                    new UpdateCameraPositionMessage(
                        component.PlayerCameraEntityID,
                        new Vector3(
                            worldPosition.X,
                            worldPosition.Y,
                            worldPosition.Z + 2
                        )
                    ).Send();
                    new UpdateCameraFocusPositionMessage(
                        component.PlayerCameraEntityID,
                        worldPosition
                    ).Send();
                }
            });
        }
    }
    internal class Program {
        static async Task Main(string[] args) {
            int framebufferWidth = 80;
            int framebufferHeight = 65;
            int fontWidth = 16;
            int fontHeight = 16;
            int tickRate = 166666;
            int simulationStep = tickRate;
            PaletteInfo[] palette = [
                new() {
                    Index = 0,
                    Color = new() {
                        R = 0,
                        G = 0,
                        B = 0
                    }
                }
            ];
            SpriteEntity arenaSpriteEntity = new(
                0, 0, Vector3.Zero, true, []
            );
            ArenaEntity arenaEntity = new(
                arenaSpriteEntity.EntityID,
                200,
                200
            );
            SpriteEntity playerSpriteEntity = new(
                1, 1, Vector3.Zero, true, [new() {
                    ForegroundColorIndex = 11,
                    BackgroundColorIndex = 0,
                    CharacterCode = 2
                }]
            );
            CameraEntity cameraEntity = new(Vector3.Zero, Vector3.Zero);
            PlayerEntity playerEntity = new(
                playerSpriteEntity.EntityID,
                cameraEntity.EntityID
            );
            PlayerControlEntity playerControlEntity = new(
                playerEntity.EntityID
            );
            DirectInputEntity directInputEntity = new();
            ConsoleRenderEntity consoleRenderEntity = new(
                framebufferWidth,
                framebufferHeight,
                fontWidth,
                fontHeight,
                new() {
                    ForegroundColorIndex = 0,
                    BackgroundColorIndex = 0,
                    CharacterCode = ' '
                },
                palette
            );
            var scene = new Scene() {
                Entities = [
                    directInputEntity,
                    arenaSpriteEntity,
                    playerSpriteEntity,
                    arenaEntity,
                    playerEntity,
                    playerControlEntity,
                    cameraEntity
                ],
                Palette = palette
            };
            var stage = new Stage([scene], scene) {
                Systems = [
                    new DirectInputSystem(),
                    new PlayerControlSystem(),
                    new ArenaSystem(),
                    // new FontDebuggerSystem(),
                    new CameraSystem(),
                    new SpriteSystem(
                        consoleRenderEntity.EntityID,
                        framebufferWidth,
                        framebufferHeight
                    ),
                    new ConsoleRenderSystem()
                ]
            };
            var app = new DeathmatchApplication(
                tickRate,
                simulationStep,
                stage
            );
            var appTask = app.Startup();
            await appTask;
        }
    }
}
