using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace CefSharp.WinForms.Example
{
    class connectTool
    {
        public static SQLiteConnection connect()
        {
            if (File.Exists("msg.db"))
            {
                string connectString = @"Data Source=msg.db;Pooling=true;FailIfMissing=false";
                SQLiteConnection conn = new SQLiteConnection(connectString);
                try
                {
                    
                    return conn;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    return null;
                }
                   
            }
            else
            {
                Console.WriteLine("database file is removed,please recover it");

                return null;
            }
        }

        public static MySqlConnection connectMysql(String constr)
        {
            MySqlConnection conn = new MySqlConnection(constr);
            return conn;
        }
       
    }
}
