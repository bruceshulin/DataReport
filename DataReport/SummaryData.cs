using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class SummaryData
    {
        static SummaryData instance = null;

        internal static SummaryData Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new SummaryData();
                }
                return instance; 
            }
        }
        //当前记录的汇总地
        List<Field> ListSummaryDataField = new List<Field>();

        List<string> ListSummaryField = new List<string>();
        public SummaryData()
        {
            ListSummaryField.Clear();
            ListSummaryField.Add("keymd5");
            ListSummaryField.Add("日期");
            ListSummaryField.Add("星期");
            ListSummaryField.Add("广告来源");
            ListSummaryField.Add("设备类型");
            ListSummaryField.Add("广告系列");
            ListSummaryField.Add("广告分组");
            ListSummaryField.Add("广告关键词");
            ListSummaryField.Add("展现量");
            ListSummaryField.Add("点击量");
            ListSummaryField.Add("消费");
            ListSummaryField.Add("点击率");
            ListSummaryField.Add("平均点击价格");
            ListSummaryField.Add("登陆");
            ListSummaryField.Add("注册");
            ListSummaryField.Add("APP下载");
            ListSummaryField.Add("注册成功");
            ListSummaryField.Add("首参");
        }

        bool isRepate = false;

        public bool IsRepate
        {
            get { return isRepate; }
            set { isRepate = value; }
        }
        List<string> listkeymd5 = new List<string>();

        public List<string> Listkeymd5
        {
            get { return listkeymd5; }
            set { listkeymd5 = value; }
        }
        internal bool ConvertDicToField(Dictionary<string, string> dictHeaderValue)
        { 
            bool result = false;
           // if (dictHeaderValue.ContainsKey("keymd5") == true)
           // {
           //     if (listkeymd5.Contains(dictHeaderValue["keymd5"]) == true)
           //     {
           //         return false;
           //     }
           //     else
           //     {
           //         listkeymd5.Add(dictHeaderValue["keymd5"]);
           //     }
           // }
           // else
           // {
           //     return false;
           // }

           
            ListSummaryDataField.Clear();
            try
            {
                foreach (string item in ListSummaryField)
                {
                    if (dictHeaderValue.ContainsKey(item) == true)
                    {
                        Field field = new Field();
                        field.Chname = item;
                        field.Value = dictHeaderValue[item];
                        ListSummaryDataField.Add(field);
                    }
                }
                result = true;
            }
            catch (Exception err)
            {
                throw err;
            }
            return result;

        }

        internal string GetSqlInsert()
        {
            string sql = "";
            string day = "";
            StringBuilder sbfield = new StringBuilder();
            StringBuilder sbvalue = new StringBuilder();
            foreach (var item in ListSummaryDataField)
            {
                
                if (item.Value == "" || item.Value == null)
                {
                    continue;
                }
                if(item.Chname == "日期")
                {
                    day = item.Value;　
                }
                sbfield.Append("'" + item.Chname + "',");
                sbvalue.Append("'" + item.Value + "',");
            }
            if (day == "" || day == null)
            {
	        	 return "";
            }
            sql = string.Format("insert into  " + Common.GetCurrentSummaryTableName(day) + "( " + sbfield.ToString().Trim(',') + ")values(" + sbvalue.ToString().Trim(',') + ");");

            return sql;
        }

        internal string GetSqlUpdate(string id)
        {
            string day = "";
            StringBuilder sqlUpdate = new StringBuilder();
            foreach (var item in ListSummaryDataField)
            {
                if (item.Value == "" || item.Value == null)
                {
                    continue;
                }
                if (item.Chname == "日期")
                {
                    day = item.Value;
                }
                sqlUpdate.Append("[" + item.Chname + "] = '" + item.Value + "', ");
            }

            if (day == "" || day == null)
            {
                return "";
            }
            string updatesql = sqlUpdate.ToString().Trim().Trim(',');
            sqlUpdate = new StringBuilder();
            sqlUpdate.Append("update " + Common.GetCurrentSummaryTableName(day) + " set " + updatesql + " where id = '" + id + "'");

            return sqlUpdate.ToString();
        }

        internal string GetSQL(string day,string keymd5)
        {
            DataBaseSqlite.getInstance().IsSelectRepate = isRepate;
            string id = DataBaseSqlite.getInstance().IsExist(day, keymd5); 
            return GetSqlInsert();

            if (id == null || id == "")
            {
               
            }
            else
            {
               // return GetSqlUpdate(id);
            }
        }
    }
}
