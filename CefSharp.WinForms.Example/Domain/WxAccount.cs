using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CefSharp.WinForms.Example.Domain
{
    class WxAccount
    {
        public String WxUin { get; set; }
        
        public int Status { get; set; }
        public String Sid { get; set; }
        public int Userid { get; set; }
        public string Token { get; set; }
        public string Timestamp { get; set; }
    }
}
