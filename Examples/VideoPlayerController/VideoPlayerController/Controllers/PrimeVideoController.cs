using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    public class PrimeVideoController : AbstractController
    {
        public PrimeVideoController() : base("Prime Video", "Prime Video")
        {
        }

        public override Point GetMouseClickToSend(Controller controller, Buttons buttons, Rectangle windowRect)
        {

            // Press the Next Episode button in the lower right-hand corner
            if (buttons.IsPressed(Buttons.B))
            {   
                return new Point((int) (windowRect.Width - (windowRect.Width *0.1)), (int) (windowRect.Height - (windowRect.Height * 0.05)));
            }

            return base.GetMouseClickToSend(controller, buttons, windowRect);
        }

        public override IShortcutKey GetKeysToSend(Controller controller, Buttons buttons, string currentWindowTitle)
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
            if (buttons.IsPressed(Buttons.Y))
                keys += "m";

            // Refresh the browser window
            /*if (buttons.IsPressed(Buttons.X))
                keys += "^({F5})";*/

            // Cycle available subtitles
            if (buttons.IsPressed(Buttons.A))
                keys += "c";

            if (!string.IsNullOrEmpty(keys))
                return new SendKeyShortcutKey(keys);

            // Fall-back to the default keys to send option if nothing is available
            return base.GetKeysToSend(controller, buttons, currentWindowTitle);
        }
    }
}
