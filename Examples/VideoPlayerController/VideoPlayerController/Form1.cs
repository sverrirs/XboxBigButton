using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxBigButton;

namespace VideoPlayerController
{
    public partial class Form1 : Form
    {
        public enum Players
        {
            None,
            VLC,
            Netflix
        }

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window. 
        /// Keyboard input is directed to the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>If the window was brought to the foreground, the return value is true.
        /// If the window was not brought to the foreground, the return value is false.</returns>
        [DllImport("user32.DLL")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Determines whether the specified window handle identifies an existing window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// The big button controller
        /// </summary>
        private readonly XboxBigButtonDevice _device;

        /// <summary>
        /// Time when the last keypress was received from the controller, use this to discard excessive repeating keys
        /// </summary>
        private DateTime _lastKeyTime = DateTime.MinValue;

        /// <summary>
        /// Handle to the VLC window
        /// </summary>
        private IntPtr _windowHandle = IntPtr.Zero;

        /// <summary>
        /// Which player is selected in the UI
        /// </summary>
        private Players _player;
        
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

                // Force a refetching of the window
                _windowHandle = IntPtr.Zero;
            }
        }

        public Form1()
        {
            InitializeComponent();

            _device = new XboxBigButtonDevice();
            _device.ButtonStateChanged += _device_ButtonStateChanged;

            _device.Connect();
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
            // If the last key event was within tolerance then don't listen
            if ((DateTime.Now - _lastKeyTime).TotalMilliseconds < 500)
                return;

            // The back button controls which player we're dealing with
            if (buttons.IsPressed(Buttons.Back))
            {
                if (cbUsingVLCMediaPlayer.Checked)
                    cbUsingNetflix.Checked = true;
                else
                    cbUsingVLCMediaPlayer.Checked = true;

                // Save the time if we successfully sent keys
                _lastKeyTime = DateTime.Now;

                return;
            }

            string keysToSend;

            // Which player are we dealing with
            if (this.Player == Players.VLC)
                keysToSend = ControlVLCPlayer(controller, buttons);
            else if (this.Player == Players.Netflix)
                keysToSend = ControlNetflix(controller, buttons);
            else
                return;

            // If no keys then don't do anything
            if (string.IsNullOrEmpty(keysToSend))
                return;

            // Set the window to foreground, only send keys if the window was successfully brought forward
            if (SetForegroundWindow(_windowHandle))
            {
                // Send the keys
                SendKeys.SendWait(keysToSend);

                // Save the time if we successfully sent keys
                _lastKeyTime = DateTime.Now;
            }
        }

        private string ControlNetflix(Controller controller, Buttons buttons)
        {
            // If we don't have the VLC window yet or the window has been restarted (has a new handle)
            // then let's try to find the handle
            if (!IsWindowHandleValid(_windowHandle))
            {
                _windowHandle = FindWindowHandle("Netflix - ");

                // No valid Netflix browser windowhandle could be found, exit
                if (!IsWindowHandleValid(_windowHandle))
                    return null;
            }

            // Translate key mappings
            return GetNetflixKeysToSend(controller, buttons);
        }

        private string ControlVLCPlayer(Controller controller, Buttons buttons)
        {
            // If we don't have the VLC window yet or the window has been restarted (has a new handle)
            // then let's try to find the handle
            if (!IsWindowHandleValid(_windowHandle))
            {
                _windowHandle = FindWindowHandle("VLC media player");

                // No valid VLC windowhandle could be found, exit
                if (!IsWindowHandleValid(_windowHandle))
                    return null;
            }

            // Translate key mappings
            return GetVLCKeysToSend(controller, buttons);
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
                keys += "f";

            // Mute
            if (buttons.IsPressed(Buttons.Y))
                keys += "m";

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
                keys += "^({LEFT})";

            // Skip medium backward
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
            if (buttons.IsPressed(Buttons.Y))
                keys += "m";

            return keys;
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
            return windowHandle != IntPtr.Zero && IsWindow(windowHandle);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(_device != null )
            {
                _device.Disconnect();
                _device.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initially select the VLC button
            cbUsingVLCMediaPlayer.Checked = true;
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
