using System;
using System.Windows.Forms;

namespace WorkTimer
{
    public partial class SettingsForm : Form
    {
        public int IdleSeconds { get; private set; }
        public int ManualPauseSeconds { get; private set; }
        public int Wpm { get; private set; }
        public int OpacityValue { get; private set; }
        public bool AlwaysOnTop { get; private set; }


        public SettingsForm(int idleSeconds, int manualPauseSeconds, int wpm, int opacityValue, bool alwaysOnTop)
        {
            InitializeComponent();
            numIdleTime.Value = idleSeconds;
            numAutoResume.Value = manualPauseSeconds;
            numWpm.Value = wpm;
            opacityTrackBar.Value = opacityValue;
            chkAlwaysOnTop.Checked = alwaysOnTop;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            IdleSeconds = (int)numIdleTime.Value;
            ManualPauseSeconds = (int)numAutoResume.Value;
            Wpm = (int)numWpm.Value;
            OpacityValue = opacityTrackBar.Value;
            AlwaysOnTop = chkAlwaysOnTop.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

