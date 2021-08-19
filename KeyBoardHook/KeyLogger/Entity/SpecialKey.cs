using System;
using KeyBoardHook.KeyLogger.Enums;

namespace KeyBoardHook.KeyLogger.Entity
{
    [Serializable]
    public class SpecialKey : KeyLogEntry
    {
        public SpecialKey(SpecialKeyType keyType, bool isDown)
        {
            KeyType = keyType;
            IsDown = isDown;
        }

        private SpecialKey()
        {
        }

        public bool IsDown { get; set; }
        public SpecialKeyType KeyType { get; set; }

        public override string ToString()
        {
            return KeyType.ToString();
        }
    }
}