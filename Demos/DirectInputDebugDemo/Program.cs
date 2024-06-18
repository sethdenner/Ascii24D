using SharpDX.DirectInput;
using Engine.Input;
using Engine.Characters.UI;
using Engine.Render;
using Engine.Native;
using Engine.Core;
using System.Numerics;
using Engine.Characters;
using System.Runtime.InteropServices;

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
    internal interface IDebugInputState {
        public Dictionary<Guid, KeyboardState[]> KeyboardStates {
            get; set;
        }
        public Dictionary<Guid, MouseState[]> MouseStates {
            get; set;
        }
        public Dictionary<Guid, JoystickState[]> JoystickStates {
            get; set;
        }
        public List<(Guid, DeviceType)> InputDevices {
            get; set;
        }
    }
    internal interface IFrameTimeCounterState {
        public long FrameTimeCounterUpdateEveryTicks {
            get; set;
        }
        public List<long> FrameTimeCounterFrameTimes {
            get; set;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class DemoApplicationState(
        Stage initialStage,
        int framebufferWidth,
        int framebufferHeight
        ) :
        ApplicationState(
            initialStage,
            framebufferWidth,
            framebufferHeight
        ),
        IApplicationStateInput,
        IDebugInputState,
        IFrameTimeCounterState {
        public Dictionary<Guid, KeyboardState[]> KeyboardStates {
            get; set;
        } = [];
        public Dictionary<Guid, MouseState[]> MouseStates {
            get; set;
        } = [];
        public Dictionary<Guid, JoystickState[]> JoystickStates {
            get; set;
        } = [];
        public List<(Guid, DeviceType)> InputDevices {
            get; set;
        } = [];
        public long FrameTimeCounterUpdateEveryTicks {
            get; set;
        } = 10000000;
        public List<long> FrameTimeCounterFrameTimes {
            get; set;
        } = [];
        public MessageFrame InputFrame {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void DeserializeHistoryFields(Memory<byte> data) {
            throw new NotImplementedException();
        }

        public Memory<byte> SerializeHistoryFields() {
            throw new NotImplementedException();
        }
    }
    internal sealed class FrameTimeCounterSimulation : Simulation {
        public override void Cleanup(IApplicationState state) {
        }

        public override void Setup(IApplicationState state) {
            if (state is not DemoApplicationState appState) {
                return;
            }
            Messenger<FrameEndMessage>.Register((frameTime) => {
                appState.FrameTimeCounterFrameTimes.Add(frameTime);
            });
        }

        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            if (state is not DemoApplicationState appState) {
                return;
            }
            float frameTimeSum = 0;
            for (int i = 0; i < appState.FrameTimeCounterFrameTimes.Count; ++i) {
                frameTimeSum += appState.FrameTimeCounterFrameTimes[i];
            }

            if (frameTimeSum > appState.FrameTimeCounterUpdateEveryTicks) {
                double frameTimeAverage =
                    frameTimeSum / appState.FrameTimeCounterFrameTimes.Count;
                double frameTimeAvgMs = Math.Round(frameTimeAverage / 10000f, 5);
                appState.FrameTimeCounterFrameTimes.Clear();

                var characters = CollectionsMarshal.AsSpan(
                    appState.CurrentScene.Characters
                );
                for (
                    int i = 0;
                    i < characters.Length;
                    ++i
                ) {
                    if (
                        characters[i] is not
                        IFrameTimeCounterSimulationObject character
                    ) {
                        continue;
                    }
                    character.Text = frameTimeAvgMs.ToString() + "ms";
                    character.Width = character.Text.Length;
                    character.Position = new Vector3(
                        state.FramebufferWidth - character.Text.Length,
                        0,
                        0
                    );
                }
            }
        }

        public override async Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            await Task.Run(() => { });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal sealed class DebugInputSimulation : Simulation {
        /// <summary>
        /// 
        /// </summary>
        public override void Cleanup(IApplicationState state) {
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Setup(IApplicationState state) {
            if (state is not DemoApplicationState appState) {
                return;
            }
            Message.Register<FoundDeviceMessage.FoundDeviceMessageDelegate>(
                (guid, type) => {
                   appState.InputDevices.Add((guid, type));
                }
            );
            Message.Register<KeyboardMessage.KeyboardMessageDelegate>(
                (device, update) => {
                    if (!appState.KeyboardStates.ContainsKey(device.DeviceGuid)) {
                        appState.KeyboardStates.Add(
                            device.DeviceGuid,
                            new KeyboardState[(int)Key.MediaSelect + 1]
                        );
                    }
                    appState.KeyboardStates[
                        device.DeviceGuid
                    ][(int)update.Key] = new() {
                        IsPressed = update.IsPressed,
                        Key = update.Key,
                        Sequence = update.Sequence,
                        Timestamp = update.Timestamp,
                        Value = update.Value
                    };
                }
            );
            Message.Register<MouseMessage.MouseMessageDelegate>(
                (device, update) => {
                    if (!appState.MouseStates.ContainsKey(device.DeviceGuid)) {
                        appState.MouseStates.Add(
                            device.DeviceGuid,
                            new MouseState[(int)MouseOffset.Buttons7 + 1]
                        );
                    }
                    appState.MouseStates[
                        device.DeviceGuid
                    ][(int)update.Offset] = new() {
                        Offset = update.Offset,
                        Sequence = update.Sequence,
                        Timestamp = update.Timestamp,
                        Value = update.Value,
                        IsButton = update.IsButton
                    };
                }
            );
            Message.Register<JoystickMessage.JoystickMessageDelegate>(
                (device, update) => {
                if (!appState.JoystickStates.TryGetValue(
                    device.DeviceGuid,
                    out JoystickState[]? value
                )) {
                    value = new JoystickState[
                        (int)JoystickOffset.ForceSliders1 + 1
                    ];
                    appState.JoystickStates.Add(
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
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="headless"></param>
        public override void Simulate(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            var appState = state as DemoApplicationState;
            if (null == appState || null == appState.CurrentScene) {
                return;
            }
            for (int i = 0; i < appState.CurrentScene.Characters.Count; ++i) {
                if (
                    appState.CurrentScene.Characters[i] is not
                    IDebugInputSimulationObject character
                ) {
                    continue;
                }
                // store all the text for the text window in debugText variable.
                List<string> debugText = [];
                foreach ((Guid guid, DeviceType type) in appState.InputDevices) {
                    if (DeviceType.Keyboard == type) {
                        debugText.Add($"Keyboard: {guid}");
                        if (!appState.KeyboardStates.ContainsKey(guid))
                            continue;

                        KeyboardState[] inputState = appState.KeyboardStates[
                            guid
                        ];
                        foreach (var s in inputState) {
                            if (0 == s.Timestamp)
                                continue;
                            debugText.Add(string.Join(", ", [
                                $"Value: {s.Value}",
                                $"Sequence: {s.Sequence}",
                                $"Key: {s.Key}",
                                $"IsPressed: {s.IsPressed}",
                                $"Timestamp: {s.Timestamp}"
                            ]));
                        }
                    }
                    if (
                        DeviceType.Joystick == type ||
                        DeviceType.Gamepad == type
                    ) {
                        debugText.Add($"Joystick: {guid}");
                        if (!appState.JoystickStates.ContainsKey(guid))
                            continue;

                        JoystickState[] inputState = appState.JoystickStates[
                            guid
                        ];
                        foreach (var s in inputState) {
                            if (0 == s.Timestamp)
                                continue;
                            debugText.Add(
                                $"Value: {s.Value}, " +
                                $"Sequence: {s.Sequence}, " +
                                $"Offset: {s.Offset}," +
                                $" Timestamp: {s.Timestamp}"
                            );
                        }
                    }
                    if (DeviceType.Mouse == type) {
                        debugText.Add($"Mouse: {guid}");
                        if (!appState.MouseStates.ContainsKey(guid))
                            continue;
                        MouseState[] inputState = appState.MouseStates[
                            guid
                        ];
                        foreach (var s in inputState) {
                            if (0 == s.Timestamp)
                                continue;
                            debugText.Add(
                                $"Value: {s.Value}, " +
                                $"Sequence: {s.Sequence}, " +
                                $"Offset: {s.Offset}, " +
                                $"IsButton: {s.IsButton} " +
                                $"Timestamp: {s.Timestamp}"
                            );
                        }
                    }
                }
                character.Text = string.Join("\n", [.. debugText]);
                character.Width = state.FramebufferWidth - 2;
                character.Height = state.FramebufferHeight - 2;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="headless"></param>
        /// <returns></returns>
        public override Task SimulateAsync(
            IApplicationState state,
            long step,
            bool headless = false
        ) {
            throw new NotImplementedException();
        }
    }
    internal interface IFrameTimeCounterSimulationObject {
        public string Text {
            get; set;
        }
        public Vector3 Position {
            get; set;
        }
        public int Width {
            get; set;
        }
        public int Height {
            get; set;
        }
    }
    internal interface IDebugInputSimulationObject {
        /// <summary>
        /// 
        /// </summary>
        public string Text {
            get; set;
        }
        public int Width {
            get; set;
        }
        public int Height {
            get; set;
        }
    }

    internal sealed class DebugInputCharacter :
        UIWindowText,
        IDebugInputSimulationObject {
        public DebugInputCharacter(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            byte textForegroundColorIndex,
            byte textBackgroundColorIndex,
            string windowText = ""
        ) : base(
            width,
            height,
            position,
            backgroundPixel,
            borderPixel,
            textForegroundColorIndex,
            textBackgroundColorIndex,
            windowText
        ) {
        }
    }
    internal sealed class FpsCounterCharacter :
        UIWindowText,
        IFrameTimeCounterSimulationObject {
        public FpsCounterCharacter(
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            byte textForegroundColorIndex,
            byte textBackgroundColorIndex,
            string windowText = ""
        ) : base(
            width,
            height,
            position,
            backgroundPixel,
            borderPixel,
            textForegroundColorIndex,
            textBackgroundColorIndex,
            windowText
        ) {
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
        Stage initialStage,
        DemoApplicationState applicationState
    ) : Engine.Core.Application(
        tickRate,
        simulationStep,
        initialStage,
        applicationState
    ) {

    }
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
            DemoScene scene = new() {
                Palette = [
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
                ],
                Characters = [
                    new DebugInputCharacter(
                        framebufferWidth - 2, // Width
                        framebufferHeight - 2, // Height
                        new Vector3(1, 1, 0), // Position
                        new ConsolePixel() { // Background Pixel
                            ForegroundColorIndex = 0,
                            BackgroundColorIndex = 0,
                            CharacterCode = ' '
                        },
                        new ConsolePixel() { // Foreground Pixel
                            ForegroundColorIndex = 1,
                            BackgroundColorIndex = 0,
                            CharacterCode = '#'
                        },
                        2, // textForegroundColor
                        0 // textBackgroundColor
                    ) {
                        BorderWidth = 1
                    },
                    new FpsCounterCharacter(
                       3,
                       1,
                       new Vector3(framebufferWidth - 3, 0, 0),
                       new ConsolePixel() { // Background Pixel
                           ForegroundColorIndex = 0,
                           BackgroundColorIndex = 0,
                           CharacterCode = ' '
                       },
                       new ConsolePixel() { },
                       2, // textForegroundColor
                       0 // textBackgroundColor
                   )
               ]
            };
            Stage stage = new([scene], scene) {
                Simulations = [
                    new DebugInputSimulation(),
                    new SimulationDirectInput(),
                    new FrameTimeCounterSimulation(),
                    new SimulationConsoleRender(
                        framebufferWidth,
                        framebufferHeight,
                        fontWidth,
                        fontHeight,
                        new ConsolePixel() {
                        }
                    )
                ]
            };
            DemoApplicationState state = new(
                stage,
                framebufferWidth,
                framebufferHeight
            );
            stage.AddScene(scene);
            long tick = 166666;
            long step = tick;
            DemoApplication app = new(
                tick,
                step,
                stage,
                state
            ) {
                LoadedStages = [stage],
                CurrentStage = stage
            };
            Task appTask = app.Startup();
            await appTask;
        }
    }
}
