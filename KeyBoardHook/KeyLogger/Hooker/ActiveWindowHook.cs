using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Native;

namespace KeyBoardHook.KeyLogger.Hooker
{
    internal class ActiveWindowHook
    {
    
        private IntPtr _hookHandleTitleChange;
        private IntPtr _hookHandleWinChange;
        private string _lastWindowTitle;
        public event EventHandler<ActiveWindowChangedEventArgs> ActiveWindowChanged;

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        public void UnHook()
        {
            if (_hookHandleWinChange != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(_hookHandleWinChange);
                _hookHandleWinChange = IntPtr.Zero;
            }

            if (_hookHandleTitleChange != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(_hookHandleTitleChange);
                _hookHandleTitleChange = IntPtr.Zero;
            }
        }


        public void RaiseOne()
        {
            WinEventProc(IntPtr.Zero, 0, IntPtr.Zero, 0, 0, 0, 0);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (ActiveWindowChanged != null)
            {
                var title = GetActiveWindowTitle();
                if (!string.IsNullOrEmpty(title) && _lastWindowTitle != title)
                    ActiveWindowChanged(this, new ActiveWindowChangedEventArgs(_lastWindowTitle = title));
            }
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var buff = new StringBuilder(nChars);
            var handle = NativeMethods.GetForegroundWindow();

            return NativeMethods.GetWindowText(handle, buff, nChars) > 0 ? buff.ToString() : null;
        }

        public void Hook()
        {

            if (_hookHandleWinChange==IntPtr.Zero)
                _hookHandleWinChange = NativeMethods.SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero,
                    WinEventProc, 0, 0, WINEVENT_OUTOFCONTEXT);
                
            
            if (_hookHandleWinChange == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }

            if (_hookHandleTitleChange==IntPtr.Zero)
                _hookHandleTitleChange = NativeMethods.SetWinEventHook(EVENT_OBJECT_NAMECHANGE, EVENT_OBJECT_NAMECHANGE,
                    IntPtr.Zero,
                    WinEventProc, 0, 0, WINEVENT_OUTOFCONTEXT);

            if (_hookHandleTitleChange == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
            RaiseOne();
        }
    }
}