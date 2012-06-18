using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevEck.Devices.Video;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace DevEck.Tests
{
    public partial class CaptureTest : Form
    {
        public CaptureTest()
        {
            InitializeComponent();
        }

        private void CaptureTest_Shown(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }

        private void comboBoxCaptureDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Device selectedDevice = comboBoxCaptureDevice.SelectedItem as Device;

            imageCapture.Device = selectedDevice;
            imageCapture.Start();
        }


        private void UpdateDeviceList()
        {
            try
            {
                comboBoxCaptureDevice.BeginUpdate();
                comboBoxCaptureDevice.Items.Clear();

                foreach (Device device in Device.FindDevices())
                    comboBoxCaptureDevice.Items.Add(device);
            }
            finally
            {
                comboBoxCaptureDevice.EndUpdate();
            }
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (imageCapture.Device == null)
                MessageBox.Show("No Webcam selected!", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Bitmap captureImage = imageCapture.Capture();

                if (saveFileDlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        captureImage.Save(saveFileDlg.FileName, ImageFormat.Jpeg);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error saving image: " + ex.ToString());
                        MessageBox.Show("Error saving image", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        
        
    }
}
