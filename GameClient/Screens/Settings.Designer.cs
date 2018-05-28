namespace LevelRunner
{
    partial class Settings
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.volumeTracker = new System.Windows.Forms.TrackBar();
            this.playerColorPickButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.screenModePicker = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTracker)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            this.label1.Location = new System.Drawing.Point(529, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SCREEN MODE:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            this.label2.Location = new System.Drawing.Point(529, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "VOLUME LEVEL:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            this.label3.Location = new System.Drawing.Point(529, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "STANDARD CHUNK SIZE:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            this.label4.Location = new System.Drawing.Point(529, 182);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "PLAYER COLOR:";
            // 
            // volumeTracker
            // 
            this.volumeTracker.AutoSize = false;
            this.volumeTracker.Location = new System.Drawing.Point(759, 270);
            this.volumeTracker.Maximum = 100;
            this.volumeTracker.Name = "volumeTracker";
            this.volumeTracker.Size = new System.Drawing.Size(150, 30);
            this.volumeTracker.TabIndex = 4;
            this.volumeTracker.Value = 1;
            this.volumeTracker.Scroll += new System.EventHandler(this.volumeTracker_Scroll);
            // 
            // playerColorPickButton
            // 
            this.playerColorPickButton.BackColor = System.Drawing.Color.Transparent;
            this.playerColorPickButton.FlatAppearance.BorderSize = 0;
            this.playerColorPickButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Maroon;
            this.playerColorPickButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.playerColorPickButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playerColorPickButton.Font = new System.Drawing.Font("Courier New", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.playerColorPickButton.ForeColor = System.Drawing.Color.LightGoldenrodYellow;
            this.playerColorPickButton.Location = new System.Drawing.Point(759, 176);
            this.playerColorPickButton.Name = "playerColorPickButton";
            this.playerColorPickButton.Size = new System.Drawing.Size(150, 25);
            this.playerColorPickButton.TabIndex = 5;
            this.playerColorPickButton.Text = "CHOOSE";
            this.playerColorPickButton.UseVisualStyleBackColor = false;
            this.playerColorPickButton.Click += new System.EventHandler(this.playerColorPickButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(759, 226);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(150, 20);
            this.textBox1.TabIndex = 6;
            // 
            // screenModePicker
            // 
            this.screenModePicker.BackColor = System.Drawing.SystemColors.Window;
            this.screenModePicker.FormattingEnabled = true;
            this.screenModePicker.Items.AddRange(new object[] {
            "FULLSCREEN",
            "WINDOWED"});
            this.screenModePicker.Location = new System.Drawing.Point(759, 132);
            this.screenModePicker.Name = "screenModePicker";
            this.screenModePicker.Size = new System.Drawing.Size(150, 21);
            this.screenModePicker.TabIndex = 7;
            this.screenModePicker.SelectedIndexChanged += new System.EventHandler(this.screenModePicker_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.okButton.BackColor = System.Drawing.Color.Transparent;
            this.okButton.FlatAppearance.BorderSize = 0;
            this.okButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Maroon;
            this.okButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Courier New", 16F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.okButton.ForeColor = System.Drawing.Color.Brown;
            this.okButton.Location = new System.Drawing.Point(540, 350);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(377, 33);
            this.okButton.TabIndex = 27;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::LevelRunner.Properties.Resources.SettingsBackground;
            this.ClientSize = new System.Drawing.Size(1360, 768);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.screenModePicker);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.playerColorPickButton);
            this.Controls.Add(this.volumeTracker);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Settings";
            this.Text = "Settings";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.volumeTracker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar volumeTracker;
        private System.Windows.Forms.Button playerColorPickButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox screenModePicker;
        private System.Windows.Forms.Button okButton;
    }
}