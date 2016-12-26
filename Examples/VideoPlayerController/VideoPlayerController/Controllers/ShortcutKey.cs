using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayerController.Controllers
{
    public interface IShortcutKey
    {

    }

    public class SendKeyShortcutKey : IShortcutKey
    {
        public string Key { get; private set; }

        public SendKeyShortcutKey(string key)
        {
            Key = key;
        }
    }

    public class SystemShortcutKey : IShortcutKey
    {
        public int Key { get; private set; }

        public SystemShortcutKey(int key)
        {
            Key = key;
        }
    }
}
