namespace WorkTimer
{
    partial class SetTimeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            numHours = new NumericUpDown();
            numMinutes = new NumericUpDown();
            numSeconds = new NumericUpDown();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            btnSet = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)numHours).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSeconds).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(160, 21);
            label1.TabIndex = 0;
            label1.Text = "Set Timer Duration";
            // 
            // numHours
            // 
            numHours.Font = new Font("Segoe UI", 12F);
            numHours.Location = new Point(16, 60);
            numHours.Maximum = new decimal(new int[] { 23, 0, 0, 0 });
            numHours.Name = "numHours";
            numHours.Size = new Size(70, 29);
            numHours.TabIndex = 1;
            // 
            // numMinutes
            // 
            numMinutes.Font = new Font("Segoe UI", 12F);
            numMinutes.Location = new Point(111, 60);
            numMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            numMinutes.Name = "numMinutes";
            numMinutes.Size = new Size(70, 29);
            numMinutes.TabIndex = 2;
            numMinutes.Value = new decimal(new int[] { 45, 0, 0, 0 });
            // 
            // numSeconds
            // 
            numSeconds.Font = new Font("Segoe UI", 12F);
            numSeconds.Location = new Point(206, 60);
            numSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            numSeconds.Name = "numSeconds";
            numSeconds.Size = new Size(70, 29);
            numSeconds.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 42);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 4;
            label2.Text = "Hours";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(111, 42);
            label3.Name = "label3";
            label3.Size = new Size(50, 15);
            label3.TabIndex = 5;
            label3.Text = "Minutes";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(206, 42);
            label4.Name = "label4";
            label4.Size = new Size(51, 15);
            label4.TabIndex = 6;
            label4.Text = "Seconds";
            // 
            // btnSet
            // 
            btnSet.Location = new Point(120, 115);
            btnSet.Name = "btnSet";
            btnSet.Size = new Size(75, 29);
            btnSet.TabIndex = 7;
            btnSet.Text = "Set";
            btnSet.UseVisualStyleBackColor = true;
            btnSet.Click += btnSet_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(201, 115);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 29);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // SetTimeForm
            // 
            AcceptButton = btnSet;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(294, 156);
            Controls.Add(btnCancel);
            Controls.Add(btnSet);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(numSeconds);
            Controls.Add(numMinutes);
            Controls.Add(numHours);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SetTimeForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Set Time";
            ((System.ComponentModel.ISupportInitialize)numHours).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSeconds).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NumericUpDown numHours;
        private NumericUpDown numMinutes;
        private NumericUpDown numSeconds;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button btnSet;
        private Button btnCancel;
    }
}
