using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeADGroupKey:DataTypeInterface
    {
        string adgroupid;

        public string Adgroupid
        {
            get { return adgroupid; }
            set { Modifly(); adgroupid = value; }
        }
        string adgroup;

        public string Adgroup
        {
            get { return adgroup; }
            set { Modifly(); adgroup = value; }
        }
        public TableName getTableName()
        {
            return TableName.adgroupkey;
        }

        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
