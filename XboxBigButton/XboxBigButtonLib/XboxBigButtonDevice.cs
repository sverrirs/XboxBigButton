using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MadWizard.WinUSBNet;

namespace XboxBigButton
{
    public class XboxBigButtonDevice : IDisposable
    {
        #region Members

        /// <summary>
        /// This is the default device GUID that is installed with the supplied custom driver. If you change this value in the driver .inf file then 
        /// this must be overwritten as well through the constructor.
        /// </summary>
        private string _deviceInterfaceGuid = @"{EF5B294C-E3E3-4914-B5BB-037866450C41}";

        /// <summary>
        /// The actual WinUSB connected device, if connected and working properly this will be a non-null value
        /// </summary>
        private USBDevice _device = null;

        /// <summary>
        /// The background thread that is monitoring the asynchonous input pipe on the USB device
        /// This is just a simple BackgroundWorker for now.
        /// </summary>
        private BackgroundWorker _bgWorker = new BackgroundWorker();

        /// <summary>
        /// Boolean used to terminate the background thread and processing on the USB port, the device will still be connected though
        /// </summary>
        private bool _terminate = false;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the one or more buttons are pressed on one of the BigButton device.
        /// </summary>
        public event EventHandler<XboxBigButtonDeviceEventArgs> ButtonStateChanged;

        protected virtual void OnButtonStateChanged(XboxBigButtonDeviceEventArgs e)
        {
            ButtonStateChanged?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// When set the device will filter out repeated presses on the same button
        /// </summary>
        public bool ExcludeRepeats { get; private set; }
        

        /// <summary>
        /// Constructs a new instance of the XboxBigButtonDevice
        /// </summary>
        /// <param name="customDeviceInterfaceGuid">Optional custom deviceInterfaceGuid in case the default GUID in the supplied driver is modified</param>
        public XboxBigButtonDevice(string customDeviceInterfaceGuid = null)
        {
            if(!string.IsNullOrWhiteSpace(customDeviceInterfaceGuid) )
                _deviceInterfaceGuid = customDeviceInterfaceGuid;

            _bgWorker.DoWork += BgWorkerOnDoWork;
            _bgWorker.RunWorkerCompleted += BgWorkerOnRunWorkerCompleted;
        }

        public void Connect()
        {
            if( _device != null)
                throw new InvalidOperationException("XboxBigButtonDevice is already connected.");

            if( _terminate )
                throw new InvalidOperationException("XboxBigButtonDevice has already been terminated.");

            try
            {
                _device = USBDevice.GetSingleDevice(_deviceInterfaceGuid);

                // Start listening to the input channel
                _bgWorker.RunWorkerAsync(_device);
            }
            catch (Exception)
            {
                _device = null;
                throw;
            }
        }

        public void Disconnect()
        {
            if (_device == null)
                return;

            _terminate = true;

            // TODO: Perhaps we should wait until termination has been confirmed?
        }


        public void Dispose()
        {
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
        }

        private void BgWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var device = e.Argument as USBDevice;

            // TODO: for now just get the first pipe, this will always be the subclass=93 interface that is the one that we want
            var dInterface = device?.Interfaces[0];

            if (dInterface?.InPipe != null)
            {
                // Only 5 bytes are ever transferred from the device
                var inbuffer = new byte[5];
                var prevbuffer = new byte[5];

                while (true)
                {
                    if (_terminate)
                        break;

                    // TODO: Include a timeout for the task so that the terminate clause can be successfully observed!
                    Task<int> t = Task<int>.Factory.FromAsync(dInterface.InPipe.BeginRead, dInterface.InPipe.EndRead, inbuffer, 0, inbuffer.Length, null);

                    t.ContinueWith(result =>
                    {
                        try
                        {
                            // If exclude repeated pushes of the same button sequence is enabled then ignore
                            if (ExcludeRepeats)
                            {
                                if (inbuffer.SequenceEqual(prevbuffer))
                                    return;

                                inbuffer.CopyTo(prevbuffer, 0);
                            }

                            // TODO: Impose a minimum time between button presses (so that we won't get swamped with repeated button presses for the same buttons)
                            
                            //Send the message to processing and raise the correct events
                            ProcessIncomingMessage(inbuffer);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                    });

                    // Sleep for 200msec to allow for the USB port to process more messages
                    Thread.Sleep(200);
                }
            }
        }

        private void ProcessIncomingMessage(byte[] inbuffer)
        {
            if (inbuffer.Length != 5)
                return;

            // Unknown controller index!
            int controlerIdx = inbuffer[2];
            if (controlerIdx > 3)
                return;

            // Parse the controller
            Controller controller = controlerIdx == 0 ? Controller.Green : controlerIdx == 1 ? Controller.Red : controlerIdx == 2 ? Controller.Blue : Controller.Yellow;

            // Parse the buttons, long and ugly for now
            Buttons buttons = Buttons.None;

            // Buttons are spread over two bytes [3 and 4]
            // Check each byte, the values were discovered through experimentation
            if ((inbuffer[3] & (int) Buttons.Up) == (int) Buttons.Up)
                buttons |= Buttons.Up;
            if ((inbuffer[3] & (int)Buttons.Down) == (int)Buttons.Down)
                buttons |= Buttons.Down;
            if ((inbuffer[3] & (int)Buttons.Left) == (int)Buttons.Left)
                buttons |= Buttons.Left;
            if ((inbuffer[3] & (int)Buttons.Right) == (int)Buttons.Right)
                buttons |= Buttons.Right;
            if ((inbuffer[3] & (int)Buttons.Back) == (int)Buttons.Back)
                buttons |= Buttons.Back;
            if ((inbuffer[3] & (int)Buttons.Start) == (int)Buttons.Start)
                buttons |= Buttons.Start;

            // These need to be checked against values as the enums don't share the same values
            // as the bytes coming from the USB
            if ((inbuffer[4] & 4) == 4)
                buttons |= Buttons.Home;
            if ((inbuffer[4] & 8) == 8)
                buttons |= Buttons.BigButton;
            if ((inbuffer[4] & 16) == 16)
                buttons |= Buttons.A;
            if ((inbuffer[4] & 32) == 32)
                buttons |= Buttons.B;
            if ((inbuffer[4] & 64) == 64)
                buttons |= Buttons.X;
            if ((inbuffer[4] & 128) == 128)
                buttons |= Buttons.Y;

            // Now raise the changed event and continue
            OnButtonStateChanged(new XboxBigButtonDeviceEventArgs(controller, buttons));
        }

        private void BgWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: Process any possible error termination signals here and communicate back to caller
        }
    }
}
