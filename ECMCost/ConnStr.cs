using System;
using System.Collections.Generic;
using System.Text;
using FirebirdSql.Data.Firebird;
using System.Configuration;
using System.Windows.Forms;
using System.IO;



namespace ECMCost
{
    class ConnStr
    {
        public string connStr;
        public ConnStr()
        {
            FbConnectionStringBuilder cs = new FbConnectionStringBuilder();

            cs.DataSource = "localhost";
            cs.UserID = "SYSDBA";
            cs.Password = "masterkey";
                    
             // create reader & open file
            string path = Application.StartupPath+@"\connectstring.txt";
            char[] charsToTrim = { '\'' };
            // Open the file to read from.
            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);

                foreach (string s in readText)
                {
                    cs.Database = s.Trim(charsToTrim);
                }
            }
            else
            {  
                cs.Database = @"w2k3:D:\_Recept_Base\Agro\cbd_user.gdb";
            }            

            cs.Port = 3050;
            cs.Charset = "WIN1251";
            cs.Pooling = true;
            cs.ConnectionLifeTime = 30;
            // To use the embedded server set the ServerType to 1
            cs.ConnectionLifeTime = 15;
            cs.Dialect = 3;
            cs.Role = "";
            cs.Pooling = true;
            cs.MinPoolSize = 0;
            cs.MaxPoolSize = 50;
            cs.PacketSize = 8192;
            connStr = cs.ToString();
        }
    }
}
//sql connect
//ora01-gala-01-h
//sa
//sa