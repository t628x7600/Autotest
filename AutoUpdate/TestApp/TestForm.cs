﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using AutoUpdate;
using System.Net;
using System.IO;
using System.Diagnostics;
namespace TestApp
{
    public partial class TestForm : Form
    {
        private string PassArg ="";

        private string XmlUri = @"https://raw.githubusercontent.com/t628x7600/Autotest/master/project.xml";
        private string DownLoadInfo = @"info.xml";
        private string DownLoadPath = @"DownLoad_Information";

        private string DownLoadFormName = "DownLoadForm.exe";
        private string DownLoadFormPath = Environment.CurrentDirectory;
        AutoUpdate.AutoUpdate UpdateInterface;
        public TestForm()
        {
            InitializeComponent();
        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            Assembly currAsm = Assembly.GetExecutingAssembly();
            Uri uri = new Uri(XmlUri);
            UpdateInterface = new AutoUpdate.AutoUpdate(currAsm, this, uri);
            UpdateInterface.UpdateComplete += this.UpdateComplete;
            UpdateInterface.DoUpdate();
        }
        private  void UpdateComplete(bool update)
        {
            if (!update)
                return;
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, cert, chain, sslPolicyErrors) => true;//ignore ssl Certificate
            WebClient client = new WebClient();
            client.DownloadFileCompleted += DownLoadComplete;
            Uri uri = new Uri(XmlUri);
            string SavePath = GetDir(Environment.CurrentDirectory, DownLoadPath);
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
            SavePath = GetDir(SavePath, DownLoadInfo);
            PassArg = SavePath;
            try
            {
                client.DownloadFileAsync(uri,SavePath);
            }
            catch
            {
                MessageBox.Show("網路異常");
            
            }

        
        }

        private void DownLoadComplete(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string target = GetDir(DownLoadFormPath, DownLoadFormName);
                ProcessStartInfo pInfo = new ProcessStartInfo(target);
                pInfo.Arguments = PassArg;
                using (Process p = new Process())
                {
                    p.StartInfo = pInfo;
                    p.Start();
                }
                Application.Exit();
            }
            catch
            {
                MessageBox.Show("網路異常");

            }           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "當前版本號碼" + "\n     " + ProductVersion; 
        }

        private string GetDir(string path, string dir)
        {
            return path + @"\" + dir; 
        }
    }
}
