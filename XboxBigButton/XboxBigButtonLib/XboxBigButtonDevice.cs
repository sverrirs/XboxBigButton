using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using MadWizard.WinUSBNet;

namespace XboxBigButton
{
    public class XboxBigButtonDevice
    {
        #region Members
        private readonly Guid _devicePublicId = Guid.NewGuid();

        /// <summary>
        /// This is the default device GUID that is installed with the supplied custom driver. If you change this value in the driver .inf file then 
        /// this must be overwritten as well through the constructor.
        /// </summary>
        private readonly string _deviceInterfaceGuid = @"{EF5B294C-E3E3-4914-B5BB-037866450C41}";

        /// <summary>
        /// The actual WinUSB connected device, if connected and working properly this will be a non-null value
        /// </summary>
        private USBDevice _device = null;

        /// <summary>
        /// The background thread that is monitoring the asynchonous input pipe on the USB device
        /// This is just a simple BackgroundWorker for now.
        /// </summary>
        private readonly BackgroundWorker _bgWorker = new BackgroundWorker();

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
            var handler = ButtonStateChanged;
            if( handler != null )
                handler.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// When set the device will filter out repeated presses on the same button
        /// </summary>
        public bool ExcludeRepeats { get; set; }

        public Guid DevicePublicId => _devicePublicId;


        /// <summary>
        /// Constructs a new instance of the XboxBigButtonDevice
        /// </summary>
        /// <param name="customDeviceInterfaceGuid">Optional custom deviceInterfaceGuid in case the default GUID in the supplied driver is modified</param>
        public XboxBigButtonDevice(string customDeviceInterfaceGuid = null)
        {
            if(!string.IsNullOrEmpty(customDeviceInterfaceGuid) )
                _deviceInterfaceGuid = customDeviceInterfaceGuid;

            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += BgWorkerOnDoWork;
            _bgWorker.RunWorkerCompleted += BgWorkerOnRunWorkerCompleted;
            _bgWorker.ProgressChanged += BgWorkerOnProgressChanged;
        }

        public void Connect()
        {
            if (_terminate)
                throw new InvalidOperationException("XboxBigButtonDevice has already been terminated.");

            if ( _device != null)
                throw new InvalidOperationException("XboxBigButtonDevice is already connected.");

            try
            {
                _device = USBDevice.GetSingleDevice(_deviceInterfaceGuid);
                _bgWorker.RunWorkerAsync(_device);
            }
            catch (Exception e)
            {
                Trace.WriteLine("XBB: Connect Exception: "+e.ToString());
                _device = null;
                throw;
            }
        }

        public void Disconnect()
        {
            if (_device == null || _terminate )
                return;

            // Indicate that we want to terminate
            _terminate = true;

            // Abort any waiting tasks on the main input pipe
            try
            {
                _device.Interfaces[0].InPipe.Abort();
            }
            catch (USBException e)
            {
                // This exception happens if the USB device has been unplugged 
                // before disconnecting, we can safely ignore it
                //USBException: Failed to abort pipe. -
            }

            // TODO: Perhaps we should wait until termination has been confirmed?
        }
        
        private void BgWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var device = e.Argument as USBDevice;

            // TODO: for now just get the first pipe, this will always be the subclass=93 interface that is the one that we want
            var dInterface = device != null ? device.Interfaces[0] : null;

            if (dInterface != null && dInterface.InPipe != null)
            {
                // Only 5 bytes are ever transferred from the device
                var inbuffer = new byte[5];
                var prevbuffer = new byte[5];

                while (true)
                {
                    // If we have been instructed to terminate the USB connection end the background thread 
                    // and move into cleanup.
                    if (_terminate)
                        return;

                    int bytesRead = 0;
                    try
                    {
                        bytesRead = dInterface.InPipe.Read(inbuffer, 0, inbuffer.Length);
                    }
                    catch (USBException uex)    
                    {
                        // This exception is thrown when we terminate the Pipe when disposing of the connection
                        // if the error string contains this value then silently ignore the error and move to terminate the app
                        if (!string.IsNullOrEmpty(uex.Message) && !uex.Message.Contains("Failed to read from pipe."))
                            throw;
                    }

                    if (bytesRead > 0)
                    {
                        bool sameMessageAsPreviously = false;

                        // If exclude repeated pushes of the same button sequence is enabled then ignore
                        if (ExcludeRepeats)
                        {
                            // Quickly and dirtily compare the two buffers to see if we're dealing with a repeated message
                            if (inbuffer[0] == prevbuffer[0] && inbuffer[1] == prevbuffer[1] &&
                                inbuffer[2] == prevbuffer[2] && inbuffer[3] == prevbuffer[3] &&
                                inbuffer[4] == prevbuffer[4])
                            {
                                sameMessageAsPreviously = true;
                            }
                            else
                            {
                                inbuffer.CopyTo(prevbuffer, 0);
                            }
                        }

                        // If not same message or we don't care about repeated messages
                        if (!ExcludeRepeats || !sameMessageAsPreviously)
                        {
                            //Send the message to processing and raise the correct events
                            ProcessIncomingMessage(inbuffer);
                        }
                    }
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

            // Now raise the changed event as a progress event and continue
            _bgWorker.ReportProgress(1, new XboxBigButtonDeviceEventArgs(controller, buttons));
        }

        private void BgWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var buttonState = e.UserState as XboxBigButtonDeviceEventArgs;
            if (buttonState != null)
                OnButtonStateChanged(buttonState);
        }

        private void BgWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Cleanup the USB device connection and free all resources
            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
        }
    }
}
