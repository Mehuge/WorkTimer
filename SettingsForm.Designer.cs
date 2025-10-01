namespace WorkTimer
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.numIdleTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numAutoResume = new System.Windows.Forms.NumericUpDown();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.numWpm = new System.Windows.Forms.NumericUpDown();
            this.opacityTrackBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numIdleTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoResume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWpm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Auto-pause after (seconds of idle)";
            // 
            // numIdleTime
            // 
            this.numIdleTime.Location = new System.Drawing.Point(212, 11);
            this.numIdleTime.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numIdleTime.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numIdleTime.Name = "numIdleTime";
            this.numIdleTime.Size = new System.Drawing.Size(75, 20);
            this.numIdleTime.TabIndex = 1;
            this.numIdleTime.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(189, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Auto-resume after (seconds if paused)";
            // 
            // numAutoResume
            // 
            this.numAutoResume.Location = new System.Drawing.Point(212, 45);
            this.numAutoResume.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numAutoResume.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numAutoResume.Name = "numAutoResume";
            this.numAutoResume.Size = new System.Drawing.Size(75, 20);
            this.numAutoResume.TabIndex = 3;
            this.numAutoResume.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(131, 214);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(212, 214);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Typing Speed for Full Meter (wpm)";
            // 
            // numWpm
            // 
            this.numWpm.Location = new System.Drawing.Point(212, 79);
            this.numWpm.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numWpm.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numWpm.Name = "numWpm";
            this.numWpm.Size = new System.Drawing.Size(75, 20);
            this.numWpm.TabIndex = 7;
            this.numWpm.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // opacityTrackBar
            // 
            this.opacityTrackBar.Location = new System.Drawing.Point(12, 133);
            this.opacityTrackBar.Maximum = 100;
            this.opacityTrackBar.Minimum = 20;
            this.opacityTrackBar.Name = "opacityTrackBar";
            this.opacityTrackBar.Size = new System.Drawing.Size(275, 45);
            this.opacityTrackBar.TabIndex = 8;
            this.opacityTrackBar.TickFrequency = 10;
            this.opacityTrackBar.Value = 100;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Window Opacity";
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.AutoSize = true;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(16, 184);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(98, 17);
            this.chkAlwaysOnTop.TabIndex = 10;
            this.chkAlwaysOnTop.Text = "Always on Top";
            this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(304, 251);
            this.Controls.Add(this.chkAlwaysOnTop);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.opacityTrackBar);
            this.Controls.Add(this.numWpm);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.numAutoResume);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numIdleTime);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numIdleTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoResume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWpm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numIdleTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numAutoResume;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numWpm;
        private System.Windows.Forms.TrackBar opacityTrackBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAlwaysOnTop;
    }
}

