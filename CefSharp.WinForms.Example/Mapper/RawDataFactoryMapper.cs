﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CefSharp.WinForms.Example.Domain;
using System.Collections;
using log4net;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Net.Http;


namespace CefSharp.WinForms.Example.Mapper
{
    class RawDataFactoryMapper:Dao.IRawDataFactory
    {
        ILog log = log4net.LogManager.GetLogger("logger");
        
        public Result insertData(RawDataFactory rawDataFactory)
        {
            Result result = new Result();
            HttpClient http = new HttpClient(new HttpClientHandler());
            http.DefaultRequestHeaders.ExpectContinue = false;
            try
            {
                String url = Uri.server_address+"index.php?m=Home&c=Rawdata&a=insertData";

                String jsondata = JsonHelper.SerializeObject(rawDataFactory);

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