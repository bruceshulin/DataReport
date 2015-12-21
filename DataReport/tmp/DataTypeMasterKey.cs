using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeMasterKey : DataTypeInterface
    {
        string masterkeyid;
        public string Masterkeyid
        {
          get { return masterkeyid; }
          set { Modifly(); masterkeyid = value; }
        }
        string dateid;

        public string Dateid
        {
            get { return dateid; }
            set { Modifly(); dateid = value; }
        }
        string adseriesid;

        public string Adseriesid
        {
            get { return adseriesid; }
            set { Modifly(); adseriesid = value; }
        }
        string adgroup;

        public string Adgroup
        {
            get { return adgroup; }
            set { Modifly(); adgroup = value; }
        }
        string adkey;

        public string Adkey
        {
            get { return adkey; }
            set { Modifly(); adkey = value; }
        }
        public TableName getTableName()
        {
            return TableName.masterkey;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
