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

            // If we can't find the correct window then exit
            if (!FindWindow(this.Player))
                return;

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

                // Now figure out the size of the window to be able to send mouse click events to it
                var windowRect = GetForegroundWindowBounds(_windowHandle);
                if( windowRect != Rectangle.Empty)
                { 
                    // What mouse-click should we send
                    Point mousePointToClick = this.Player == Players.VLC 
                                                ? GetVLCPlayerMouseClickToSend(controller, buttons, windowRect) 
                                                : this.Player == Players.Netflix 
                                                ? GetNetflixMouseClickToSend(controller, buttons, windowRect)
                                                : Point.Empty;
                

                    // Send the mouse click
                    if ( mousePointToClick != Point.Empty )
                    { 
                        var oldPos = Cursor.Position;

                        // get screen coordinates
                        Win32.ClientToScreen(_windowHandle, ref mousePointToClick);

                        // Move the mouse to the position
                        Cursor.Position = new Point(mousePointToClick.X, mousePointToClick.Y);

                        // Instigate a "click" event (down and up)
                        var inputMouseDown = new Win32.INPUT {Type = 0};  // Type=0 is mouse
                        inputMouseDown.Data.Mouse.Flags = 0x0002; // left button down
                        var inputMouseUp = new Win32.INPUT {Type = 0};
                        inputMouseUp.Data.Mouse.Flags = 0x0004; // left button up
                        var inputs = new[] { inputMouseDown, inputMouseUp };
                        Win32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Win32.INPUT)));

                        // return mouse 
                        Cursor.Position = oldPos;
                    }
                }

                // Save the time if we successfully sent keys
                _lastKeyTime = DateTime.Now;
            }
        }

        private Rectangle GetForegroundWindowBounds(IntPtr windowHandle)
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

            // Press that annoying "are you still watching" dialog
            if (buttons.IsPressed(Buttons.B))
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
            if (buttons.IsPressed(Buttons.Y))
                keys += "m";

            return keys;
        }

        private bool FindWindow(Players player)
        {
            // If we don't have the VLC window yet or the window has been restarted (has a new handle)
            // then let's try to find the handle
            if (!IsWindowHandleValid(_windowHandle))
            {
                _windowHandle = FindWindowHandle(player == Players.Netflix ? "Netflix - " : "VLC media player");

                // No valid Netflix browser windowhandle could be found, exit
                if (!IsWindowHandleValid(_windowHandle))
                    return false;
            }

            return true;
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Disconnect the device if it has been created
            _device?.Disconnect();
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
