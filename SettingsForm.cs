using System;
using System.Windows.Forms;

namespace WorkTimer
{
    public partial class SettingsForm : Form
    {
        public int IdleSeconds { get; private set; }
        public int ManualPauseSeconds { get; private set; }
        public int Wpm { get; private set; }
        public bool AlwaysOnTop { get; private set; }
        public double OpacityValue { get; private set; }

        public SettingsForm(int idleSeconds, int manualPauseSeconds, int wpm, bool alwaysOnTop, double opacity)
        {
            InitializeComponent();

            // Convert seconds to minutes for display
            txtIdleTime.Text = (idleSeconds / 60).ToString();
            txtAutoResume.Text = (manualPauseSeconds / 60).ToString();
            numWpm.Value = wpm;
            chkAlwaysOnTop.Checked = alwaysOnTop;
            opacityTrackBar.Value = (int)(opacity * 100);
            lblOpacityValue.Text = $"{opacityTrackBar.Value}%";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // Validate Idle Time
            if (!int.TryParse(txtIdleTime.Text, out int idleMinutes) || idleMinutes < 0)
            {
                MessageBox.Show("Please enter a valid whole number for the idle time.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate Auto-Resume Time
            if (!int.TryParse(txtAutoResume.Text, out int autoResumeMinutes) || autoResumeMinutes < 0)
            {
                MessageBox.Show("Please enter a valid whole number for the auto-resume time.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Convert minutes back to seconds before saving
            IdleSeconds = idleMinutes * 60;
            ManualPauseSeconds = autoResumeMinutes * 60;
            Wpm = (int)numWpm.Value;
            AlwaysOnTop = chkAlwaysOnTop.Checked;
            OpacityValue = opacityTrackBar.Value / 100.0;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void opacityTrackBar_Scroll(object sender, EventArgs e)
        {
            lblOpacityValue.Text = $"{opacityTrackBar.Value}%";
        }
    }
}

