using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    public abstract class AbstractController
    {
        /// <summary>
        /// Returns  the human-friendly name of the player. This will be displayed on-screen so make it descriptive.
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// Returns the full or partial name of the window that the player has, this value is used to locate the 
        /// window in the list of open windows in the OS, so this must be unique to the player e.g. "VLC" or "Windows Media Player" or something like that
        /// (Can be a partial of the window name, e.g. if the name of the file playing is a part of the name)
        /// </summary>
        public string WindowTitle { get; private set; }

        public string ProcessName { get; private set; }

        protected AbstractController(string playerName, string windowTitle, string processName = null)
        {
            PlayerName = playerName;
            WindowTitle = windowTitle;
            ProcessName = processName;
        }

        /// <summary>
        /// Returns an absolute point on the screen that should receive a single click (mouse up and mouse down) event
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="buttons"></param>
        /// <param name="windowRect"></param>
        /// <returns>Valid point or Point.Empty if nothing</returns>
        public virtual Point GetMouseClickToSend(Controller controller, Buttons buttons, Rectangle windowRect)
        {
            return Point.Empty;
        }

        /// <summary>
        /// Returns a short-cut key command to issue to the window, this can either be on the SendKeys form (string) or a System Shortcut (int)
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual IShortcutKey GetKeysToSend(Controller controller, Buttons buttons)
        {
            // Volume Up
            if (buttons.IsPressed(Buttons.Up))
                return new SystemShortcutKey(Win32.APPCOMMAND_VOLUME_UP);

            // Volume Down
            if (buttons.IsPressed(Buttons.Down))
                return new SystemShortcutKey(Win32.APPCOMMAND_VOLUME_DOWN);

            // Mute
            /*if (buttons.IsPressed(Buttons.Y))
                return new SystemShortcutKey(Win32.APPCOMMAND_VOLUME_MUTE);
                */

            // Nothing
            return null;
        }
    }
}
