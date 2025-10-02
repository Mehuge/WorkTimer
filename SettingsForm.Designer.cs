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
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numWpm = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.opacityTrackBar = new System.Windows.Forms.TrackBar();
            this.lblOpacityValue = new System.Windows.Forms.Label();
            this.txtIdleTime = new System.Windows.Forms.TextBox();
            this.txtAutoResume = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numWpm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Auto-pause after idle for";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Auto-resume after manual pause for";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(125, 226);
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
            this.btnCancel.Location = new System.Drawing.Point(206, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(245, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "minutes";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(245, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "minutes";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Typing speed for full acitvity at";
            // 
            // numWpm
            // 
            this.numWpm.Location = new System.Drawing.Point(204, 83);
            this.numWpm.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numWpm.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numWpm.Name = "numWpm";
            this.numWpm.Size = new System.Drawing.Size(35, 20);
            this.numWpm.TabIndex = 9;
            this.numWpm.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(245, 85);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "WPM";
            // 
            // chkAlwaysOnTop
            // 
            this.chkAlwaysOnTop.AutoSize = true;
            this.chkAlwaysOnTop.Location = new System.Drawing.Point(16, 120);
            this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
            this.chkAlwaysOnTop.Size = new System.Drawing.Size(130, 17);
            this.chkAlwaysOnTop.TabIndex = 11;
            this.chkAlwaysOnTop.Text = "Always keep on top";
            this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Opacity:";
            // 
            // opacityTrackBar
            // 
            this.opacityTrackBar.Location = new System.Drawing.Point(16, 175);
            this.opacityTrackBar.Maximum = 100;
            this.opacityTrackBar.Minimum = 20;
            this.opacityTrackBar.Name = "opacityTrackBar";
            this.opacityTrackBar.Size = new System.Drawing.Size(265, 45);
            this.opacityTrackBar.TabIndex = 13;
            this.opacityTrackBar.TickFrequency = 10;
            this.opacityTrackBar.Value = 100;
            this.opacityTrackBar.Scroll += new System.EventHandler(this.opacityTrackBar_Scroll);
            // 
            // lblOpacityValue
            // 
            this.lblOpacityValue.AutoSize = true;
            this.lblOpacityValue.Location = new System.Drawing.Point(65, 154);
            this.lblOpacityValue.Name = "lblOpacityValue";
            this.lblOpacityValue.Size = new System.Drawing.Size(33, 13);
            this.lblOpacityValue.TabIndex = 14;
            this.lblOpacityValue.Text = "100%";
            // 
            // txtIdleTime
            // 
            this.txtIdleTime.Location = new System.Drawing.Point(204, 10);
            this.txtIdleTime.Name = "txtIdleTime";
            this.txtIdleTime.Size = new System.Drawing.Size(35, 20);
            this.txtIdleTime.TabIndex = 15;
            // 
            // txtAutoResume
            // 
            this.txtAutoResume.Location = new System.Drawing.Point(204, 46);
            this.txtAutoResume.Name = "txtAutoResume";
            this.txtAutoResume.Size = new System.Drawing.Size(35, 20);
            this.txtAutoResume.TabIndex = 16;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(294, 261);
            this.Controls.Add(this.txtAutoResume);
            this.Controls.Add(this.txtIdleTime);
            this.Controls.Add(this.lblOpacityValue);
            this.Controls.Add(this.opacityTrackBar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkAlwaysOnTop);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numWpm);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numWpm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numWpm;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkAlwaysOnTop;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar opacityTrackBar;
        private System.Windows.Forms.Label lblOpacityValue;
        private System.Windows.Forms.TextBox txtIdleTime;
        private System.Windows.Forms.TextBox txtAutoResume;
    }
}

