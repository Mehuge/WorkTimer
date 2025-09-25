using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WorkTimer
{
    public partial class TimerForm : Form
    {
        #region LogEntry Class
        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string EventType { get; set; }
            public string? Details { get; set; }
            public TimeSpan RemainingTime { get; set; }

            public LogEntry(string eventType, TimeSpan remainingTime, string? details = null)
            {
                Timestamp = DateTime.Now;
                EventType = eventType;
                RemainingTime = remainingTime;
                Details = details;
            }

            public override string ToString()
            {
                string time = $"[{Timestamp:HH:mm:ss} | {RemainingTime:hh\\:mm\\:ss}]";
                return $"{time} {EventType,-15} {Details ?? ""}";
            }

            public static LogEntry? FromString(string logLine)
            {
                try
                {
                    // Simple parsing, assumes format is consistent.
                    var parts = logLine.Split(new[] { ' ' }, 4);
                    var timestamp = DateTime.Parse(parts[0].Trim('[', ']'));
                    var remaining = TimeSpan.Parse(parts[1].Trim('[', ']'));
                    var eventType = parts[2];
                    var details = parts.Length > 3 ? parts[3] : null;

                    return new LogEntry(eventType, remaining, details) { Timestamp = timestamp };
                }
                catch
                {
                    return null; // Could not parse line
                }
            }
        }
        #endregion

        // Core Timer state
        private TimeSpan _totalTime;
        private TimeSpan _remainingTime;
        private bool _isTimerRunning = false;
        private bool _wasAutoPaused = false;

        // Manual Pause state for auto-unpause logic
        private bool _isManuallyPaused = false;
        private DateTime _manualPauseTime;

        // Settings (with default values)
        private int _idleSecondsForPause = 300; // 5 minutes
        private int _manualPauseSecondsForAutoResume = 60; // 1 minute
        private int _keyboardWeight = 10;
        private int _typingSpeedForMax = 60; // WPM for full meter

        // Charting
        private Dictionary<DateTime, int> _activityData = new Dictionary<DateTime, int>();
        private List<(DateTime Start, DateTime End)> _absencePeriods = new List<(DateTime, DateTime)>();
        private DateTime? _currentAbsenceStart = null;
        private System.Windows.Forms.Timer? _chartUpdateTimer;

        // Real-time Activity Meter
        private GlobalActivityHook _activityHook;
        private double _currentActivityLevel = 0;
        private System.Windows.Forms.Timer _activityDecayTimer;

        // Logging
        private List<LogEntry> _currentLog = new List<LogEntry>();
        private string? _currentLogFileName;

        public TimerForm()
        {
            InitializeComponent();
            InitializeCustomComponents();

            // Setup the global hook
            _activityHook = new GlobalActivityHook();
            _activityHook.OnActivity += GlobalHook_OnActivity;

            // Setup the decay timer
            _activityDecayTimer = new System.Windows.Forms.Timer { Interval = 50 }; // Fast decay
            _activityDecayTimer.Tick += ActivityDecayTimer_Tick;
            _activityDecayTimer.Start();
        }

        private void InitializeCustomComponents()
        {
            // Form properties
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = "Work Timer";
            this.BackColor = Color.White;

            // Enable double buffering on custom-drawn panels to prevent flicker
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, progressRingPanel, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, activityMeterPanel, new object[] { true });

            // Chart setup
            SetupActivityChart();

            // Opacity slider setup
            opacityTrackBar.Value = (int)(this.Opacity * 100);
            opacityTrackBar.Scroll += (s, e) => { this.Opacity = opacityTrackBar.Value / 100.0; };

            // Initial state
            ResetTimerToInput();

            // Handle resizing
            this.Resize += (s, e) =>
            {
                this.progressRingPanel.Invalidate();
                this.activityMeterPanel.Invalidate();
            };

            // Chart update timer
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 5000; // Update chart every 5 seconds
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;
        }

        #region Timer Logic

        private void mainTimer_Tick(object sender, EventArgs e)
        {
            if (_isTimerRunning)
            {
                _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));
                progressRingPanel.Invalidate(); // Redraw the ring and time

                if (_remainingTime <= TimeSpan.Zero)
                {
                    StopTimer(isFinished: true);
                }
            }
        }

        private void activityMonitorTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan idleTime = UserActivityMonitor.GetIdleTime();
            bool isActive = idleTime < TimeSpan.FromSeconds(1);

            if (_isTimerRunning)
            {
                UpdateActivityData(isActive);
            }

            // Auto-Pause Logic
            if (_isTimerRunning && !_isManuallyPaused && idleTime > TimeSpan.FromSeconds(_idleSecondsForPause))
            {
                PauseTimer(autoPaused: true);
            }

            // Auto-Resume Logic
            if (isActive)
            {
                if (_wasAutoPaused)
                {
                    ResumeTimer();
                }
                else if (_isManuallyPaused && (DateTime.Now - _manualPauseTime) > TimeSpan.FromSeconds(_manualPauseSecondsForAutoResume))
                {
                    ResumeTimer();
                }
            }
        }

        private void StartNewTimerSession()
        {
            // If timer is at zero, set it from the input fields first
            if (_remainingTime <= TimeSpan.Zero)
            {
                ResetTimerToInput();
                // Don't start if the user set the time to zero.
                if (_remainingTime <= TimeSpan.Zero) return;
            }

            // Clear data for the new session
            _activityData.Clear();
            _absencePeriods.Clear();
            activityChart.Series["Activity"].Points.Clear();
            _currentActivityLevel = 0;
            _currentLog.Clear();

            // Create log entry for starting
            string taskName = string.IsNullOrWhiteSpace(txtTaskName.Text) ? "Untitled Task" : txtTaskName.Text;
            AddLogEntry("Session Start", $"Task: {taskName}");

            ResumeTimer(); // Continue with common resume logic
        }

        private void ResumeTimer()
        {
            // End the current absence period if there is one.
            if (_currentAbsenceStart.HasValue)
            {
                _absencePeriods.Add((_currentAbsenceStart.Value, DateTime.Now));
                _currentAbsenceStart = null;
                AddLogEntry("Active", "User activity resumed.");
            }

            _isTimerRunning = true;
            _wasAutoPaused = false;
            _isManuallyPaused = false;
            btnPlayPause.Text = "Pause";
            lblStatus.Text = "Running...";
            mainTimer.Start();
            activityMonitorTimer.Start();
            _chartUpdateTimer?.Start();
            SetTimeControlsEnabled(false);
            txtTaskName.Enabled = false;
        }

        private void PauseTimer(bool autoPaused = false)
        {
            // Start tracking the absence period if we aren't already in one.
            if (_currentAbsenceStart == null && _isTimerRunning)
            {
                if (autoPaused)
                {
                    // Refund the idle time back to the timer.
                    _remainingTime = _remainingTime.Add(TimeSpan.FromSeconds(_idleSecondsForPause));
                    progressRingPanel.Invalidate(); // Update the display immediately

                    // Set the absence start time to when the user actually went idle.
                    _currentAbsenceStart = DateTime.Now.Subtract(TimeSpan.FromSeconds(_idleSecondsForPause));
                    AddLogEntry("Inactive", $"Auto-paused after {_idleSecondsForPause}s idle.");
                }
                else
                {
                    // For manual pauses, the absence starts now.
                    _currentAbsenceStart = DateTime.Now;
                    AddLogEntry("Paused", "Timer manually paused.");
                }
            }

            _isTimerRunning = false;
            btnPlayPause.Text = "Play";

            if (autoPaused)
            {
                _wasAutoPaused = true;
                lblStatus.Text = "Auto-Paused (Idle)";
            }
            else
            {
                _isManuallyPaused = true;
                _manualPauseTime = DateTime.Now;
                lblStatus.Text = "Paused";
            }
        }

        private void StopTimer(bool isFinished)
        {
            // End any ongoing absence period when the timer stops.
            if (_currentAbsenceStart.HasValue)
            {
                _absencePeriods.Add((_currentAbsenceStart.Value, DateTime.Now));
                _currentAbsenceStart = null;
            }

            mainTimer.Stop();
            activityMonitorTimer.Stop();
            _chartUpdateTimer?.Stop();

            _isTimerRunning = false;
            _wasAutoPaused = false;
            _isManuallyPaused = false;
            btnPlayPause.Text = "Play";
            SetTimeControlsEnabled(true);
            txtTaskName.Enabled = true;


            if (isFinished)
            {
                _remainingTime = TimeSpan.Zero;
                progressRingPanel.Invalidate();
                lblStatus.Text = "Time's up!";
                AddLogEntry("Finished", "Timer completed.");
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Work session complete!", "Work Timer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Stopped. Set time to begin.";
                AddLogEntry("Stopped", "Timer manually stopped.");
            }

            SaveCurrentLog();
        }

        #endregion

        #region UI Handlers & Methods

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (_isTimerRunning)
            {
                PauseTimer(autoPaused: false);
            }
            else
            {
                // If the timer has never run or was reset, start a new session. Otherwise, just resume.
                if (Math.Abs(_remainingTime.TotalSeconds - _totalTime.TotalSeconds) < 1 || _totalTime.TotalSeconds == 0 || !_currentLog.Any())
                {
                    StartNewTimerSession();
                }
                else
                {
                    ResumeTimer();
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            StopTimer(isFinished: false);
            ResetTimerToInput();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            using (var settingsForm = new SettingsForm(_idleSecondsForPause, _manualPauseSecondsForAutoResume, _keyboardWeight, _typingSpeedForMax))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _idleSecondsForPause = settingsForm.IdleSeconds;
                    _manualPauseSecondsForAutoResume = settingsForm.ManualPauseSeconds;
                    _keyboardWeight = settingsForm.KeyboardWeight;
                    _typingSpeedForMax = settingsForm.TypingSpeedForMax;
                }
            }
            this.TopMost = true;
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNote.Text))
            {
                AddLogEntry("Note", txtNote.Text.Trim());
                txtNote.Clear();
            }
        }

        private void btnViewLogs_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            using (var logBrowser = new LogBrowserForm())
            {
                logBrowser.ShowDialog();
            }
            this.TopMost = true;
        }

        private void ResetTimerToInput()
        {
            _remainingTime = new TimeSpan((int)numHours.Value, (int)numMinutes.Value, (int)numSeconds.Value);
            _totalTime = _remainingTime;
            lblStatus.Text = "Ready to start";
            progressRingPanel.Invalidate();
        }

        private void SetTimeControlsEnabled(bool isEnabled)
        {
            numHours.Enabled = isEnabled;
            numMinutes.Enabled = isEnabled;
            numSeconds.Enabled = isEnabled;
        }

        #endregion

        #region Charting Methods

        private void SetupActivityChart()
        {
            activityChart.Series.Clear();
            var series = new Series("Activity")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.DodgerBlue,
                BorderWidth = 2,
                XValueType = ChartValueType.DateTime
            };
            activityChart.Series.Add(series);

            var chartArea = activityChart.ChartAreas[0];
            chartArea.BackColor = Color.FromArgb(245, 245, 245);
            chartArea.AxisX.LabelStyle.Format = "HH:mm";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.Title = "Time";

            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "Active Seconds";
            chartArea.AxisY.Maximum = 60; // 1 minute * 60 seconds
            chartArea.AxisY.Interval = 10;

            activityChart.Legends[0].Enabled = false;
        }

        private void UpdateActivityData(bool isActive)
        {
            if (!isActive) return;

            var now = DateTime.Now;
            var intervalStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            if (!_activityData.ContainsKey(intervalStart))
            {
                _activityData[intervalStart] = 0;
            }
            _activityData[intervalStart]++;
        }

        private void ChartUpdateTimer_Tick(object? sender, EventArgs e)
        {
            var series = activityChart.Series["Activity"];
            var chartArea = activityChart.ChartAreas[0];

            series.Points.Clear();
            chartArea.AxisX.StripLines.Clear();

            // Add absence periods as semi-transparent grey bars
            foreach (var absence in _absencePeriods)
            {
                var stripLine = new StripLine
                {
                    Interval = 0,
                    IntervalOffset = absence.Start.ToOADate(),
                    StripWidth = (absence.End - absence.Start).TotalDays,
                    BackColor = Color.FromArgb(50, 128, 128, 128)
                };
                chartArea.AxisX.StripLines.Add(stripLine);
            }

            // Add the activity line data points
            foreach (var record in _activityData.OrderBy(kvp => kvp.Key))
            {
                series.Points.AddXY(record.Key, record.Value);
            }
        }

        #endregion

        #region Activity Meter Logic

        private void GlobalHook_OnActivity(object? sender, GlobalActivityHook.ActivityEventArgs e)
        {
            // Calculate the "max" activity level based on typing speed
            // WPM -> Chars per second -> Activity units per second
            double charsPerSecond = (_typingSpeedForMax * 5.0) / 60.0;
            double maxActivityPerSecond = charsPerSecond * _keyboardWeight;

            double activityAmount = 0;
            if (e.ActivityType == GlobalActivityHook.ActivityType.Keyboard)
            {
                activityAmount = _keyboardWeight;
            }
            else if (e.ActivityType == GlobalActivityHook.ActivityType.Mouse)
            {
                // Give a small boost for mouse movement
                activityAmount = 1;
            }

            // Normalize the activity boost
            // A single keypress should contribute a fraction of the max level
            _currentActivityLevel += (activityAmount / maxActivityPerSecond) * 100.0;

            if (_currentActivityLevel > 100) _currentActivityLevel = 100;

            activityMeterPanel.Invalidate();
        }

        private void ActivityDecayTimer_Tick(object? sender, EventArgs e)
        {
            // Decay the activity level over time
            _currentActivityLevel *= 0.90; // Adjust for faster/slower decay
            if (_currentActivityLevel < 0.1)
            {
                _currentActivityLevel = 0;
            }
            activityMeterPanel.Invalidate();
        }

        #endregion

        #region Logging

        private void AddLogEntry(string eventType, string? details = null)
        {
            var entry = new LogEntry(eventType, _remainingTime, details);
            _currentLog.Add(entry);
        }

        private void SaveCurrentLog()
        {
            if (!_currentLog.Any()) return;

            try
            {
                string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WorkTimerLogs");
                Directory.CreateDirectory(logDirectory);

                string taskName = SanitizeFileName(string.IsNullOrWhiteSpace(txtTaskName.Text) ? "Untitled Task" : txtTaskName.Text);
                string timestamp = _currentLog.First().Timestamp.ToString("yyyy-MM-dd_HH-mm-ss");
                _currentLogFileName = Path.Combine(logDirectory, $"{timestamp}_{taskName}.log");

                var logLines = _currentLog.Select(entry => entry.ToString()).ToList();
                File.WriteAllLines(_currentLogFileName, logLines);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save log file.\n\nError: {ex.Message}", "Log Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }

        #endregion

        #region Custom Drawing

        private void progressRingPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = progressRingPanel.ClientRectangle;
            int diameter = Math.Min(rect.Width, rect.Height) - 20;
            int x = (rect.Width - diameter) / 2;
            int y = (rect.Height - diameter) / 2;
            Rectangle drawRect = new Rectangle(x, y, diameter, diameter);

            using (var backgroundPen = new Pen(Color.FromArgb(220, 220, 220), 15))
            {
                g.DrawEllipse(backgroundPen, drawRect);
            }

            if (_totalTime.TotalSeconds > 0 && _remainingTime.TotalSeconds > 0)
            {
                using (var progressPen = new Pen(Color.DodgerBlue, 15))
                {
                    progressPen.StartCap = LineCap.Round;
                    progressPen.EndCap = LineCap.Round;
                    float percentage = (float)(_remainingTime.TotalSeconds / _totalTime.TotalSeconds);
                    float sweepAngle = 360 * percentage;
                    g.DrawArc(progressPen, drawRect, -90, sweepAngle);
                }
            }

            string timeText = $"{_remainingTime:hh\\:mm\\:ss}";
            using (var font = new Font("Segoe UI Semibold", Math.Max(8, diameter / 9f), FontStyle.Bold))
            {
                TextRenderer.DrawText(g, timeText, font, progressRingPanel.ClientRectangle, Color.FromArgb(64, 64, 64), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void activityMeterPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = activityMeterPanel.ClientRectangle;
            g.Clear(Color.FromArgb(245, 245, 245));

            int barCount = 12;
            float barHeight = (float)rect.Height / barCount;
            float spacing = barHeight * 0.2f;

            // Determine how many bars to light up
            int barsToLight = (int)Math.Ceiling((_currentActivityLevel / 100.0) * barCount);

            for (int i = 0; i < barCount; i++)
            {
                // Draw from bottom to top
                float yPos = rect.Height - (i + 1) * barHeight + (spacing / 2);
                RectangleF barRect = new RectangleF(0, yPos, rect.Width, barHeight - spacing);

                if (i < barsToLight)
                {
                    // Gradient color from green to red
                    Color barColor = i < barCount * 0.5 ? Color.LawnGreen : (i < barCount * 0.8 ? Color.Orange : Color.Red);
                    using (var brush = new SolidBrush(barColor))
                    {
                        g.FillRectangle(brush, barRect);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(Color.FromArgb(220, 220, 220)))
                    {
                        g.FillRectangle(brush, barRect);
                    }
                }
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up the hook
            _activityHook.Dispose();
            // Save any pending log
            SaveCurrentLog();
            base.OnFormClosing(e);
        }
    }
}

