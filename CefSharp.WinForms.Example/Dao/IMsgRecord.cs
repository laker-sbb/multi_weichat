using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp.WinForms.Example.Domain;

namespace CefSharp.WinForms.Example.Dao
{
    interface IMsgRecord
    {
        HashSet<string> getContractList(WxAccount account);
        List<ChatRecord> getChatRecord(MsgRecordCondition condition);
        
    }
}
