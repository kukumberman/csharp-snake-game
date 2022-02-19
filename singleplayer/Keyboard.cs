using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace singleplayer
{
    class Keyboard : IKeyboard
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static readonly Dictionary<KeyboardKey, VirtualKeys> Keys = new Dictionary<KeyboardKey, VirtualKeys>()
        {
            { KeyboardKey.Space, VirtualKeys.VK_SPACE },
            { KeyboardKey.ArrowUp, VirtualKeys.VK_UP },
            { KeyboardKey.ArrowDown, VirtualKeys.VK_DOWN },
            { KeyboardKey.ArrowLeft, VirtualKeys.VK_LEFT },
            { KeyboardKey.ArrowRight, VirtualKeys.VK_RIGHT },
        };

        public bool IsKeyPressed(KeyboardKey key)
        {
            int virtualKey = (int)Keys[key];
            return (GetAsyncKeyState(virtualKey) & 1) != 0;
        }
    }
}
