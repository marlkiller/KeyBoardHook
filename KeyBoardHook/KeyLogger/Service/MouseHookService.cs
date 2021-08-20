using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using KeyBoardHook.Common.Native;
using KeyBoardHook.KeyLogger.Enums;

namespace KeyBoardHook.ExternalWindow
{
    public class MouseHookService
    {
        
        private Point point;
        private Point Point
        {
            get { return point; }
            set
            {
                if (point != value)
                {
                    point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.Left, 0, point.X, point.Y, 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }
        
        private IntPtr _mourseHookHandle;

        public MouseHookService()
        {
            this.Point = new Point(); 
            
        }
        
       
        
        public IntPtr Hook(HookType hookType,IntPtr hMod,IntPtr threadId)
        {
            if (_mourseHookHandle==IntPtr.Zero)
                _mourseHookHandle = NativeMethods.SetWindowsHookEx(hookType, MouseHookProc, IntPtr.Zero,threadId);
            return _mourseHookHandle;
        }
        public void UnHook()
        {
            if (_mourseHookHandle != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_mourseHookHandle);
                _mourseHookHandle = IntPtr.Zero;
            }
        }

        protected const int WH_KEYBOARD = 2;
        protected const int WH_KEYBOARD_LL = 13;
        protected const int WH_MOUSE = 7;
        protected const int WH_MOUSE_LL = 14;
        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_KEYUP = 0x101;
        protected const int WM_LBUTTONDBLCLK = 0x203;
        protected const int WM_LBUTTONDOWN = 0x201;
        protected const int WM_LBUTTONUP = 0x202;
        protected const int WM_MBUTTONDBLCLK = 0x209;
        protected const int WM_MBUTTONDOWN = 0x207;
        protected const int WM_MBUTTONUP = 0x208;
        protected const int WM_MOUSEMOVE = 0x200;
        protected const int WM_MOUSEWHEEL = 0x020A;
        protected const int WM_RBUTTONDBLCLK = 0x206;
        protected const int WM_RBUTTONDOWN = 0x204;
        protected const int WM_RBUTTONUP = 0x205;
        protected const int WM_SYSKEYDOWN = 0x104;
        protected const int WM_SYSKEYUP = 0x105;
       
        
        private MouseButtons GetButton(Int32 wParam)
        {
            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;
                default:
                    return MouseButtons.None;

            }
        }
        
        private string GetEventType(Int32 wParam)
        {
            switch (wParam)
            {

                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return "MouseDown";
                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return "MouseUp";
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return "DoubleClick";
                case WM_MOUSEWHEEL:
                    return "MouseWheel";
                case WM_MOUSEMOVE:
                    return "MouseMove";
                default:
                    return "None";

            }
        }

        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode>-1)
            {
                var int32 = wParam.ToInt32();
                NativeMethods.MouseLLHookStruct mouseHookStruct = (NativeMethods.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MouseLLHookStruct));
                MouseButtons button = GetButton(int32);
                string eventType = GetEventType(int32);
                if (button == MouseButtons.Right && mouseHookStruct.flags != 0)
                {
                    eventType = "None";
                }
                MouseEventArgs e = new MouseEventArgs(
                    button,
                    (eventType.Equals("DoubleClick") ? 2 : 1),
                    mouseHookStruct.pt.x,
                    mouseHookStruct.pt.y,
                    (eventType.Equals("MouseWheel") ? (short)((mouseHookStruct.mouseData >> 16) & 0xffff) : 0));
               
                MouseMoveEvent(this, e);

            }
            
            return NativeMethods.CallNextHookEx(_mourseHookHandle, nCode, wParam, lParam);

        }
        
        
        //委托+事件（把钩到的消息封装为事件，由调用者处理）
        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
        public event MouseMoveHandler MouseMoveEvent;
    }
}