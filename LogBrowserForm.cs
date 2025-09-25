using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static WorkTimer.TimerForm;

namespace WorkTimer
{
    public partial class LogBrowserForm : Form
    {
        private readonly string _logDirectory;

        public LogBrowserForm()
        {
            InitializeComponent();
            _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WorkTimerLogs");
            this.Load += LogBrowserForm_Load;
        }

        private void LogBrowserForm_Load(object? sender, EventArgs e)
        {
            LoadLogFiles();
        }

        private void LoadLogFiles()
        {
            lstLogFiles.Items.Clear();
            txtLogDetails.Clear();

            if (!Directory.Exists(_logDirectory))
            {
                lstLogFiles.Items.Add("No logs found.");
                return;
            }

            var logFiles = Directory.GetFiles(_logDirectory, "*.log")
                                    .Select(f => new FileInfo(f))
                                    .OrderByDescending(f => f.LastWriteTime)
                                    .ToList();

            if (!logFiles.Any())
            {
                lstLogFiles.Items.Add("No logs found.");
                return;
            }

            foreach (var file in logFiles)
            {
                // Display a more friendly name in the list
                string displayName = file.Name.Replace(".log", "").Replace("_", " ");
                var item = new ListViewItem(displayName) { Tag = file.FullName };
                lstLogFiles.Items.Add(item);
            }
        }

        private void lstLogFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstLogFiles.SelectedItems.Count > 0)
            {
                var selectedItem = lstLogFiles.SelectedItems[0];
                string filePath = selectedItem.Tag as string ?? string.Empty;

                if (File.Exists(filePath))
                {
                    try
                    {
                        string[] logLines = File.ReadAllLines(filePath);
                        txtLogDetails.Text = string.Join(Environment.NewLine, logLines);
                    }
                    catch (Exception ex)
                    {
                        txtLogDetails.Text = $"Error reading log file:\n{ex.Message}";
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogFiles();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
