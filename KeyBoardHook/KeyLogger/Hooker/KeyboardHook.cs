using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using KeyBoardHook.Common.Native;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Enums;
using Keys = KeyBoardHook.KeyLogger.Enums.Keys;

namespace KeyBoardHook.KeyLogger.Hooker
{
    internal class KeyboardHook
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        private IntPtr _keyboardHookHandle;
        private ComboBox comboBox;
             
        public event KeyEventHandler keyEventHandler;
        
        public KeyboardHook(ComboBox comboBox)
        {
            this.comboBox = comboBox;
        }

        public void Hook(HookType hookType,IntPtr hMod,IntPtr threadId)
        {
            if (_keyboardHookHandle != IntPtr.Zero)
                return;
            
            // using (var curProcess = Process.GetCurrentProcess())
            // using (var curModule = curProcess.MainModule)
            {
                _keyboardHookHandle = NativeMethods.SetWindowsHookEx(
                    hookType,
                    KeyboardHookProc, hMod,
                    threadId);
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

      
       
        
        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 当前进程钩子
            //     全局钩子
            // 指定窗口进程钩子
            if (nCode >= 0 && comboBox.Text.Equals("全局钩子"))
            {
                var wParamInt = wParam.ToInt32();
                var myKeyboardHookStruct = (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));
                // MessageBox.Show(Chr(myKeyboardHookStruct.VirtualKeyCode));
                if (wParamInt == WM_KEYDOWN || wParamInt == WM_SYSKEYDOWN)
                {
                    // StringDown
                    System.Windows.Forms.Keys keyData = (System.Windows.Forms.Keys) myKeyboardHookStruct.VirtualKeyCode;
                    keyEventHandler(this,new KeyEventArgs(keyData));
                }
                
                if ((wParamInt == WM_KEYUP || wParamInt == WM_SYSKEYUP))
                {
                
                    // StringUp
                }
                return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
            }

            if (nCode<=0 && comboBox.Text.Equals("当前进程钩子"))
            {
                var bitStr = Convert.ToString((int)lParam, 2);
                if (bitStr.Length < 32) {
                    bitStr = new string('0', 32 - bitStr.Length) + bitStr;
                }
                var isKeyUp = int.Parse(bitStr.Substring(0, 1));
                if (isKeyUp.Equals(0))
                {
                    System.Windows.Forms.Keys keyData = (System.Windows.Forms.Keys)wParam.ToInt32();
                    keyEventHandler(this,new KeyEventArgs(keyData));
                }
                
                return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
            }
            return NativeMethods.CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }
        
       
   
    }
}