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

        public OnScreenPopupForm()
        {
            InitializeComponent();

            if (DesignMode)
                return;

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
            
            // If being shown then we want to place it in the lower bottom corner of the screen
            int y = Screen.PrimaryScreen.Bounds.Bottom - this.Height;
            this.Location = new Point(0, y);
            this.TopMost = true;

            // Start timer to hide form again in 5 sec
            _hideTimer.Change(5000, Timeout.Infinite);
        }

        public void ShowMessage(string message, IWin32Window parent = null)
        {
            this.Message = message;

            if( !this.Visible )
                this.Show(parent);
        }
    }
}
