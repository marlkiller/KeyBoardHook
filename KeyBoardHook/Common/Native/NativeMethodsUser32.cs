﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using KeyBoardHook.KeyLogger.Enums;

// ReSharper disable InconsistentNaming

namespace KeyBoardHook.Common.Native
{
    public static partial class NativeMethods
    {
        
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public const int WM_LBUTTONDOWN = 0x201;

        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

       
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

     
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        internal static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, [Out] StringBuilder lpChar,
            uint uFlags);

        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();
        
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(IntPtr lpModule);
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        // SetWindowsHookEx(
        // idHook: Integer;   {钩子类型}
        // lpfn: TFNHookProc; {函数指针}
        // hmod: HINST;       {是模块的句柄，在本机代码中，对应 dll 的句柄（可在 dll 的入口函数中获取）; 一般是 HInstance; 如果是当前线程这里可以是 0}
        // dwThreadId: DWORD  {关联的线程; 可用 GetCurrentThreadId 获取当前线程; 0 表示是系统级钩子}
        // ): HHOOK;            {返回钩子的句柄; 0 表示失败}
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, IntPtr dwThreadId);
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowsHookEx")]
        internal static extern IntPtr SetWindowsHookEx2(HookType hookType, HookProc2 lpfn, IntPtr hMod, IntPtr dwThreadId);

        
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint = "CallNextHookEx")]
        internal static extern IntPtr CallNextHookEx2(IntPtr hhk, int nCode, IntPtr wParam, keyboardHookStruct lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, IntPtr idThread, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    
        [DllImport("user32.dll")]
        internal static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);


        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate IntPtr HookProc2(int code, IntPtr wParam, ref NativeMethods.keyboardHookStruct lParam);

        internal delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr lpszParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        
        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

    }
}