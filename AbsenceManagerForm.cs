using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WorkTimer
{
    public partial class AbsenceManagerForm : Form
    {
        private List<AbsencePeriod> _allAbsences;
        public List<AbsencePeriod> AbsencesToRemove { get; private set; }

        public AbsenceManagerForm(List<AbsencePeriod> absences)
        {
            InitializeComponent();
            _allAbsences = absences;
            AbsencesToRemove = new List<AbsencePeriod>();
        }

        private void AbsenceManagerForm_Load(object sender, EventArgs e)
        {
            foreach (var absence in _allAbsences)
            {
                var duration = absence.End - absence.Start;
                lstAbsences.Items.Add($"From: {absence.Start:HH:mm:ss}  |  Duration: {duration:mm\\:ss}");
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstAbsences.SelectedIndices.Count == 0)
            {
                MessageBox.Show("Please select one or more absences to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to remove the selected absence(s)? This will subtract the time from your countdown.", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                foreach (int index in lstAbsences.SelectedIndices.Cast<int>().OrderByDescending(i => i))
                {
                    AbsencesToRemove.Add(_allAbsences[index]);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

