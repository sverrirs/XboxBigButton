using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    public sealed class NetflixController : AbstractController
    {
        public NetflixController() : base("Netflix", "Netflix - ")
        {

        }

        public override Point GetMouseClickToSend(Controller controller, Buttons buttons, Rectangle windowRect)
        {
            // Press the next-up window in the lower right-hand corner
            if (buttons.IsPressed(Buttons.A))
            {
                // 350 from the right og 280 from the bottom
                return new Point(windowRect.Width - 350, windowRect.Height - 280);
            }

            // Press the new "next episode button" in the top right corner
            if (buttons.IsPressed(Buttons.B))
            {
                // 281 from the right og 51 from the top
                return new Point(windowRect.Width - 281, 51);
            }

            // Press that annoying "are you still watching" dialog
            if (buttons.IsPressed(Buttons.Y))
            {
                // The window is 409x286 pixels and is placed directly in the middle of the screen
                // the continue button is in the top third part of the screen, so click 1/6th down from the top (48px) and half of the width of the screen
                return new Point(
                    windowRect.Width / 2,
                    ((windowRect.Height - 286) / 2) + 48
                    );
            }

            return Point.Empty;
        }

        public override IShortcutKey GetKeysToSend(Controller controller, Buttons buttons)
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
                keys += "f"; // Use the full screen shortcut in Netflix as they changed their look 2016-nov. To use the Chrome full screen command: keys += "{F11}";

            // Mute
            /*if (buttons.IsPressed(Buttons.Y))
                keys += "m";*/

            // Refresh the browser window
            if (buttons.IsPressed(Buttons.X))
                keys += "^({F5})";

            if (!string.IsNullOrEmpty(keys))
                return new SendKeyShortcutKey(keys);

            // Fall-back to the default keys to send option if nothing is available
            return base.GetKeysToSend(controller, buttons);
        }
    }
}
