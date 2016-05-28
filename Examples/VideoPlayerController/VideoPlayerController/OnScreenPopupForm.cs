using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace VideoPlayerController
{
    public partial class OnScreenPopupForm : Form
    {
        private Timer _hideTimer;

        public string Message { get { return lblMessage.Text; } private set { lblMessage.Text = value ?? string.Empty; } }

        public DisplayLocation DisplayAt { get; set; } = DisplayLocation.BottomLeft;

        /// <summary>
        /// The screen that the popup form is displayed on
        /// </summary>
        public Screen DisplayScreen { get; set; }

        public OnScreenPopupForm()
        {
            InitializeComponent();

            if (DesignMode)
                return;

            DisplayScreen = Screen.PrimaryScreen;

            // Create the timer, don't trigger
            _hideTimer = new Timer(_hideTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
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

                _hideTimer?.Dispose();
                _hideTimer = null;
            }
            base.Dispose(disposing);
        }

        private void _hideTimerCallback(object state)
        {
            BeginInvoke(new Action(HidePopup) );
        }

        private void HidePopup()
        {
            if (this.Visible)
                this.Visible = false;

            _hideTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }


        private void lblMessage_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            var loc = Point.Empty;

            switch (DisplayAt)
            {
                case DisplayLocation.BottomLeft:
                    // If being shown then we want to place it in the lower bottom corner of the screen
                    loc = new Point(DisplayScreen.Bounds.X,
                                    DisplayScreen.Bounds.Bottom - this.Height);
                    break;
                case DisplayLocation.TopRight:                   
                    loc = new Point(DisplayScreen.Bounds.Right - this.Width,
                                    DisplayScreen.Bounds.Top);
                    break;
            }

            // If no location is set then don't show the form
            if (loc == Point.Empty)
            return;

            this.Location = loc;
            this.TopMost = true;

            // Start timer to hide form again in x sec
            _hideTimer.Change(3000, Timeout.Infinite);
        }

        public void ShowMessage(string message, IWin32Window parent = null)
        {
            this.Message = message;

            // Resize the form to better fit the text
            this.Width = ((int) this.CreateGraphics().MeasureString(message, lblMessage.Font).Width) + 20;

            if ( !this.Visible )
                this.Show(parent);
        }
    }

    public enum DisplayLocation
    {
        BottomLeft, 
        TopRight
    }
}
