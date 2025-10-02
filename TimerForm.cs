using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Media;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WorkTimer
{
    public partial class TimerForm : Form
    {
        #region Member Variables

        // Core Timer state
        private TimeSpan _totalTime;
        private TimeSpan _remainingTime;
        private bool _isTimerRunning = false;
        private bool _wasAutoPaused = false;
        private string _currentLogFile = string.Empty;
        private List<LogEntry> _logEntries = new List<LogEntry>();

        // Manual Pause state for auto-unpause logic
        private bool _isManuallyPaused = false;
        private DateTime _manualPauseTime;

        // Settings (with default values)
        private int _idleSecondsForPause = 300; // 5 minutes
        private int _manualPauseSecondsForAutoResume = 60; // 1 minute
        private int _wpmForFullMeter = 60;
        private AppSettings _appSettings = new AppSettings();


        // Charting & Absences
        private Dictionary<DateTime, int> _activityData = new Dictionary<DateTime, int>();
        private List<AbsencePeriod> _absences = new List<AbsencePeriod>();
        private System.Windows.Forms.Timer? _chartUpdateTimer;

        // Real-time Activity Meter
        private GlobalActivityHook _activityHook;
        private System.Windows.Forms.Timer _activityDecayTimer;
        private double _activityScore = 0;
        private const double MOUSE_WEIGHT = 0.5;
        private const double KEYBOARD_WEIGHT = 5.0; // Keyboard is 10x more valuable
        private const double ACTIVITY_DECAY_RATE = 0.90;

        // Alarm
        private System.Windows.Forms.Timer? _alarmSequenceTimer;
        private int _alarmCount = 0;

        #endregion

        public TimerForm()
        {
            // REVERTED to original, stable order
            InitializeComponent();
            _activityDecayTimer = new System.Windows.Forms.Timer();
            _activityHook = new GlobalActivityHook();
            _activityHook.OnActivity += _activityHook_OnActivity;

            LoadSettings();
            InitializeCustomComponents();
            RestoreSession();
        }

        private void InitializeCustomComponents()
        {
            // Form properties are now set in LoadSettings to ensure they are applied correctly on startup
            this.Text = "Work Timer";

            // Chart setup
            SetupActivityChart();

            // Initial state from settings (will be overridden by session if it exists)
            numHours.Value = _appSettings.LastHours;
            numMinutes.Value = _appSettings.LastMinutes;
            numSeconds.Value = _appSettings.LastSeconds;
            ResetTimerToInput();


            // Chart update timer
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 5000; // Update chart every 5 seconds
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;

            // Activity Meter Timer
            _activityDecayTimer.Interval = 100; // Decay activity score 10 times per second
            _activityDecayTimer.Tick += ActivityDecayTimer_Tick;
            _activityDecayTimer.Start();

            // Alarm Timer
            _alarmSequenceTimer = new System.Windows.Forms.Timer();
            _alarmSequenceTimer.Interval = 1000; // 1 second
            _alarmSequenceTimer.Tick += AlarmSequenceTimer_Tick;
        }

        #region Session and Settings Persistence

        private string GetAppDataPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WorkTimer");
            Directory.CreateDirectory(path);
            return path;
        }

        private void LoadSettings()
        {
            string settingsPath = Path.Combine(GetAppDataPath(), "settings.json");
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    _appSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                // Handle corrupted settings file by resetting to defaults
                _appSettings = new AppSettings();
            }

            // Apply settings, ensuring valid values to prevent invisible window
            this.TopMost = _appSettings.AlwaysOnTop;
            this.Opacity = Math.Max(0.2, _appSettings.Opacity); // Ensure opacity is at least 20%
        }

        private void SaveSettings()
        {
            _appSettings.LastHours = (int)numHours.Value;
            _appSettings.LastMinutes = (int)numMinutes.Value;
            _appSettings.LastSeconds = (int)numSeconds.Value;

            string settingsPath = Path.Combine(GetAppDataPath(), "settings.json");
            try
            {
                string json = JsonSerializer.Serialize(_appSettings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsPath, json);
            }
            catch { /* Silently fail if settings can't be saved */ }
        }

        private void RestoreSession()
        {
            string sessionPath = Path.Combine(GetAppDataPath(), "session.json");
            if (File.Exists(sessionPath))
            {
                try
                {
                    string json = File.ReadAllText(sessionPath);
                    var state = JsonSerializer.Deserialize<SessionState>(json);

                    if (state != null)
                    {
                        // Restore all state
                        _remainingTime = state.RemainingTime;
                        _totalTime = state.TotalTime;
                        txtTaskName.Text = state.TaskName;
                        _activityData = state.ActivityData;
                        _absences = state.Absences;
                        _logEntries = state.LogEntries;
                        _currentLogFile = state.LogFile;

                        // Calculate downtime and add as an absence only if it exceeds the idle threshold
                        var downtime = DateTime.Now - state.ExitTime;
                        if (downtime.TotalSeconds > _idleSecondsForPause)
                        {
                            _absences.Add(new AbsencePeriod { Start = state.ExitTime, End = DateTime.Now });
                            AddLogEntry(LogEventType.Note, $"Application was closed for {downtime:hh\\:mm\\:ss}.");
                        }

                        // Resume the timer
                        ResumeTimer(autoResumed: false, isNewSession: false);

                        // Update chart view and redraw immediately
                        ChartUpdateTimer_Tick(null, EventArgs.Empty);

                        File.Delete(sessionPath); // Clean up session file
                        return; // Skip default reset
                    }
                }
                catch
                {
                    // If session restore fails, delete the corrupt file and start fresh
                    File.Delete(sessionPath);
                }
            }
            // If no session file exists or restore failed, reset to last used values
            ResetTimerToInput();
        }

        private void SaveSession()
        {
            if (!_isTimerRunning) return;

            var state = new SessionState
            {
                RemainingTime = _remainingTime,
                TotalTime = _totalTime,
                TaskName = txtTaskName.Text,
                ActivityData = _activityData,
                Absences = _absences,
                LogEntries = _logEntries,
                LogFile = _currentLogFile,
                ExitTime = DateTime.Now
            };

            string sessionPath = Path.Combine(GetAppDataPath(), "session.json");
            try
            {
                string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(sessionPath, json);
            }
            catch { /* Silently fail */ }
        }

        #endregion

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
                    ResumeTimer(autoResumed: true);
                }
                else if (_isManuallyPaused && (DateTime.Now - _manualPauseTime) > TimeSpan.FromSeconds(_manualPauseSecondsForAutoResume))
                {
                    ResumeTimer(autoResumed: false);
                }
            }
        }

        private void StartNewSession()
        {
            StopAlarm();
            ResetTimerToInput();
            if (_remainingTime <= TimeSpan.Zero) return;

            // Clear data for the new session
            _activityData.Clear();
            _absences.Clear();
            _logEntries.Clear();
            activityChart.Series["Activity"].Points.Clear();
            _currentLogFile = string.Empty;

            // Set initial chart view
            var chartArea = activityChart.ChartAreas[0];
            chartArea.AxisX.Minimum = DateTime.Now.ToOADate();
            chartArea.AxisX.Maximum = DateTime.Now.AddMinutes(10).ToOADate();


            AddLogEntry(LogEventType.Start, $"Timer started for task: {txtTaskName.Text}");
            ResumeTimer(autoResumed: false, isNewSession: true);
        }

        private void ResumeTimer(bool autoResumed, bool isNewSession = false)
        {
            StopAlarm();
            if (!isNewSession)
            {
                string reason = autoResumed ? "auto-resumed due to activity" : "resumed by user";
                AddLogEntry(LogEventType.Resume, $"Timer {reason}");
                if (_absences.Any())
                {
                    var lastAbsence = _absences.Last();
                    if (lastAbsence.End == DateTime.MaxValue)
                    {
                        _absences[^1] = new AbsencePeriod { Start = lastAbsence.Start, End = DateTime.Now }; // Finalize the end time of the absence
                    }
                }
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
            _isTimerRunning = false;
            btnPlayPause.Text = "Play";

            if (autoPaused)
            {
                _wasAutoPaused = true;
                lblStatus.Text = "Auto-Paused (Idle)";

                // "Refund" the idle time
                _remainingTime = _remainingTime.Add(TimeSpan.FromSeconds(_idleSecondsForPause));

                // Accurately record when the absence began
                var absenceStart = DateTime.Now.Subtract(TimeSpan.FromSeconds(_idleSecondsForPause));
                _absences.Add(new AbsencePeriod { Start = absenceStart, End = DateTime.MaxValue }); // Mark end time as max until resumed
                AddLogEntry(LogEventType.Inactive, $"Auto-paused after {_idleSecondsForPause}s of inactivity.");
            }
            else
            {
                _isManuallyPaused = true;
                _manualPauseTime = DateTime.Now;
                lblStatus.Text = "Paused";
                _absences.Add(new AbsencePeriod { Start = DateTime.Now, End = DateTime.MaxValue }); // Mark end time as max until resumed
                AddLogEntry(LogEventType.Pause, "Timer paused by user.");
            }
        }

        private void StopTimer(bool isFinished)
        {
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
                lblStatus.Text = "Time's up! Click Reset to stop alarm.";
                AddLogEntry(LogEventType.Finish, "Timer finished.");

                // Start alarm sequence
                _alarmCount = 0;
                _alarmSequenceTimer?.Start();
            }
            else
            {
                lblStatus.Text = "Stopped. Set time to begin.";
                AddLogEntry(LogEventType.Stop, "Timer stopped by user.");
            }
            SaveLogFile();

            // Clean up session file on clean stop/reset
            string sessionPath = Path.Combine(GetAppDataPath(), "session.json");
            if (File.Exists(sessionPath)) File.Delete(sessionPath);
        }

        #endregion

        #region Activity and Alarm Methods
        private void _activityHook_OnActivity(object? sender, GlobalActivityHook.ActivityEventArgs e)
        {
            if (e == null) return;
            _activityScore += e.IsKeyboard ? KEYBOARD_WEIGHT : MOUSE_WEIGHT;
        }

        private void ActivityDecayTimer_Tick(object? sender, EventArgs e)
        {
            _activityScore *= ACTIVITY_DECAY_RATE;
            if (_activityScore < 0.1) _activityScore = 0;
            activityMeterPanel.Invalidate();
        }

        private void AlarmSequenceTimer_Tick(object? sender, EventArgs e)
        {
            SystemSounds.Exclamation.Play();
            _alarmCount++;
            if (_alarmCount >= 10)
            {
                StopAlarm();
            }
        }

        private void StopAlarm()
        {
            _alarmSequenceTimer?.Stop();
            _alarmCount = 0;
            if (lblStatus.Text.StartsWith("Time's up!"))
            {
                lblStatus.Text = "Finished";
            }
        }

        #endregion

        #region UI Handlers & Methods

        private void TimerForm_Load(object sender, EventArgs e)
        {
            btnAddNote.Location = new Point(txtNote.Right - btnAddNote.Width, txtNote.Bottom + 5);
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (_isTimerRunning)
            {
                PauseTimer(autoPaused: false);
            }
            else
            {
                if (!_logEntries.Any())
                {
                    StartNewSession();
                }
                else
                {
                    ResumeTimer(autoResumed: false);
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            StopAlarm();
            StopTimer(isFinished: false);
            ResetTimerToInput();

            // Clear activity data and update the chart
            _activityData.Clear();
            _absences.Clear();
            activityChart.Series["Activity"].Points.Clear();

            // Reset chart view to default
            var chartArea = activityChart.ChartAreas[0];
            chartArea.AxisX.Minimum = DateTime.Now.ToOADate();
            chartArea.AxisX.Maximum = DateTime.Now.AddHours(1).ToOADate();

            activityChart.Invalidate();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            using (var settingsForm = new SettingsForm(_idleSecondsForPause, _manualPauseSecondsForAutoResume, _wpmForFullMeter, _appSettings.AlwaysOnTop, _appSettings.Opacity))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _idleSecondsForPause = settingsForm.IdleSeconds;
                    _manualPauseSecondsForAutoResume = settingsForm.ManualPauseSeconds;
                    _wpmForFullMeter = settingsForm.Wpm;
                    _appSettings.AlwaysOnTop = settingsForm.AlwaysOnTop;
                    _appSettings.Opacity = settingsForm.OpacityValue;

                    // Apply settings immediately
                    this.TopMost = _appSettings.AlwaysOnTop;
                    this.Opacity = _appSettings.Opacity;
                    SaveSettings();
                }
            }
            this.TopMost = _appSettings.AlwaysOnTop; // Re-apply in case dialog was cancelled
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNote.Text))
            {
                AddLogEntry(LogEventType.Note, txtNote.Text);
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
            this.TopMost = _appSettings.AlwaysOnTop;
        }

        private void btnManageAbsences_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            var completedAbsences = _absences.Where(a => a.End != DateTime.MaxValue).ToList();

            using (var absenceManager = new AbsenceManagerForm(completedAbsences))
            {
                if (absenceManager.ShowDialog() == DialogResult.OK)
                {
                    var absencesToRemove = absenceManager.AbsencesToRemove;
                    if (absencesToRemove.Any())
                    {
                        foreach (var absenceToRemove in absencesToRemove)
                        {
                            var originalAbsence = _absences.FirstOrDefault(a => a.Start == absenceToRemove.Start && a.End == absenceToRemove.End);
                            if (originalAbsence.Start != default)
                            {
                                _absences.Remove(originalAbsence);

                                // Subtract time from countdown
                                var duration = originalAbsence.End - originalAbsence.Start;
                                _remainingTime = _remainingTime.Subtract(duration);
                                AddLogEntry(LogEventType.Note, $"Absence from {originalAbsence.Start:HH:mm:ss} to {originalAbsence.End:HH:mm:ss} removed. Time subtracted from countdown.");
                            }
                        }
                        ChartUpdateTimer_Tick(null, EventArgs.Empty);
                    }
                }
            }
            this.TopMost = _appSettings.AlwaysOnTop;
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
            // REVERTED to original, stable styling
            chartArea.BackColor = Color.FromArgb(245, 245, 245);

            chartArea.AxisX.LabelStyle.Format = "HH:mm";
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.Title = "Time";
            chartArea.AxisX.IsMarginVisible = false;

            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.Title = "Active Seconds";
            chartArea.AxisY.Maximum = 60;
            chartArea.AxisY.Interval = 10;
            chartArea.AxisY.IsMarginVisible = false;

            activityChart.Legends[0].Enabled = false;
            activityChart.PostPaint += ActivityChart_PostPaint;
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
            series.Points.Clear();

            if (!_activityData.Any()) return;

            var chartArea = activityChart.ChartAreas[0];
            var orderedData = _activityData.OrderBy(kvp => kvp.Key).ToList();

            foreach (var record in orderedData)
            {
                series.Points.AddXY(record.Key, record.Value);
            }

            // Dynamically adjust the X-axis to keep the graph current.
            chartArea.AxisX.Minimum = orderedData.First().Key.ToOADate();
            chartArea.AxisX.Maximum = DateTime.Now.AddMinutes(1).ToOADate(); // Show 1 minute into the future

            activityChart.Invalidate();
        }

        private void ActivityChart_PostPaint(object? sender, ChartPaintEventArgs e)
        {
            if (e.ChartElement is ChartArea chartArea)
            {
                var xAxis = chartArea.AxisX;
                var yAxis = chartArea.AxisY;

                RectangleF plotArea = chartArea.Position.ToRectangleF();
                float plotX = plotArea.X * e.Chart.Width / 100f;
                float plotY = plotArea.Y * e.Chart.Height / 100f;
                float plotWidth = plotArea.Width * e.Chart.Width / 100f;
                float plotHeight = plotArea.Height * e.Chart.Height / 100f;


                using (var brush = new SolidBrush(Color.FromArgb(50, Color.Gray)))
                {
                    foreach (var absence in _absences)
                    {
                        DateTime endTime = (absence.End == DateTime.MaxValue) ? DateTime.Now : absence.End;

                        double x1 = xAxis.ValueToPixelPosition(absence.Start.ToOADate());
                        double x2 = xAxis.ValueToPixelPosition(endTime.ToOADate());
                        double y1 = yAxis.ValueToPixelPosition(yAxis.Maximum);
                        double y2 = yAxis.ValueToPixelPosition(yAxis.Minimum);

                        x1 = Math.Max(x1, plotX);
                        x2 = Math.Min(x2, plotX + plotWidth);

                        if (x1 < x2)
                        {
                            e.ChartGraphics.Graphics.FillRectangle(brush, (float)x1, (float)y1, (float)(x2 - x1), (float)(y2 - y1));
                        }
                    }
                }
            }
        }

        #endregion

        #region Painting Methods

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
            g.Clear(this.BackColor);

            int ledCount = 12;
            int ledGap = 4;
            int barWidth = activityMeterPanel.ClientSize.Width - 10;
            int barX = 5;

            int totalLedHeight = activityMeterPanel.ClientSize.Height - ((ledCount - 1) * ledGap);
            int ledHeight = Math.Max(1, totalLedHeight / ledCount);

            double maxScorePerTick = (_wpmForFullMeter * 5.0 / 60.0 * KEYBOARD_WEIGHT) / 10.0;
            double maxReasonableScore = maxScorePerTick / (1 - ACTIVITY_DECAY_RATE);
            double percentage = Math.Min(1.0, _activityScore / maxReasonableScore);

            int ledsToLight = (int)(percentage * ledCount);

            for (int i = 0; i < ledCount; i++)
            {
                int ledY = activityMeterPanel.ClientSize.Height - ((i + 1) * ledHeight) - (i * ledGap);
                Rectangle ledRect = new Rectangle(barX, ledY, barWidth, ledHeight);

                Color ledColor;
                if (i < ledsToLight)
                {
                    if (i < ledCount * 0.5)
                        ledColor = Color.LimeGreen;
                    else if (i < ledCount * 0.8)
                        ledColor = Color.Gold;
                    else
                        ledColor = Color.Crimson;
                }
                else
                {
                    ledColor = Color.Gainsboro;
                }

                using (var brush = new SolidBrush(ledColor))
                {
                    g.FillRectangle(brush, ledRect);
                }
            }
        }
        #endregion

        #region Logging

        private void AddLogEntry(LogEventType type, string message)
        {
            _logEntries.Add(new LogEntry(type, message));
        }

        private void SaveLogFile()
        {
            if (!_logEntries.Any()) return;

            string logDir = Path.Combine(GetAppDataPath(), "WorkTimerLogs");
            Directory.CreateDirectory(logDir);

            if (string.IsNullOrEmpty(_currentLogFile))
            {
                string taskName = string.IsNullOrWhiteSpace(txtTaskName.Text) ? "Untitled" : txtTaskName.Text;
                string safeTaskName = string.Join("_", taskName.Split(Path.GetInvalidFileNameChars()));
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                _currentLogFile = Path.Combine(logDir, $"{safeTaskName}_{timestamp}.log");
            }

            try
            {
                string json = JsonSerializer.Serialize(_logEntries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_currentLogFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings();
            SaveSession();
            _activityHook?.Dispose();
            base.OnFormClosing(e);
        }
    }

    #region Data Structures

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogEventType EventType { get; set; }
        public string Message { get; set; }

        public LogEntry(LogEventType eventType, string message)
        {
            Timestamp = DateTime.Now;
            EventType = eventType;
            Message = message;
        }
        public LogEntry()
        {
            Message = string.Empty;
        }
    }

    public enum LogEventType
    {
        Start, Stop, Pause, Resume, Inactive, Note, Finish
    }

    public struct AbsencePeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class AppSettings
    {
        public int LastHours { get; set; } = 0;
        public int LastMinutes { get; set; } = 25;
        public int LastSeconds { get; set; } = 0;
        public bool AlwaysOnTop { get; set; } = true;
        public double Opacity { get; set; } = 1.0;
    }

    public class SessionState
    {
        public TimeSpan RemainingTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public Dictionary<DateTime, int> ActivityData { get; set; } = new Dictionary<DateTime, int>();
        public List<AbsencePeriod> Absences { get; set; } = new List<AbsencePeriod>();
        public List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
        public string LogFile { get; set; } = string.Empty;
        public DateTime ExitTime { get; set; }
    }

    #endregion
}

