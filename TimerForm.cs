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
        private int _opacityValue = 100;

        // Charting & Absences
        private Dictionary<DateTime, int> _activityData = new Dictionary<DateTime, int>();
        private List<(DateTime Start, DateTime End)> _absences = new List<(DateTime, DateTime)>();
        private System.Windows.Forms.Timer? _chartUpdateTimer;

        // Real-time Activity Meter
        private GlobalActivityHook _activityHook;
        private System.Windows.Forms.Timer _activityDecayTimer;
        private double _activityScore = 0;
        private const double MOUSE_WEIGHT = 0.5;
        private const double KEYBOARD_WEIGHT = 5.0;
        private const double ACTIVITY_DECAY_RATE = 0.90;

        #endregion

        public TimerForm()
        {
            InitializeComponent();
            _activityDecayTimer = new System.Windows.Forms.Timer();
            InitializeCustomComponents();
            _activityHook = new GlobalActivityHook();
            _activityHook.OnActivity += _activityHook_OnActivity;
        }

        private void InitializeCustomComponents()
        {
            // Chart setup
            SetupActivityChart();

            // Chart update timer
            _chartUpdateTimer = new System.Windows.Forms.Timer();
            _chartUpdateTimer.Interval = 5000;
            _chartUpdateTimer.Tick += ChartUpdateTimer_Tick;

            // Activity Meter Timer
            _activityDecayTimer.Interval = 100;
            _activityDecayTimer.Tick += ActivityDecayTimer_Tick;
            _activityDecayTimer.Start();
        }

        private void TimerForm_Load(object sender, EventArgs e)
        {
            // Load persisted timer settings
            LoadSettings();

            // Auto-reset the timer to the loaded values on startup
            ResetTimerToInput();

            // Programmatically position the Add Note button to guarantee correct placement.
            btnAddNote.Top = txtNote.Bottom + 6; // 6 pixel gap below the text box.
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
            ResetTimerToInput();
            if (_remainingTime <= TimeSpan.Zero) return;

            // Clear data for the new session
            _activityData.Clear();
            _absences.Clear();
            _logEntries.Clear();
            activityChart.Series["Activity"].Points.Clear();
            _currentLogFile = string.Empty;

            AddLogEntry(LogEventType.Start, $"Timer started for task: {txtTaskName.Text}");
            ResumeTimer(autoResumed: false, isNewSession: true);
        }

        private void ResumeTimer(bool autoResumed, bool isNewSession = false)
        {
            if (!isNewSession)
            {
                string reason = autoResumed ? "auto-resumed due to activity" : "resumed by user";
                AddLogEntry(LogEventType.Resume, $"Timer {reason}");
                if (_absences.Any())
                {
                    var lastAbsence = _absences.Last();
                    if (lastAbsence.End == DateTime.MaxValue)
                    {
                        _absences[^1] = (lastAbsence.Start, DateTime.Now); // Finalize the end time of the absence
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
                _absences.Add((absenceStart, DateTime.MaxValue)); // Mark end time as max until resumed
                AddLogEntry(LogEventType.Inactive, $"Auto-paused after {_idleSecondsForPause}s of inactivity.");
            }
            else
            {
                _isManuallyPaused = true;
                _manualPauseTime = DateTime.Now;
                lblStatus.Text = "Paused";
                _absences.Add((DateTime.Now, DateTime.MaxValue)); // Mark end time as max until resumed
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
                lblStatus.Text = "Time's up!";
                AddLogEntry(LogEventType.Finish, "Timer finished.");
                SystemSounds.Exclamation.Play();
                MessageBox.Show("Work session complete!", "Work Timer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Stopped. Set time to begin.";
                AddLogEntry(LogEventType.Stop, "Timer stopped by user.");
            }
            SaveLogFile();
        }

        #endregion

        #region Activity Meter
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
            StopTimer(isFinished: false);
            ResetTimerToInput();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            bool originalTopMost = this.TopMost;
            this.TopMost = false;
            using (var settingsForm = new SettingsForm(_idleSecondsForPause, _manualPauseSecondsForAutoResume, _wpmForFullMeter, _opacityValue, originalTopMost))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _idleSecondsForPause = settingsForm.IdleSeconds;
                    _manualPauseSecondsForAutoResume = settingsForm.ManualPauseSeconds;
                    _wpmForFullMeter = settingsForm.Wpm;
                    _opacityValue = settingsForm.OpacityValue;
                    this.Opacity = _opacityValue / 100.0;
                    this.TopMost = settingsForm.AlwaysOnTop;
                }
                else
                {
                    this.TopMost = originalTopMost;
                }
            }
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
            this.TopMost = true;
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
                                var duration = originalAbsence.End - originalAbsence.Start;

                                // Subtract the time from the countdown to reflect the work done during the "absence"
                                _remainingTime = _remainingTime.Subtract(duration);
                                AddLogEntry(LogEventType.Note, $"Absence from {originalAbsence.Start:HH:mm:ss} to {originalAbsence.End:HH:mm:ss} removed. Time subtracted to reflect work done.");
                            }
                        }
                        // Redraw the chart and progress ring to reflect the changes
                        progressRingPanel.Invalidate();
                        ChartUpdateTimer_Tick(null, EventArgs.Empty);
                    }
                }
            }
            this.TopMost = true;
        }

        private void ResetTimerToInput()
        {
            _remainingTime = new TimeSpan((int)numHours.Value, (int)numMinutes.Value, (int)numSeconds.Value);
            _totalTime = _remainingTime;
            lblStatus.Text = "Ready to start";
            progressRingPanel.Invalidate();
            SaveSettings(); // Save the new values whenever they are set
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
            chartArea.AxisX.IsMarginVisible = false;

            // Fix for axis label flicker
            chartArea.AxisX.Minimum = DateTime.Now.ToOADate();
            chartArea.AxisX.Maximum = DateTime.Now.AddHours(1).ToOADate();


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
            if (!_activityData.Any()) return;

            var series = activityChart.Series["Activity"];
            series.Points.Clear();

            foreach (var record in _activityData.OrderBy(kvp => kvp.Key))
            {
                series.Points.AddXY(record.Key, record.Value);
            }

            activityChart.ChartAreas[0].AxisX.Minimum = _activityData.Keys.Min().ToOADate();
            activityChart.ChartAreas[0].AxisX.Maximum = DateTime.Now.ToOADate();

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
                    foreach (var (start, end) in _absences)
                    {
                        DateTime endTime = (end == DateTime.MaxValue) ? DateTime.Now : end;

                        if (xAxis.Maximum < start.ToOADate() || xAxis.Minimum > endTime.ToOADate()) continue;

                        double x1 = xAxis.ValueToPixelPosition(start.ToOADate());
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

        #region Logging & Settings Persistence

        private void AddLogEntry(LogEventType type, string message)
        {
            _logEntries.Add(new LogEntry(type, message));
        }

        private void SaveLogFile()
        {
            if (!_logEntries.Any()) return;

            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkTimerLogs");
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

        private string GetSettingsFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "WorkTimer");
            Directory.CreateDirectory(appFolder);
            return Path.Combine(appFolder, "settings.json");
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    LastHours = (int)numHours.Value,
                    LastMinutes = (int)numMinutes.Value,
                    LastSeconds = (int)numSeconds.Value
                };
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetSettingsFilePath(), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                string filePath = GetSettingsFilePath();
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings != null)
                    {
                        numHours.Value = settings.LastHours;
                        numMinutes.Value = settings.LastMinutes;
                        numSeconds.Value = settings.LastSeconds;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load settings: {ex.Message}");
            }
        }


        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveSettings(); // Save current values on exit
            _activityHook?.Dispose();
            base.OnFormClosing(e);
        }
    }

    public class AppSettings
    {
        public int LastHours { get; set; }
        public int LastMinutes { get; set; }
        public int LastSeconds { get; set; }
    }


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
        Start,
        Stop,
        Pause,
        Resume,
        Inactive,
        Note,
        Finish
    }
}

