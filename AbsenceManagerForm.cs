using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WorkTimer
{
    public partial class AbsenceManagerForm : Form
    {
        public List<(DateTime Start, DateTime End)> AbsencesToRemove { get; private set; }

        private List<(DateTime Start, DateTime End)> _allAbsences;

        public AbsenceManagerForm(List<(DateTime Start, DateTime End)> absences)
        {
            InitializeComponent();
            _allAbsences = absences;
            AbsencesToRemove = new List<(DateTime Start, DateTime End)>();
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
            AbsencesToRemove.Clear();
            foreach (int selectedIndex in lstAbsences.SelectedIndices)
            {
                if (selectedIndex >= 0 && selectedIndex < _allAbsences.Count)
                {
                    AbsencesToRemove.Add(_allAbsences[selectedIndex]);
                }
            }

            if (AbsencesToRemove.Any())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select at least one absence to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

