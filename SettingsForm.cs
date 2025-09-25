using System;
using System.Windows.Forms;

namespace WorkTimer
{
    public partial class SettingsForm : Form
    {
        public int IdleSeconds { get; private set; }
        public int ManualPauseSeconds { get; private set; }
        public int KeyboardWeight { get; private set; }
        public int TypingSpeedForMax { get; private set; }

        public SettingsForm(int idleSeconds, int manualPauseSeconds, int keyboardWeight, int typingSpeed)
        {
            InitializeComponent();
            numIdleTime.Value = idleSeconds / 60; // Convert to minutes for UI
            numAutoResumeTime.Value = manualPauseSeconds;
            numKeyboardWeight.Value = keyboardWeight;
            numTypingSpeed.Value = typingSpeed;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            IdleSeconds = (int)numIdleTime.Value * 60; // Convert back to seconds
            ManualPauseSeconds = (int)numAutoResumeTime.Value;
            KeyboardWeight = (int)numKeyboardWeight.Value;
            TypingSpeedForMax = (int)numTypingSpeed.Value;
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

