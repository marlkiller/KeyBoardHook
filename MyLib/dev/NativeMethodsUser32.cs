using System;
using System.Runtime.InteropServices;
using System.Text;
using Enums;

// ReSharper disable InconsistentNaming

namespace Native
{
    public static partial class NativeMethods
    {

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, IntPtr dwThreadId);

        // [DllImport("user32.dll")]
        // public static extern int SendMessage(IntPtr Handle, int WParam, int LParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();
        
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);   

        
        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }
        
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
        
        [StructLayout(LayoutKind.Sequential)]
        public class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

    }
}