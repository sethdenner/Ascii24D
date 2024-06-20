using Engine.Characters;
using Engine.Core;
using Engine.Input;
using Engine.Render;
using SharpDX.DirectInput;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Deathmatch {
    internal class DeathmatchApplicationState(
        Stage initialStage,
        int framebufferWidth,
        int framebufferHeight
    ) : ApplicationState(
        initialStage,
        framebufferWidth,
        framebufferHeight
    ) {
    }
    internal class DeathmatchApplication(
        long tickRate,
        long simulationStep,
        Stage initialStage,
        IApplicationState state
    ) : Application(
        tickRate,
        simulationStep,
        initialStage,
        state
    ) {
    }
    internal class FontDebuggerSimulation : Simulation {
        public int PageDelta = 0;
        public override void Cleanup(IApplicationState state) { }
        public override void Setup(IApplicationState state) {
            Messenger<KeyboardMessage>.Register((device, update) => {
                if (
                    update.IsPressed &&
                    update.Key == SharpDX.DirectInput.Key.Left
                ) {
                    PageDelta -= 1;
                } else if (
                    update.IsPressed &&
                    update.Key == SharpDX.DirectInput.Key.Right
                ) {
                    PageDelta += 1;
                }
            });
        }
        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            for (int i = 0; i < state.CurrentScene.Entities.Count; ++i) {
                if (
                    state.CurrentScene.Entities[i] is not
                    IPageable character
                ) {
                    continue;
                }

                character.CurrentPage += PageDelta;
                if (character.CurrentPage < 0) {
                    character.CurrentPage =
                        character.TotalPages + character.CurrentPage + 1;
                } else if (character.CurrentPage > character.TotalPages) {
                    character.CurrentPage -= character.TotalPages + 1;
                }
                PageDelta = 0;
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
        public override Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
        }
    }
    internal interface IPageable {
        public int CurrentPage {
            get; set;
        }
        public int TotalPages {
            get; set;
        }
        public long UpdateEveryTicks {
            get; set;
        }
        public long TicksSinceLastUpdate {
            get; set;
        }
    }
    internal class FontDebugger : Character, IPageable {
        public int CurrentPage {
            get; set;
        } = 38;
        public int TotalPages {
            get; set;
        } = ushort.MaxValue / 256;
        public long UpdateEveryTicks {
            get; set;
        } = 5000000; // 5 Seconds.
        public long TicksSinceLastUpdate {
            get; set;
        } = 0;
        public override void GenerateSprites() {
            Sprite sprite = new(16, 17);
            for (int i = 0; i < 256; ++i) {
                sprite.BufferPixels[i] = new() {
                    ForegroundColorIndex = 8,
                    BackgroundColorIndex = 9,
                    CharacterCode = (ushort)(i + (CurrentPage * 256))
                };
            }
            var pageString = 
                CurrentPage.ToString() +
                "/" +
                TotalPages.ToString();
            ;
            for (int i = 0; i < pageString.Length; ++i) {
                sprite.BufferPixels[i + 256] = new() {
                    ForegroundColorIndex = 8,
                    BackgroundColorIndex = 9,
                    CharacterCode = pageString[i]
                };
            }
            Sprites.Clear();
            Sprites.Add(sprite);
        }
    }
    internal class SimulationPlayerControl : Simulation {
        public int MoveDeltaX = 0;
        public int MoveDeltaY = 0;
        public const float MoveSpeed = 0.25f;
        public Vector3 MoveVector = new();
        public Guid DeviceID = Guid.Empty;
        public override void Cleanup(IApplicationState state) {
        }

        public override void Setup(IApplicationState state) {
            Messenger<KeyboardMessage>.Register((device, update) => {
                if (Guid.Empty == DeviceID) {
                    DeviceID = device.DeviceGuid;
                }
                if (DeviceID != device.DeviceGuid) {
                    return;
                }
                MoveDeltaX = 0;
                MoveDeltaY = 0;
                if (
                    update.Key == SharpDX.DirectInput.Key.Left ||
                    update.Key == SharpDX.DirectInput.Key.A
                ) {
                    if (update.IsPressed) {
                        MoveDeltaX -= 1;
                    } else {
                        MoveDeltaX += 1;
                    }
                }   
                if (
                    update.Key == SharpDX.DirectInput.Key.Right ||
                    update.Key == SharpDX.DirectInput.Key.D
                ) {
                    if (update.IsPressed) {
                        MoveDeltaX += 1;
                    } else {
                        MoveDeltaX -= 1;
                    }
                }
                if (
                    update.Key == SharpDX.DirectInput.Key.Up ||
                    update.Key == SharpDX.DirectInput.Key.W
                ) {
                    if (update.IsPressed) {
                        MoveDeltaY -= 1;
                    } else {
                        MoveDeltaY += 1;
                    }
                } 
                if (
                    update.Key == SharpDX.DirectInput.Key.Down ||
                    update.Key == SharpDX.DirectInput.Key.S
                ) {
                    if (update.IsPressed) {
                        MoveDeltaY += 1;
                    } else {
                        MoveDeltaY -= 1;
                    }
                }
                MoveVector = new Vector3(
                    MoveVector.X + MoveDeltaX,
                    MoveVector.Y + MoveDeltaY,
                    0
                );

            });
        }

        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            for (int i = 0; i < state.CurrentScene.Entities.Count; ++i) {
                if (
                    state.CurrentScene.Entities[i] is not
                    IInputControllable character
                ) {
                    continue;
                }
                if (Vector3.Zero != MoveVector) {
                    character.Position += Vector3.Normalize(MoveVector) * MoveSpeed;
                    if (0 != MoveVector.X && 0 != MoveVector.Y) {
                        // If moving diagonally bump to the closest point on the
                        // pixel diagonal to avoid jittering.
                        double signX = MoveVector.X >= 0 ? 1 : -1;
                        double signY = MoveVector.Y >= 0 ? 1 : -1;
                        double floorX = signX >= 0 ?
                            Math.Floor(character.Position.X) :
                            Math.Ceiling(character.Position.X);
                        double floorY = signY >= 0 ?
                            Math.Floor(character.Position.Y) :
                            Math.Ceiling(character.Position.Y);
                        double subX = character.Position.X - floorX;
                        double subY = character.Position.Y - floorY;
                        double avgSub = (Math.Abs(subX) + Math.Abs(subY)) / 2;
                        double positionX = floorX + avgSub * signX;
                        double positionY = floorY + avgSub * signY;
                        character.Position = new Vector3(
                            (float)positionX,
                            (float)positionY,
                            character.Position.Z
                        );
                    }
                }
            }
        }

        public override Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
        }
    }
    internal interface IInputControllable {
        public Vector3 Position {
            get; set;
        }
    }
    internal class CharacterPlayer : Character, IInputControllable {
        public override void GenerateSprites() {
            Sprite sprite = new(1, 1);
            sprite.SetPixel(0, new() {
                ForegroundColorIndex = 11,
                BackgroundColorIndex = 0,
                CharacterCode = 2
            });
            Sprites.Clear();
            Sprites.Add(sprite);
        }
    }

    internal class CharacterArena : Character {
        public override void GenerateSprites() { }
    }
    internal class SimulationArena : Simulation {
        public override void Cleanup(IApplicationState state) { }

        public override void Setup(IApplicationState state) {
            for (int i = 0; i < state.CurrentScene.Entities.Count; ++i) {
                if (
                    state.CurrentScene.Entities[i] is not
                    CharacterArena character
                ) {
                    continue;
                }

                character.Sprites.Clear();
                Sprite sprite = new(200, 200, 0, 0);
                for (int j = 0; j < sprite.BufferPixels.Length; ++j) {
                    sprite.BufferPixels[j] = new Engine.Native.ConsolePixel() {
                        ForegroundColorIndex = 7,
                        BackgroundColorIndex = 8,
                        CharacterCode = 0 == j % 3 ||0 == j % 8 ? '+' : ' '
                    };
                }
                character.Sprites.Add(sprite);

            }
        }

        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
        }

        public override Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
        }
    }
    internal class CharacterCamera : Character {
        public Character? FocusCharacter = null;
        public bool Active {
            get;
            internal set;
        }

        public Matrix4x4 View() {
            Vector3 targetPosition = null != FocusCharacter ?
                FocusCharacter.Position : new(0, 0, 0);
            Vector3 forward = Vector3.Normalize(
                Position - targetPosition
            );
            Vector3 right = Vector3.Normalize(Vector3.Cross(
                new(0, 1, 0), forward
            ));
            Vector3 up = Vector3.Normalize(Vector3.Cross(
                forward, right
            ));

            float translationX = Vector3.Dot(Position, right);
            float translationY = Vector3.Dot(Position, up);
            float translationZ = Vector3.Dot(Position, forward);

            return new Matrix4x4(
                right.X, up.X, forward.X, 0,
                right.Y, up.Y, forward.Y, 0,
                right.Z, up.Z, forward.Z, 0,
                -translationX, -translationY, -translationZ, 1
            );
        }
        public override void GenerateSprites() { }
    }
    internal class SimulationCamera : Simulation {
        public override void Cleanup(IApplicationState state) {
        }

        public override void Setup(IApplicationState state) {
            for (int i = 0; i < state.CurrentScene.Entities.Count; ++i) {
                if (
                    state.CurrentScene.Entities[i] is not
                    CharacterCamera character
                ) {
                    continue;
                }
                if (null != character.FocusCharacter) {
                    character.Position = new (
                        character.FocusCharacter.Position.X,
                        character.FocusCharacter.Position.Y,
                        4
                    );
                    
                } else {
                    character.Position = new(
                        state.FramebufferWidth / 2,
                        state.FramebufferHeight / 2,
                        4
                    );
                }
            }
        }

        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            CharacterCamera? activeCamera = null;
            for (int i = 0; i < state.CurrentScene.Entities.Count; ++i) {
                if (
                    state.CurrentScene.Entities[i] is
                    CharacterCamera character
                ) {
                    if (null != character.FocusCharacter) {
                        character.Position = new(
                            character.FocusCharacter.Position.X,
                            character.FocusCharacter.Position.Y,
                            character.Position.Z
                        );
                    }
                    if (character.Active) {
                        activeCamera = character;
                    }
                }
            }

            if (null != activeCamera) {
                state.ViewMatrix = activeCamera.View();
            }
        }

        public override Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
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
            Character playerCharacter = new CharacterPlayer() {
                Position = new Vector3() {
                    X = framebufferWidth / 2,
                    Y = framebufferHeight / 2
                }
            };
            var scene = new Scene() {
                Entities = [
                    new CharacterArena(),
                    playerCharacter,
                    new CharacterCamera() { 
                        FocusCharacter = playerCharacter,
                        Active = true
                    },
                    /* new FontDebugger() {
                        Position = new Vector3(
                            framebufferWidth / 2,
                            framebufferHeight / 2,
                            0
                        )
                    } */
                ],
                Palette = [
                    new() {
                        Index = 0,
                        Color = new() {
                            R = 0,
                            G = 0,
                            B = 0
                        }
                    }
                ]
            };
            var stage = new Stage([scene], scene) {
                Systems = [
                    new SimulationDirectInput(),
                    new SimulationPlayerControl(),
                    new SimulationArena(),
                    // new FontDebuggerSimulation(),
                    new SimulationCamera(),
                    new SimulationConsoleRender(
                        framebufferWidth,
                        framebufferHeight,
                        fontWidth,
                        fontHeight,
                        new() {
                            ForegroundColorIndex = 0,
                            BackgroundColorIndex = 0,
                            CharacterCode = ' '
                        }
                    )
                ]
            };
            var state = new DeathmatchApplicationState(
                stage,
                framebufferWidth,
                framebufferHeight
            );
            var app = new DeathmatchApplication(
                tickRate,
                simulationStep,
                stage,
                state
            );
            var appTask = app.Startup();
            await appTask;
        }
    }
}
