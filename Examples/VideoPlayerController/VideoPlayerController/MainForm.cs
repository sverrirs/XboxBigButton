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
using VideoPlayerController.Controllers;
using XboxBigButton;

namespace VideoPlayerController
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The big button controller
        /// </summary>
        private XboxBigButtonDevice _device;

        /// <summary>
        /// Time when the last keypress was received from the controller, use this to discard excessive repeating keys
        /// </summary>
        private Tuple<Controller, Buttons, DateTime> _lastKeyTime = null;

        /// <summary>
        /// Handle to the player window
        /// </summary>
        private IntPtr _windowHandle = IntPtr.Zero;

        /// <summary>
        /// The video player controllers that are currently available
        /// </summary>
        private AbstractController[] _players = new AbstractController[] { new VLCController(), new NetflixController(), new PrimeVideoController(), new RuvSarpurController(), new WMPController() };

        private int _playerCurrent = -1;

        private OnScreenPopupForm _messageBox = new OnScreenPopupForm() {DisplayAt = DisplayLocation.BottomLeft};

        private OnScreenPopupForm _clockBox = new OnScreenPopupForm() {DisplayAt = DisplayLocation.TopRight, BackColor = Color.DarkSlateGray};

        /// <summary>
        ///  Used to lock the <see cref="SendKeysToWindow"/> function critical section
        /// </summary>
        private readonly object _buttonHandlerLock = new object();

        private Screen _movieScreen;

        private AbstractController CurrentPlayer
        {
            get
            {
                if (_playerCurrent < 0 || _playerCurrent > _players.Length )
                    _playerCurrent = 0;
                return _players[_playerCurrent];
            }
        }

        private AbstractController NextPlayer
        {
            get
            {
                _playerCurrent = (++_playerCurrent%_players.Length);
                var curr = CurrentPlayer;

                _messageBox?.ShowMessage(curr.PlayerName);
                
                // Force a refetching of the window
                _windowHandle = IntPtr.Zero;

                return curr;
            }
        }

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

            // Initially select the first player
            SwitchPlayers();
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
                if (_lastKeyTime != null &&
                    _lastKeyTime.Item1 == controller &&
                    _lastKeyTime.Item2 == buttons &&
                    (timeNow - _lastKeyTime.Item3).TotalSeconds < .5d)
                {
                    // If the last key event was within tolerance then don't process it (if key events happen too quickly)
                    return;
                }

                // Save the time if we successfully sent keys
                _lastKeyTime = new Tuple<Controller, Buttons, DateTime>(controller, buttons, timeNow);

                // If the start button is pressed then we just want to show the current time overlay
                if (buttons.IsPressed(Buttons.Start))
                {
                    _clockBox.ShowMessage(DateTime.Now.ToString("HH:mm"), this);
                    return;
                }

                // The back button controls which player we're dealing with
                if (buttons.IsPressed(Buttons.Back))
                {
                    SwitchPlayers();
                    return;
                }
                
                // If we can't find the correct window then exit
                if (!FindWindow(this.CurrentPlayer.WindowTitle, this.CurrentPlayer.ProcessName))
                    return;

                // Set the window to foreground, only send keys if the window was successfully brought forward
                if (!Win32.SetForegroundWindow(_windowHandle))
                    return;

                // Figure out what keys to send to the window
                var windowTitleBarText = Win32.GetWindowTitleBarText(_windowHandle);
                var keysToSend = this.CurrentPlayer.GetKeysToSend(controller, buttons, windowTitleBarText);
                if( keysToSend != null )
                { 
                    // Send the keys
                    if (keysToSend is SendKeyShortcutKey)
                    {
                        SendKeys.SendWait(((SendKeyShortcutKey)keysToSend).Key);
                    }
                    else
                    {
                        // Check to see if we should send any windows system keys, e.g. for Netflix audio control
                        int systemKeysToSend = ((SystemShortcutKey) keysToSend).Key;
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
                }

                // Now figure out the size of the window to be able to send mouse click events to it
                var windowRect = GetWindowBounds(_windowHandle);
                if (windowRect == Rectangle.Empty)
                    return;

                // What mouse-click should we send
                Point mousePointToClick = this.CurrentPlayer.GetMouseClickToSend(controller, buttons, windowRect);
                
                // Send the mouse click only if there is a point to send
                if (mousePointToClick == Point.Empty)
                    return;
                
                // Retain the old cursor pos so that we can restore it afterwards
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

                // return mouse to old coords
                Cursor.Position = oldPos;
            }
        }

        private void SetSelectedPlayer(AbstractController player)
        {
            btnCurrentPlayer.Text = player.PlayerName;
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

        private bool FindWindow(string windowTitle, string processName)
        {
            // Reset the main movie screen before attempting to locate the handle again
            //MovieScreen = Screen.PrimaryScreen;

            // If we don't have the VLC window yet or the window has been restarted (has a new handle)
            // then let's try to find the handle
            if (!IsWindowHandleValid(_windowHandle))
            {
                _windowHandle = FindWindowHandle(windowTitle, processName);

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


        private IntPtr FindWindowHandle(string windowTitle, string processName)
        {
            foreach (Process proc in Process.GetProcesses())
            {
                var foundTitle = proc.MainWindowTitle;
                if (string.IsNullOrWhiteSpace(foundTitle))
                    continue;

                if (foundTitle.Contains(windowTitle))
                {
                    if (processName == null || proc.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        //Debug.Print(foundTitle);
                        //Debug.Print("   " + proc.ProcessName);
                        return proc.MainWindowHandle;
                    }
                }
            }
            return IntPtr.Zero;
        }

        private bool IsWindowHandleValid(IntPtr windowHandle)
        {
            return windowHandle != IntPtr.Zero && Win32.IsWindow(windowHandle);
        }

        private void SwitchPlayers()
        {
            SetSelectedPlayer(NextPlayer);

            // Do a test to find the player window and print a status message
            lblPlayerWindowStatus.Text = FindWindow(CurrentPlayer.WindowTitle, CurrentPlayer.ProcessName)
                                            ? $"Success {CurrentPlayer.PlayerName} window found"
                                            : $"FAILURE {CurrentPlayer.PlayerName} not found";

            // Append to the label text
            lblPlayerWindowStatus.Text += " | Click to switch players.";
        }

        private void btnCurrentPlayer_Click(object sender, EventArgs e)
        {
            SwitchPlayers();
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
