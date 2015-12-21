using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataReport
{
    class DataBaseSqlite:DataBaseInterface
    {
        public event ShowLoadingDataTimes myLoadingDataTimesEvent;

        public void SetmyLoadingDataTimesEvent(int currentnum, int count, string name)
        {
            if (myLoadingDataTimesEvent != null)
            {
                myLoadingDataTimesEvent(currentnum, count, name);
            }
        }
        //事件区
        //查询并返回数据
        public event ShowSelectDataDelegate mySelectDataEvent;
        
        public void SetmySelectDataevent(DataTable dt)
        {
            if (mySelectDataEvent != null)
            {
                mySelectDataEvent(dt); 
            }
        }
        //更新或插入数据
        public event ShowUpdateDataDelegate myUpdateDataEvent;
        public void SetmyUpdateDataEvent(bool istrue)
        {
            if (myUpdateDataEvent != null)
            {
                myUpdateDataEvent(istrue);
            }
        }
        //-------------





        private static DataBaseSqlite instance = null;

        internal static DataBaseSqlite Instance
        {
            get { return getInstance(); }
        }
        private static class SingletonFactory
        {
            public static DataBaseSqlite instance = new DataBaseSqlite();
        }
        /* 获取实例 */
        public static DataBaseSqlite getInstance()
        {
            return SingletonFactory.instance;
        }  



        Lv.Database.SQLite sqlite = null;

        public DataBaseSqlite()
        {
            init();
            InitDicDayKeyMd5();
            //基础数据加载
        }

        List<string> ListUserField = new List<string>();
        List<string> ListWDField = new List<string>();
        List<string> ListyuanField = new List<string>();
        List<string> ListAccessField = new List<string>();

        List<string> ListSummaryUserField = new List<string>();

        string databasenamePath = AppDomain.CurrentDomain.BaseDirectory+"databasename.txt"; //这里面的数据，如果找到了说明已经创建了
        public void init()
        {
            string SqliteFile="";
            try
            {
                 SqliteFile = CreateDataBaseName("");
                if (System.IO.File.Exists(SqliteFile) == true)
                {
                    sqlite = new Lv.Database.SQLite(Lv.Database.Static.SqliteConn(SqliteFile));
                    Console.WriteLine("sqlite 数据连接成功");
                    Lv.Log.Write("sqlite 数据连接成功 :" + SqliteFile, Lv.Log.MessageType.Info);
                    //sqlite.PassWord = "bruce";
                    
                }
                else
                {
                    //创建数据库
                    CreateDataBase("");
                    
                    
                }
                
               
            }
            catch (Exception err)
            {
                Console.WriteLine("sqlite init() 初始化错误");
                Lv.Log.Write("sqlite init() 初始化错误" + SqliteFile, Lv.Log.MessageType.Error);
            }
            
        }

        private void CreateDataBase(string day)
        {
            string year = Common.GetYear(day);
            string SqliteFile = CreateDataBaseName( year);
            SQLiteConnection.CreateFile(SqliteFile);
            Thread.Sleep(1000);
            if (System.IO.File.Exists(SqliteFile) == true)
            {
                sqlite = new Lv.Database.SQLite(Lv.Database.Static.SqliteConn(SqliteFile));
                //sqlite.PassWord = "bruce";
                Console.WriteLine("sqlite 数据连接成功" + SqliteFile);
                Lv.Log.Write("sqlite 数据连接成功 " +SqliteFile, Lv.Log.MessageType.Info);
                Create(day);//创建工作需要的表
            }
            else
            {
                Console.WriteLine("sqlite 连接数据库失败,请关闭程序，重新打开。");
                Lv.Log.Write("sqlite 连接数据库失败,请关闭程序，重新打开。" + SqliteFile, Lv.Log.MessageType.Error);
                throw new Exception("无法创建sqlite数据库请检查");
            }
        }
        string currentDatabaseKey = "";

        /// <summary>
        /// 当前数据库年份key
        /// </summary>
        public string CurrentDataBaseKey
        {
            get { return currentDatabaseKey; }
            set { currentDatabaseKey = value; }
        }
        private string currentDataTableKey = "";

        public string CurrentDataTableKey
        {
            get { return currentDataTableKey; }
            set { currentDataTableKey = value; }
        }
        private string CreateDataBaseName( string year)
        {
            string   path = AppDomain.CurrentDomain.BaseDirectory;
            if (year == "")
            {
                year = DateTime.Now.Year.ToString();
            }
            currentDatabaseKey = Common.GetCurrentSummaryDataBaseName(year);
            string databasename = "database_" + currentDatabaseKey + ".db3";
            string SqliteFile = path + databasename;
            return SqliteFile;
        }

        /// <summary>
        /// 初始化创建表
        /// </summary>
        /// <returns></returns>
        public bool Create(string day)
        {

            string week;
            if (day == "")
            {
                day = DateTime.Now.ToShortDateString();
                week = Common.GetWeekDate(day);
            }
            else
            {
                week = Common.GetWeekDate(day);
            }
            string datetmp = week;
            string tablekey = Common.GetDayTableKey(day);
            string tablename = Common.GetCurrentSummaryTableName(day);
            currentDataTableKey = tablekey;
            try
            {

                sqlite.ExeQuery("CREATE TABLE  if not exists  " + tablename + "([Id] integer PRIMARY KEY AUTOINCREMENT, [keymd5] Text, [日期] Text, [星期] Text, [广告来源] Text, [设备类型] Text, [广告系列] Text, [广告分组] Text, [广告关键词] Text, [展现量] Text, [点击量] Text, [消费] Text, [点击率] Text, [平均点击价格] Text, [登陆] Text, [注册] Text, [APP下载] Text, [注册成功] Text, [首参] Text);");
                System.IO.File.AppendAllText(databasenamePath, CurrentDataBaseKey + "|" + tablekey + "|" + datetmp + "\r\n");
                InitDicDayKeyMd5();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("sqlite init() 初始化错误 " +err.Message);
                Lv.Log.Write("sqlite init() 初始化错误 " + err.Message, Lv.Log.MessageType.Error);
                return false;
            }
        }
        ExcelTypeEm type = ExcelTypeEm.User;

        public ExcelTypeEm Type
        {
            get { return type; }
            set { type = value; }
        }

        private string GetCurrentSummaryTableName()
        {
            //这个地方如果没有创建表那么就没有当前key 只有把日期传过来，在查一下,没有的话需要创建表
            return "Summary_" + currentDataTableKey;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public bool SeaveData(List<string> listSql)
        {
            if (sqlite == null)
            {
                Console.WriteLine("sqlite 没有初始化");
                Lv.Log.Write("sqlite 没有初始化" , Lv.Log.MessageType.Error);
                return false;
            }
            try
            {
                sqlite.BeginTrans();
                for (int i = 0; i < listSql.Count; i++)
                {
                    string sql = listSql[i];
                    if (sql == null)
                    {
                        Console.WriteLine("sql语句组合为null，请确认是否有数据读取出来了");
                        Lv.Log.Write("SeaveData sql语句组合为null，请确认是否有数据读取出来了 ", Lv.Log.MessageType.Error);
                        continue;
                    }
                    sqlite.ExeQuery(sql);
                    if (i % 1000 == 0)
                    {
                        sqlite.CommitTrans();
                        sqlite.BeginTrans();
                    }
                }
                sqlite.CommitTrans();
                return true;
            }
            catch (Exception err)
            {
                Console.WriteLine("插入数据出错 " + err.Message);
                Lv.Log.Write("插入数据出错 " + err.Message, Lv.Log.MessageType.Error);
                return false;
            }
        }

 /*
        internal void BuilderSummary()
        {
            //user 加进来的字段有 
            //wd数据　汇总更新
            wdDataUpdateToSummary();
            yuanDataUpdateToSummary();
        }

        private void yuanDataUpdateToSummary()
        {
            //取得总行数 select count(id)as num from wddata
            int limit = 0;
            int fixlimit = 100;
            int count = sqlite.ExeSqlIdentity("select count(id)as num from yuandata");
            while (true)
            {
                limit += fixlimit;
                StringBuilder sqlupdate = new StringBuilder();
                StringBuilder sqlinsert = new StringBuilder();
                DataTable data = new DataTable();
                data = sqlite.GetDataTable("select [keymd5],[日期],[星期],[广告来源],[设备类型],[广告系列],[广告分组],[广告关键词],[注册成功],[首参]  from yuanData  limit " + (limit - fixlimit).ToString() + "," + fixlimit.ToString());
                if (data == null)
                {
                    break;
                }
                if (data.Rows.Count < 1)
                {
                    break;  //唯一出口
                }
                foreach (DataRow item in data.Rows)
                {
                    sqlinsert.Append(string.Format("insert into " + GetCurrentSummaryTableName() + " ([keymd5],[日期],[星期],[广告来源],[设备类型],[广告系列],[广告分组],[广告关键词],[注册成功],[首参],[#2],[#3] )values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');", item["keymd5"].ToString(), item["日期"].ToString(), item["星期"].ToString(), item["广告来源"].ToString(), item["设备类型"].ToString(), item["广告系列"].ToString(), item["广告分组"].ToString(), item["广告关键词"].ToString(), item["注册成功"].ToString(), item["首参"].ToString(), (item["广告系列"].ToString() + "#" + item["广告分组"].ToString()).Replace("#", "-#-"), (item["广告系列"].ToString() + "#" + item["广告分组"].ToString() + "#" + item["广告关键词"].ToString()).Replace("##", "-#-#-")));
                }
                sqlite.ExeQuery(sqlinsert.ToString());
            }
        }

        private void wdDataUpdateToSummary()
        {
            
            //取得总行数 select count(id)as num from wddata
            int limit = 0;
            int fixlimit = 100;
            int count = sqlite.ExeSqlIdentity("select count(id)as num from wddata");
            while (true)
            {
                limit += fixlimit;
                StringBuilder sqlupdate = new StringBuilder();
                StringBuilder sqlinsert = new StringBuilder();
                DataTable data = new DataTable();
                data = sqlite.GetDataTable("select `keymd5`,`日期`,`设备类型`,`广告来源`,`广告系列`,`广告分组`,`广告关键词`,`支点登陆完成数`,`支点注册完成数`,`Mobile端全部APP下载完成数` from WDData  limit " + (limit - fixlimit).ToString() + "," + fixlimit.ToString());
                if (data == null)
                {
                    break;
                }
                if (data.Rows.Count < 1)
                {
                    break;  //唯一出口
                }
                foreach (DataRow item in data.Rows)
                {
                    string checksql = "select id  from " + GetCurrentSummaryTableName() + " where keymd5='" + item["keymd5"].ToString() + "'";
                    int index = sqlite.ExeSqlIdentity(checksql);
                    if (index > 0)
                    {
                        sqlupdate.Append(" update " + GetCurrentSummaryTableName() + " set `登陆`='" + item["支点登陆完成数"].ToString() + "',`注册`='" + item["支点注册完成数"].ToString() + "',`APP下载`='" + item["Mobile端全部APP下载完成数"].ToString() + "' where `keymd5`='" + item["keymd5"].ToString() + "';");
                    }
                    else
                    {
                        sqlinsert.Append(string.Format("insert into " + GetCurrentSummaryTableName() + " (`keymd5`,`日期`,`星期`,`广告来源`,`设备类型`,`广告系列`,`广告分组`,`广告关键词`,`#2`,`#3`,`登陆`,`注册`,`APP下载`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", item["keymd5"].ToString(), item["日期"].ToString(), Common.GetWeekDate(item["日期"].ToString()), item["广告来源"].ToString(), item["设备类型"].ToString(), item["广告系列"].ToString(), item["广告分组"].ToString(), item["广告关键词"].ToString(), item["广告系列"].ToString() + "#" + item["广告分组"].ToString(), item["广告系列"].ToString() + "#" + item["广告分组"].ToString() + "#" + item["广告关键词"].ToString(), item["支点登陆完成数"].ToString(), item["支点注册完成数"].ToString(), item["Mobile端全部APP下载完成数"].ToString()));
                    }
                }
                sqlite.ExeQuery(sqlupdate.ToString().Replace(",'#',", ",'-#-',").Replace(",'##',", ",'-#-#-',"));
                sqlite.ExeQuery(sqlinsert.ToString().Replace(",'#',", ",'-#-',").Replace(",'##',", ",'-#-#-',"));
            }

            //select `keymd5`,`支点登陆完成数`,`支点注册完成数`,`Mobile端全部APP下载完成数` from WDData  limit 1
            //update summary set `登陆`='1',`注册`='2',`APP下载`='3' where `keymd5`='a9365e8c702a44fd2'
        }
        */

        internal void OutPutExcel(string path,string select)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            OfficeOpenXml.ExcelPackage ep = new OfficeOpenXml.ExcelPackage(file);
            OfficeOpenXml.ExcelWorkbook wb = ep.Workbook;
            OfficeOpenXml.ExcelWorksheet ws = wb.Worksheets.Add("sheet1");
            /*
             //配置文件属性
            wb.Properties.Category = "类别";
            wb.Properties.Author = "作者";
            wb.Properties.Comments = "备注";
            wb.Properties.Company = "公司";
            wb.Properties.Keywords = "关键字";
            wb.Properties.Manager = "管理者";
            wb.Properties.Status = "内容状态";
            wb.Properties.Subject = "主题";
            wb.Properties.Title = "标题";
            wb.Properties.LastModifiedBy = "最后一次保存者";
             */
            //写标题
            ws.Cells[1, 1].Value = "日期";
            ws.Cells[1, 2].Value = "星期";
            ws.Cells[1, 3].Value = "广告来源";
            ws.Cells[1, 4].Value = "设备类型";
            ws.Cells[1, 5].Value = "广告系列";
            ws.Cells[1, 6].Value = "广告分组";
            ws.Cells[1, 7].Value = "广告关键词";
            ws.Cells[1, 8].Value = "#2";
            ws.Cells[1, 9].Value = "#3";
            ws.Cells[1, 10].Value = "展现量";
            ws.Cells[1, 11].Value = "点击量";
            ws.Cells[1, 12].Value = "消费";
            ws.Cells[1, 13].Value = "点击率";
            ws.Cells[1, 14].Value = "平均点击价格";
            ws.Cells[1, 15].Value = "登陆";
            ws.Cells[1, 16].Value = "注册";
            ws.Cells[1, 17].Value = "APP下载";
            ws.Cells[1, 18].Value = "注册成功";
            ws.Cells[1, 19].Value = "首参";

            //写内容
            int row = 2;
            foreach (var itemkeymd5 in dicFileWeekMd5)
            {
                string tablename = Common.Combination(itemkeymd5.Value);

                int index = 0;
                int fixnum = 10000;
                while (true)
                {
                    index += fixnum;
                    DataTable data = sqlite.GetDataTable("select 日期,星期,广告来源,设备类型,广告系列,广告分组,广告关键词,[广告系列]||'#'||[广告分组] as `2#`,[广告系列]||'#'||[广告分组] ||'#' ||[广告关键词]as `3#`,展现量,点击量,消费,点击率,平均点击价格,登陆,注册,APP下载,注册成功,首参 from " + tablename + " " + select + " limit " + (index - fixnum).ToString() + "," + fixnum.ToString() + ";");
                    if (data == null)
                    {
                        break;
                    }
                    if (data.Rows.Count < 1)
                    {
                        break;  //唯一退出条件
                    }
                    foreach (DataRow item in data.Rows)
                    {
                        ws.Cells[row, 1].Value = item["日期"].ToString();
                        ws.Cells[row, 2].Value = item["星期"].ToString();
                        ws.Cells[row, 3].Value = item["广告来源"].ToString();
                        ws.Cells[row, 4].Value = item["设备类型"].ToString();
                        ws.Cells[row, 5].Value = item["广告系列"].ToString();
                        ws.Cells[row, 6].Value = item["广告分组"].ToString();
                        ws.Cells[row, 7].Value = item["广告关键词"].ToString();
                        ws.Cells[row, 8].Value = item["2#"].ToString();
                        ws.Cells[row, 9].Value = item["3#"].ToString();
                        ws.Cells[row, 10].Value = item["展现量"].ToString();
                        ws.Cells[row, 11].Value = item["点击量"].ToString();
                        ws.Cells[row, 12].Value = item["消费"].ToString();
                        ws.Cells[row, 13].Value = item["点击率"].ToString();
                        ws.Cells[row, 14].Value = item["平均点击价格"].ToString();
                        ws.Cells[row, 15].Value = item["登陆"].ToString();
                        ws.Cells[row, 16].Value = item["注册"].ToString();
                        ws.Cells[row, 17].Value = item["APP下载"].ToString();
                        ws.Cells[row, 18].Value = item["注册成功"].ToString();
                        ws.Cells[row, 19].Value = item["首参"].ToString();
                        row++;
                    }
                }
            }
            ep.Save();


        }
        internal void OutPutExcelCSV(string path, object select)
        {
            using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.Default))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("日期").Append(",");
                sb.Append("星期").Append(",");
                sb.Append("广告来源").Append(",");
                sb.Append("设备类型").Append(",");
                sb.Append("广告系列").Append(",");
                sb.Append("广告分组").Append(",");
                sb.Append("广告关键词").Append(",");
                sb.Append("#2").Append(",");
                sb.Append("#3").Append(",");
                sb.Append("展现量").Append(",");
                sb.Append("点击量").Append(",");
                sb.Append("消费").Append(",");
                sb.Append("点击率").Append(",");
                sb.Append("平均点击价格").Append(",");
                sb.Append("登陆").Append(",");
                sb.Append("注册").Append(",");
                sb.Append("APP下载").Append(",");
                sb.Append("注册成功").Append(",");
                sb.Append("首参").Append(",");
                streamWriter.WriteLine(sb.ToString());
                
                //要写的数据源
                //写内容
                int row = 2;
                foreach (var itemkeymd5 in dicFileWeekMd5)
                {
                    string tablename = Common.Combination(itemkeymd5.Value);

                    int index = 0;
                    int fixnum = 10000;
                    while (true)
                    {
                        index += fixnum;
                        SetmyLoadingDataTimesEvent(index, index, tablename);
                        string sql = "select 日期,星期,广告来源,设备类型,广告系列,广告分组,广告关键词,[广告系列]||'#'||[广告分组] as `2#`,[广告系列]||'#'||[广告分组] ||'#' ||[广告关键词]as `3#`,展现量,点击量,消费,点击率,平均点击价格,登陆,注册,APP下载,注册成功,首参 from " + tablename + " " + select + " limit " + (index - fixnum).ToString() + "," + fixnum.ToString() + ";";
                        DataTable data = sqlite.GetDataTable(sql);
                        if (data == null)
                        {
                            break;
                        }
                        if (data == null)
                        {
                            break;
                        }
                        if (data.Rows.Count < 1)
                        {
                            break;  //唯一退出条件
                        }
                        foreach (DataRow item in data.Rows)
                        {
                            sb = new StringBuilder();
                            for (int i = 0; i < item.ItemArray.Length; i++)
                            {
                                if (item[i].ToString().Contains(",") == true)
                                {
                                    sb.Append("\"" + item[i].ToString() + "\"").Append(",");
                                }
                                else
                                {
                                    sb.Append(item[i].ToString()).Append(",");
                                }
                                
                            }
                            streamWriter.WriteLine(sb.ToString());
                            row++;
                        }
                    }
                }
                sb = new StringBuilder();
                streamWriter.WriteLine("");
                streamWriter.WriteLine("");
                streamWriter.WriteLine(sb.ToString());

                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public void SelectField(ref List<string> listday, ref List<string> listweek, ref List<string> listsource, ref List<string> listdevice, ref List<string> listserice, ref List<string> listgroup, ref List<string> listkey)
        {
            List<string> listtmp = new List<string>();
            foreach (var itemkeymd5 in dicFileWeekMd5)
            {
                string tablename = Common.Combination(itemkeymd5.Value);
                string sql = "select distinct[日期] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listday);

                sql = "select distinct[星期] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listweek);

                sql = "select distinct[广告来源] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listsource);

                sql = "select distinct[设备类型] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listdevice);

                sql = "select distinct[广告系列] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listserice);

                sql = "select distinct[广告分组] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listgroup);

                sql = "select distinct[广告关键词] from " + tablename + " limit 0,1000";
                listtmp.Clear();
                listtmp.AddRange(GetSelectField(sql));
                ListFilterRepeatItem(listtmp, ref listkey);
            }
        }

        private void ListFilterRepeatItem(List<string> listtmp, ref List<string> listday)
        {
            foreach (string item in listtmp)
            {
                if (listday.Contains(item) == true)
                {
                    continue;
                }
                else
                {
                    listday.Add(item);
                }
            }
        }

        private List<string> GetSelectField( string sql)
        {
            List<string> list = new List<string>();
            DataTable data = sqlite.GetDataTable(sql);
            if (data == null)
            {
                return null;
            }
            if (data.Rows.Count > 0)
            {
                foreach (DataRow item in data.Rows)
                {
                    list.Add(item[0].ToString());
                }
            }
            return list;
        }

        internal void SelectData(string sqlwhere)
        {
            DataTable data = new DataTable();
            data.Columns.Add("id");
            data.Columns.Add("keymd5");
            data.Columns.Add("日期");
            data.Columns.Add("星期");
            data.Columns.Add("广告来源");
            data.Columns.Add("设备类型");
            data.Columns.Add("广告系列");
            data.Columns.Add("广告分组");
            data.Columns.Add("广告关键词");
            data.Columns.Add("2#");
            data.Columns.Add("3#");
            data.Columns.Add("展现量");
            data.Columns.Add("点击量");
            data.Columns.Add("消费");
            data.Columns.Add("点击率");
            data.Columns.Add("平均点击价格");
            data.Columns.Add("登陆");
            data.Columns.Add("注册");
            data.Columns.Add("APP下载");
            data.Columns.Add("注册成功");
            data.Columns.Add("首参");
            if (sqlwhere.Contains("日期") == true)
            {

                string day = Lv.Text.GetInsideStr(sqlwhere, "[日期]='", "'");
                string tablename = Common.GetCurrentSummaryTableName(day);
                int index = 0;
                int fixnum = 10000; //超过10000条的话不显示　
                //查找数据
                DataTable datatmp = sqlite.GetDataTable("select  ID,keymd5,日期,星期,广告来源,设备类型,广告系列,广告分组,广告关键词,[广告系列]||'#'||[广告分组] as `2#`,[广告系列]||'#'||[广告分组] ||'#' ||[广告关键词]as `3#`,展现量,点击量,消费,点击率,平均点击价格,登陆,注册,APP下载,注册成功,首参  from " + tablename + " " + sqlwhere + " limit " + (index - fixnum).ToString() + "," + fixnum.ToString() + ";");
                if (datatmp == null)
                {
                    return;
                }
                if (datatmp.Rows.Count < 1)
                {
                    return;
                }
                SetmySelectDataevent(datatmp);
                return;
            }
            else
            {
                foreach (var itemkeymd5 in dicFileWeekMd5)
                {
                    string tablename = Common.Combination(itemkeymd5.Value);
                    int index = 0;
                    int fixnum = 10000; //超过10000条的话不显示　
                    //查找数据
                    DataTable datatmp = sqlite.GetDataTable("select ID,keymd5,日期,星期,广告来源,设备类型,广告系列,广告分组,广告关键词,[广告系列]||'#'||[广告分组] as `2#`,[广告系列]||'#'||[广告分组] ||'#' ||[广告关键词]as `3#`,展现量,点击量,消费,点击率,平均点击价格,登陆,注册,APP下载,注册成功,首参 from " + tablename + " " + sqlwhere + " limit " + (index - fixnum).ToString() + "," + fixnum.ToString() + ";");
                    if (datatmp == null)
                    {
                        continue;
                    }
                    if (datatmp.Rows.Count<1)
                    {
                        continue;
                    }
                    foreach (DataRow item in datatmp.Rows)
                    {
                        //DataRow tmpitme = new DataRow();
                        //tmpitme.ItemArray = (object[])item.ItemArray.Clone();
                        data.Rows.Add((object[])item.ItemArray.Clone());
                    }
                }
                SetmySelectDataevent(data);
            }
        }

        internal bool UpdateData(string sqlupdate)
        {
            string index = "";
            string day = Lv.Text.GetInsideStr(sqlupdate, "[日期] = '", "'");
            string tablename = Common.GetCurrentSummaryTableName(day);
            foreach (var item in dicFileWeekMd5)
            {
                if (tablename.Contains(item.Value))
                {
                    try
                    {
                        index = sqlite.ExeSqlScalar("update " + tablename + " set " + sqlupdate);
                        SetmyUpdateDataEvent(true);
                        return true;
                    }
                    catch (Exception)
                    {
                        SetmyUpdateDataEvent(false);
                        return false;
                        throw;
                    }
                   
                }
            }
            SetmyUpdateDataEvent(false);
            return false;
        }
        internal bool InsertData(string insert)
        {
            int index = 0;
            index = sqlite.ExeSqlIdentity("insert into " + GetCurrentSummaryTableName() + " " + insert);
            bool isSuccess = false;

            if (index >= 0)
            {
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }
            SetmyUpdateDataEvent(isSuccess);
            return isSuccess;
        }

        internal void UpdateDataOption(string sql,int type)
        {
            if (type == 0)
            {
                UpdateData(sql);
            }
            else 
            {
                InsertData(sql);
            }
        }


        Dictionary<string, string> dicFileWeekMd5 = new Dictionary<string, string>();

        public Dictionary<string, string> DicFileWeekMd5
        {
            get { return dicFileWeekMd5; }
            set { dicFileWeekMd5 = value; }
        }
        /// <summary>
        /// keymd5 和id的值
        /// </summary>
        Dictionary<string,string> dickeymd5Id = new Dictionary<string,string>();
        /// <summary>
        /// 日期和所对应的md5值　
        /// </summary>
        Dictionary<string, string> dicDayKeyMd5 = new Dictionary<string, string>();

        public Dictionary<string, string> DicDayKeyMd5
        {
            get { return dicDayKeyMd5; }
            set { dicDayKeyMd5 = value; }
        }
        bool isSelectRepate = false;

        public bool IsSelectRepate
        {
            get { return isSelectRepate; }
            set { isSelectRepate = value; }
        }
        internal string IsExist(string day, string keymd5)
        {
            bool isTableExist = false;
            isTableExist = IsTableExist(day);
            if (isTableExist == true)
            {
                bool isKeymd5Exist = false;
                string id = "";
                //isKeymd5Exist = IsTableExistKeyMd5(ref id, keymd5,day);
                return id;
            }
            else
            {
                //表不存在

                //创建表
                Create(day);
                //返回null
                //更新databasepash.txt文件
                //更新dicDayKeyMd5

                return null;
            }



            return null;
        }

        private bool IsTableExistKeyMd5(ref string id, string keymd5,string day)
        {
            bool result = false;
            if (dickeymd5Id.ContainsKey(keymd5) == true)
            {
                id = dickeymd5Id[keymd5];
                result = true;
            }
            else
            {
                if (isSelectRepate)
                {
                    //id = sqlite.ExeSqlScalar("select * from " + Common.GetCurrentSummaryTableName(day) + " where [keymd5] = '" + keymd5 + "' ;");
                    if (id != null && id != "")
                    {
                        //如果没有找到，那么说明一次都没有插入相同的数据，所以不需要保存
                        if (dickeymd5Id.Count > 1000000)
                        {
                            dickeymd5Id.Clear();
                        }
                        dickeymd5Id.Add(keymd5, id);
                        result = true;
                    }
                }
                else
                {
                    //不做重复检查
                }
               
            }
            return result;
        }

        private bool IsTableExist(string day)
        {
            bool result = false;
            if (dicDayKeyMd5.ContainsKey(day) == true)
            {
                result = true;
            }
            else
            {
                 string datanameContent = "";
                try
                {
                    datanameContent = System.IO.File.ReadAllText(databasenamePath);

                }
                catch (Exception err)
                {
                    Console.WriteLine("IsExist 文件没有找到: " + databasenamePath);
                    Lv.Log.Write("IsExist 文件没有找到: " + databasenamePath, Lv.Log.MessageType.Error);
                    throw err;
                }
                string tablekey = Common.GetDayTableKey(day);
                if (datanameContent.Contains(tablekey) == true)
                {
                    if (dicDayKeyMd5.Keys.Contains(day) == false)
                    {
                        dicDayKeyMd5.Add(day, tablekey);
                    }
                    result = true;
                }
            }
            return result;

        }

        private void InitDicDayKeyMd5()
        {
            dicFileWeekMd5.Clear();
            string datanameContent = System.IO.File.ReadAllText(databasenamePath);
            string[] strtmp = datanameContent.Split('\n');
            foreach (string item in strtmp)
            {
                string[] strlist = item.Trim('\r').Split('|');
                if (strlist.Length>=2 )
                {
                    if (dicFileWeekMd5.Keys.Contains(strlist[2]) == false)
                    {
                        dicFileWeekMd5.Add(strlist[2], strlist[1]);
                    }
                    
                }
                
            }
            
        }

        internal void OutPutDataRepot()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "Report.xlsx";
            if (System.IO.File.Exists(filename) == true)
            {
                filename = AppDomain.CurrentDomain.BaseDirectory + "Report" + Lv.Crypt.md5(DateTime.Now.ToString(), 16).Substring(1, 5)+ ".xlsx";
            }
            System.IO.FileInfo file = new System.IO.FileInfo(filename);
            OfficeOpenXml.ExcelPackage ep = new OfficeOpenXml.ExcelPackage(file);
            OfficeOpenXml.ExcelWorkbook wb = ep.Workbook;
            OfficeOpenXml.ExcelWorksheet ws = wb.Worksheets.Add("sheet1");
            /*
             //配置文件属性
            wb.Properties.Category = "类别";
            wb.Properties.Author = "作者";
            wb.Properties.Comments = "备注";
            wb.Properties.Company = "公司";
            wb.Properties.Keywords = "关键字";
            wb.Properties.Manager = "管理者";
            wb.Properties.Status = "内容状态";
            wb.Properties.Subject = "主题";
            wb.Properties.Title = "标题";
            wb.Properties.LastModifiedBy = "最后一次保存者";
             */
            int row = 10;
            //写标题
            ws.Cells[row, 1].Value = "日期";
            ws.Cells[row, 2].Value = "展现量";
            ws.Cells[row, 3].Value = "点击量";
            ws.Cells[row, 4].Value = "消费";
            ws.Cells[row, 5].Value = "CTR";
            ws.Cells[row, 6].Value = "CPC";
            ws.Cells[row, 7].Value = "登陆";
            ws.Cells[row, 8].Value = "注册";
            ws.Cells[row, 9].Value = "APP下载";
            ws.Cells[row, 10].Value = "转化";
            ws.Cells[row, 11].Value = "转化成本";
            ws.Cells[row, 12].Value = "注册成功";
            ws.Cells[row, 13].Value = "注册成功成本";
            ws.Cells[row, 14].Value = "注册率";
            ws.Cells[row, 15].Value = "首参";
            ws.Cells[row, 16].Value = "首参成本";
            ws.Cells[row, 17].Value = "首参率";

            //写内容
            row++;
            DataTable data = sqlite.GetDataTable("select * from summary_日期view");
            if (data == null)
            {
                ep.Save();
                return;
            }
            if (data.Rows.Count < 1)
            {
                ep.Save();
                return;
            }
            foreach (DataRow item in data.Rows)
            {
                ws.Cells[row, 1].Value = item["日期"].ToString();
                ws.Cells[row, 2].Value = item["展现量"].ToString();
                ws.Cells[row, 3].Value = item["点击量"].ToString();
                ws.Cells[row, 4].Value = item["消费"].ToString();
                ws.Cells[row, 5].Value = item["CTR"].ToString();
                //ws.Cells[row, 6].Value = item["CPC"].ToString();
                ws.Cells[row, 7].Value = item["登陆"].ToString();
                    ws.Cells[row, 10].Value = item["注册"].ToString();
                    ws.Cells[row, 11].Value = item["APP下载"].ToString();
                    //ws.Cells[row, 12].Value = item["转化"].ToString();
                    //ws.Cells[row, 13].Value = item["转化成本"].ToString();
                    ws.Cells[row, 14].Value = item["注册成功"].ToString();
                    //ws.Cells[row, 15].Value = item["注册成功成本"].ToString();
                    ws.Cells[row, 16].Value = item["注册率"].ToString();
                    ws.Cells[row, 17].Value = item["首参"].ToString();
                    //ws.Cells[row, 18].Value = item["首参成本"].ToString();
                    ws.Cells[row, 19].Value = item["首参率"].ToString();
                row++;
            }
            ep.Save();

        }
    }
            
}
