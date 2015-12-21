using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataReport
{
    class Summary
    {
        DataBaseSqlite sqlite = DataBaseSqlite.getInstance();

        public Summary()
        {
            //先把其他表里的数据汇总到新的表里
                //创建新表
                sqlite.Type = ExcelTypeEm.Summary;
                //先把user 表全部导到汇总表里面去
                //在把wddata表全部导到汇总表里面去
            //在把汇总表导出成ＥＸＣＥＬ
        }
        public void BuilderStart()
        {
            //sqlite.BuilderSummary();
        }


        internal void OutPut(string path ,string select)
        {
           
        }


        internal void Select(string sqlwhere)
        {
            sqlite.SelectData(sqlwhere);
        }
    }
}
