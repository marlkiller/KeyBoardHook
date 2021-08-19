using System;

namespace KeyBoardHook.KeyLogger.Entity
{
    [Serializable]
    public class NormalText : KeyLogEntry
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}