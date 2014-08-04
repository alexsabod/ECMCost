using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.Firebird;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;

namespace ECMCost
{
    public partial class Form1 : Form
    {
        public int ID;
        ConnStr connstr = new ConnStr();
        public Form1()
        {
           
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {      
            if (find_java())
            {          
                serch_BD_ather_item();
          //      check_type();               //определяем Премикс или БВМД  
            }

        }
        void serch_BD_ather_item()
        {
            string externalID, priceEU = "0", price = "0", dateconvert = "0";
            string sqlquary;
            char[] arr;
            string sCxn;
            //sql value
            SqlConnection sqlConnection;
            string sqlconnectionString = GetConnectionString(); // connection string SQL
            FbDataReader rd;
            SqlCommand sqlCommand;
            SqlDataReader sqlrd, sqlrd1;
            //firebird value
            FbConnection connection = new FbConnection(connstr.connStr);
            FbCommand command = new FbCommand();
          //ищем элементы
            try
            {
                connection.Open();
                FbTransaction transaction = connection.BeginTransaction();
                // поиск всех элементов за исключением витаминных канцентратов , т.е. гр 29
                command = new FbCommand("SELECT RAW_DICT.RAW_SHORT_NAME, RAW_DICT.EXTERNAL_ID, RCPREP.RAW_PERCENT, RAW_DICT.PRIMARY_QLT_ID " +
                    " FROM RCPREP, RAW_DICT " +
                    " WHERE RCPREP.RCP_ID= " + ID + " AND RCPREP.RAW_ID = RAW_DICT.RAW_ID " +
                    // исключаем Витаминные концентраты
                     "and RCPREP.RAW_ID <>-432390 AND " + //КС-4 (1.5%)
                     "RCPREP.RAW_ID <>-432162 AND " + //КС-1 А
                     "RCPREP.RAW_ID <>-432398 AND " + //КС-3(3 %)
                     "RCPREP.RAW_ID <>-432161 AND " + //КС-3 А
                     "RCPREP.RAW_ID <>-432160 AND " + //КС-4 А
                     "RCPREP.RAW_ID <>-432396 AND " + //П1-1Кобб(2,5%)
                     "RCPREP.RAW_ID <>-432397 AND " + //П1-2(0,8%)                        
                     "RCPREP.RAW_ID <>-432596 AND " + //П6-1(15%)
                     "RCPREP.RAW_ID <>-432597 AND " + //П6-1(15%)ФИН-2
                     "RCPREP.RAW_ID <>-432393 AND " + //П6-1(2,5%)
                     "RCPREP.RAW_ID <>-432493 AND " + //П60-1(2,5%)
                     "RCPREP.RAW_ID <>-432394 AND " + //П60-3(2,5%)
                     "RCPREP.RAW_ID <>-432494 AND " + //П60-3(2,5%)
                     "RCPREP.RAW_ID <>-432389 AND " + //ПКР-2(1%)
                     "RCPREP.RAW_ID <>-432392 AND " + //П5-1(3,5%)
                     "RCPREP.RAW_ID <>-432712 AND " + //П5-1(4,5%)
                     "RCPREP.RAW_ID <>-432703 AND " + //КС-4(2%)
                     "RCPREP.RAW_ID <>-432714 AND " + //П1-1Кобб(3%)Troun Nu
                     "RCPREP.RAW_ID <>-432758 AND " + //П5-1(15%)МИАВИТ
                     "RCPREP.RAW_ID <> -432918 AND " +
                     "RCPREP.RAW_ID <> -433035 AND " +
                     "RCPREP.RAW_ID <> -432977 AND " +
                     "RCPREP.RAW_ID <> -432811 AND " +
                     "RCPREP.RAW_ID <> -432812 AND " +
                     "RCPREP.RAW_ID <> -432969 AND " +
                     "RCPREP.RAW_ID <> -432805 AND " +
                     "RCPREP.RAW_ID <> -432819 AND " +
                     "RCPREP.RAW_ID <> -433209 AND " +
                     "RCPREP.RAW_ID <> -433074 AND " +
                     "RCPREP.RAW_ID <> -433036 " +
                    " AND RCPREP.RAW_PERCENT <> 0  " +
                "", connection);

                command.Transaction = transaction;
                rd = command.ExecuteReader();

                dataGridView1.Rows.Clear();

                while (rd.Read())//условия выборки элементов
                {
                    // берем певых 5 цифр

                    arr = rd[1].ToString().ToCharArray(0, 5);  
                    externalID = new string(arr);

                    //ищем стоимость элементов




                    sqlquary =
                    "    select top 1 T$katmc.F$name, T$spsopr.F$price,  T$spsopr.F$dsopr " +
"from T$katmc, T$spsopr  " +
"where T$katmc.F$barkod2 = '" + externalID + "'  " +
"         and T$spsopr.F$cmcusl = T$katmc.F$nrec   " +
"       and T$spsopr.F$price > 0  " +
"order by T$spsopr.F$dsopr desc ";


                        
                        
                        /*"select top 1 T$katmc.F$name, T$spsopr.F$price, max( T$spsopr.F$dsopr) " +
                                       " from T$katmc, T$spsopr  " +
                                       " where T$katmc.F$barkod2 = '" + externalID + "'  " +
                                       "         and T$spsopr.F$cmcusl = T$katmc.F$nrec   " +
                                       "       and T$spsopr.F$price > 0  " +
                                       "     group by T$katmc.F$name, T$spsopr.F$price " +
                                       " order by T$spsopr.F$price desc";

                        */
                       
                    sCxn = "Data Source=ORA01-GALA-01-H;Initial Catalog=test3;Persist Security Info=True;User ID=sa;Password=sa";
                    sqlConnection = new SqlConnection(sCxn);

                    try
                    {
                        sqlCommand = new SqlCommand(sqlquary, sqlConnection);
                        sqlConnection.Open();
                        sqlrd = sqlCommand.ExecuteReader();

                        while (sqlrd.Read())
                        {
                            price = sqlrd[1].ToString();
                            dateconvert = sqlrd[2].ToString();
                        }
                        sqlrd.Close();
                    }
                    catch (Exception et)
                    {
                        MessageBox.Show(et.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();                      
                    }

                    //поиск курса евро

                    sqlquary =      " select t$CURSVAL.F$sumrubl " +
                                    "  from t$CURSVAL,t$klval " +
                                    "  where " +
                                    "   t$CURSVAL.F$kodvalut = t$klval.f$nrec  " +
                                    "  and t$klval.f$SIMVOLV = 'eur' " +
                                    "  and t$CURSVAL.f$datval = '" + dateconvert + "'";     

                    try
                    {
                        sqlCommand = new SqlCommand(sqlquary, sqlConnection);
                        sqlConnection.Open();
                        sqlrd = sqlCommand.ExecuteReader();                       

                        while (sqlrd.Read())
                        {
                            priceEU = sqlrd[0].ToString();                          
                        }
                        sqlrd.Close();                       
                    }
                    catch (Exception et)
                    {
                        MessageBox.Show(et.ToString());
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }                                    
                    
                    dataGridView1.Rows.Add(rd[0].ToString(),
                                   rd[1].ToString(),
                                   rd[2].ToString(),
                                  ((float.Parse(rd[2].ToString()) * 1000) / 100) * 1000, price, priceEU);   
                }
                rd.Close();        
            }//try
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: Нет Названия рецепта" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code, 
            // you can retrieve it from a configuration file.
            return "Data Source=ORA01-GALA-01-H;Initial Catalog=test3;Persist Security Info=True;User ID=sa;Password=Adifosa1";
        }

        private bool find_java()
        {  
            Process p = new Process();
            StreamWriter sw;
            StreamReader sr;
            ProcessStartInfo psI = new ProcessStartInfo(Application.StartupPath + "\\runZAO.bat");
            psI.UseShellExecute = false;
            psI.RedirectStandardInput = true;
            psI.RedirectStandardOutput = true;
            psI.RedirectStandardError = true;
            psI.CreateNoWindow = true;
            p.StartInfo = psI;
            p.Start();
            sw = p.StandardInput;
            sr = p.StandardOutput;
            sw.AutoFlush = true;

            sw.WriteLine(tbID.Text);

            sw.Close();

            int i = 0;
            char[] tmp_mas = sr.ReadToEnd().ToCharArray();//koll tonn
            i = 0;
            string str_tmp = "";
            foreach (char temp in tmp_mas)
            {
                if (char.IsNumber(temp)) str_tmp += tmp_mas.GetValue(i).ToString();
                i++;
            }
            if (str_tmp == "")
            {
                MessageBox.Show("карточка не найдена");
                return false;
            }

            ID = int.Parse(str_tmp);

            sr.Close();
            return true;
        }
               
    }
}

/*  альтернатива коннект стринга
                  using (SqlConnection connectionsql = new SqlConnection())
                  {
                      connectionsql.ConnectionString = sqlconnectionString;

                      SqlCommand sqlCommand = new SqlCommand(sqlquary, connectionsql);

                      connectionsql.Open();

                      SqlDataReader sqlrd = sqlCommand.ExecuteReader();

                      while (sqlrd.Read())
                      {
                          price = sqlrd[1].ToString();
                          dateconvert = int.Parse(sqlrd[2].ToString());
                      }
                      MessageBox.Show(dateconvert.ToString());
                      sqlrd.Close();

                  }
                    */
/* поиск курса валюты
      select t$CURSVAL.F$sumrubl
      from t$CURSVAL,t$klval
      where
       t$CURSVAL.F$kodvalut = t$klval.f$nrec 
      and t$klval.f$SIMVOLV = 'eur'
      and t$CURSVAL.f$datval = '131991314'
*/

/*
 
 select T$KATMC.F$NAME, T$SPSOPR.F$PRICE
from T$KATMC,T$SPSOPR
where T$KATMC.F$BARKOD2 = '09017'
and T$SPSOPR.F$CMCUSL = T$KATMC.F$NREC
*/

//ищем элементы витаминных канцентратов
/*  
  command = new FbCommand("select qm_dict.qlt_short_name,qm_dict.qlt_id " +
                       " from rawqdict ,qm_dict " +
                       " where rawqdict.qlt_id <> 104 and rawqdict.qlt_id <> 101 and rawqdict.qlt_id <> 49 and " +
                       " rawqdict.qlt_id <> 102 and rawqdict.qlt_id <> 50 and rawqdict.qlt_id <> 77  and  rawqdict.qlt_id = qm_dict.qlt_id " +
                       "  AND qm_dict.qm_group_id <> 5 and rawqdict.qlt_val > 0 and rawqdict.raw_id = ( " +
                       " select raw_dict.raw_id " +
                       " from raw_dict,rcprep " +
                       " where raw_dict.raw_group_id = 29 "+ 
                       " and  rcprep.rcp_id = " + ID + " " +
                       " and raw_dict.raw_id <>-432725" +
                       " and raw_dict.raw_id <>-433121" +
                       " and raw_dict.raw_id <>-433120" +
                       " and raw_dict.raw_id <>-432915" +
                       " and raw_dict.raw_id <>-432916" +
                       " and raw_dict.raw_id <>-432964" +
                       " and raw_dict.raw_id <>-432931" +
                       " and raw_dict.raw_id <>-432932" +
                       " and raw_dict.raw_id <>-432639" +
                       " and raw_dict.raw_id <>-432414" +
                       " and raw_dict.raw_id <>-432930" +
                       " and raw_dict.raw_id <>-432933" +
                       " and raw_dict.raw_id <>-432580" +
                       " and raw_dict.raw_id <>-433122" +
                       " and raw_dict.raw_id <>-432727" +
                       " and rcprep.raw_id = raw_dict.raw_id  )", connection);

   command.Transaction = transaction;
   rd = command.ExecuteReader();
   i = 0;
   while (rd.Read())
   {
       for (int j = 0; j < arr.Length; j++)
       {
           if (arr[j] == rd[1].ToString())
           {
               isExist = true; break;
           }
       }                
   }
   rd.Close();                
   //находим название рецепта
   command = new FbCommand("SELECT  " +
      "CAST(RAW_DICT.KPROD AS varchar(120)),raw_dict.\"ProdDestination\", RAW_DICT.RAW_NAME " +
      "FROM RCPLIST,DOC_DCT, RAW_DICT " +
      "WHERE  RCPLIST.RCP_ID = " + ID + " AND " +
      "DOC_DCT.DOC_ID = RCPLIST.RCP_ID AND RCPLIST.PROD_ID = RAW_DICT.RAW_ID", connection);
                
   command.Transaction = transaction;
   rd = command.ExecuteReader();

   while (rd.Read())
   {
       tb_Korm_2.Text = tb8_old.Text = tb8.Text = rd[0].ToString();                  
   }
   rd.Close();
   */

/*"select top 1 T$katmc.F$name, T$spsopr.F$price, T$spsopr.F$dsopr " +
                               " from T$katmc, T$spsopr " +
                               " where T$katmc.F$barkod2 = '" + externalID + "' " +
                               " and T$spsopr.F$cmcusl = T$katmc.F$nrec  " +
                               " and T$spsopr.F$price > 0 " +
                               " and T$spsopr.F$dsopr = " +
                               " ( " +
                               " select max (T$spsopr.F$dsopr) " +
                               " from T$katmc, T$spsopr " +
                               " where T$katmc.F$barkod2 = '" + externalID + "' " +
                               " and T$spsopr.F$cmcusl = T$katmc.F$nrec  " +
                               " and T$spsopr.F$price > 0 " +
                               " ) ";
  */