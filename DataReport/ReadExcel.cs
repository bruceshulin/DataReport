using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;

namespace DataReport
{
    public delegate void MyBruceInvoke(string str);
    class ReadExcel
    {

        static ReadExcel instance = null;

        internal static ReadExcel Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new ReadExcel();
                }
                return instance; 
            }
        }
        bool isRepate = false;

        public bool IsRepate
        {
            get { return isRepate; }
            set { isRepate = value; }
        }
        public event ShowLoadingDataTimes myLoadingDataTimesEvent;

        public void SetmyLoadingDataTimesEvent(int currentnum, int count,string name)
        {
            if (myLoadingDataTimesEvent != null)
            {
                myLoadingDataTimesEvent(currentnum, count, name);
            }
        }
        public void SetTxt(string str)
        {
            mainfrom.Text = "hello";
        }
        DataReport.Form1 mainfrom = null;
        public void ReadExcelFile(object obj)
        {
            List<ExcelType> listPath = new List<ExcelType>();
            listPath = GetExcelPath();

            
            foreach (ExcelType item in listPath)
            {
                if (item.Type == ExcelTypeEm.Yuan)
                {
                    EpplusExcel2007ReadYuan(item);
                }
                else if (item.Type == ExcelTypeEm.User)
                {
                    if (item.Path.EndsWith(".csv") == true)
                    {
                        CSVRead(item);
                    }
                    else
                    {
                        EpplusExcel2007Read(item);
                    }
                }
                else if (item.Type == ExcelTypeEm.WD)
                {
                    EpplusExcel2007Read(item);
                }
                
            }
            /*
            ///* 账户表已测试完成
            Pathtype.Path = diruser1;
            Pathtype.Type = Pathtype.getExcelTypeEM();
            EpplusExcel2007Read(Pathtype);
            Pathtype.Path = diruser2;
            Pathtype.Type = Pathtype.getExcelTypeEM();
            EpplusExcel2007Read(Pathtype);
            Pathtype.Path = diruser;
            Pathtype.Type = Pathtype.getExcelTypeEM();
            EpplusExcel2007Read(Pathtype);
            //*/
            /*
             //* WD数据表
            //
            Pathtype.Path = dirwd;
            Pathtype.Type = Pathtype.getExcelTypeEM();
            EpplusExcel2007Read(Pathtype);
            //EpplusExcel2007Read(dirwd);
            //EpplusExcel2007Read(dirOriginal);
            */
        }
        /// <summary>
        /// csv读取
        /// </summary>
        /// <param name="item"></param>
        private void CSVRead(ExcelType item)
        {
           // Encoding encoding = Common.GetType(item.Path); //Encoding.ASCII;//
            List<string> listSQL = new List<string>();
            FileStream fs = new FileStream(item.Path, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //保存表头信息
            Dictionary<int, string> dictHeader = new Dictionary<int, string>();
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            int rowcount = 0;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                rowcount++;
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);

                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    columnCount = tableHead.Length;
                    if (columnCount<2)
                    {
                        continue;
                    }
                    IsFirst = false;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        dictHeader[i] = tableHead[i].Replace(" ", "");
                    }
                }
                else
                {
                    Dictionary<string, string> dictHeadervalue = new Dictionary<string, string>();
                    string result = "";
                    aryLine = strLine.Split(',');
                    if (aryLine.Length > columnCount)
                    {
                        System.Text.RegularExpressions.Match mc
                        = System.Text.RegularExpressions.Regex.Match(strLine, "\"[^\"]*?,[^\"]*?\",", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (mc.Value != null && mc.Value != "")
                        {
                            result = mc.Value.Trim(',').Trim('\"');
                            string tmpresult = result.Replace(",", "&&&&&&&&&&");
                            strLine = strLine.Replace(result, tmpresult);
                        }
                        aryLine = null;
                        aryLine = strLine.Split(',');
                    }
                    for (int j = 0; j < columnCount; j++)
                    {
                        //每一行的数据
                        dictHeadervalue[dictHeader[j]] = aryLine[j].Trim('\"').Replace("&&&&&&&&&&",",");         //标题，值
                    }
                   
                    //解析当前数据在每个表里的数据，然后把这些表都保存一次
                    //当前字段和值都在这里 dictHeadervalue
                    if (dictHeadervalue.Count < 1)
                    {
                        continue;
                    }
                    if (switchExcelToSqlite(ref dictHeadervalue, item) == true)
                    {
                        if (dictHeadervalue.ContainsKey("日期") == false)
                        {
                            continue;
                        }
                        SummaryData.Instance.IsRepate = isRepate;
                        if (SummaryData.Instance.ConvertDicToField(dictHeadervalue) == true)
                        {
                            listSQL.Add(SummaryData.Instance.GetSQL(dictHeadervalue["日期"], dictHeadervalue["keymd5"]));
                        }
                        else
                        {
                            continue;
                        }
                        
                        //listdata.Add(dictHeadervalue);
                    }
                    else
                    {
                        //这里表示从Excel里转换数据失败，属于严重错误，想办法处理
                        Console.WriteLine("这里表示从Excel里转换数据失败，属于严重错误，想办法处理");
                        Lv.Log.Write("数据导入失败  Excel里转换数据失败", Lv.Log.MessageType.Error);
                        MessageBox.Show("数据导入失败，");
                        return;
                    }
                    if (rowcount % 500 == 0)
                    {
                        sqlite.SeaveData(listSQL);
                        listSQL.Clear();
                        SetmyLoadingDataTimesEvent(rowcount, rowcount, System.IO.Path.GetFileName( item.Path));
                    }
                }
            }
            sqlite.SeaveData(listSQL);
            listSQL.Clear();

            sr.Close();
            fs.Close();
        }


        DataBaseSqlite sqlite = DataBaseSqlite.getInstance();
        private void EpplusExcel2007ReadYuan(ExcelType path)
        {
            try
            {
                //实例化一个计时器
                Stopwatch watch = new Stopwatch();
                //开始计时/*此处为要计算的运行代码
                watch.Start();
                //保存表头信息
                Dictionary<int, string> dictHeader = new Dictionary<int, string>();
                //文件信息
                FileInfo newFile = new FileInfo(path.Path);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    string time = watch.ElapsedMilliseconds.ToString();
                    Console.WriteLine("加载完文件时间:" + time);
                    Lv.Log.Write("加载完文件时间: " + time, Lv.Log.MessageType.Info);

                    int vSheetCount = package.Workbook.Worksheets.Count; //获取总Sheet页
                    int page = 1;
                    if (path.Path.Contains("百度") == true)
                    {
                        page = 2;
                    }
                    for (int pagei = 1; pagei <= page; pagei++)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[pagei];//选定 指定页
                        
                        time = watch.ElapsedMilliseconds.ToString();
                        Console.WriteLine("到打开表时间:" + time);
                        Lv.Log.Write("到打开表时间: " + time, Lv.Log.MessageType.Info);
                        watch.Stop();//结束计时

                        int colStart = worksheet.Dimension.Start.Column;//工作区开始列
                        int colEnd = worksheet.Dimension.End.Column;    //工作区结束列
                        int rowStart = worksheet.Dimension.Start.Row;   //工作区开始行号
                        int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号
                        //现在用不到，以后用得到

                        //1　每个表的样式都不一样，如果统一起来处理的话不好处理，怎么处理呢？
                        //传每个表的类型过来
                        
                        sqlite.Type = path.Type;
                        List<string> listSQL = new List<string>();
                        int off = 0;    //起始行偏移量

                        if (path.Type == ExcelTypeEm.WD)
                        {
                            off = 6;
                        }
                        //将每列标题添加到字典中
                        for (int i = colStart; i <= colEnd; i++)
                        {
                            if (worksheet.Cells[rowStart + off, i].Value == null)
                            {
                                continue;
                            }
                            string titlestr = worksheet.Cells[rowStart + off, i].Value.ToString();
                            if( titlestr == null || titlestr == "")
                            {
                                continue;
                            }
                            dictHeader[i] = titlestr.Replace(" ", "");
                        }
                        off += 1;
                        int count = 0;
                        //遍历每一行
                        for (int row = rowStart + off; row <= rowEnd; row++)
                        {
                            //每一行的数据
                            Dictionary<string, string> dictHeadervalue = new Dictionary<string, string>();
                            if (path.Path.Contains("百度") == true)
                            {
                                if (pagei == 1)
                                {
                                    dictHeadervalue.Add("设备类型", "PC");
                                    dictHeadervalue.Add("广告来源", "Baidu");
                                }
                                else if (pagei == 2)
                                {
                                    dictHeadervalue.Add("设备类型", "MOB");
                                    dictHeadervalue.Add("广告来源", "MBaidu");
                                }
                            }
                            if (path.Path.Contains("360") == true)
                            {
                                if (pagei == 1)
                                {
                                    dictHeadervalue.Add("设备类型", "PC");
                                    dictHeadervalue.Add("广告来源", "360");
                                }
                            }
                            //   if (count > 50) break;
                            count++;

                            for (int col = colStart; col <= colEnd; col++)
                            {

                                //得到单元格信息
                                ExcelRange cell = null;
                                try
                                {
                                    cell = worksheet.Cells[row, col];
                                }
                                catch (Exception err)
                                {
                                    Console.WriteLine("" + err.Message);
                                    Lv.Log.Write("提取单元数据出错　row" + row.ToString() + " col" + col.ToString() + err.Message, Lv.Log.MessageType.Error);
                                }
                                if (cell.Value == null)
                                    continue;
                                string text = cell.RichText.Text;
                                dictHeadervalue[dictHeader[col]] = text;         //标题，值
                            }

                            //解析当前数据在每个表里的数据，然后把这些表都保存一次
                            //当前字段和值都在这里 dictHeadervalue
                            if (dictHeadervalue.Count < 1)
                            {
                                continue;
                            }
                            if (dictHeadervalue.ContainsKey("来源") == false)
                            {
                                //continue;
                                dictHeadervalue.Add("来源","");
                            }
                            if (switchExcelToSqlite(ref dictHeadervalue, path) == true)
                            {
                                 SummaryData.Instance.IsRepate = isRepate;
                                 if (SummaryData.Instance.ConvertDicToField(dictHeadervalue)==true)
                                 {
                                     listSQL.Add(SummaryData.Instance.GetSQL(dictHeadervalue["日期"], dictHeadervalue["keymd5"]));
                                 }
                                 else
                                 {
                                     continue;
                                 }
                            }
                            else
                            {
                                //这里表示从Excel里转换数据失败，属于严重错误，想办法处理
                                Console.WriteLine("这里表示从Excel里转换数据失败，属于严重错误，想办法处理");
                                Lv.Log.Write("数据导入失败 Excel里转换数据失败", Lv.Log.MessageType.Error);
                                MessageBox.Show("数据导入失败，");
                                return;
                            }
                            if (row % 500 == 0)
                            {
                                sqlite.SeaveData(listSQL);
                                listSQL.Clear();

                                SetmyLoadingDataTimesEvent(count, rowEnd, package.Workbook.Worksheets[pagei].Name);
                            }
                        }
                        sqlite.SeaveData(listSQL);
                        listSQL.Clear();
                        Console.WriteLine("总处理行数:" + count);
                        Lv.Log.Write("总处理行数: " + count, Lv.Log.MessageType.Info);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("加载excel出错了　" + err.Message);
                Lv.Log.Write("加载excel出错了　 " + err.Message, Lv.Log.MessageType.Error);
            }
        }

        private List<ExcelType> GetExcelPath()
        {
            string pathwd = @"\数据表\WD数据";
            string pathuser = @"\数据表\账户数据";
            string pathyang = @"\数据表\转化数据";
            string basepath = System.Environment.CurrentDirectory;
            //string path2 = System.AppDomain.CurrentDomain.BaseDirectory;
            if (System.IO.Directory.Exists(basepath + pathwd) == false)
            {
                System.IO.Directory.CreateDirectory(basepath + pathwd);
            }
            if (System.IO.Directory.Exists(basepath + pathuser) == false)
            {
                System.IO.Directory.CreateDirectory(basepath + pathuser);
            }
            if (System.IO.Directory.Exists(basepath + pathyang) == false)
            {
                System.IO.Directory.CreateDirectory(basepath + pathyang);
            }
            List<ExcelType> list = new List<ExcelType>();
            list.AddRange(getPath(basepath + pathwd, ExcelTypeEm.WD));
            list.AddRange(getPath(basepath + pathuser, ExcelTypeEm.User));
            list.AddRange(getPath(basepath + pathyang, ExcelTypeEm.Yuan));
            return list;
        }

        private IEnumerable<ExcelType> getPath(string path,ExcelTypeEm type)
        {
            List<ExcelType> list = new List<ExcelType>();
            string[] strfiles = new string[]{};
            if (type == ExcelTypeEm.Yuan)
            {
                strfiles = System.IO.Directory.GetFiles(path + "\\转化-原数据");
            }else
            {
                strfiles = System.IO.Directory.GetFiles(path);
            }
            

            for (int i = 0; i < strfiles.Count(); i++)
            {
                if (strfiles[i].Contains(".xlsx") == true || strfiles[i].Contains(".csv") == true)
                {
                    ExcelType et = new ExcelType();
                    et.Type = type;
                    et.Path = strfiles[i];
                    list.Add(et);
                }
            }
            return list;
        }

        /// <summary>
        /// 表目录，表类型
        /// </summary>
        /// <param name="path"></param>
        private void EpplusExcel2007Read(ExcelType path)
        {
            try
            {
                //实例化一个计时器
                Stopwatch watch = new Stopwatch();
                //开始计时/*此处为要计算的运行代码
                watch.Start();
                //保存表头信息
                Dictionary<int, string> dictHeader = new Dictionary<int, string>();
                //文件信息
                FileInfo newFile = new FileInfo(path.Path);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    string time = watch.ElapsedMilliseconds.ToString();
                    Console.WriteLine("加载完文件时间:" + time);
                    Lv.Log.Write("加载完文件时间 " + time, Lv.Log.MessageType.Info);
                    int vSheetCount = package.Workbook.Worksheets.Count; //获取总Sheet页
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];//选定 指定页

                    time = watch.ElapsedMilliseconds.ToString();
                    Console.WriteLine("到打开表时间:" + time);
                    Lv.Log.Write("到打开表时间 " + time, Lv.Log.MessageType.Info);
                    watch.Stop();//结束计时

                    int colStart = worksheet.Dimension.Start.Column;//工作区开始列
                    int colEnd = worksheet.Dimension.End.Column;    //工作区结束列
                    int rowStart = worksheet.Dimension.Start.Row;   //工作区开始行号
                    int rowEnd = worksheet.Dimension.End.Row;       //工作区结束行号
                    //现在用不到，以后用得到

                    //1　每个表的样式都不一样，如果统一起来处理的话不好处理，怎么处理呢？
                    //传每个表的类型过来
                    //DataBaseSqlite sqlite = new DataBaseSqlite();
                    sqlite.Type = path.Type;
                    List<string> listSQL = new List<string>();
                    int off = 0;    //起始行偏移量
                    
                    if (path.Type == ExcelTypeEm.WD)
                    {
                        off = 6;
                    }
                    //将每列标题添加到字典中
                    for (int i = colStart ; i <= colEnd; i++)
                    {
                        dictHeader[i] = worksheet.Cells[rowStart + off, i].Value.ToString().Replace(" ","");
                    }
                    off += 1;
                    int count = 0;
                    //遍历每一行
                    for (int row = rowStart + off; row <= rowEnd; row++)
                    {
                        //每一行的数据
                        Dictionary<string, string> dictHeadervalue = new Dictionary<string, string>();
                        //   if (count > 50) break;
                        count++;

                        for (int col = colStart; col <= colEnd; col++)
                        {

                            //得到单元格信息
                            ExcelRange cell = null;
                            try
                            {
                                cell = worksheet.Cells[row, col];
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine("" + err.Message);
                                Lv.Log.Write("excel 数据提取 出错 row"+row.ToString()+" col"+col.ToString() + err.Message, Lv.Log.MessageType.Error);
                            }
                            if (cell.Value == null)
                                continue;
                            string text = cell.RichText.Text;
                            dictHeadervalue[dictHeader[col]] = text;         //标题，值
                        }

                        //解析当前数据在每个表里的数据，然后把这些表都保存一次
                        //当前字段和值都在这里 dictHeadervalue
                        if (dictHeadervalue.Count < 1)
                        {
                            continue;
                        }
                        if (switchExcelToSqlite(ref dictHeadervalue, path) == true)
                        {
                             SummaryData.Instance.IsRepate = isRepate;
                             if (SummaryData.Instance.ConvertDicToField(dictHeadervalue) == true)
                             {
                                 listSQL.Add(SummaryData.Instance.GetSQL(dictHeadervalue["日期"], dictHeadervalue["keymd5"]));
                             }
                             else
                             {
                                 continue;
                             }
                             
                        }
                        else
                        {
                            //这里表示从Excel里转换数据失败，属于严重错误，想办法处理
                            Console.WriteLine("这里表示从Excel里转换数据失败，属于严重错误，想办法处理");
                            Lv.Log.Write("数据导入失败  Excel里转换数据失败" , Lv.Log.MessageType.Error);
                            MessageBox.Show("数据导入失败，");
                            return;
                        }
                        if (row % 500 == 0)
                        {
                            sqlite.SeaveData(listSQL);
                            listSQL.Clear();
                            SetmyLoadingDataTimesEvent(count, rowEnd, package.Workbook.Worksheets[1].Name);
                        }
                    }
                    sqlite.SeaveData(listSQL);
                    listSQL.Clear();
                    Console.WriteLine("总处理行数:" + count);
                    Lv.Log.Write("总处理行数: " + count, Lv.Log.MessageType.Info);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("加载excel出错了　" + err.Message);
                Lv.Log.Write("加载excel出错了　 " + err.Message, Lv.Log.MessageType.Error);
            }

        }

        private bool IsAddHeaderValuedic(ref Dictionary<string, string> dictHeadervalue, string key,string keyindex)
        {

            if (dictHeadervalue.ContainsKey(key) == true)
            {
                if (dictHeadervalue.ContainsKey(keyindex) == false)
                {
                    dictHeadervalue.Add(keyindex, dictHeadervalue[key]);
                }
                else
                {
                    dictHeadervalue[keyindex] = dictHeadervalue[key];
                }
            }

            return true;
        }
        private bool UserExcelToSqlite(ref Dictionary<string, string> dictHeaderValue, ExcelType pathtype)
        {
            try
            {
                //过滤一些字段不对应的字段
                Dictionary<string, string> dictitle = new Dictionary<string, string>();
                dictitle.Add("推广账户", "账户");
                dictitle.Add("展示次数", "展现量");
                dictitle.Add("点击次数", "点击量");
                dictitle.Add("展现", "展现量");
                dictitle.Add("点击", "点击量");
                dictitle.Add("平均每次点击费用", "平均点击价格");
                dictitle.Add("总费用", "消费");
                dictitle.Add("推广计划", "广告系列");
                dictitle.Add("推广组", "广告分组");
                dictitle.Add("推广单元", "广告分组");
                dictitle.Add("关键词", "广告关键词");




                foreach (var item in dictitle)
                {
                    if (IsAddHeaderValuedic(ref dictHeaderValue, item.Key, item.Value) == false)
                    {
                        Console.WriteLine(item.Key + "    " + item.Value + " 索引出错");
                        Lv.Log.Write(item.Key + "    " + item.Value + " 索引出错", Lv.Log.MessageType.Error);
                    }
                }

                string filename = Path.GetFileName(pathtype.Path).Replace(".xlsx", "").Replace("端", "").Replace("账户", "");
                //filename = filename.Replace(".xls", "");
                //string[] str = Regex.Split(filename, "数据-");
                //if (str.Length < 1)
               // {
                 //   Console.WriteLine("文件名没有按标准的来，请把　数据-　放在中间,例 '360账户数据-PC端.xlsx' ");
                //    Lv.Log.Write("文件名没有按标准的来，请把　数据-　放在中间,例 '360账户数据-PC端.xlsx'  " , Lv.Log.MessageType.Error);
               // }
                
                string device = "";
                string source ="";
                if (filename.Contains( "百度") == true )
                {
                    
                    if (filename.Contains( "PC") == true)
                    {
                        source = "Baidu";
                        device = "PC";
                    }
                    else if (filename.Contains( "MOB") == true)
                    {
                        source = "MBaidu";
                        device = "MOB";
                    }
                    else 
                    {
                        Console.WriteLine("出现了其他的设置类型");    
                        Lv.Log.Write("出现了其他的设置类型 ", Lv.Log.MessageType.Error);
                    }
                    
                }else if (filename.Contains("360") == true)
                {
                    if (filename.Contains("PC") == true)
                    {
                        source = "360";
                        device = "PC";
                    }
                    else if (filename.Contains("MOB") == true)
                    {
                        source = "M360";
                        device = "MOB";
                    }
                    else 
                    {
                        Console.WriteLine("出现了其他的设置类型");
                        Lv.Log.Write("出现了其他的设置类型" , Lv.Log.MessageType.Error);
                    }
                }
            
                //这个最好是有人工确定，不然没法弄
                dictHeaderValue["设备类型"] = device;
                dictHeaderValue["广告来源"] = source;
                string timetmp = "";
                if (dictHeaderValue["广告系列"] == "" && dictHeaderValue["广告分组"] == "" && dictHeaderValue["广告关键词"] == "")
                {
                    timetmp = Lv.Crypt.md5(DateTime.Now.ToString() + Guid.NewGuid(), 16);
                }
                string md5key = Lv.Crypt.md5(dictHeaderValue["日期"] + dictHeaderValue["广告系列"] + dictHeaderValue["广告分组"] + dictHeaderValue["广告关键词"]+timetmp, 16);
                dictHeaderValue["keymd5"]= md5key;

                dictHeaderValue["星期"] = Common.GetWeekDate(dictHeaderValue["日期"]);
                DateTime date = new DateTime();
                DateTime.TryParse(dictHeaderValue["日期"], out date);
                dictHeaderValue["日期"] = date.ToShortDateString();
                
                //点击率
                int clicknum = 0;
                int shownum = 0;
                int.TryParse(dictHeaderValue["点击量"], out clicknum);
                int.TryParse(dictHeaderValue["展现量"], out shownum);
                if (shownum == 0 || clicknum == 0)
                {
                    dictHeaderValue["点击率"] = "0%";
                }
                else
                {
                    dictHeaderValue["点击率"] = (((double)clicknum / (double)shownum) * 100).ToString() + "%";
                }
                //平均点击价格
                double price = 0;
                double.TryParse(dictHeaderValue["消费"], out price);
                if (clicknum == 0 || price == 0)
                {
                    dictHeaderValue["平均点击价格"] = "0";
                }
                else
                {
                    dictHeaderValue["平均点击价格"] = (price / (double)clicknum).ToString();
                }
            }
            catch (Exception err)
            {
                return false;
                Console.WriteLine("user table Excel switch sqlite 出错 "+err.Message);
                Lv.Log.Write("user table Excel switch sqlite 出错 " + err.Message, Lv.Log.MessageType.Error);
            }
            return true;
            
        }

        private bool switchExcelToSqlite(ref Dictionary<string, string> dictHeaderValue, ExcelType pathtype)
        {
            bool result = false;

            //string tmpstrkey = item.Chname.Replace(" ", "").Replace("（", "(").Replace("）", ")");
            //把字段里这些的都过滤掉 要从ＥＸＣＥＬ表里过滤掉

            switch (pathtype.Type)
            {
                case ExcelTypeEm.User:
                    result = UserExcelToSqlite(ref dictHeaderValue, pathtype);
                    break;
                case ExcelTypeEm.WD:
                    result = WDExcelToSqlite(ref dictHeaderValue, pathtype);
                    break;
                case ExcelTypeEm.Yuan:
                    result = yuanExcelToSqlite(ref dictHeaderValue, pathtype);
                    break;
                default:
                    break;
            }
             

            return true;
        }

        private bool yuanExcelToSqlite(ref Dictionary<string, string> dictHeaderValue, ExcelType pathtype)
        {
            bool result = false;
            //过滤一些字段不对应的字段
            Dictionary<string, string> dictitle = new Dictionary<string, string>();

            if (pathtype.Path.Contains("注册用户"))
            {
                dictitle.Add("首注册时间", "日期");
                dictHeaderValue.Add("注册成功", "1");
            }
            if (pathtype.Path.Contains("付费用户"))
            {
                dictitle.Add("首付费时间", "日期");
                dictHeaderValue.Add("首参", "1");
            }

            foreach (var item in dictitle)
            {
                if (IsAddHeaderValuedic(ref dictHeaderValue, item.Key, item.Value) == false)
                {
                    Console.WriteLine(item.Key + "    " + item.Value + " 索引出错");
                    Lv.Log.Write(item.Key + "    " + item.Value + " 索引出错", Lv.Log.MessageType.Error);
                }
            }
            //-----解析url----
            string series = "";
            string group = "";
            string key = "";
            try
            {
                string url = System.Web.HttpUtility.UrlDecode(dictHeaderValue["来源"]);


                //http://jyzd.sina.com/?f360sem=通用-行情-0528&股票查询&股票查询&utm_source=360&utm_medium=search_cpc&utm_AdConten
                string patten = @"=(?<series>[\s\S]*?)&(?<group>[\s\S]*?)&(?<key>[\s\S]*?)&utm|=(?<series>[\s\S]*?)&(?<group>[\s\S]*?)&utm|=(?<series>[\s\S]*?)&utme";
               
                Regex regex = new Regex(patten, RegexOptions.IgnoreCase);
                Match mc = regex.Match(url);
                series = mc.Groups["series"].ToString();
                group = mc.Groups["group"].ToString();
                key = mc.Groups["key"].ToString();
                if (key == "")
                {
                    System.IO.File.AppendAllText("error_url.txt", url);
                }
                if (key.Contains("utm_source=")== true || key.Contains("utm_medium"))
                {
                    key = "";
                }
                if (group.Contains("utm_source=") == true || group.Contains("utm_medium"))
                {
                    group = "";
                }
                if (series.Contains("utm_source=") == true || series.Contains("utm_medium"))
                {
                    series = "";
                }
                //这个地方存在　http://jyzd.sina.com/?f360sem=HB 通用词-软件&软件-%　这种情况，目前没有回复要怎么处理
                //dictHeaderValue["日期"]
                if (series.Contains("=") == true)
                {
                    string[] tmp = series.Split('=');
                    if (tmp.Length > 1 && tmp[1] != "")
                    {
                        series = tmp[1];
                    }
                    else
                    {
                        series = "-";
                    }
                }
                
            }
            catch (Exception err)
            {
                //当资源数据没有，为空，那么不写里面的内容
                //throw err;
            }
            if (series == "" && group == "" && key == "")
            {
                series = "其他";
            }
            dictHeaderValue.Add("广告系列", series);
            dictHeaderValue.Add("广告分组", group);
            dictHeaderValue.Add("广告关键词", key);


            string timetmp = "";
            if (dictHeaderValue["广告系列"] == "" && dictHeaderValue["广告分组"] == "" && dictHeaderValue["广告关键词"] == "")
            {

                timetmp = Lv.Crypt.md5(DateTime.Now.ToString() + Guid.NewGuid(), 16);
            }
            string md5key = Lv.Crypt.md5(dictHeaderValue["日期"] + dictHeaderValue["广告系列"] + dictHeaderValue["广告分组"] + dictHeaderValue["广告关键词"] + timetmp, 16);
            dictHeaderValue.Add("keymd5", md5key);
           
            //----end---

            dictHeaderValue.Add("星期", Common.GetWeekDate(dictHeaderValue["日期"]));
            DateTime date = new DateTime();
            DateTime.TryParse(dictHeaderValue["日期"], out date);
            dictHeaderValue["日期"] = date.ToShortDateString();

            return result;
        }

        private bool WDExcelToSqlite(ref Dictionary<string, string> dictHeaderValue, ExcelType pathtype)
        {
            bool result = false;
            //过滤一些字段不对应的字段
            Dictionary<string, string> dictitle = new Dictionary<string, string>();
            dictitle.Add("支点登陆完成数", "登陆");
            dictitle.Add("支点注册完成数", "注册");
            dictitle.Add("Mobile端全部APP下载完成数", "APP下载");
            
            foreach (var item in dictitle)
            {
                if (IsAddHeaderValuedic(ref dictHeaderValue, item.Key, item.Value) == false)
                {
                    Console.WriteLine(item.Key + "    " + item.Value + " 索引出错");
                    Lv.Log.Write(item.Key + "    " + item.Value + " 索引出错", Lv.Log.MessageType.Error);
                }
            }
            DateTime date = new DateTime();
            DateTime.TryParse(dictHeaderValue["日期"], out date);
            dictHeaderValue["日期"] = date.ToShortDateString();
            if (dictHeaderValue.ContainsKey("星期") == true)
            {
                dictHeaderValue.Add("星期", Common.GetWeekDate(dictHeaderValue["日期"]));
            }
            else 
            {
                dictHeaderValue["星期"] = Common.GetWeekDate(dictHeaderValue["日期"]);
            }
            //分析字段 
            string timetmp = "";
            if (dictHeaderValue["广告系列"].StartsWith("%") == true)
            {
                dictHeaderValue["广告系列"] = System.Web.HttpUtility.UrlDecode(dictHeaderValue["广告系列"]);
            }
            if (dictHeaderValue["广告分组"].StartsWith("%") == true)
            {
                dictHeaderValue["广告分组"] = System.Web.HttpUtility.UrlDecode(dictHeaderValue["广告分组"]);
            }
            if (dictHeaderValue["广告关键词"].StartsWith("%") == true)
            {
                dictHeaderValue["广告关键词"] = System.Web.HttpUtility.UrlDecode(dictHeaderValue["广告关键词"]);
            }
            if (dictHeaderValue["广告系列"] == "" && dictHeaderValue["广告分组"] == "" && dictHeaderValue["广告关键词"] == "")
            {
                timetmp = Lv.Crypt.md5(DateTime.Now.ToString() + Guid.NewGuid(), 16);
            }
            string md5key = Lv.Crypt.md5(dictHeaderValue["日期"] + dictHeaderValue["广告系列"] + dictHeaderValue["广告分组"] + dictHeaderValue["广告关键词"] + timetmp, 16);
            dictHeaderValue.Add("keymd5", md5key);
            if (dictHeaderValue.ContainsKey("设备类型"))
            {
                if (dictHeaderValue["设备类型"].ToLower() == "pad" || dictHeaderValue["设备类型"].ToLower() == "phone" )
                {
                    dictHeaderValue["设备类型"] = "MOB";
                    if (dictHeaderValue["广告来源"] == "Baidu")
                    {
                        dictHeaderValue["广告来源"] = "MBaidu";
                    }
                    if (dictHeaderValue["广告来源"] == "360")
                    {
                        dictHeaderValue["广告来源"] = "M360";
                    }
                }
                               
            }
            if (dictHeaderValue.ContainsKey("广告来源") == true)
            {
                if (dictHeaderValue["广告来源"] == "Search" || dictHeaderValue["广告来源"] == "-")
                {
                    if (dictHeaderValue["设备类型"] == "MOB")
                    {
                        dictHeaderValue["广告来源"] = "MBaidu";
                    }
                    if (dictHeaderValue["设备类型"] == "PC")
                    {
                        dictHeaderValue["广告来源"] = "Baidu";
                    }
                }
            }
            if (dictHeaderValue["设备类型"] == "PC")
            {
                if(dictHeaderValue["广告来源"].StartsWith("MBaidu") == true)
                { 
                    dictHeaderValue["广告来源"] = "Baidu";
                }
                else if (dictHeaderValue["广告来源"].StartsWith("M360") == true)
                {
                    dictHeaderValue["广告来源"] = "360";
                }
                else
                {
                    //dictHeaderValue["广告来源"] = "Baidu";//其他类型以后找到后在说
                }
               
            }
            if (dictHeaderValue["设备类型"] == "MOB" )
            {
                if (dictHeaderValue["广告来源"].StartsWith("Baidu") == true)
                {
                    dictHeaderValue["广告来源"] = "MBaidu";
                }
                else if (dictHeaderValue["广告来源"].StartsWith("360") == true)
                {
                    dictHeaderValue["广告来源"] = "M360";
                }
                else
                {
                    //dictHeaderValue["广告来源"] = "Baidu";//其他类型以后找到后在说
                }
            }
            
            return result;
        }

        /// <summary>
        /// 字符串联接ＥＸＣＥＬ2007
        /// </summary>
        /// <param name="path"></param>
        private void OpenConExcel(string path)
        {

            string strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES\"";
            OleDbConnection myConn = new OleDbConnection(strCon);
            string strCom = " SELECT * FROM [Sheet1$]";
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
            DataSet myDataSet = new DataSet();
            MessageBox.Show("OK");
            myCommand.Fill(myDataSet, "[Sheet1$]");
            myConn.Close();
            // return myDataSet;

        }

        /// <summary>
        /// 读取小文件的excel2007文件可行，大文件获取workbook的时候就出错了
        /// 读取大一点的csv格式也会出错
        /// </summary>
        /// <param name="path"></param>
        private void NPOIReadExcel2007(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            //获取Excel2007工作簿
            XSSFWorkbook book = new XSSFWorkbook(fs);

            //获取Excel2007工作表
            ISheet sheet = book.GetSheetAt(0);

            //获取Excel总行

            int rowcount = sheet.LastRowNum;

            MessageBox.Show("总行数:" + rowcount);
        }

        /// <summary>
        /// *生成导出EXCEl文件对话框
        /// *同时将导出文件类型定义为Excel 
        /// *保存时只需指明对应的文件名即可
        /// </summary>
        /// <param name="path"></param>
        private void NPOICreateExcel2007Table(string path)
        {
            SaveFileDialog sdfexport = new SaveFileDialog();
            sdfexport.Filter = "Excel文件|*.xlsx";
            if (sdfexport.ShowDialog() == DialogResult.No)
            {
                return;
            }
            string filename = sdfexport.FileName;

            //创建Excel2007工作簿
            IWorkbook book = new XSSFWorkbook();

            //创建Excel2007工作表
            ISheet sheet = book.CreateSheet("standard_template");

            //创建Excel行
            IRow row = sheet.CreateRow(0);

            //给单元格赋值
            row.CreateCell(0).SetCellValue("序号");
            row.CreateCell(1).SetCellValue("大区(区域)");
            row.CreateCell(2).SetCellValue("省(简)");
            row.CreateCell(3).SetCellValue("说明");

            /*
             * 将Excel文件写入相应的Excel文件中
             */
            FileStream fs = File.Create(filename);
            book.Write(fs);
            fs.Close();
        }
    }
}




/*string dt = date.DayOfWeek.ToString();
            switch (dt)
            {
                case "Monday":
                    week = "星期一";
                    break;
                case "Tuesday":
                    week = "星期二";
                    break;
                case "Wednesday":
                    week = "星期三";
                    break;
                case "Thursday":
                    week = "星期四";
                    break;
                case "Friday":
                    week = "星期五";
                    break;
                case "Saturday":
                    week = "星期六";
                    break;
                case "Sunday":
                    week = "星期日";
                    break;
            } */