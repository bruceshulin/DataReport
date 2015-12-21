using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public enum DataBaseType
    {
        sqlite,
        sqlServe
    }
    class DataBaseFactory
    {
        public DataBaseInterface Process(DataBaseType database)
        {
            switch (database)
            {
                case DataBaseType.sqlite:
                    return new DataBaseSqlite();
                    break;
                case DataBaseType.sqlServe:
                    //return new DataBaseSqlServe();
                    return null;
                    break;
                default:
                    return null;
                    break;
            }
        }
    }
}
