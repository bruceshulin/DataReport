using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeADKeyTable:DataTypeInterface      
    {
        string adkeyid;

        public string Adkeyid
        {
            get { return adkeyid; }
            set { Modifly(); adkeyid = value; }
        }
        string adkey;

        public string Adkey
        {
            get { return adkey; }
            set { Modifly(); adkey = value; }
        }
        public TableName getTableName()
        {
            return TableName.adkeytable;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
