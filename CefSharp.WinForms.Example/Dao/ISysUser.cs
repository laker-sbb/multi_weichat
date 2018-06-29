using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp.WinForms.Example.Domain;

namespace CefSharp.WinForms.Example.Dao
{
    interface ISysUser
    {
        Result checkValid(SysUser user);
     
    }
}
