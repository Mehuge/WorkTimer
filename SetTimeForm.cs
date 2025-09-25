namespace WorkTimer
{
    public partial class SetTimeForm : Form
    {
        public TimeSpan SelectedTime { get; private set; }

        public SetTimeForm()
        {
            InitializeComponent();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            int hours = (int)numHours.Value;
            int minutes = (int)numMinutes.Value;
            int seconds = (int)numSeconds.Value;

            if (hours == 0 && minutes == 0 && seconds == 0)
            {
                MessageBox.Show("Please set a time greater than zero.", "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedTime = new TimeSpan(hours, minutes, seconds);
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

