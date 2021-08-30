using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using KeyBoardHook.Common.Native;
using KeyBoardHook.KeyLogger.Entity;
using KeyBoardHook.KeyLogger.Enums;
using KeyBoardHook.KeyLogger.Service;
// using ClassLibrary1;

namespace KeyBoardHook
{
    public partial class Form1 : Form
    {
        private KeyLoggerService KeyLoggerService ;

        // [DllImport("ClassLibrary1.dll", EntryPoint="add")]
        // public static extern bool add(int num); //引用外部连接，这个是第三方开发的一个读com口的，需要把RFIDLIB.dll这个文件
        public Form1()
        {
            InitializeComponent();
            // global::MyLib.MyLib _myLib = new global::MyLib.MyLib();
            // MessageBox.Show("引入 内部 DLL 项目" + _myLib.add(1) + "");
            // MessageBox.Show("引入 外部 DLL 项目" + new MyLibEx().add(1) + "");

        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService == null)
            {
                KeyLoggerService = new KeyLoggerService(this.textBox1,this.textBox2,this.comboBox1,textBox5);
            }
            KeyLoggerService.Start(comboBox1.Text,textBox3.Text, textBox4.Text);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService != null)
            {
                KeyLoggerService.Stop(comboBox1.Text,textBox3.Text, textBox4.Text);
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;

            textBox1.ScrollToCaret(); 
        }


        private void button3_Click(object sender, EventArgs e)
        {
            
            // 获取程序 基址
            // List<string> modules = new List<string>();
            //
            // var hWnd = NativeMethods.FindWindow(null,"无标题 - 记事本");
            // IntPtr threadId;
            // var windowThreadProcessId = NativeMethods.GetWindowThreadProcessId(hWnd, out threadId);
            //
            // IntPtr[] modulePointers = new IntPtr[0];
            // int bytesNeeded;
            //
            // var openProcess = NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, false, threadId);
            // NativeMethods.EnumProcessModulesEx(openProcess,modulePointers,0, out bytesNeeded,NativeMethods.LIST_MODULES_ALL);
            // modulePointers = new IntPtr[bytesNeeded / IntPtr.Size];
            // NativeMethods.EnumProcessModulesEx(openProcess, modulePointers, modulePointers.Length * IntPtr.Size, out bytesNeeded, NativeMethods.LIST_MODULES_ALL);
            //
            //
            // for (int i = 0; i < modulePointers.Length; i++)
            // {
            //     StringBuilder modName = new StringBuilder(256);
            //     if (NativeMethods.GetModuleFileNameEx(openProcess, modulePointers[i], modName, modName.Capacity) != 0)
            //         modules.Add(modName.ToString());
            // }
            //
            // NativeMethods.CloseHandle(openProcess);
            //
            // MessageBox.Show(String.Format("程序基址 name {0}, address {1}", modules[0], modulePointers[0].ToString("X") ));
            //
            // C# 注入本地方法
            // int pid = Process.GetCurrentProcess().Id;
            // string fileName = Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost", "");     
            // NativeMethods.ThreadProc proc = new NativeMethods.ThreadProc(MyThreadProc);
            // IntPtr fpProc = Marshal.GetFunctionPointerForDelegate(proc);
            // MessageBox.Show(fpProc.ToString("X"));
            //
            //
            //
            //
            // IntPtr lpAddress = NativeMethods.VirtualAllocEx(IntPtr.Zero, (IntPtr)null, (IntPtr)2020, NativeMethods.Commit,NativeMethods.ExecuteReadWrite);
            // IntPtr lpNumberOfBytesWritten;
            // var writeProcessMemory = NativeMethods.WriteProcessMemory(IntPtr.Zero, lpAddress, BitConverter.GetBytes(1), 2020, out lpNumberOfBytesWritten);
            //
            //
            // IntPtr dwThreadId;
            // IntPtr hThread = NativeMethods.CreateRemoteThread(
            //     Process.GetCurrentProcess().Handle,
            //     IntPtr.Zero,
            //     IntPtr.Zero, 
            //     fpProc, new IntPtr(12),
            //     0,
            //     (IntPtr)null);
            // NativeMethods.WaitForSingleObject(hThread,60*1000);


        }

        static int MyThreadProc(IntPtr param1 )
        {
            int pid = Process.GetCurrentProcess().Id;
            MessageBox.Show("MyThreadProc : " + param1 );
            return 1;
        }
        
        static int MyThreadProc(IntPtr param1,IntPtr param2)
        {
            int pid = Process.GetCurrentProcess().Id;
            MessageBox.Show("MyThreadProc : " + param1 + ":" + param2);
            return 1;
        }

        // class Dev
        // {
        //     IntPtr param1 = IntPtr.Zero;
        //     IntPtr param2 = new IntPtr(12);
        // }

    }
}