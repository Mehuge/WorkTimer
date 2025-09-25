using System;
using System.Runtime.InteropServices;

namespace WorkTimer
{
    /// <summary>
    /// A static helper class to determine the system-wide user idle time.
    /// It uses the GetLastInputInfo function from user32.dll.
    /// </summary>
    public static class UserActivityMonitor
    {
        // Structure to hold the last input information
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        // Import the GetLastInputInfo function from user32.dll
        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// Gets the amount of time that has elapsed since the last user input (mouse or keyboard).
        /// </summary>
        /// <returns>A TimeSpan representing the idle time.</returns>
        public static TimeSpan GetIdleTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            if (!GetLastInputInfo(ref lastInputInfo))
            {
                // If the function fails, assume no idle time.
                return TimeSpan.Zero;
            }

            // Environment.TickCount is the number of milliseconds since the system was started.
            // It wraps around every ~49.7 days.
            uint systemUptime = (uint)Environment.TickCount;

            // dwTime is the tick count when the last input event was received.
            uint lastInputTime = lastInputInfo.dwTime;

            // Calculate the idle time in milliseconds.
            // Handle the case where the tick count has wrapped around.
            uint idleTimeMs = systemUptime - lastInputTime;
            if (systemUptime < lastInputTime)
            {
                // Tick count has wrapped, adjust calculation
                idleTimeMs = uint.MaxValue - lastInputTime + systemUptime;
            }

            return TimeSpan.FromMilliseconds(idleTimeMs);
        }
    }
}
