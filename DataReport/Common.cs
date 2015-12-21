using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    static class  Common
    {
        public static string GetCurrentSummaryDataBaseName(string year)
        {
            if (year == "")
            {
                return null;
            }
            return  Lv.Crypt.md5(year, 16).Substring(1, 9);
        }
        /// <summary>
        /// 当回当前星期的md5值
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetCurrentSummaryTableName(string day)
        {
            string tablekey = GetDayTableKey(day);
            //这个地方如果没有创建表那么就没有当前key 只有把日期传过来，在查一下,没有的话需要创建表
            return "Summary_" + tablekey;
        }

        public static string GetDayTableKey(string day)
        {
            string tablekey = "";
            string week = "";
            week = GetWeekDate(day);
            tablekey = GetWeekMd5(week); 
            return tablekey;
        }
        public static string GetWeekSummaryTableName(string week)
        {
            string tablekey = "";
            tablekey = GetWeekMd5(week);
            return "Summary_" + tablekey;
        }

        private static string GetWeekMd5(string week)
        {
            return Lv.Crypt.md5(week, 16).Substring(1, 9); 
        }

        /// <summary>
        /// 返回当前某个星期的前后日期
        /// </summary>
        /// <param name="CurrentDate"></param>
        /// <returns></returns>
        public static string GetWeekDate(string CurrentDate)
        {
            //星期　在某日期范围
            string DateStr = "";
            try
            {
                DateTime date = new DateTime();
                DateTime dateoption = new DateTime();
                DateTime.TryParse(CurrentDate, out date);
                DateTime.TryParse(CurrentDate, out dateoption);
                int week = Convert.ToInt32(date.DayOfWeek.ToString("d"));
                int tmp = 7 - week;
                if (tmp == 7)
                {
                    //说明当前是星期天
                    string datetmp = dateoption.AddDays(-6).ToShortDateString();
                    datetmp +=  "-" + dateoption.ToShortDateString();
                    DateStr = datetmp;
                }
                else
                {
                    /*   这个是从星期日到星期六的结果 2015-10-11 - 2015-10-17
                    int tmpday = 0 - Convert.ToInt16(date.DayOfWeek);
                    DateStr = dateoption.AddDays((Double)tmpday).ToShortDateString();
                    dateoption = date;
                    tmpday = 6 - Convert.ToInt16(date.DayOfWeek);
                    DateStr += "-" + dateoption.AddDays(Convert.ToDouble(tmpday)).ToShortDateString();
                    */
                    /* 这个是从星期1　－　星期天*/
                    int tmpday = 1 - Convert.ToInt16(date.DayOfWeek);
                    DateStr = dateoption.AddDays((Double)tmpday).ToShortDateString();
                    dateoption = date;
                    tmpday = 7 - Convert.ToInt16(date.DayOfWeek);
                    DateStr += "-" + dateoption.AddDays(Convert.ToDouble(tmpday)).ToShortDateString();

                }
            }
            catch
            {
                return "";
            }
            return DateStr;
        }

        internal static string GetYear(string day)
        {
            if (day == "")
            {
                return DateTime.Now.Year.ToString();
            }
            DateTime date = new DateTime();
            DateTime.TryParse(day, out date);
            return date.Year.ToString();
        }

        internal static string Combination(string keymd5)
        {
            return "Summary_" + keymd5;
        }

        internal static bool CheckAccessField(Dictionary<int, string> dictitle)
        {
            List<string> ListSummaryField = new List<string>();
            ListSummaryField.Add("日期");
            ListSummaryField.Add("广告系列");
            ListSummaryField.Add("广告分组");
            ListSummaryField.Add("广告关键词");
            foreach (var itemfield in ListSummaryField)
            {
                bool isfield = false;
                foreach (var itemtitle in dictitle)
                {
                    if (itemtitle.Value == itemfield)
                    {
                        isfield = true;
                    }
                }
                if (isfield == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
