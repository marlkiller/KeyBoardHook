using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// using Enums;
// using ExternalWindow;
// using Native;

namespace MyLib
{
    public class MyLib
    {
        // private static MouseHookService mouseHookService = new MouseHookService();
         IntPtr staticEdithWnd = new IntPtr(0); 
        
        // static MyLib ()
        // {
            // MessageBox.Show("static MyLib ()");
            // try
            // {
            //     var parentWindow = NativeMethods.FindWindow(null,"Form1");
            //     if (parentWindow.Equals(IntPtr.Zero))
            //     {
            //         MessageBox.Show("parentWindow.Equals(IntPtr.Zero)");
            //     }
            //     IntPtr EdithWnd = new IntPtr(0); 
            //
            //     EdithWnd = NativeMethods.FindWindowEx(parentWindow,EdithWnd,null,"ddddddd"); 
            //     if (EdithWnd.Equals(IntPtr.Zero))
            //     {
            //         MessageBox.Show("EdithWnd.Equals(IntPtr.Zero)");
            //         return;
            //     }
            //     MessageBox.Show("parentWindow " + parentWindow + " EdithWnd " + EdithWnd);
            //     staticEdithWnd = EdithWnd;
            // }
            //
            // catch (Exception e)
            // {
            //     MessageBox.Show(e.Message);
            // }
            // mouseHookService = new MouseHookService();
            // mouseHookService.MouseMoveEvent += mh_MouseMoveEvent;
            // hook();

        // }
       
        // const int WM_GETTEXT = 0x000D;
        // const int WM_SETTEXT = 0x000C;
        // const int WM_CLICK = 0x00F5;
        // private static void mh_MouseMoveEvent(object sender, MouseEventArgs e)
        // {
            // MessageBox.Show("Move：[" + e.X + ":" + e.Y + "]");
            // NativeMethods.SendMessage(staticEdithWnd, WM_SETTEXT, (IntPtr)0, "Move：[" + e.X+ ":" + e.Y + "]");
        // }

        public static int demo(string s)
        {
            MessageBox.Show(s);
            
            return 1;
        }

        public static void hook()
        {
            // https://my.oschina.net/u/4264209/blog/4291728
            
            // var currentThreadId = NativeMethods.GetCurrentThreadId();
            // var moduleHandle = NativeMethods.LoadLibrary("user32.dll");
            // MessageBox.Show("currentThreadId " + currentThreadId + " new IntPtr(Process.GetCurrentProcess().Id) " + new IntPtr(Process.GetCurrentProcess().Id));
            // mouseHookService.Hook(HookType.WH_MOUSE,IntPtr.Zero, threadId);

            // mouseHookService.Hook(HookType.WH_MOUSE,IntPtr.Zero, currentThreadId);
        }
    }
}