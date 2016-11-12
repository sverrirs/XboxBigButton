using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxBigButton;

namespace VideoPlayerController
{
    public partial class MainForm : Form
    {
        public enum Players
        {
            None,
            VLC,
            Netflix
        }

        /// <summary>
        /// The big button controller
        /// </summary>
        private XboxBigButtonDevice _device;

        /// <summary>
        /// Time when the last keypress was received from the controller, use this to discard excessive repeating keys
        /// </summary>
        private Tuple<Controller, Buttons, DateTime> _lastKeyTime = null;

        /// <summary>
        /// Handle to the VLC window
        /// </summary>
        private IntPtr _windowHandle = IntPtr.Zero;

        /// <summary>
        /// Which player is selected in the UI
        /// </summary>
        private Players _player;

        private OnScreenPopupForm _messageBox = new OnScreenPopupForm() {DisplayAt = DisplayLocation.BottomLeft};

        private OnScreenPopupForm _clockBox = new OnScreenPopupForm() {DisplayAt = DisplayLocation.TopRight, BackColor = Color.DarkSlateGray};

        /// <summary>
        ///  Used to lock the <see cref="SendKeysToWindow"/> function critical section
        /// </summary>
        private readonly object _buttonHandlerLock = new object();

        private Screen _movieScreen;

        /// <summary>
        /// The screen that the movie player that is currently selected is playing on
        /// </summary>
        public Screen MovieScreen
        {
            get { return _movieScreen; }
            private set
            {
                _movieScreen = value;
                // set the OSD controls to show up on this main screen
                _messageBox.DisplayScreen = _movieScreen;
                _clockBox.DisplayScreen = _movieScreen;
            }
        }

        /// <summary>
        /// Gets or sets which video player the remote controls control
        /// </summary>
        public Players Player
        {
            get { return _player; }
            set
            {
                // If setting to the same thing, just exit
                if (_player == value)
                    return;

                // Switch player
                _player = value;

                _messageBox?.ShowMessage($"{_player} Player");

                // Force a refetching of the window
                _windowHandle = IntPtr.Zero;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            // Initialize the movie screen
            MovieScreen = Screen.PrimaryScreen;

            _device = new XboxBigButtonDevice();
            _device.ButtonStateChanged += _device_ButtonStateChanged;

            _device.Connect();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                components = null;

                _messageBox?.Dispose();
                _messageBox = null;

                _clockBox?.Dispose();
                _clockBox = null;

                _device?.Disconnect();
                _device = null;
            }

            base.Dispose(disposing);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Disconnect the device if it has been created
            _device?.Disconnect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Append the version number to the form title
            var version = typeof(MainForm).Assembly.GetName().Version;
            if (version != null)
                this.Text += $" | v{version}";

            // Initially select the VLC button
            cbUsingVLCMediaPlayer.Checked = true;
            this.Player = Players.VLC;
        }

        private void _device_ButtonStateChanged(object sender, XboxBigButtonDeviceEventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(() => this.SendKeysToWindow(e.Controller, e.ButtonState)));
            else
                this.SendKeysToWindow(e.Controller, e.ButtonState);
        }

        private void SendKeysToWindow(Controller controller, Buttons buttons)
        {
            lock (_buttonHandlerLock)
            {
                var timeNow = DateTime.Now;

                // If the last key event was within tolerance then don't listen
                if (_lastKeyTime != null &&
                    _lastKeyTime.Item1 == controller &&
                    _lastKeyTime.Item2 == buttons &&
                    (timeNow - _lastKeyTime.Item3).TotalSeconds < .5d)
                {
                    //Debug.Print($"[{DateTime.Now.ToString("HH:mm:ss:ffff")}] [{Thread.CurrentThread.ManagedThreadId}] Ignoring Multiple Keypresses for {_lastKeyTime.Item1} > {_lastKeyTime.Item2} : Time since last {(timeNow - _lastKeyTime.Item3).TotalMilliseconds}msec");
                    return;
                }

                // Save the time if we successfully sent keys
                _lastKeyTime = new Tuple<Controller, Buttons, DateTime>(controller, buttons, timeNow);

                //Debug.Print($"[{DateTime.Now.ToString("HH:mm:ss:ffff")}] [{Thread.CurrentThread.ManagedThreadId}] Processing Keypress for {controller} > {buttons} : Time {timeNow.ToString("HH:mm:ss:fffff")}");
                
                // The back button controls which player we're dealing with
                if (buttons.IsPressed(Buttons.Back))
                {
                    if (cbUsingVLCMediaPlayer.Checked)
                        cbUsingNetflix.Checked = true;
                    else
                        cbUsingVLCMediaPlayer.Checked = true;

                    // Save the time if we successfully sent keys
                    _lastKeyTime = new Tuple<Controller, Buttons, DateTime>(controller, buttons, timeNow);

                    return;
                }

                // If the start button is pressed then we just want to show the current time overlay
                if (buttons.IsPressed(Buttons.Start))
                {
                    _clockBox.ShowMessage(DateTime.Now.ToString("HH:mm"), this);
                    _lastKeyTime = new Tuple<Controller, Buttons, DateTime>(controller, buttons, timeNow);
                    return;
                }

                // If we can't find the correct window then exit
                if (!FindWindow(this.Player))
                {
                    //Debug.Print($"[{DateTime.Now.ToString("HH:mm:ss:ffff")}] [{Thread.CurrentThread.ManagedThreadId}] {controller} > {buttons} : Error no window found for {this.Player}");
                    return;
                }

                // Set the window to foreground, only send keys if the window was successfully brought forward
                if (Win32.SetForegroundWindow(_windowHandle))
                {
                    // Figure out what keys to send to the window
                    string keysToSend = this.Player == Players.VLC
                        ? GetVLCKeysToSend(controller, buttons)
                        : this.Player == Players.Netflix
                            ? GetNetflixKeysToSend(controller, buttons)
                            : null;

                    // Send the keys
                    if (!string.IsNullOrEmpty(keysToSend))
                    {
                        SendKeys.SendWait(keysToSend);
                    }
                    else
                    {
                        // Check to see if we should send any windows system keys, e.g. for Netflix audio control
                        // Todo: refactor this to simplify
                        int systemKeysToSend = GetSystemKeysToSend(controller, buttons);
                        if (systemKeysToSend > 0)
                        {
                            if (systemKeysToSend == Win32.APPCOMMAND_VOLUME_MUTE)
                            {
                                var muteState = AudioManager.ToggleMasterVolumeMute() ? "MUTED" : "UNMUTED";
                                _messageBox.ShowMessage($"Volume is {muteState}");
                            }
                            else if (systemKeysToSend == Win32.APPCOMMAND_VOLUME_DOWN)
                                _messageBox.ShowMessage($"Volume {Math.Round(AudioManager.StepMasterVolume(-10))}%");
                            else if (systemKeysToSend == Win32.APPCOMMAND_VOLUME_UP)
                                _messageBox.ShowMessage($"Volume {Math.Round(AudioManager.StepMasterVolume(10))}%");
                        }
                    }

                    // Now figure out the size of the window to be able to send mouse click events to it
                    var windowRect = GetWindowBounds(_windowHandle);
                    if (windowRect != Rectangle.Empty)
                    {
                        // What mouse-click should we send
                        Point mousePointToClick = this.Player == Players.VLC
                            ? GetVLCPlayerMouseClickToSend(controller, buttons, windowRect)
                            : this.Player == Players.Netflix
                                ? GetNetflixMouseClickToSend(controller, buttons, windowRect)
                                : Point.Empty;


                        // Send the mouse click
                        if (mousePointToClick != Point.Empty)
                        {
                            var oldPos = Cursor.Position;

                            // get screen coordinates
                            Win32.ClientToScreen(_windowHandle, ref mousePointToClick);

                            // Move the mouse to the position
                            Cursor.Position = new Point(mousePointToClick.X, mousePointToClick.Y);

                            // Instigate a "click" event (down and up)
                            var inputMouseDown = new Win32.INPUT {Type = 0}; // Type=0 is mouse
                            inputMouseDown.Data.Mouse.Flags = 0x0002; // left button down
                            var inputMouseUp = new Win32.INPUT {Type = 0};
                            inputMouseUp.Data.Mouse.Flags = 0x0004; // left button up
                            var inputs = new[] {inputMouseDown, inputMouseUp};
                            Win32.SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof (Win32.INPUT)));

                            // return mouse 
                            Cursor.Position = oldPos;
                        }
                    }

                   // Debug.Print($"[{DateTime.Now.ToString("HH:mm:ss:ffff")}] [{Thread.CurrentThread.ManagedThreadId}] Finished {controller} > {buttons} : Time {timeNow.ToString("HH:mm:ss:fffff")}");
                }
            }
        }

        private int GetSystemKeysToSend(Controller controller, Buttons buttons)
        {
            // Volume Up
            if (buttons.IsPressed(Buttons.Up))
                return Win32.APPCOMMAND_VOLUME_UP;

            // Volume Down
            if (buttons.IsPressed(Buttons.Down))
                return Win32.APPCOMMAND_VOLUME_DOWN;

            // Mute
            /*if (buttons.IsPressed(Buttons.Y))
                return Win32.APPCOMMAND_VOLUME_MUTE;
                */
            // Nothing
            return 0;
        }

        private Rectangle GetWindowBounds(IntPtr windowHandle)
        {
            Win32.RECT rct;
            if (!Win32.GetWindowRect(windowHandle, out rct))
            {
                return Rectangle.Empty;
            }
            return rct.ToRectangle();
        }

        private Point GetVLCPlayerMouseClickToSend(Controller controller, Buttons buttons, Rectangle windowRect)
        {
            return Point.Empty;
        }

        private Point GetNetflixMouseClickToSend(Controller controller, Buttons buttons, Rectangle windowRect)
        {
            // Press the next-up window in the lower right-hand corner
            if (buttons.IsPressed(Buttons.A))
            {
                // 350 from the right og 280 from the bottom
                return new Point(windowRect.Width - 350, windowRect.Height - 280);
            }

            // Press the new "next episode button" in the top right corner
            if (buttons.IsPressed(Buttons.B))
            {
                // 281 from the right og 51 from the top
                return new Point(windowRect.Width - 281, 51);
            }

            // Press that annoying "are you still watching" dialog
            if (buttons.IsPressed(Buttons.Y))
            {
                // The window is 409x286 pixels and is placed directly in the middle of the screen
                // the continue button is in the top third part of the screen, so click 1/6th down from the top (48px) and half of the width of the screen
                return new Point(
                    windowRect.Width/2,
                    ((windowRect.Height-286)/2)+48 
                    );
            }

            return Point.Empty;
        }
        
        private string GetNetflixKeysToSend(Controller controller, Buttons buttons)
        {
            string keys = "";

            // For more info on SendKeys see:
            // https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx

            // Play/pause
            if (buttons.IsPressed(Buttons.BigButton))
                keys += " ";

            // Skip medium forward
            if (buttons.IsPressed(Buttons.Left))
                keys += "{LEFT}";

            // Skip medium backward
            if (buttons.IsPressed(Buttons.Right))
                keys += "{RIGHT}";

            // Fullscreen
            if (buttons.IsPressed(Buttons.Home))
                keys += "f"; // Use the full screen shortcut in Netflix as they changed their look 2016-nov. To use the Chrome full screen command: keys += "{F11}";

            // Mute
            /*if (buttons.IsPressed(Buttons.Y))
                keys += "m";*/

            // Refresh the browser window
            if (buttons.IsPressed(Buttons.X))
                keys += "^({F5})";

            return keys;
        }

        /// <summary>
        /// Translates the big button controller keys into VLC keys
        /// </summary>
        private string GetVLCKeysToSend(Controller controller, Buttons buttons)
        {
            string keys = "";

            // For more info on SendKeys see:
            // https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx
            
            // Play/pause
            if (buttons.IsPressed(Buttons.BigButton))
                keys += " ";

            // Skip medium forward
            if (buttons.IsPressed(Buttons.Left))
                keys += "%({LEFT})";

            // Skip short backward
            if (buttons.IsPressed(Buttons.Right))
                keys += "^({RIGHT})";

            // Volume Up
            if (buttons.IsPressed(Buttons.Up))
                keys += "^({UP})";

            // Volume Down
            if (buttons.IsPressed(Buttons.Down))
                keys += "^({DOWN})";

            // Subtitles 
            if (buttons.IsPressed(Buttons.A))
                keys += "v";

            // Audio
            if (buttons.IsPressed(Buttons.B))
                keys += "b";

            // Fullscreen
            if (buttons.IsPressed(Buttons.Home))
                keys += "f";

            // Mute
            //if (buttons.IsPressed(Buttons.Y))
            //    keys += "m";

            return keys;
        }

        private bool FindWindow(Players player)
        {
            // Reset the main movie screen before attempting to locate the handle again
            //MovieScreen = Screen.PrimaryScreen;

            // If we don't have the VLC window yet or the window has been restarted (has a new handle)
            // then let's try to find the handle
            if (!IsWindowHandleValid(_windowHandle))
            {
                _windowHandle = FindWindowHandle(player == Players.Netflix ? "Netflix - " : "VLC media player");

                // No valid Netflix browser windowhandle could be found, exit
                if (!IsWindowHandleValid(_windowHandle))
                    return false;
            }

            // Find the screen that the movie is running on (important for multi-monitor setup)
            MovieScreen = DetectScreen(_windowHandle);

            return true;
        }

        private Screen DetectScreen(IntPtr windowHandle)
        {
            // Figure out which screen the window is located on (in a multi screen setup)
            // this is necessary to show the overlay window and other on-screen display 
            // elements on the right screen (i.e. the screen the video is playing on)
            var movieWindowBounds = GetWindowBounds(windowHandle);
            foreach (var screen in Screen.AllScreens)
            {
                // If the movie window is not fullscreen then the screen.bounds will contain the movie
                // if the movie window is fullscreen then IT will actually contain the screen bounds :)
                if (screen.Bounds.Contains(movieWindowBounds) ||
                    movieWindowBounds.Contains(screen.Bounds))
                {
                    return screen;
                }
            }

            // By default use the primary screen
            return Screen.PrimaryScreen;
        }


        private IntPtr FindWindowHandle(string windowTitle)
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.MainWindowTitle.Contains(windowTitle))
                {
                    return proc.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }

        private bool IsWindowHandleValid(IntPtr windowHandle)
        {
            return windowHandle != IntPtr.Zero && Win32.IsWindow(windowHandle);
        }


        private void cbUsingVLCMediaPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbUsingVLCMediaPlayer.Checked)
                return;

            cbUsingNetflix.Checked = false;
            this.Player = Players.VLC;
        }

        private void cbUsingNetflix_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbUsingNetflix.Checked) 
                return;

            cbUsingVLCMediaPlayer.Checked = false;
            this.Player = Players.Netflix;
        }

        
    }

    public static class ButtonsExtensions
    {
        /// <summary>
        /// Checks if the supplied check buttons are currently pressed (note that other buttons can be pressed)
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsPressed(this Buttons currentState, Buttons check)
        {
            return (currentState & check) == check;
        }
    }
}
