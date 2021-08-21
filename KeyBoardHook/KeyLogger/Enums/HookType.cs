namespace KeyBoardHook.KeyLogger.Enums
{
    public enum HookType
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        
        // 对于 WH_KEYBOARD_LL 和 WH_MOUSE_LL，SetWindowsHookEx 方法里面根本没有使用这个模块做什么真正的事情，它只是验证一下一个模块而已。只要存在于你的进程中。
        // 所以，传入其他的模块都是可以的
        // HookType 参数指定了低级类型（Low Level，HookType 枚举后面带了 LL 后缀的），这时只能全局设置钩子。意味着你的第四个参数必须传入 0
        // 闪退? 在 HookType 的所有种类中，
        // 只有 WH_MOUSE_LL 和 WH_KEYBOARD_LL 是不需要注入到目标进程的，其他都必须将 dll 注入到目标进程才可以完成挂接
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }
}