using SharpDX.DirectInput;
using Engine.Input;
using Engine.Characters.UI;
using Engine.Render;
using Engine.Native;
using Engine.Core;
using System.Numerics;
using Engine.Core.ECS;
using System.ComponentModel;

namespace DirectInputDebugDemo
{
    /// <summary>
    /// <c>JoystickState</c> is a struct that represents a row
    /// of joystick state data to be rendered in the debug window.
    /// </summary>
    struct JoystickState {
        /// <summary>
        /// 
        /// </summary>
        public int Value {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Timestamp {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Sequence {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public JoystickOffset Offset {
            get; set;
        }
    }
    /// <summary>
    /// <c>KeyboardState</c> is a struct that represents a row
    /// of keyboard state data to be rendered in the debug window.
    /// </summary>
    struct KeyboardState {
        /// <summary>
        /// 
        /// </summary>
        public int Value {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Timestamp {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Sequence {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public Key Key {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsPressed {
            get; set;
        }
    }
    /// <summary>
    /// <c>MouseState</c> is a struct that represents a row
    /// of mouse state data to be rendered in the debug window.
    /// </summary>
    struct MouseState {
        public int Value {
            get; set;
        }
        public int Timestamp {
            get; set;
        }
        public int Sequence {
            get; set;
        }
        public MouseOffset Offset {
            get; set;
        }
        public bool IsButton {
            get; set;
        }
    }
    internal struct DebugInputComponent(int uiProceduralSpritesEntityID) {
        public int UIProceduralSpritesEntityID = uiProceduralSpritesEntityID;
        public Dictionary<Guid, KeyboardState[]> KeyboardStates = [];
        public Dictionary<Guid, MouseState[]> MouseStates = [];
        public Dictionary<Guid, JoystickState[]> JoystickStates = [];
        public List<(Guid, DeviceType)> InputDevices = [];
    }

    internal sealed class DebugInputEntity : Entity {
        public DebugInputEntity(int uiProceduralSpritesEntityID) {
            AddComponent(new DebugInputComponent(uiProceduralSpritesEntityID));
            Message.Register<FoundDeviceMessage.Delegate>(
                (guid, type) => {
                    var component = GetComponent<DebugInputComponent>();
                    component.InputDevices.Add((guid, type));
                    SetComponent(component);
                }
            );
            Message.Register<KeyboardMessage.Delegate>(
                (device, update) => {
                    var component = GetComponent<DebugInputComponent>();
                    if (
                        !component.KeyboardStates.ContainsKey(device.DeviceGuid)
                    ) {
                        component.KeyboardStates.Add(
                            device.DeviceGuid,
                            new KeyboardState[(int)Key.MediaSelect + 1]
                        );
                    }
                    component.KeyboardStates[
                        device.DeviceGuid
                    ][(int)update.Key] = new() {
                        IsPressed = update.IsPressed,
                        Key = update.Key,
                        Sequence = update.Sequence,
                        Timestamp = update.Timestamp,
                        Value = update.Value
                    };
                    SetComponent(component);
                }
            );
            Message.Register<MouseMessage.Delegate>(
                (device, update) => {
                    var component = GetComponent<DebugInputComponent>();
                    if (!component.MouseStates.ContainsKey(device.DeviceGuid)) {
                    component.MouseStates.Add(
                            device.DeviceGuid,
                            new MouseState[(int)MouseOffset.Buttons7 + 1]
                        );
                    }
                    component.MouseStates[
                        device.DeviceGuid
                    ][(int)update.Offset] = new() {
                        Offset = update.Offset,
                        Sequence = update.Sequence,
                        Timestamp = update.Timestamp,
                        Value = update.Value,
                        IsButton = update.IsButton
                    };
                    SetComponent(component);
                }
            );
            Message.Register<JoystickMessage.Delegate>(
                (device, update) => {
                    var component = GetComponent<DebugInputComponent>();
                    if (!component.JoystickStates.TryGetValue(
                        device.DeviceGuid,
                        out JoystickState[]? value
                    )) {
                        value = new JoystickState[
                            (int)JoystickOffset.ForceSliders1 + 1
                        ];
                        component.JoystickStates.Add(
                            device.DeviceGuid,
                            value
                        );
                    }

                    value[(int)update.Offset] = new() {
                        Offset = update.Offset,
                        Sequence = update.Sequence,
                        Timestamp = update.Timestamp,
                        Value = update.Value
                    };
                    SetComponent(component);
                }
            );
        }
    }
    internal class DebugInputSystem() : System<DebugInputComponent> {
        public override void Cleanup() { }

        public override void SetupComponent(
            ref DebugInputComponent component
        ) { }

        public override void UpdateComponent(
            ref DebugInputComponent component,
            long step,
            bool headless = false
        ) {
            // store all the text for the text window in debugText variable.
            List<string> debugText = [];
            foreach ((var guid, var type) in component.InputDevices) {
                if (DeviceType.Keyboard == type) {
                    debugText.Add($"Keyboard: {guid}");
                    if (!component.KeyboardStates.ContainsKey(guid))
                        continue;

                    KeyboardState[] inputState = component.KeyboardStates[
                        guid
                    ];
                    foreach (var state in inputState) {
                        if (0 == state.Timestamp)
                            continue;
                        debugText.Add(string.Join(", ", [
                            $"Value: {state.Value}",
                            $"Sequence: {state.Sequence}",
                            $"Key: {state.Key}",
                            $"IsPressed: {state.IsPressed}",
                            $"Timestamp: {state.Timestamp}"
                        ]));
                    }
                }
                if (DeviceType.Joystick == type || DeviceType.Gamepad == type) {
                    debugText.Add($"Joystick: {guid}");
                    if (!component.JoystickStates.ContainsKey(guid))
                        continue;

                    JoystickState[] inputState = component.JoystickStates[guid];
                    foreach (var state in inputState) {
                        if (0 == state.Timestamp)
                            continue;
                        debugText.Add(
                            $"Value: {state.Value}, " +
                            $"Sequence: {state.Sequence}, " +
                            $"Offset: {state.Offset}," +
                            $" Timestamp: {state.Timestamp}"
                        );
                    }
                }
                if (DeviceType.Mouse == type) {
                    debugText.Add($"Mouse: {guid}");
                    if (!component.MouseStates.ContainsKey(guid))
                        continue;
                    MouseState[] inputState = component.MouseStates[guid];
                    foreach (var state in inputState) {
                        if (0 == state.Timestamp)
                            continue;
                        debugText.Add(
                            $"Value: {state.Value}, " +
                            $"Sequence: {state.Sequence}, " +
                            $"Offset: {state.Offset}, " +
                            $"IsButton: {state.IsButton} " +
                            $"Timestamp: {state.Timestamp}"
                        );
                    }
                }
            }
            /*
            // WTH? Should the position and what not be updated every frame?
            Width = Native.Native.WindowWidth - 2;
            Height = Native.Native.WindowHeight - 2;
            WorldPosition = new Vector3() {
                X = 1,
                Y = 1,
                Z = 10002
            };
            */

            MessageOutbox.Add(new UpdateProceduralUITextMessage(
                component.UIProceduralSpritesEntityID,
                string.Join("\n", [.. debugText])
            ));
        }
    }
    internal struct FrameTimeComponent(
        int uiProceduralSpritesEntityID,
        int framebufferWidth,
        long frameTimeCounterUpdateEveryTicks
    ) {
        public int UIProceduralSpritesEntityID = uiProceduralSpritesEntityID;
        public int FramebufferWidth = framebufferWidth;
        public long FrameTimeCounterUpdateEveryTicks = (
            frameTimeCounterUpdateEveryTicks
        );
        public List<long> FrameTimeCounterFrameTimes = [];
    }
    internal sealed class FrameTimeUpdateMessage : Message {
        public delegate void FrameTimeUpdateMessageDelegate(string text);
        public override void Send() {
            throw new NotImplementedException();
        }
    }
    internal sealed class FrameTimeSystem : System<FrameTimeComponent> {
        public override void Cleanup() {
        }

        public override void SetupComponent(ref FrameTimeComponent component) {
        }

        public override void UpdateComponent(
            ref FrameTimeComponent component,
            long step,
            bool headless = false
        ) {
            float frameTimeSum = 0;
            for (int i = 0; i < component.FrameTimeCounterFrameTimes.Count; ++i) {
                frameTimeSum += component.FrameTimeCounterFrameTimes[i];
            }

            if (frameTimeSum > component.FrameTimeCounterUpdateEveryTicks) {
                double frameTimeAverage =
                    frameTimeSum / component.FrameTimeCounterFrameTimes.Count;
                double frameTimeAvgMs = Math.Round(frameTimeAverage / 10000f, 5);
                component.FrameTimeCounterFrameTimes.Clear();

                var text = frameTimeAvgMs.ToString() + "ms";
                MessageOutbox.Add(new UpdateProceduralUITextMessage(
                    component.UIProceduralSpritesEntityID,
                    text
                ));
                MessageOutbox.Add(new UpdateProceduralUIWidthMessage(
                    component.UIProceduralSpritesEntityID,
                    text.Length
                ));
                MessageOutbox.Add(new UpdateProceduralUIHeightMessage(
                    component.UIProceduralSpritesEntityID,
                    1
                ));
                MessageOutbox.Add(new UpdateProceduralUIPositionMessage(
                    component.UIProceduralSpritesEntityID,
                    new Vector3(
                        component.FramebufferWidth - text.Length,
                        0,
                        0
                    )
                ));
            }
        }
    }
    internal sealed class FrameTimeEntity : Entity {
        public FrameTimeEntity(
            int uiProceduralSpritesEntityID,
            int framebufferWidth,
            long frameTimeCounterUpdateEveryTicks
        ) {
            AddComponent(new FrameTimeComponent(
                uiProceduralSpritesEntityID,
                framebufferWidth,
                frameTimeCounterUpdateEveryTicks
            ));
            Messenger<FrameEndMessage>.Register((frameTime) => {
                var component = GetComponent<FrameTimeComponent>();
                component.FrameTimeCounterFrameTimes.Add(frameTime);
                SetComponent(component);
            });
            Messenger<ApplicationWindowResizedMessage.Delegate>.Register(
                (SMALL_RECT newSizeRect) => {
                    var component = GetComponent<FrameTimeComponent>();
                    component.FramebufferWidth = newSizeRect.Right - newSizeRect.Left;
                    SetComponent(component);
                }
            );
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tickRate"></param>
    /// <param name="simulationStep"></param>
    /// <param name="applicationState"></param>
    internal sealed class DemoApplication(
        long tickRate,
        long simulationStep,
        Stage initialStage
    ) : Engine.Core.Application(
        tickRate,
        simulationStep,
        initialStage
    ) { }
    internal sealed class DemoScene : Scene {
    }
    /// <summary>
    /// Program entry point class.
    /// </summary>
    internal class Program {
        static async Task Main(string[] args) {
            int framebufferWidth = 80;
            int framebufferHeight = 50;
            int fontWidth = 12;
            int fontHeight = 12;
            PaletteInfo[] palette = [
                   new PaletteInfo() { // Black
                       Color = new Engine.Native.ConsoleColor() {
                           R = 0,
                           G = 0,
                           B = 0
                       },
                       Index = 0
                   },
                new PaletteInfo() { // White
                    Color = new Engine.Native.ConsoleColor() {
                        R = 255,
                        G = 255,
                        B = 255
                    },
                    Index = 1
                },
                new PaletteInfo() { // Red
                    Color = new Engine.Native.ConsoleColor() {
                        R = 255,
                        G = 0,
                        B = 0
                    },
                    Index = 2
                },
                new PaletteInfo() { // Light Gray
                    Color = new Engine.Native.ConsoleColor() {
                        R = 76,
                        G = 76,
                        B = 76
                    },
                    Index = 3
                },
                new PaletteInfo() { // Dark Gray
                    Color = new Engine.Native.ConsoleColor() {
                        R = 25,
                        G = 25,
                        B = 25
                    },
                    Index = 4
                }
            ];

            var debugUISpriteEntity = new SpriteEntity(
                0, 0, Vector3.Zero, false, []
            );
            var debugUIEntity = new UIProceduralSpritesEntity(
                debugUISpriteEntity.EntityID,
                framebufferWidth - 2, // Width
                framebufferHeight - 2, // Height
                new Vector3(1, 1, 0), // WorldPosition
                new ConsolePixel() { // Background Pixel
                    ForegroundColorIndex = 8,
                    BackgroundColorIndex = 9,
                    CharacterCode = '@'
                },
                new ConsolePixel() { // Border Pixel
                    ForegroundColorIndex = 1,
                    BackgroundColorIndex = 0,
                    CharacterCode = '#'
                },
                1, // Border Width
                0, // Padding top
                0, // Padding right
                0, // Padding bottom
                0, // Padding left
                "", // text
                2, // textForegroundColor
                0, // Text background color.
                ChildLayout.Vertical,
                true,
                1
            );
            var debugInputEntity = new DebugInputEntity(debugUIEntity.EntityID);
            var frameTimeUISpriteEntity = new SpriteEntity(
                0, 0, Vector3.Zero, false, []
            );
            var frameTimeUIEntity = new UIProceduralSpritesEntity(
                frameTimeUISpriteEntity.EntityID,
                0, // Width
                0, // Height
                Vector3.Zero, // WorldPosition
                new ConsolePixel() {
                }, // Background Pixel
                new ConsolePixel() { },// Foreground Pixel
                0, // Padding top
                0, // Padding right
                0, // Padding bottom
                0, // Padding left
                0, // Border Width
                "", // text
                4, // textForegroundColor
                5, // Text background color.
                ChildLayout.Vertical,
                false
            );
            var frameTimeEntity = new FrameTimeEntity(
                frameTimeUIEntity.EntityID,
                framebufferWidth,
                10000000 // 1 second in ticks
            );
            var consoleRenderEntity = new ConsoleRenderEntity(
                framebufferWidth,
                framebufferHeight,
                fontWidth,
                fontHeight,
                new ConsolePixel() { },
                palette
            );
            var directInputEntity = new DirectInputEntity();
            DemoScene scene = new() {
                Palette = palette,
                Entities = [
                    directInputEntity,
                    debugUISpriteEntity,
                    debugUIEntity,
                    debugInputEntity,
                    frameTimeUISpriteEntity,
                    frameTimeUIEntity,
                    frameTimeEntity
                ]
            };
            Stage stage = new([scene], scene) {
                Systems = [
                    new DirectInputSystem(),
                    new DebugInputSystem(),
                    new UIProceduralSpritesSystem(),
                    new FrameTimeSystem(),
                    new SpriteSystem(
                        consoleRenderEntity.EntityID,
                        framebufferWidth,
                        framebufferHeight
                    ),
                    new ConsoleRenderSystem()
                ]
            };
            stage.AddScene(scene);
            long tick = 166666;
            long step = tick;
            DemoApplication app = new(
                tick,
                step,
                stage
            ) {
                LoadedStages = [stage],
                CurrentStage = stage
            };
            Task appTask = app.Startup();
            await appTask;
        }
    }
}
