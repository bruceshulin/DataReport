using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public enum ExcelTypeEm
    {
        User,
        WD,
        Yuan,
        Summary,
        Access
    }
    class ExcelType
    {
        string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        ExcelTypeEm type;

        public ExcelTypeEm Type
        {
            get { return type; }
            set { type = value; }
        }

        internal ExcelTypeEm getExcelTypeEM()
        {
            if (path.Contains(@"WD数据\") == true)
            {
                return ExcelTypeEm.WD;
            }
            else if (path.Contains(@"账户数据\") == true)
            {
                return ExcelTypeEm.User;
            }
            else if (path.Contains(@"转化数据\") == true)
            {
                return ExcelTypeEm.Yuan;
            }
            return ExcelTypeEm.User;
        }
    }
}
