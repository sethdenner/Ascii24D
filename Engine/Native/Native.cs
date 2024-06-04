namespace Engine.Native
{
    /// <summary>
    /// Native API static class contains helper methods that interop with
    /// native code.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Static constructor. Runs once when application starts.
        /// </summary>
        static Native()
        {
        }
        /// <summary>
        /// Copies a buffer of <c>CHAR_INFO</c> structs to the area of the
        /// console window defined by the passed parameters.
        /// </summary>
        /// <param name="buffer">
        /// An array of <c>CHAR_INFO</c> structs that represent the buffer
        /// pixels that are to be copied to the console window screen buffer.
        /// </param>
        /// <param name="width">
        /// The width of the area of the screen buffer to write to.
        /// </param>
        /// <param name="height">
        /// The height of the area of the screen buffer to write to.
        /// </param>
        /// <param name="left">
        /// The left (x) position of the screen buffer area to write to.
        /// Position is relative to the left side of the console screen. Units
        /// are in console columns.
        /// </param>
        /// <param name="top">
        /// The top (y) position of the screen buffer area to write to.
        /// Position is relative to the top of the console screen. Units are in
        /// console rows.
        ///</param>
        public static void CopyBufferToScreen(
            ConsolePixel[] buffer,
            int width,
            int height,
            int left,
            int top
        )
        {
            NativeWindows.CopyBufferToScreen(
                _consoleWindow,
                buffer,
                width,
                height,
                left,
                top
            );
        }
        /// <summary>
        /// <c>_consoleWindow</c> is a handle to the <c>CConsoleWindows</c>
        /// instance constructed in native code.
        /// </summary>
        private static nint _consoleWindow;
        /// <summary>
        /// <c>InitializeSurface</c> is a static method that handles calling
        /// native code responsible for initializing the console properties to
        /// enable features required for engine operation.
        /// </summary>
        /// <param name="width">
        /// The desired width to set the console window to.
        /// </param>
        /// <param name="height">
        /// The desired height to set the console window to.
        /// </param>
        /// <param name="fontWidth">
        /// The desired font width the console window will use. Common ratios to
        /// use are 1x2 or 1x1 but you do you.
        /// </param>
        /// <param name="fontHeight">
        /// The desired font height the console window will use. Common ratios
        /// to use are 1x2 or 1x1 but you do you.
        /// </param>
        public static void InitalizeSurface(
            int width,
            int height,
            int fontWidth,
            int fontHeight
        )
        {
            NativeWindows.InitializeConsole(
                _consoleWindow,
                width,
                height,
                fontWidth,
                fontHeight
            );
            WindowWidth = width;
            WindowHeight = height;
        }
        public static void SetScreenColors(PaletteInfo[] paletteInfo) {
            _ = NativeWindows.SetScreenColors(
                _consoleWindow,
                paletteInfo.Length,
                paletteInfo
            );
        }
        /// <summary>
        /// <c>ClearScreen</c> is a static method that handles calling native
        /// core responsible for clearing the console window screen buffer.
        /// Every pixel in the screen buffer will be set to the provided
        /// attribute bit field. The character used for clearing is a blank
        /// space.
        /// </summary>
        /// <param name="attributes">
        /// A bit field representing the attributes to clear the screen buffer
        /// to.
        /// </param>
        /// <param name="width">
        /// The width of the area on the screen buffer to clear.
        /// </param>
        /// <param name="height">
        /// The height of the area on the screen buffer to clear.
        /// </param>
        /// <param name="left">
        /// The left position of the area on the screen buffer to clear.
        /// </param>
        /// <param name="top">
        /// The top position of the area on the screen buffer to clear.
        /// </param>
        public static void ClearScreen(
            ConsolePixel clearPixel,
            int width,
            int height,
            int left = 0,
            int top = 0
        )
        {
            NativeWindows.ClearScreen(
                _consoleWindow,
                clearPixel,
                width,
                height,
                left,
                top
            );
        }
        /// <summary>
        /// <c>HandleWindowResize</c> is a static method that handles calling
        /// the native code responsible for detecting if the console window has
        /// been resized and if so re-initializing the console window after a
        /// resize event occurs.
        /// </summary>
        /// <param name="newWindowSize">
        /// <paramref name="newWindowSize"/> is set by the native code and
        /// and marshaled to the <c>SMALL_RECT</c> type. This value is saved in
        /// the static members <c>WindowWidth</c> and <c>WindowHeight</c>.
        /// </param>
        /// <returns>Returns true is the window has been resized.</returns>
        public static bool HandleWindowResize(out SMALL_RECT newWindowSize)
        {
            bool result = NativeWindows.HandleWindowResize(
                _consoleWindow,
                out newWindowSize
            );
            if (result)
            {
                WindowWidth = newWindowSize.Right - newWindowSize.Left + 1;
                WindowHeight = newWindowSize.Bottom - newWindowSize.Top + 1;
            }
            return result;
        }
        /// <summary>
        /// <c>UpdateConsoleTitle</c> is a static method that handles calling
        /// the native code responsible for updating information in the title of
        /// the console window.
        /// </summary>
        /// <param name="elapsedSeconds">
        /// <paramref name="elapsedSeconds"/> is a float representing the total
        /// elapsed time the application has been running. Used for animations
        /// and other time based calculations.
        /// </param>
        public static void UpdateConsoleTitle(float elapsedSeconds)
        {
            NativeWindows.UpdateConsoleTitle(_consoleWindow, elapsedSeconds);
        }
        /// <summary>
        /// <c>CreateConsoleWindow</c> is a static method that handles calling
        /// the native code responsible for creating a handle to the console
        /// window that the application is running in for use in subsequent 
        /// native calls.
        /// </summary>
        public static void CreateConsoleWindow()
        {
            _consoleWindow = NativeWindows.CreateConsoleWindows();
        }
        /// <summary>
        /// The current width of the console buffer in console columns.
        /// </summary>
        public static int WindowWidth { get; set; }
        /// <summary>
        /// The current height of the console buffer in console rows.
        /// </summary>
        public static int WindowHeight { get; set; }
    }
}
