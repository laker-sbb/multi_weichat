using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Net;
using CefSharp.WinForms.Example.Properties;
using MySql.Data.MySqlClient;
using CefSharp.WinForms.Example.Domain;
using System.Diagnostics;
using System.Threading;



namespace CefSharp.WinForms.Example
{
    

    public partial class ChatForm : Form
    {
        //private String username = null;
        
       // public int userid;
        public SysUser user;
        public SessionStatus sessionStatus;
        public int maxClientCount;
        public Color BorderColor = Color.Red;
        public int BorderWidth = 1;
        public ButtonBorderStyle BorderLineStyle =  ButtonBorderStyle.Solid;
        public String conStr;

       
        private delegate void changeBtnTxtEvent(Object obj, String txt, Image bmp);

        private delegate void sessionExpireEvent(String message);

        private Hashtable wxTable = new Hashtable();
        private ArrayList wxKeys = new ArrayList(); //store key
        
        //private WebView CurrentWebView;

        private UIobj currentUIobj;

        private BrowserSettings settings = new BrowserSettings();

        //public ChatForm(SysUser user)
        public ChatForm()
        {
            
            //this.user = user;
            
            //this.maxClientCount = user.MaxClientCount;

            InitializeComponent();
            //this.Text = "Hi," + user.username + "! 欢迎进入袋鼠信息系统";
            //userinfo.Text = "当前登录用户:" + user.username + " 终端最大上限:" + maxClientCount;
            WebView web_view = new WebView("about:blank", settings);

            web_view.Dock = DockStyle.Fill;
            this.toolStripContainer.ContentPanel.Controls.Add(web_view);
            this.conStr = web_view.GetStr();
            UIobj uiobj = new UIobj();
            uiobj.View = web_view;
            //CurrentWebView = web_view;
            currentUIobj = uiobj;
            new RequestHandler(uiobj, web_view, this);
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (wxTable.Count >= maxClientCount)
            {
                MessageBox.Show("已达到授权终端数最大限制!", "警告提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Object obj = addTab();

                createNewBrowser(obj);
            }
        }

        private Object addTab()
        {
            // 
            // new button
            // 
            int scrollBarValue = this.splitContainer1.Panel1.HorizontalScroll.Value;
            int x = wxTable.Count * 115 + 3 - scrollBarValue;
            //Button newTab = new Button();
            Panel newTab = new Panel();
            newTab.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            
            newTab.Location = new System.Drawing.Point(x, 12);
            newTab.Name = "newBtn";
            newTab.Size = new System.Drawing.Size(100, 100);
            

            newTab.BackgroundImage = Resources.login;
            newTab.BackgroundImageLayout = ImageLayout.Center;
            //newTab.UseVisualStyleBackColor = false;
            //this.OperatePanel1.Controls.Add(newTab);
            this.splitContainer1.Panel1.Controls.Add(newTab);

            newTab.Click += new System.EventHandler(this.newTab_Click);



            //newTab.MouseEnter += new System.EventHandler(this.newTab_MouseEnter);


            return newTab;
        }

       
        private void createNewBrowser(Object obj)
        {
            WebView view = new WebView("https://wx.qq.com", settings);

            view.Dock = DockStyle.Fill;
            // 
            // label1
            //
            int scrollBarValue = this.splitContainer1.Panel1.HorizontalScroll.Value;
            int x = wxTable.Count * 115 + 3 - scrollBarValue;
            
            Label newlabel = new Label();
            newlabel.BackColor = Color.Transparent;
            newlabel.Location = new System.Drawing.Point(x, 114);
            newlabel.Name = "label1";
            newlabel.Size = new System.Drawing.Size(100, 2);
            newlabel.TabIndex = 5;
            newlabel.Text = "label1";            
            //newlabel.Visible = false;
            this.splitContainer1.Panel1.Controls.Add(newlabel);
            //((Panel)obj).Controls.Add(newlabel);
            

            PictureBox picbox = new System.Windows.Forms.PictureBox();
            picbox.BackColor = System.Drawing.Color.Transparent;
            picbox.Image = Resources.close_default;
            picbox.Location = new System.Drawing.Point(71, -3);
            picbox.Name = "pictureBox1";
            picbox.Size = new System.Drawing.Size(32, 32);
            picbox.TabIndex = 5;
            picbox.TabStop = false;
            picbox.Visible = false;

            picbox.MouseEnter += new System.EventHandler(this.picbox_MouseEnter);
            picbox.MouseLeave += new System.EventHandler(this.picbox_MouseLeave);
            picbox.Click += new System.EventHandler(this.picbox_Click);
            ((Panel)obj).Controls.Add(picbox);

            UIobj uiobj = new UIobj();
            uiobj.View = view;
            uiobj.Labelborder = newlabel;
            uiobj.CloseBtn = picbox;

            wxTable.Add(obj, uiobj);
            wxKeys.Add(obj);
            

            new RequestHandler(obj,view,this);
        }

        private void newTab_Click(object sender, EventArgs e)
        {
            //int id = sender.GetHashCode();
           

            UIobj uiobj = (UIobj)wxTable[sender];

            //remove old view
            this.toolStripContainer.ContentPanel.Controls.Remove(currentUIobj.View);

            //set invisiable
            if (currentUIobj.Labelborder != null)
            {
                //currentUIobj.Labelborder.Visible = false;
                currentUIobj.Labelborder.BackColor = Color.Transparent;
                currentUIobj.CloseBtn.Visible = false;
            }

            //add new View
            this.toolStripContainer.ContentPanel.Controls.Add(uiobj.View);

            //CurrentWebView = uiobj.View;
            currentUIobj = uiobj;

            //uiobj.Labelborder.Visible = true;
            uiobj.Labelborder.BackColor = Color.ForestGreen;
            uiobj.CloseBtn.Visible = true;


            //Console.WriteLine("btn clicked:" + sender.GetHashCode());

        }

        private void picbox_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = Resources.close;
        }

        private void picbox_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).Image = Resources.close_default;
        }

        private void picbox_Click(object sender, EventArgs e)
        {
            int visible = this.splitContainer1.Panel1.HorizontalScroll.Value;
            
            currentUIobj.View.Stop();
            //currentUIobj.View.Load("about:blank");
            currentUIobj.View.CloseBrowser();
            //((RequestHandler)currentUIobj.View.RequestHandler).UpdateAccountStatus((int)AccountStatus.Logout);
            this.toolStripContainer.ContentPanel.Controls.Remove(currentUIobj.View);
            this.splitContainer1.Panel1.Controls.Remove(currentUIobj.Labelborder);

            object delKey = ((PictureBox)sender).Parent;

            bool special = false;
            if (wxKeys[wxKeys.Count - 1] == delKey)
            {
                special = true;
            }

            int delIdx = -1;
            for (int i = 0; i < wxKeys.Count; i++)
            {
                if (special == true)
                {
                    int index = (wxKeys.Count == 1 ? 0 : wxKeys.Count - 2);
                    newTab_Click(wxKeys[index], new EventArgs());
                    break;
                }
                if (wxKeys[i] == delKey && i > 0)
                {
                    newTab_Click(wxKeys[i - 1], new EventArgs());
                    delIdx = i;
                    continue;
                }
                else if (wxKeys[i] == delKey)
                {
                    newTab_Click(wxKeys[1], new EventArgs());
                    delIdx = i;
                    continue;
                }
                if (delIdx != -1)
                {
                    Panel panel = (Panel)wxKeys[i];
                    int newX = panel.Location.X - 115;
                    //tab shift
                    panel.Location = new System.Drawing.Point(newX, 12);
                    //label shift
                    ((UIobj)wxTable[wxKeys[i]]).Labelborder.Location = new System.Drawing.Point(newX, 114);
                    
                }

            }

            this.splitContainer1.Panel1.Refresh();
            wxTable.Remove(delKey);
            wxKeys.Remove(delKey);

            this.splitContainer1.Panel1.Controls.Remove((Panel)delKey);

            ((Panel)delKey).Dispose();

            



        }
        private void refreshBtn_Click(object sender, EventArgs e)
        {
            currentUIobj.View.Stop();
            currentUIobj.View.Reload();
              
        }

        

        public object EvaluateScript(WebView view,string script)
        {
            return view.EvaluateScript(script);
        }

        private void changeBtnTxt(Object obj,String txt,Image bmp)
        {
            
            ((Panel)obj).BackgroundImage = bmp;
            
        }

        public void Calldelegate(Object obj,String txt,Image bmp)
        {
            this.BeginInvoke(new changeBtnTxtEvent(changeBtnTxt), new Object[] { obj, txt,bmp });
        }

        

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (currentUIobj != null)
            {
                //((RequestHandler)currentUIobj.View.RequestHandler).UpdateAccountStatus((int)AccountStatus.Logout);

                currentUIobj.View.CloseBrowser();
            }

            if (this.sessionStatus == SessionStatus.expire)
            {
                Program.runOnce.Close();
                Application.ExitThread();
                Thread thtmp = new Thread(new ParameterizedThreadStart(run));
                object appName = Application.ExecutablePath;
                Thread.Sleep(1);
                thtmp.Start(appName);
 
            }
              
        }

        private void run(Object obj)
        {
            Process ps = new Process();
            ps.StartInfo.FileName = obj.ToString();
            ps.Start();
            
            
        }
        private void ViewPanel_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(BorderColor);

            e.Graphics.DrawLine(pen, 0, 0, this.Width - 1, 0);
            e.Graphics.DrawLine(pen, 0, this.Height - 1, this.Width - 1, this.Height - 1);
        }

        private void lookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new ChatRecordForm().Show();
        }

        private void addBtn_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackgroundImage = Resources.add;
            //((PictureBox)sender).BackColor = Color.FromArgb(249, 249, 249);
            ((PictureBox)sender).BackColor = Color.Transparent;
        }

        

        private void refreshBtn_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackgroundImage = Resources.refresh;
            //((PictureBox)sender).BackColor = Color.FromArgb(249, 249, 249);
            ((PictureBox)sender).BackColor = Color.Transparent;
        }

        private void addBtn_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackgroundImage = Resources.add_hover;
            ((PictureBox)sender).BackColor = Color.White;
        }

        private void refreshBtn_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackgroundImage = Resources.refresh_hover;
            ((PictureBox)sender).BackColor = Color.White;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //session expire popup,avoid login in different place
        private void showExpireBox(String message)
        {
            this.sessionStatus = SessionStatus.expire;
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.Close();
           
        }

        public void callback_showExpireBox(String message)
        {
            this.BeginInvoke(new sessionExpireEvent(showExpireBox), new Object[] { message });
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                this.user = loginForm.user;
                this.maxClientCount = user.MaxClientCount;
                userinfo.Text = "当前登录用户:" + user.username + " 终端最大上限:" + maxClientCount;
            }
            else
            {
                this.Close();
            }
        }

        private void ChatForm_Shown(object sender, EventArgs e)
        {
            this.Refresh();
        }

    }
}
