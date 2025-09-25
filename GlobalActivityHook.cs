using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WorkTimer
{
    public class GlobalActivityHook : IDisposable
    {
        public enum ActivityType { Mouse, Keyboard }

        public class ActivityEventArgs : EventArgs
        {
            public ActivityType ActivityType { get; }
            public ActivityEventArgs(ActivityType type) { ActivityType = type; }
        }

        public event EventHandler<ActivityEventArgs>? OnActivity;

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_MOUSEMOVE = 0x0200;

        private IntPtr _keyboardHookID = IntPtr.Zero;
        private IntPtr _mouseHookID = IntPtr.Zero;

        // Need to keep a reference to the delegate so it's not garbage collected
        private LowLevelProc _keyboardProc;
        private LowLevelProc _mouseProc;

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        public GlobalActivityHook()
        {
            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;
            _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);
            _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
        }

        private IntPtr SetHook(LowLevelProc proc, int hookType)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                if (curModule == null) return IntPtr.Zero;
                return SetWindowsHookEx(hookType, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                OnActivity?.Invoke(this, new ActivityEventArgs(ActivityType.Keyboard));
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEMOVE)
            {
                OnActivity?.Invoke(this, new ActivityEventArgs(ActivityType.Mouse));
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        #region P/Invoke
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
    }
}

