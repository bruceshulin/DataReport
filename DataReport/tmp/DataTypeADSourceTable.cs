using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeADSourceTable:DataTypeInterface       
    {
        string adsourceid;

        public string Adsourceid
        {
            get { return adsourceid; }
            set { Modifly(); adsourceid = value; }
        }
        string adsource;

        public string Adsource
        {
            get { return adsource; }
            set { Modifly(); adsource = value; }
        }
        public TableName getTableName()
        {
            return TableName.adsourcetable;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
