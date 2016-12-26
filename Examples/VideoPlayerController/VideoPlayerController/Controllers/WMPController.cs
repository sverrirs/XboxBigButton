using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    // Windows Media Player
    public sealed class WMPController : AbstractController
    {
        public WMPController() : base("Windows Media Player", "Windows Media Player", "wmplayer")
        {
        }

        public override IShortcutKey GetKeysToSend(Controller controller, Buttons buttons)
        {
            string keys = "";

            // For more info on SendKeys see:
            // https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys(v=vs.110).aspx

            // Play/pause
            if (buttons.IsPressed(Buttons.BigButton))
                keys += "^({p})";

            // Start/Stop fast forward
            if (buttons.IsPressed(Buttons.Right))
                keys += "^+({f})";

            // Start/Stop rewind
            if (buttons.IsPressed(Buttons.Left))
                keys += "^+({b})";

            // Fullscreen
            if (buttons.IsPressed(Buttons.Home))
                keys += "%({ENTER})";

            if (!string.IsNullOrEmpty(keys))
                return new SendKeyShortcutKey(keys);

            // Fall-back to the default keys to send option if nothing is available
            return base.GetKeysToSend(controller, buttons);
        }
    }
}
