using System;

namespace XboxBigButton
{
    public class XboxBigButtonDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// The controller that raised the event (has a new button pressed)
        /// </summary>
        public Controller Controller { get; private set; }

        /// <summary>
        /// The combined button state for the controller
        /// </summary>
        public Buttons ButtonState { get; private set; }

        public XboxBigButtonDeviceEventArgs(Controller controller, Buttons buttonState)
        {
            Controller = controller;
            ButtonState = buttonState;
        }
    }
}