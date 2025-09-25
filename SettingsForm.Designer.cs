namespace WorkTimer
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NumericUpDown numIdleTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numAutoResumeTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numKeyboardWeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numTypingSpeed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.numIdleTime = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numAutoResumeTime = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numKeyboardWeight = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numTypingSpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numIdleTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoResumeTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyboardWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTypingSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // numIdleTime
            // 
            this.numIdleTime.Location = new System.Drawing.Point(220, 25);
            this.numIdleTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numIdleTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numIdleTime.Name = "numIdleTime";
            this.numIdleTime.Size = new System.Drawing.Size(80, 20);
            this.numIdleTime.TabIndex = 0;
            this.numIdleTime.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Auto-pause after idle (minutes):";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(144, 180);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(225, 180);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Auto-resume after manual pause (sec):";
            // 
            // numAutoResumeTime
            // 
            this.numAutoResumeTime.Location = new System.Drawing.Point(220, 60);
            this.numAutoResumeTime.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numAutoResumeTime.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAutoResumeTime.Name = "numAutoResumeTime";
            this.numAutoResumeTime.Size = new System.Drawing.Size(80, 20);
            this.numAutoResumeTime.TabIndex = 5;
            this.numAutoResumeTime.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Keyboard Weight (x):";
            // 
            // numKeyboardWeight
            // 
            this.numKeyboardWeight.Location = new System.Drawing.Point(220, 95);
            this.numKeyboardWeight.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numKeyboardWeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numKeyboardWeight.Name = "numKeyboardWeight";
            this.numKeyboardWeight.Size = new System.Drawing.Size(80, 20);
            this.numKeyboardWeight.TabIndex = 7;
            this.numKeyboardWeight.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Typing Speed for Max (WPM):";
            // 
            // numTypingSpeed
            // 
            this.numTypingSpeed.Location = new System.Drawing.Point(220, 130);
            this.numTypingSpeed.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.numTypingSpeed.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numTypingSpeed.Name = "numTypingSpeed";
            this.numTypingSpeed.Size = new System.Drawing.Size(80, 20);
            this.numTypingSpeed.TabIndex = 9;
            this.numTypingSpeed.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(324, 221);
            this.Controls.Add(this.numTypingSpeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numKeyboardWeight);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numAutoResumeTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numIdleTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numIdleTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoResumeTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numKeyboardWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTypingSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

