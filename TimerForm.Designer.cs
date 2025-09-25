namespace WorkTimer
{
    partial class TimerForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimerForm));
            this.mainTimer = new System.Windows.Forms.Timer(this.components);
            this.activityMonitorTimer = new System.Windows.Forms.Timer(this.components);
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.opacityTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.activityChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.numHours = new System.Windows.Forms.NumericUpDown();
            this.numMinutes = new System.Windows.Forms.NumericUpDown();
            this.numSeconds = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressRingPanel = new System.Windows.Forms.Panel();
            this.activityMeterPanel = new System.Windows.Forms.Panel();
            this.txtTaskName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.btnAddNote = new System.Windows.Forms.Button();
            this.btnViewLogs = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTimer
            // 
            this.mainTimer.Interval = 1000;
            this.mainTimer.Tick += new System.EventHandler(this.mainTimer_Tick);
            // 
            // activityMonitorTimer
            // 
            this.activityMonitorTimer.Interval = 1000;
            this.activityMonitorTimer.Tick += new System.EventHandler(this.activityMonitorTimer_Tick);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Location = new System.Drawing.Point(17, 189);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(75, 23);
            this.btnPlayPause.TabIndex = 4;
            this.btnPlayPause.Text = "Play";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(98, 189);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(179, 189);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 6;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(14, 225);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(81, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Ready to start...";
            // 
            // opacityTrackBar
            // 
            this.opacityTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.opacityTrackBar.Location = new System.Drawing.Point(260, 189);
            this.opacityTrackBar.Maximum = 100;
            this.opacityTrackBar.Minimum = 10;
            this.opacityTrackBar.Name = "opacityTrackBar";
            this.opacityTrackBar.Size = new System.Drawing.Size(306, 45);
            this.opacityTrackBar.TabIndex = 7;
            this.opacityTrackBar.TickFrequency = 10;
            this.opacityTrackBar.Value = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(260, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Opacity";
            // 
            // activityChart
            // 
            this.activityChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.activityChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.activityChart.Legends.Add(legend1);
            this.activityChart.Location = new System.Drawing.Point(17, 275);
            this.activityChart.Name = "activityChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.activityChart.Series.Add(series1);
            this.activityChart.Size = new System.Drawing.Size(549, 161);
            this.activityChart.TabIndex = 12;
            this.activityChart.Text = "chart1";
            // 
            // numHours
            // 
            this.numHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numHours.Location = new System.Drawing.Point(17, 154);
            this.numHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numHours.Name = "numHours";
            this.numHours.Size = new System.Drawing.Size(45, 22);
            this.numHours.TabIndex = 1;
            // 
            // numMinutes
            // 
            this.numMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numMinutes.Location = new System.Drawing.Point(88, 154);
            this.numMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numMinutes.Name = "numMinutes";
            this.numMinutes.Size = new System.Drawing.Size(45, 22);
            this.numMinutes.TabIndex = 2;
            this.numMinutes.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // numSeconds
            // 
            this.numSeconds.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numSeconds.Location = new System.Drawing.Point(158, 154);
            this.numSeconds.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numSeconds.Name = "numSeconds";
            this.numSeconds.Size = new System.Drawing.Size(45, 22);
            this.numSeconds.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(68, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = ":";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(138, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = ":";
            // 
            // progressRingPanel
            // 
            this.progressRingPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressRingPanel.Location = new System.Drawing.Point(260, 12);
            this.progressRingPanel.Name = "progressRingPanel";
            this.progressRingPanel.Size = new System.Drawing.Size(306, 158);
            this.progressRingPanel.TabIndex = 18;
            this.progressRingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.progressRingPanel_Paint);
            // 
            // activityMeterPanel
            // 
            this.activityMeterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activityMeterPanel.Location = new System.Drawing.Point(572, 12);
            this.activityMeterPanel.Name = "activityMeterPanel";
            this.activityMeterPanel.Size = new System.Drawing.Size(20, 424);
            this.activityMeterPanel.TabIndex = 19;
            this.activityMeterPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.activityMeterPanel_Paint);
            // 
            // txtTaskName
            // 
            this.txtTaskName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTaskName.Location = new System.Drawing.Point(17, 34);
            this.txtTaskName.Name = "txtTaskName";
            this.txtTaskName.Size = new System.Drawing.Size(237, 22);
            this.txtTaskName.TabIndex = 0;
            this.txtTaskName.Text = "My Work Task";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Task Name";
            // 
            // txtNote
            // 
            this.txtNote.AcceptsReturn = true;
            this.txtNote.Location = new System.Drawing.Point(17, 62);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(237, 51);
            this.txtNote.TabIndex = 22;
            // 
            // btnAddNote
            // 
            this.btnAddNote.Location = new System.Drawing.Point(179, 119);
            this.btnAddNote.Name = "btnAddNote";
            this.btnAddNote.Size = new System.Drawing.Size(75, 23);
            this.btnAddNote.TabIndex = 23;
            this.btnAddNote.Text = "Add Note";
            this.btnAddNote.UseVisualStyleBackColor = true;
            this.btnAddNote.Click += new System.EventHandler(this.btnAddNote_Click);
            // 
            // btnViewLogs
            // 
            this.btnViewLogs.Location = new System.Drawing.Point(17, 119);
            this.btnViewLogs.Name = "btnViewLogs";
            this.btnViewLogs.Size = new System.Drawing.Size(75, 23);
            this.btnViewLogs.TabIndex = 24;
            this.btnViewLogs.Text = "View Logs";
            this.btnViewLogs.UseVisualStyleBackColor = true;
            this.btnViewLogs.Click += new System.EventHandler(this.btnViewLogs_Click);
            // 
            // TimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 448);
            this.Controls.Add(this.btnViewLogs);
            this.Controls.Add(this.btnAddNote);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTaskName);
            this.Controls.Add(this.activityMeterPanel);
            this.Controls.Add(this.progressRingPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSeconds);
            this.Controls.Add(this.numMinutes);
            this.Controls.Add(this.numHours);
            this.Controls.Add(this.activityChart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.opacityTrackBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnPlayPause);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(620, 487);
            this.Name = "TimerForm";
            this.Text = "Work Timer";
            ((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.activityChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSeconds)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer mainTimer;
        private System.Windows.Forms.Timer activityMonitorTimer;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TrackBar opacityTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart activityChart;
        private System.Windows.Forms.NumericUpDown numHours;
        private System.Windows.Forms.NumericUpDown numMinutes;
        private System.Windows.Forms.NumericUpDown numSeconds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel progressRingPanel;
        private System.Windows.Forms.Panel activityMeterPanel;
        private System.Windows.Forms.TextBox txtTaskName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Button btnAddNote;
        private System.Windows.Forms.Button btnViewLogs;
    }
}

