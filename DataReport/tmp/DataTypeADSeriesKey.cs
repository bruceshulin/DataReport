using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeADSeriesKey:DataTypeInterface
    {
        string adseriesid;

        public string Adseriesid
        {
            get { return adseriesid; }
            set { Modifly(); adseriesid = value; }
        }
        string adseries;

        public string Adseries
        {
            get { return adseries; }
            set { Modifly(); adseries = value; }
        }
        public TableName getTableName()
        {
            return TableName.adserieskey;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
