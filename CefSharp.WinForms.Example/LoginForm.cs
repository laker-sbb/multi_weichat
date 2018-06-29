using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CefSharp.WinForms.Example.Dao;
using CefSharp.WinForms.Example.Domain;
using CefSharp.WinForms.Example.Mapper;
using CefSharp.WinForms.Example.Properties;
using log4net;


namespace CefSharp.WinForms.Example
{
    public partial class LoginForm : Form
    {
        private static ILog log = log4net.LogManager.GetLogger("logger");
        private delegate void login_callback(Result result);
        private delegate void loading_callback(Stream stream);
        public SysUser user = new SysUser();
        private LoadingForm loadForm;
        
        public LoginForm()
        {
            InitializeComponent();
            init();
 
        }

        private void init()
        {
         
            if (File.Exists("config.txt"))
            {
                StreamReader sr = new StreamReader("config.txt");
                String line = sr.ReadLine();
                sr.Close();
                String[] lines = line.Split(':');
                if (lines.Length == 2)
                {
                    orgId.Text = line.Split(':')[0];
                    username.Text = line.Split(':')[1];
                }
                
            }
            
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            //get orgid from config
            
            String login_user = this.username.Text;
            String login_pass = GetMd5(32,this.password.Text);
            
            //SysUser user = new SysUser();
            user.orgid = orgId.Text;
            user.username = login_user;
            user.password = login_pass;
            user.verifycode = verifycode.Text;
            
            //valid check
            
            if (!IsAlphanumeric(user.username) || !IsAlphanumeric(user.orgid) || !IsAlphanumeric(user.password))
            {
                //MessageBox.Show("组织编号错误!", "失败提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loginTip.Text = "组织编号错误!";
                return;
            }
            if (username.Text == "" || password.Text == ""||verifycode.Text=="")
            {
                loginTip.Text = "请填写登录信息!";
                return;
            }

           
            //async loading
            new Thread(new ParameterizedThreadStart(loginSystem)).Start(user);

            loadForm = new LoadingForm();

            orgId.BackColor = Color.LightGray;

            username.BackColor = Color.LightGray;

            password.BackColor = Color.LightGray;

            verifycode.BackColor = Color.LightGray;
            panel3.Enabled = false;

            loadForm.ShowDialog();
            
            
        }

        public static bool IsAlphanumeric(string source)
        {
            
            Regex pattern = new Regex("^[0-9a-zA-Z]+$");
            return pattern.IsMatch(source);
        }

        public static string GetMd5(int code, string str)
        {
            string strmd5 = "";
            byte[] result = Encoding.Default.GetBytes(str);    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string md5str = BitConverter.ToString(output).Replace("-", "").ToLower();

            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                strmd5 = md5str.Substring(8, 16);
            }
            if (code == 32) //32位加密 
            {
                strmd5 = md5str;
            }
            return strmd5;
        }

        private void GenerateVerifyCode()
        {
            
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long stamp = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;  //注意这里有时区问题，用now就要减掉8个小时
            user.timestamp = stamp.ToString();
            Stream stream = null;
            HttpWebRequest req;
            try
            {
                req = HttpWebRequest.Create(Uri.server_address + "index.php?m=Home&c=Login&a=getVerifyCode&Timestamp=" + stamp) as HttpWebRequest;
                req.Timeout = 30000;
                stream = req.GetResponse().GetResponseStream();
                //stream = WebRequest.Create(Uri.server_address + "index.php?m=Home&c=Login&a=getVerifyCode&Timestamp=" + stamp).GetResponse().GetResponseStream();

            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            finally
            {
                this.Invoke(new loading_callback(showImg), new Object[] { stream });
            }

            
           
        }

        private void refreshVerifyCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            refreshVerifyCode.Enabled = false;
            this.pictureBox1.Image = Resources.mini_loading;
            new Thread(new ThreadStart(GenerateVerifyCode)).Start();
        }

        private void loginSystem(Object obj)
        {

            ISysUser sysUser = new SysUserMapper();

            Result result = sysUser.checkValid((SysUser)obj);

            this.Invoke(new login_callback(callback), new Object[] { result});
              
        }

        private void callback(Result result)
        {
            try
            {

                if (result.Code == 200)
                {

                    FileStream fs = File.Open("config.txt", FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine(orgId.Text + ":" + user.username);
                    sw.Close();
                    fs.Close();

                    this.user = (SysUser)result.Data;
                    this.loadForm.Close();
                    this.loadForm.Dispose();
                    this.DialogResult = DialogResult.OK;
                    //this.Close();
                }
                else
                {
                    //MessageBox.Show(result.Message,"失败提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    loginTip.Text = result.Message;
                    this.panel3.Enabled = true;
                    orgId.BackColor = Color.White;

                    username.BackColor = Color.White;

                    password.BackColor = Color.White;

                    verifycode.BackColor = Color.White;
                    verifycode.ResetText();

                    new Thread(new ThreadStart(GenerateVerifyCode)).Start();
                    this.loadForm.Close();
                    this.loadForm.Dispose();

                }

            }
            catch (Exception ex)
            {
                log.Debug("file read error!");

                loginTip.Text = "用户名或密码不正确!";
                verifycode.ResetText();
                //MessageBox.Show("用户名或密码不正确!", "失败提示", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                //new Thread(new ThreadStart(GenerateVerifyCode)).Start();
                if (loadForm != null)
                {
                    this.loadForm.Close();
                    this.loadForm.Dispose();
                }
                
            }
        }

        private void showImg(Stream stream)
        {
            if (stream != null)
            {
                Image O_Image = Image.FromStream(stream);
                pictureBox1.Image = O_Image;

            }
            else
            {
                pictureBox1.Image = pictureBox1.ErrorImage;
            }

            refreshVerifyCode.Enabled = true;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            refreshVerifyCode.Enabled = false;
            new Thread(new ThreadStart(GenerateVerifyCode)).Start();
            
           
        }
    }
}
