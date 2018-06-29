using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.SQLite;
using System.Data;
using CefSharp.WinForms.Example.Dao;
using CefSharp.WinForms.Example.Mapper;
using CefSharp.WinForms.Example.Domain;
using Newtonsoft.Json.Linq;
using log4net;
using CefSharp.WinForms.Example.Properties;


namespace CefSharp.WinForms.Example
{
   
    class RequestHandler:IRequestHandler
    {
        private readonly IWebBrowser model;

        private readonly ChatForm chatForm;

        private Object obj;

        private String picStream;

        private Boolean isLogin = false;

        private String wxUin;

        private String sessionId;

        private String requestBody;

        
        ILog logger = log4net.LogManager.GetLogger("logger");

        public static void Init()
        {
            Settings settings = new Settings();
            
            settings.Locale = "zh-CN";
            //settings.LogFile = profilePath + "log";
           
            if (CEF.Initialize(settings))
            {
                //CEF.RegisterScheme("test", new SchemeHandlerFactory());
                //CEF.RegisterJsObject("bound", new BoundObject());
            }
        }
       

        public RequestHandler(Object obj,IWebBrowser model,ChatForm chatform)
        {
            this.obj = obj;

            this.model = model;

            this.chatForm = chatform;

            
            model.RequestHandler = this;

            model.LoadCompleted+=model_LoadCompleted;


            

        }
        #region IRequestHandler Members
        

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browser, IRequest request, NavigationType naigationvType, bool isRedirect)
        {
            return false;
            
        }

        bool IRequestHandler.OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {

            IRequest request = requestResponse.Request;
            IDictionary<string, string> headers = requestResponse.Request.GetHeaders();
            headers.Add("Accept-Language", "zh-cn");
            requestResponse.Request.SetHeaders(headers);
            
            //Console.WriteLine(request.Url);

            if (request.Url.Contains("cgi-bin/mmwebwx-bin/login?loginicon=true&uuid="))
            {
                if (request.Url.Contains("tip") && isLogin == false)
                {
                    //Console.WriteLine("========"+request.Url);
                    var result = chatForm.EvaluateScript((WebView)model, "(function() { if( document.getElementsByClassName(\"avatar show\")[0]!=null){var stream = document.getElementsByClassName(\"avatar show\")[0].getElementsByTagName(\"img\")[0].src; return stream;}else {return null;} })();");
                    if (result != null)
                    {
                        this.picStream = ((string)result).Substring(20);
                        isLogin = true;

                        new Thread(new ParameterizedThreadStart(SetBtnbg)).Start(obj);
                    }

                }
                else
                {
                    //logout
                   
                    this.chatForm.Calldelegate(obj, "Login", Resources.login);
                    isLogin = false;

                    //UpdateAccountStatus((int)AccountStatus.Logout);
   
                }
            }

            if (request.Url.Contains("cgi-bin/mmwebwx-bin/webwxinit"))
            {
                //{"BaseRequest":{"Uin":"1026167300","Sid":"lMUI0TrrWYf5hvrK","Skey":"@crypt_15d8816a_9d441d2d5535163041de2990174f2555","DeviceID":"e155535843921825"}}
                
                JObject o = JObject.Parse(request.Body);
                wxUin = (String)o.SelectToken("BaseRequest.Uin");
                String newsessionId = (String)o.SelectToken("BaseRequest.Sid");
                if (newsessionId != this.sessionId)
                {
                    this.sessionId = newsessionId;
                    new Thread(new ThreadStart(UpdateAccountStatus)).Start();
                    //UpdateAccountStatus((int)AccountStatus.Login);
                }

            }
            if (request.Url.Contains("cgi-bin/mmwebwx-bin/webwxsendmsg"))
            {
                
                requestBody = request.Body;
                //RawDataFactory rawDataFactory = new RawDataFactory();
                //rawDataFactory.Content = request.Body;
                //rawDataFactory.Finished = 0;
                //rawDataFactory.HttpType = 2;
                //rawDataFactory.WxUin = wxUin;
                //rawDataFactory.Sid = sessionId;

                //new Thread(new ParameterizedThreadStart(insertResponse)).Start(rawDataFactory);
                     
            }
            
            return false;
        }

        void IRequestHandler.OnResourceResponse(IWebBrowser browser, string url, int status, string statusText, string mimeType, WebHeaderCollection headers)
        {
            if (url.Contains("cgi-bin/mmwebwx-bin/webwxsendmsg"))
            {
                if (status == 200)
                {
                    
                    RawDataFactory rawDataFactory = new RawDataFactory();
                    rawDataFactory.Content = requestBody;
                    //rawDataFactory.Finished = 0;
                    rawDataFactory.HttpType = 2;
                    
                    rawDataFactory.Sid = sessionId;

                    rawDataFactory.WxUin = wxUin;
                    rawDataFactory.Uid = this.chatForm.user.Id;
                    

                    new Thread(new ParameterizedThreadStart(insertResponse)).Start(rawDataFactory);
                     
                }
            }
            
        }

        bool IRequestHandler.GetDownloadHandler(IWebBrowser browser, string mimeType, string fileName, long contentLength, ref IDownloadHandler handler)
        {
            
            return false;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password)
        {
            return false;
        }

        void IRequestHandler.GetResponseContent(string content)
        {
            //Console.WriteLine(content);
            
            RawDataFactory rawDataFactory = new RawDataFactory();
           
            rawDataFactory.Content = content;
            
            
            rawDataFactory.HttpType = 1;
            
            rawDataFactory.Sid = sessionId;

           
            rawDataFactory.WxUin = wxUin;
            rawDataFactory.Uid = this.chatForm.user.Id;
            

            new Thread(new ParameterizedThreadStart(insertResponse)).Start(rawDataFactory);

        }

        #endregion
        void model_LoadCompleted(Object sender, LoadCompletedEventArgs e)
        {
          
        }


        private void SetBtnbg(Object obj)
        {
           
            if (isLogin == true)
            {
                //convert base64 to img
                try
                {
                    byte[] arr = Convert.FromBase64String(picStream);
                    MemoryStream ms = new MemoryStream(arr);
                    Bitmap sbmp = new Bitmap(ms);
                    Bitmap dbmp = new Bitmap(sbmp, 100, 100);
                    //String filepath = profilePath + obj.GetHashCode() + ".bmp";
                    //if (File.Exists(filepath))
                    //{
                    //    Image.FromFile(filepath).Dispose();

                    //    File.Delete(filepath);
                    //}

                    //dbmp.Save(filepath, System.Drawing.Imaging.ImageFormat.Bmp);

                    ms.Close();

                    this.chatForm.Calldelegate(obj, "", dbmp);

                    picStream = null;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Base64StringToImage 转换失败\nException：" + ex.Message);
                }
            }
                   
        }

        public void UpdateAccountStatus()
        {
           
            if (wxUin != null)
            {
                
                WxAccount account = new WxAccount();
                account.WxUin = wxUin;

                //account.Status = status; //logout:1 login:0
                account.Sid = sessionId;
                account.Userid = this.chatForm.user.Id;
                String userSid = this.chatForm.user.userSid;
                DateTime timeStamp=new DateTime(1970,1,1);  //得到1970年的时间戳
                long a=(DateTime.UtcNow.Ticks-timeStamp.Ticks)/10000000;  //注意这里有时区问题，用now就要减掉8个小时
                account.Timestamp = a.ToString();
                String token = account.Timestamp+this.chatForm.conStr+account.Userid+userSid;
                account.Token = LoginForm.GetMd5(32, token);

                IWxAccount wxAccount = new WxAccountMapper();
                Result result = wxAccount.updateStatus(account);
                if (result.Code != 200 && this.chatForm.sessionStatus!=SessionStatus.expire)
                {
                    
                    this.chatForm.callback_showExpireBox(result.Message);
                }
                
                
            }
            
        }

        public void insertResponse(Object data)
        {
            if (this.chatForm.sessionStatus == SessionStatus.expire)
            {
                return;
            }
            RawDataFactory rawDataFactory = (RawDataFactory)data;

            //add timestamp,token
            DateTime timeStamp = new DateTime(1970, 1, 1);  //得到1970年的时间戳
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;  //注意这里有时区问题，用now就要减掉8个小时
            rawDataFactory.Timestamp = a.ToString();
            String userSid = this.chatForm.user.userSid;
            String token = rawDataFactory.Timestamp + this.chatForm.conStr + rawDataFactory.Uid+userSid;
            rawDataFactory.Token = LoginForm.GetMd5(32, token);

            IRawDataFactory rawDataFactoryMapper = new RawDataFactoryMapper();
            Result result = rawDataFactoryMapper.insertData(rawDataFactory);
            if (result.Code != 200 && this.chatForm.sessionStatus!=SessionStatus.expire)
            {

                this.chatForm.callback_showExpireBox(result.Message);
            }
        }
    }
}
