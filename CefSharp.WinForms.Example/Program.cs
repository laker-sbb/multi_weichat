using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CefSharp.WinForms.Example.Dao;
using log4net;
using MySql.Data.MySqlClient;

namespace CefSharp.WinForms.Example
{
    class Program
    {
        public static Mutex runOnce = null;
        [STAThread]
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            //Mutex runOnce = null;

            //if (Properties.Settings.Default.IsRestarting == true)
            //{
            //    Properties.Settings.Default.IsRestarting = false;

            //    Properties.Settings.Default.Save();
            //}

            try
            {
                bool flag = false;
                runOnce = new Mutex(true, "SINGLE_INSTANCE",out flag);
                if (flag)
                {
                    //Application.EnableVisualStyles();
                    runOnce.ReleaseMutex();
                    RequestHandler.Init();

                    Application.Run(new ChatForm());
                    CEF.Shutdown();
                }
                else
                {
                    MessageBox.Show("程序已经运行!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }
            finally
            {
                if (runOnce != null)
                {
                    runOnce.Close();
                      
                }
                
            }
            
            //Process[] processes = Process.GetProcessesByName("daishu_message");
            //if (processes.Length >= 2)
            //{
            //    MessageBox.Show("程序已经运行!", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            //else
            //{

                //LoginForm loginForm = new LoginForm();

                //loginForm.ShowDialog();

                //if (loginForm.DialogResult != DialogResult.Cancel)
                //{

                
                    //
                    
                //}
            //}
        }

        
    }
}
