using System;
using System.Windows.Forms;

namespace KeyBoardHook.KeyLogger.Entity
{
    [Serializable]
    public class StandardKey : KeyLogEntry
    {
        public StandardKey(Keys key, bool isDown)
        {
            Key = key;
            IsDown = isDown;
        }

        private StandardKey()
        {
        }

        public bool IsDown { get; set; }
        public Keys Key { get; set; }

        public override string ToString()
        {
            return Key.ToString();
        }
    }
}