using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public class AccessImport
    {
        DataBaseSqlite sqlite = DataBaseSqlite.getInstance();
        private static AccessImport instance = null;

        internal static AccessImport Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new AccessImport();
                }
                return instance; 
            }
        }

        string dataBasePath = "";

        public string DataBasePath
        {
            get { return dataBasePath; }
            set { dataBasePath = value; }
        }
        List<string> listTables = new List<string>();

        public List<string> ListTables
        {
            get { return listTables; }
            set { listTables = value; }
        }
        bool isRepate = false;

        public bool IsRepate
        {
            get { return isRepate; }
            set { isRepate = value; }
        }
        public event ShowLoadingDataTimes myLoadingDataTimesEvent;

        public void SetmyLoadingDataTimesEvent(int currentnum, int count, string name)
        {
            if (myLoadingDataTimesEvent != null)
            {
                myLoadingDataTimesEvent(currentnum, count, name);
            }
        }
        public void Access2007Import()
        {
            sqlite.Type = ExcelTypeEm.Access;
            Dictionary<int, string> dictitle = new Dictionary<int, string>();
            List<string> listSQL = new List<string>();


            string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;";  //C#读取Excel的连接字符串  
            strConnection += @"Data Source = " + dataBasePath;  //指定数据库在硬盘的物理位置  

            using (OleDbConnection objConnection = new OleDbConnection(strConnection)) //用using替代objConnection.Close()  
            {
                objConnection.Open();  //打开连接  

                foreach (var tablename in listTables)
                {

                    dictitle.Clear();
                    int tablecount = 0;
                    OleDbCommand sqlcmd = new OleDbCommand(@"SELECT Count(0) FROM [" + tablename + "];", objConnection);  //sql语句     
                    using (OleDbDataReader reader = sqlcmd.ExecuteReader())  //执行查询，用using替代reader.Close()  
                    {
                        if (reader.Read())     //这个read调用很重要！不写的话运行时将提示找不到数据  
                        {
                            tablecount = Convert.ToInt32(reader[0].ToString());
                        }
                    }


                    bool isField = false;
                    sqlcmd = new OleDbCommand(@"SELECT * from [" + tablename + "];", objConnection);  //sql语句     
                    using (OleDbDataReader reader = sqlcmd.ExecuteReader())  //执行查询，用using替代reader.Close()  
                    {
                        if (reader.Read())     //这个read调用很重要！不写的话运行时将提示找不到数据  
                        {
                            //取title
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string title = reader.GetName(i);
                                dictitle.Add(i, title);
                            }
                            if (!isField)
                            {
                                isField = Common.CheckAccessField(dictitle);
                                if (isField == false)
                                {
                                    Lv.Log.Write("表" + tablename + "里没有必须的字段，无法导入", Lv.Log.MessageType.Error);
                                    continue;
                                }
                            }

                            int count = 0;
                            while (true)
                            {

                                Dictionary<string, string> dickv = new Dictionary<string, string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string value = reader[dictitle[i]].ToString();
                                    dickv.Add(dictitle[i], value);
                                }
                                count++;

                                if (switchAccessToSqlite(ref dickv, dataBasePath) == true)
                                {
                                    SummaryData.Instance.IsRepate = isRepate;
                                    if (SummaryData.Instance.ConvertDicToField(dickv) == true)
                                    {
                                        listSQL.Add(SummaryData.Instance.GetSQL(dickv["日期"], dickv["keymd5"]));
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (reader.Read() == false)
                                {
                                    break;
                                }
                                if (count % 500 == 0)
                                {
                                    sqlite.SeaveData(listSQL);
                                    listSQL.Clear();
                                    if (tablecount < count)
                                    {
                                        tablecount = count;
                                    }
                                    SetmyLoadingDataTimesEvent(count, tablecount, tablename);
                                    Console.WriteLine("已写入数据:" + count.ToString());
                                }
                            }
                            sqlite.SeaveData(listSQL);
                            listSQL.Clear();
                            Console.WriteLine("总处理行数:" + count);
                            Lv.Log.Write("总处理行数:", Lv.Log.MessageType.Info);
                        }
                    }// end using

                }// end foreach tablename

            }// end using
        }

        private bool switchAccessToSqlite(ref Dictionary<string, string> dictHeaderValue, string path)
        {
            try
            {
                DateTime date = new DateTime();
                DateTime.TryParse(dictHeaderValue["日期"],out date);
                dictHeaderValue["日期"] = date.ToShortDateString();
                string timetmp = "";
                if (dictHeaderValue["广告系列"] == "" && dictHeaderValue["广告分组"] == "" && dictHeaderValue["广告关键词"] == "")
                {
                    timetmp = Lv.Crypt.md5(DateTime.Now.ToString() + Guid.NewGuid(), 16);
                }
                string md5key = Lv.Crypt.md5(dictHeaderValue["日期"] + dictHeaderValue["广告系列"] + dictHeaderValue["广告分组"] + dictHeaderValue["广告关键词"] + timetmp, 16);
                dictHeaderValue.Add("keymd5", md5key);
                
            }
            catch (Exception err)
            {
                
                throw err;
            }
            return true;
        }//end methon


        internal List<string> GetListTable()
        {
            List<string> Tables = new List<string>();
            string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;";  //C#读取Excel的连接字符串  
            strConnection += @"Data Source = "+dataBasePath;  //指定数据库在硬盘的物理位置  

            using (OleDbConnection objConnection = new OleDbConnection(strConnection)) //用using替代objConnection.Close()  
            {
                objConnection.Open();  //打开连接   
                DataTable dt = objConnection.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    if (row[3].ToString() == "TABLE")
                        Tables.Add(row[2].ToString());
                }
            }
            return Tables;

        }



    }
}
