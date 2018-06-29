using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using CefSharp.WinForms.Example.Domain;
using log4net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace CefSharp.WinForms.Example.Mapper
{
    class WxAccountMapper:Dao.IWxAccount
    {
        
        private static ILog log = log4net.LogManager.GetLogger("logger");

        public  Result updateStatus(WxAccount account)
        {
            Result result = new Result();
            HttpClient http = new HttpClient(new HttpClientHandler());
            http.DefaultRequestHeaders.ExpectContinue = false;
            try
            {
                String url = Uri.server_address + "index.php?m=Home&c=Rawdata&a=updateWxAccountStatus";

                String jsondata = JsonHelper.SerializeObject(account);

                var content = new StringContent(jsondata, Encoding.UTF8, "application/json");

                //await异步等待回应
                var response = http.PostAsync(url, content).Result;
              
                //确保HTTP成功状态值
                
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON
                String resdata =  response.Content.ReadAsStringAsync().Result;
                
                    JObject o = JObject.Parse(resdata);
                    int code = (int)o.SelectToken("code");
                    String msg = (String)o.SelectToken("message");
                   
                    result.Code = code;
                    result.Message = msg;
                
                 
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                
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
