﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADB_GUI_Tool
{
    public partial class MainToolWindow : Form
    {
        Process CmdLine = new Process();
        String ApkFile = "";
        public MainToolWindow()
        {
            InitializeComponent();
        }

        private void MainToolWindow_Load(object sender, EventArgs e)
        {
            //Start the command line on start up
            CmdLine.StartInfo.FileName = "cmd.exe";
            CmdLine.StartInfo.RedirectStandardInput = true;
            CmdLine.StartInfo.RedirectStandardOutput = true;
            CmdLine.StartInfo.CreateNoWindow = true;
            CmdLine.StartInfo.UseShellExecute = false;
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            String ConnectCmdOutput = "";

            if (IpAddrTextBox.Text != "")
            {
                if (ConnectButton.Text == "Connect")
                {
                    string AdbCmd = "adb connect " + IpAddrTextBox.Text;

                    CmdLine.Start();
                    CmdLine.StandardInput.WriteLine(AdbCmd.ToString());
                    CmdLine.StandardInput.Flush();
                    CmdLine.StandardInput.Close();

                    ConnectCmdOutput = CmdLine.StandardOutput.ReadToEnd();
                    CmdOutputRichTextBox.AppendText(ConnectCmdOutput.ToString());

                    if (ConnectCmdOutput.Contains("connected to") == true)
                    {
                        ConnectButton.Text = "Disconnect";
                        IpAddrTextBox.Enabled = false;
                        ApkBrowseButton.Enabled = true;
                        EnRootCheckBox.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Either not an IP Address or check output window!", "ADB GUI Tool - Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    CmdLine.Start();
                    CmdLine.StandardInput.WriteLine("adb disconnect");
                    CmdLine.StandardInput.Flush();
                    CmdLine.StandardInput.Close();

                    ConnectCmdOutput = CmdLine.StandardOutput.ReadToEnd();
                    CmdOutputRichTextBox.AppendText(ConnectCmdOutput.ToString());

                    if (ConnectCmdOutput.Contains("disconnected everything") == true)
                    {
                        ConnectButton.Text = "Connect";
                        IpAddrTextBox.Enabled = true;
                        ApkBrowseButton.Enabled = false;
                        InstallApkButton.Enabled = false;
                        EnRootCheckBox.Enabled = false;
                        EnRootCheckBox.Checked = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter an IP Address!", "ADB GUI Tool - Connection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void IpAddrTextBox_TextChanged(object sender, EventArgs e)
        {
            ConnectButton.Enabled = true;
        }

        private void ApkBrowseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ApkSelectDialog = new OpenFileDialog();
            ApkSelectDialog.Title = "Select an Android Application File";
            ApkSelectDialog.Filter = "Android Application Files (*.apk)|*.apk";

            if (ApkSelectDialog.ShowDialog() == DialogResult.OK)
            {
                ApkFile = ApkSelectDialog.FileName;
                SelectedApkLabel.Text = ApkFile.ToString();
                InstallApkButton.Enabled = true;
                InstallApkResetButton.Enabled = true;
                EnD_CheckBox.Enabled = true;
                EnR_CheckBox.Enabled = true;
            }
        }

        private void InstallApkButton_Click(object sender, EventArgs e)
        {
            String InstallApkCmdOutput = "";
            string InstallApkCmd = "";

            if (EnD_CheckBox.Checked == true)
            {
                InstallApkCmd = "adb install -d " + ApkFile.ToString();
            }
            else if (EnR_CheckBox.Checked == true)
            {
                InstallApkCmd = "adb install -r " + ApkFile.ToString();
            }
            else
            {
                InstallApkCmd = "adb install " + ApkFile.ToString();
            }
            
            CmdLine.Start();
            CmdLine.StandardInput.WriteLine(InstallApkCmd.ToString());
            CmdLine.StandardInput.Flush();
            CmdLine.StandardInput.Close();

            InstallApkCmdOutput = CmdLine.StandardOutput.ReadToEnd();
            CmdOutputRichTextBox.AppendText(InstallApkCmdOutput.ToString());

            if (InstallApkCmdOutput.Contains("Success") == true)
            {
                MessageBox.Show("Success to install APK onto the Android Device!", "ADB GUI Tool - APK Installation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to install APK onto the Android Device!", "ADB GUI Tool - APK Installation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ApkFile = null;
            SelectedApkLabel.Text = null;
            InstallApkButton.Enabled = false;
            EnD_CheckBox.Enabled = false;
            EnR_CheckBox.Enabled = false;
        }

        private void EnD_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EnD_CheckBox.Checked == true)
            {
                EnR_CheckBox.Checked = false;
                EnR_CheckBox.Enabled = false;
            }

            if (EnD_CheckBox.Checked == false)
            {
                EnR_CheckBox.Enabled = true;
            }
        }

        private void EnR_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EnR_CheckBox.Checked == true)
            {
                EnD_CheckBox.Checked = false;
                EnD_CheckBox.Enabled = false;
            }

            if (EnR_CheckBox.Checked == false)
            {
                EnD_CheckBox.Enabled = true;
            }
        }

        private void InstallApkResetButton_Click(object sender, EventArgs e)
        {
            SelectedApkLabel.Text = null;
            InstallApkButton.Enabled = false;
            InstallApkResetButton.Enabled = false;
            EnD_CheckBox.Enabled = false;
            EnR_CheckBox.Enabled = false;
        }

        private void EnRootCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EnRootCheckBox.Checked == true)
            {
                String AdbRootCmdOutput = "";

                EnRootCheckBox.Enabled = false;

                CmdLine.Start();
                CmdLine.StandardInput.WriteLine("adb root");
                CmdLine.StandardInput.Flush();
                CmdLine.StandardInput.Close();

                AdbRootCmdOutput = CmdLine.StandardOutput.ReadToEnd();
                CmdOutputRichTextBox.AppendText(AdbRootCmdOutput.ToString());
            }
        }
    }
}
