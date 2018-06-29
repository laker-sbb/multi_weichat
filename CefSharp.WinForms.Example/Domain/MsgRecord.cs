using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CefSharp.WinForms.Example.Domain
{
    public class MsgRecord
    {
        //private String wxUid;
        //private String msgId;
        //private String fromUser;
        //private String toUser;
        //private decimal msgType;
        //private String content;
        //private decimal createTime;

        public String WxUin { get; set; }
        public String MsgId { get; set; }
        public String FromUser { get; set; }
        public String ToUser { get; set; }
        public decimal MsgType { get; set; }
        public String Content { get; set; }
        public decimal CreateTime { get; set; }
        public decimal Finished { get; set; }
        


    }
}