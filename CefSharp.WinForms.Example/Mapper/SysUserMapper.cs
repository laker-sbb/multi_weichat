using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CefSharp.WinForms.Example.Domain;
using log4net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;


namespace CefSharp.WinForms.Example.Mapper
{
    class SysUserMapper:Dao.ISysUser
    {
        private static ILog log = log4net.LogManager.GetLogger("logger");
        
        public  Result checkValid(SysUser user)
        {
          //return maxclientCount -1:fail
            Result result = null;
            HttpClient http = new HttpClient(new HttpClientHandler());
            
            try
            {
                String url = Uri.server_address + "index.php?m=Home&c=Login&a=check";

                //使用FormUrlEncodedContent做HttpContent
                //var content = new FormUrlEncodedContent(new Dictionary<string, string>()       
                //    {   
                //        {"username",user.username},
                //        {"password",user.password},
                //        {"orgid", user.orgid},
                //        {"Timestamp",user.Timestamp},
                //        {"Verifycode",user.Verifycode}
                //    });
                String jsondata = JsonHelper.SerializeObject(user);
                var content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                //await异步等待回应
                var response = http.PostAsync(url, content).Result;
                
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                String resdata = response.Content.ReadAsStringAsync().Result;
                JObject o = JObject.Parse(resdata);
                int code = (int)o.SelectToken("code");
                String msg = (String)o.SelectToken("message");
                 int maxClientCount = 0;
                
                if (code == 200)
                {
                    maxClientCount = (int)o.SelectToken("maxClientCount");
                    
                    user.Id = (int)o.SelectToken("id");

                    user.userSid = (String)o.SelectToken("userSid");
                    
                }
                
                result = new Result();
                result.Code = code;
                result.Message = msg;
                user.MaxClientCount = maxClientCount;
                
                result.Data = user;

            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                result = new Result();
                result.Code = 500;
                result.Message = "网络异常或请求超时";

            }
            finally
            {
                http.Dispose();
            }

            return result;

        }
        
    }
}
