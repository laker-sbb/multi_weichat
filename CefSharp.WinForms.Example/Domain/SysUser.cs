using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CefSharp.WinForms.Example.Domain
{
    public class SysUser
    {
        public string username { get; set; }
        public string password { get; set; }
        public int Id { get; set; }
        public string orgid { get; set; }
        public int MaxClientCount { get; set; }
        
        public string timestamp { get; set; }
        public string verifycode { get; set; }
        public string userSid { get; set; }

    }
}
