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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSetShutter = new System.Windows.Forms.Button();
            this.btnSetISO = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.CameraImageReturnedThread = new System.ComponentModel.BackgroundWorker();
            this.timerPPS = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.PosVelThread = new System.ComponentModel.BackgroundWorker();
            this.timerPosVel = new System.Windows.Forms.Timer(this.components);
            this.triggerRequestThread = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
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
            // CameraImageReturnedThread
            // 
            this.CameraImageReturnedThread.WorkerReportsProgress = true;
            this.CameraImageReturnedThread.WorkerSupportsCancellation = true;
            this.CameraImageReturnedThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.cameraImageReturned_DoWork);
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
            // PosVelThread
            // 
            this.PosVelThread.WorkerReportsProgress = true;
            this.PosVelThread.WorkerSupportsCancellation = true;
            this.PosVelThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.PosVelThread_DoWork);
            // 
            // timerPosVel
            // 
            this.timerPosVel.Tick += new System.EventHandler(this.timerPosVel_Tick);
            // 
            // triggerRequestThread
            // 
            this.triggerRequestThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.triggerRequestThread_DoWork);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 471);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnSetISO);
            this.Controls.Add(this.btnSetShutter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Waldo Device Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSetShutter;
        private System.Windows.Forms.Button btnSetISO;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.ComponentModel.BackgroundWorker CameraImageReturnedThread;
        private System.Windows.Forms.Timer timerPPS;
        private System.Windows.Forms.Button button2;
        private System.ComponentModel.BackgroundWorker PosVelThread;
        private System.Windows.Forms.Timer timerPosVel;
        private System.ComponentModel.BackgroundWorker triggerRequestThread;
    }
}

