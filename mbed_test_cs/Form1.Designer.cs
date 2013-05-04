namespace mbed_test_cs
{
    partial class Form1
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
            this.btnConnectMbed = new System.Windows.Forms.Button();
            this.btnInitCamera = new System.Windows.Forms.Button();
            this.btnSWTrigger = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSetShutter = new System.Windows.Forms.Button();
            this.btnSetISO = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timerPPS = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnectMbed
            // 
            this.btnConnectMbed.Location = new System.Drawing.Point(26, 11);
            this.btnConnectMbed.Margin = new System.Windows.Forms.Padding(2);
            this.btnConnectMbed.Name = "btnConnectMbed";
            this.btnConnectMbed.Size = new System.Drawing.Size(95, 46);
            this.btnConnectMbed.TabIndex = 0;
            this.btnConnectMbed.Text = "Connect mbed";
            this.btnConnectMbed.UseVisualStyleBackColor = true;
            this.btnConnectMbed.Click += new System.EventHandler(this.btnConnectMbed_Click);
            // 
            // btnInitCamera
            // 
            this.btnInitCamera.Location = new System.Drawing.Point(142, 11);
            this.btnInitCamera.Margin = new System.Windows.Forms.Padding(2);
            this.btnInitCamera.Name = "btnInitCamera";
            this.btnInitCamera.Size = new System.Drawing.Size(86, 30);
            this.btnInitCamera.TabIndex = 1;
            this.btnInitCamera.Text = "Init Camera";
            this.btnInitCamera.UseVisualStyleBackColor = true;
            this.btnInitCamera.Click += new System.EventHandler(this.btnInitCamera_Click);
            // 
            // btnSWTrigger
            // 
            this.btnSWTrigger.Location = new System.Drawing.Point(142, 54);
            this.btnSWTrigger.Margin = new System.Windows.Forms.Padding(2);
            this.btnSWTrigger.Name = "btnSWTrigger";
            this.btnSWTrigger.Size = new System.Drawing.Size(86, 28);
            this.btnSWTrigger.TabIndex = 2;
            this.btnSWTrigger.Text = "SW Trigger";
            this.btnSWTrigger.UseVisualStyleBackColor = true;
            this.btnSWTrigger.Click += new System.EventHandler(this.btnSWTrigger_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(248, 24);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(69, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "0.50";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "shutter (ms)";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "100",
            "125",
            "160",
            "200",
            "250",
            "320",
            "400",
            "500",
            "640",
            "800",
            "1000",
            "1250",
            "1600",
            "2000",
            "2500",
            "3200",
            "4000",
            "5000",
            "6400"});
            this.comboBox1.Location = new System.Drawing.Point(248, 63);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(70, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(247, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "ISO";
            // 
            // btnSetShutter
            // 
            this.btnSetShutter.Location = new System.Drawing.Point(328, 11);
            this.btnSetShutter.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetShutter.Name = "btnSetShutter";
            this.btnSetShutter.Size = new System.Drawing.Size(37, 30);
            this.btnSetShutter.TabIndex = 7;
            this.btnSetShutter.Text = "Set";
            this.btnSetShutter.UseVisualStyleBackColor = true;
            this.btnSetShutter.Click += new System.EventHandler(this.btnSetShutter_Click);
            // 
            // btnSetISO
            // 
            this.btnSetISO.Location = new System.Drawing.Point(328, 54);
            this.btnSetISO.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetISO.Name = "btnSetISO";
            this.btnSetISO.Size = new System.Drawing.Size(37, 28);
            this.btnSetISO.TabIndex = 8;
            this.btnSetISO.Text = "Set";
            this.btnSetISO.UseVisualStyleBackColor = true;
            this.btnSetISO.Click += new System.EventHandler(this.btnSetISO_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(17, 114);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(434, 305);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Status",
            "Position/Velocity",
            "Start Data Record",
            "Stop Data Record",
            "Start Pos Stream",
            "Stop Pos Stream",
            "Start Log Message Info",
            "Stop Log Message Info",
            "Fire Trigger"});
            this.comboBox2.Location = new System.Drawing.Point(386, 17);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(174, 21);
            this.comboBox2.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(446, 47);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 34);
            this.button1.TabIndex = 11;
            this.button1.Text = "Send Message";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // timerPPS
            // 
            this.timerPPS.Enabled = true;
            this.timerPPS.Interval = 1000;
            this.timerPPS.Tick += new System.EventHandler(this.timerPPS_Tick);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.Color.Red;
            this.button2.Location = new System.Drawing.Point(475, 302);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 49);
            this.button2.TabIndex = 12;
            this.button2.Text = "STOP";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 471);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnSetISO);
            this.Controls.Add(this.btnSetShutter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnSWTrigger);
            this.Controls.Add(this.btnInitCamera);
            this.Controls.Add(this.btnConnectMbed);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Waldo Device Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnectMbed;
        private System.Windows.Forms.Button btnInitCamera;
        private System.Windows.Forms.Button btnSWTrigger;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSetShutter;
        private System.Windows.Forms.Button btnSetISO;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timerPPS;
        private System.Windows.Forms.Button button2;
    }
}

