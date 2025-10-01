using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WorkTimer
{
    /// <summary>
    /// Captures global mouse and keyboard activity using low-level Windows hooks.
    /// </summary>
    public class GlobalActivityHook : IDisposable
    {
        private readonly IntPtr _mouseHookID = IntPtr.Zero;
        private readonly IntPtr _keyboardHookID = IntPtr.Zero;

        // Keep delegates as member variables to prevent them from being garbage collected
        private readonly HookProc _mouseProc;
        private readonly HookProc _keyboardProc;

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_KEYDOWN = 0x0100;

        public event EventHandler<ActivityEventArgs>? OnActivity;

        public GlobalActivityHook()
        {
            _mouseProc = MouseHookCallback;
            _keyboardProc = KeyboardHookCallback;
            _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);
            _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);
        }

        private IntPtr SetHook(HookProc proc, int hookType)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                // Process.MainModule can be null in some circumstances (e.g., 64-bit processes under WOW64).
                // We add a null check here to make the code more robust and resolve the compiler warning.
                ProcessModule? curModule = curProcess.MainModule;
                if (curModule == null)
                {
                    // If we can't get the module, we can't set the hook.
                    // Throw an exception to indicate a fatal setup error.
                    throw new InvalidOperationException("Could not get main module of the current process to set a global hook.");
                }

                using (curModule)
                {
                    return SetWindowsHookEx(hookType, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == WM_MOUSEMOVE)
            {
                OnActivity?.Invoke(this, new ActivityEventArgs(false));
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == WM_KEYDOWN)
            {
                OnActivity?.Invoke(this, new ActivityEventArgs(true));
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        public class ActivityEventArgs : EventArgs
        {
            public bool IsKeyboard { get; }

            public ActivityEventArgs(bool isKeyboard)
            {
                IsKeyboard = isKeyboard;
            }
        }


        #region IDisposable and P/Invoke

        public void Dispose()
        {
            UnhookWindowsHookEx(_mouseHookID);
            UnhookWindowsHookEx(_keyboardHookID);
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

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

