namespace DevEck.Tests
{
    partial class CaptureTest
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
            this.comboBoxCaptureDevice = new System.Windows.Forms.ComboBox();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.imageCapture = new DevEck.Devices.Video.ImageCaptureControl();
            this.saveFileDlg = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // comboBoxCaptureDevice
            // 
            this.comboBoxCaptureDevice.BackColor = System.Drawing.Color.Silver;
            this.comboBoxCaptureDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCaptureDevice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxCaptureDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxCaptureDevice.ForeColor = System.Drawing.Color.Black;
            this.comboBoxCaptureDevice.FormattingEnabled = true;
            this.comboBoxCaptureDevice.Location = new System.Drawing.Point(12, 12);
            this.comboBoxCaptureDevice.Name = "comboBoxCaptureDevice";
            this.comboBoxCaptureDevice.Size = new System.Drawing.Size(292, 24);
            this.comboBoxCaptureDevice.TabIndex = 1;
            this.comboBoxCaptureDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxCaptureDevice_SelectedIndexChanged);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCapture.Location = new System.Drawing.Point(84, 286);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(136, 31);
            this.buttonCapture.TabIndex = 2;
            this.buttonCapture.Text = "Capture image";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // imageCapture
            // 
            this.imageCapture.BackColor = System.Drawing.Color.White;
            this.imageCapture.Device = null;
            this.imageCapture.Location = new System.Drawing.Point(12, 44);
            this.imageCapture.MaintainAspectRatio = false;
            this.imageCapture.Name = "imageCapture";
            this.imageCapture.Size = new System.Drawing.Size(292, 236);
            this.imageCapture.TabIndex = 0;
            // 
            // saveFileDlg
            // 
            this.saveFileDlg.FileName = "image.jpg";
            this.saveFileDlg.Filter = "*.jpg|*.jpg";
            // 
            // CaptureTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(316, 329);
            this.Controls.Add(this.buttonCapture);
            this.Controls.Add(this.comboBoxCaptureDevice);
            this.Controls.Add(this.imageCapture);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CaptureTest";
            this.Text = "CaptureTest";
            this.Shown += new System.EventHandler(this.CaptureTest_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private DevEck.Devices.Video.ImageCaptureControl imageCapture;
        private System.Windows.Forms.ComboBox comboBoxCaptureDevice;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.SaveFileDialog saveFileDlg;
    }
}