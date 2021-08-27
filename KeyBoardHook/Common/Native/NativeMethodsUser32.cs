using System;
using System.Runtime.InteropServices;
using System.Text;
using KeyBoardHook.KeyLogger.Enums;

// ReSharper disable InconsistentNaming

namespace KeyBoardHook.Common.Native
{
    public static unsafe  partial class NativeMethods
    {

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
        
        // Wait functions
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("kernel32.dll")]
        public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds);
        
        
        [Flags]
        public enum SnapshotFlags : uint {
            HeapList = 0x00000001,
            Process  = 0x00000002,
            Thread   = 0x00000004,
            Module   = 0x00000008,
            Module32 = 0x00000010,
            Inherit  = 0x80000000,
            All      = 0x0000001F,
            NoHeaps  = 0x40000000
        }
        
        [DllImport("KERNEL32.DLL ")]
        public static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags flags, IntPtr processid);
        
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);
        
        public const int MAX_PATH = 260;
        public const int MAX_MODULE_NAME32 = 255;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            public IntPtr modBaseAddr;
            internal uint modBaseSize;
            IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        };
        
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

     
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        internal static extern int ToAscii(uint uVirtKey, uint uScanCode, byte[] lpKeyState, [Out] StringBuilder lpChar,
            uint uFlags);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThreadId();
        
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(IntPtr lpModule);
        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
        
        
        // [1]. hProcess
        //     由OpenProcess返回的进程句柄。
        // 如参数传数据为 INVALID_HANDLE_VALUE 【即-1】目标进程为自身进程
        // [2]. lpBaseAddress
        //     要写的内存首地址
        // 在写入之前，此函数将先检查目标地址是否可用，并能容纳待写入的数据。
        // [3]. lpBuffer
        //     指向要写的数据的指针。
        // [4]. nSize
        //     要写入的字节数。
        // 返回值
        //     非零值代表成功。
        // 可用GetLastError获取更多的错误详细信息
        // void* buffer , [MarshalAs(UnmanagedType.AsAny)] object lpBuffer
        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr hwnd, IntPtr baseaddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int nsize, out IntPtr lpNumberOfBytesWritten);

        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CloseHandle(IntPtr hObject);
        
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hwnd, string lpname);
        
        [DllImport("kernel32.dll")]
        public static extern int GetModuleHandleA(string name);
        
        
        public delegate int ThreadProc(IntPtr param);

        [DllImport("kernel32")]
        public static extern IntPtr CreateThread(
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            ThreadProc lpStartAddress, // ThreadProc as friendly delegate
            IntPtr lpParameter,
            uint dwCreationFlags,
            out uint dwThreadId);
        
        //     hProcess 
        //          [输入] 进程句柄
        //     lpThreadAttributes 
        //         [输入] 线程安全描述字，指向SECURITY_ATTRIBUTES结构的指针
        //     dwStackSize 
        //         [输入] 线程栈大小，以字节表示
        //     lpStartAddress 
        //         [输入] 一个LPTHREAD_START_ROUTINE类型的指针，指向在远程进程中执行的函数地址
        //     lpParameter 
        //         [输入] 传入参数
        //     dwCreationFlags 
        //         [输入] 创建线程的其它标志
        //
        //     lpThreadId 
        //         [输出] 线程身份标志，如果为NULL,则不返回
        //
        //     返回值
        // 成功返回新线程句柄，失败返回NULL，并且可调用GetLastError获得错误值。
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
            IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct RemoteParameter
        {
            public IntPtr param1;
            public IntPtr param2;
        };
        
        public static uint LIST_MODULES_ALL = 0x03;

        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, 
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] [In][Out] IntPtr[] lphModule,
            int cb, [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded, uint dwFilterFlag);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,
            [In] [MarshalAs(UnmanagedType.U4)] int nSize);
        
        public static readonly int ExecuteReadWrite = 0x40;
        public static readonly int Commit = 0x1000;


        [DllImport("kernel32.dll")] //声明API函数
        public static extern IntPtr VirtualAllocEx(IntPtr hwnd, IntPtr lpaddress, IntPtr size, int type, int tect);


        public static readonly int PROCESS_ALL_ACCESS = 0x1F0FFF;


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, IntPtr dwProcessId);   

        
        // SetWindowsHookEx(
        // idHook: Integer;   {钩子类型}
        // lpfn: TFNHookProc; {函数指针}
        // hmod: HINST;       {是模块的句柄，在本机代码中，对应 dll 的句柄（可在 dll 的入口函数中获取）; 一般是 HInstance; 如果是当前线程这里可以是 0}
        // dwThreadId: DWORD  {关联的线程; 可用 GetCurrentThreadId 获取当前线程; 0 表示是系统级钩子}
        // ): HHOOK;            {返回钩子的句柄; 0 表示失败}
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, IntPtr dwThreadId);
        
        
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        

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

        internal delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        
       
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
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