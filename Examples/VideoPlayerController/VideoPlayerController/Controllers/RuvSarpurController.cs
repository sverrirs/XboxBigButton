using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    public class RuvSarpurController : AbstractController
    {
        public RuvSarpurController() : base("RÚV Sarpur", " | RÚV")
        {
        }

        public override IShortcutKey GetKeysToSend(Controller controller, Buttons buttons, string currentWindowTitle)
        {
            string keys = "";

            // THIS REQUIRES THE EXTENSION I BUILT TO ADD SHORTCUTS

            // Play/pause
            if (buttons.IsPressed(Buttons.BigButton))
                keys += "P";

            // Skip medium forward
            if (buttons.IsPressed(Buttons.Left))
                keys += "{LEFT}";

            // Skip medium backward
            if (buttons.IsPressed(Buttons.Right))
                keys += "{RIGHT}";

            // Fullscreen
            if (buttons.IsPressed(Buttons.Home))
                keys += "f";

            // Subtitles 
            if (buttons.IsPressed(Buttons.A))
                keys += "s";

            if (currentWindowTitle.StartsWith("Forsíða | "))
            {
                // Skip medium forward
                if (buttons.IsPressed(Buttons.Up))
                    keys += "{UP}";

                // Skip medium backward
                if (buttons.IsPressed(Buttons.Down))
                    keys += "{DOWN}";
            }

            // Refresh the browser window or go back
            if (buttons.IsPressed(Buttons.Y))
                keys += "B";

            if (!string.IsNullOrEmpty(keys))
                return new SendKeyShortcutKey(keys);

            // Fall-back to the default keys to send option if nothing is available
            return base.GetKeysToSend(controller, buttons, currentWindowTitle);
        }
    }
}
