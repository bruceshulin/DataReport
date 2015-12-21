using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeDateKey:DataTypeInterface
    {
        string dateid;

        public string Dateid
        {
            get { return dateid; }
            set { Modifly(); dateid = value; }
        }

        string date;

        public string Date
        {
            get { return date; }
            set { Modifly(); date = value; }
        }
        public TableName getTableName()
        {
            return TableName.datekey;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
