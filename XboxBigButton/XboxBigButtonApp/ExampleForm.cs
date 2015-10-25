using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxBigButton;

namespace XboxBigButtonApp
{
    public partial class ExampleForm : Form
    {
        private XboxBigButtonDevice _device;

        public ExampleForm()
        {
            InitializeComponent();

            _device = new XboxBigButtonDevice();
            _device.ButtonStateChanged += _device_ButtonStateChanged;

            _device.Connect();
        }

        private void _device_ButtonStateChanged(object sender, XboxBigButtonDeviceEventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new Action(() => PrintState(e.Controller, e.ButtonState)));
            else
                PrintState(e.Controller, e.ButtonState);
        }

        private void PrintState(Controller c, Buttons b)
        {
            switch (c)
            {
                case Controller.Green:
                    tbControllerGreen.AppendLine(b.ToString());
                    break;
                case Controller.Red:
                    tbControllerRed.AppendLine(b.ToString());
                    break;
                case Controller.Blue:
                    tbControllerBlue.AppendLine(b.ToString());
                    break;
                case Controller.Yellow:
                    tbControllerYellow.AppendLine(b.ToString());
                    break;
            }
        }   
    }

    public static class WinFormsExtensions
    {
        public static void AppendLine(this TextBox source, string value)
        {
            if (source.Text.Length == 0)
                source.Text = value;
            else
                source.AppendText("\r\n" + value);
        }
    }
}
