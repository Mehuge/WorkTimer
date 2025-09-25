# Work Timer

A smart work timer for Windows designed to track your focus, log your tasks, and visualize your productivity throughout a work session.

## Overview

Work Timer is more than just a countdown clock. It's a productivity companion that actively monitors your work patterns. It automatically pauses when you step away, logs every event for later review, and provides real-time visual feedback on your activity levels. It's designed to be an unobtrusive, "always on top" window that helps you stay on task.

## Key Features

* **Visual Progress Ring:** A circular progress bar gives you an at-a-glance understanding of the time remaining.
* **Automatic Idle Detection:** If there's no mouse or keyboard input for a configurable amount of time, the timer will automatically pause.
* **Smart Time Refund:** When the timer auto-pauses due to inactivity, the idle time is added back to your remaining time, ensuring you're only tracking focused work.
* **Automatic Resume:** As soon as you return and use your mouse or keyboard, the timer can automatically resume.
* **Detailed Task Logging:** Every session is saved to its own log file. The log records:
    * Task Start & Finish
    * Manual Pauses & Resumes
    * Inactive/Active Periods
    * Custom, timestamped user notes.
* **Built-in Log Browser:** Review your history of completed tasks and their detailed event logs directly within the application.
* **Live Activity Graph:** See your productivity over time with a line graph that plots your active seconds in 1-minute intervals. Paused periods are clearly marked as "absences."
* **Live Activity Meter:** A real-time vertical meter shows your current activity level, with keyboard input weighted more heavily than mouse movement to better represent focused work.
* **Configurable Settings:** Adjust idle time, auto-resume delays, and the typing speed that corresponds to a full activity meter.

## How to Build and Run

This is a C# Windows Forms application built on the .NET 8 framework.

**Prerequisites:**
* Windows 10/11
* .NET 8 SDK
* Visual Studio 2022 (or another C# compatible IDE)

**Steps:**
1.  Clone this repository to your local machine.
2.  Open the `WorkTimer.sln` file in Visual Studio.
3.  Build the solution (Build > Build Solution).
4.  Run the application by pressing the "Start" button or F5.

## How to Use

1.  Launch the application.
2.  (Optional) Enter a **Task Name** to identify the work session.
3.  Set the desired duration using the **Hours**, **Minutes**, and **Seconds** controls.
4.  Click the **Play** button to begin the timer.
5.  During the session, you can type a note into the text field and click **Add Note** to add a timestamped entry to the log.
6.  The timer will run on top of your other windows. You can resize it and adjust its opacity using the slider.
7.  If you stop working, the timer will auto-pause. When you return, it will auto-resume.
8.  When the timer finishes or you click **Reset**, the session log is automatically saved to a file in the `WorkTimerLogs` directory (located in the same folder as the executable).
9.  Click the **View Logs** button to open the Log Browser and review past sessions.

## Authors
* Coding by Google Gemini 2.5 Pro prompted by Mehuge
* Artwork by Google Gemini 2.5 Pro prompted by Mehuge