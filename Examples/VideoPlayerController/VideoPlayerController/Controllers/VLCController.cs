using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxBigButton;

namespace VideoPlayerController.Controllers
{
    public sealed class VLCController : AbstractController
    {
        public VLCController() : base("VLC Player", "VLC media player", "vlc")
        {
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
                keys += "%({LEFT})";

            // Skip short backward
            if (buttons.IsPressed(Buttons.Right))
                keys += "^({RIGHT})";

            // Volume Up
            if (buttons.IsPressed(Buttons.Up))
                keys += "^({UP})";

            // Volume Down
            if (buttons.IsPressed(Buttons.Down))
                keys += "^({DOWN})";

            // Subtitles 
            if (buttons.IsPressed(Buttons.A))
                keys += "v";

            // Audio
            if (buttons.IsPressed(Buttons.B))
                keys += "b";

            // Fullscreen
            if (buttons.IsPressed(Buttons.Home))
                keys += "f";

            // Mute
            //if (buttons.IsPressed(Buttons.Y))
            //    keys += "m";

            if(!string.IsNullOrEmpty(keys))
                return new SendKeyShortcutKey(keys);

            // Fall-back to the default keys to send option if nothing is available
            return base.GetKeysToSend(controller, buttons, currentWindowTitle);
        }
    }
}
