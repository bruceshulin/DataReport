using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeRawDataTable:DataTypeInterface
    {
        string masterkeyid;

        public string Masterkeyid
        {
            get { return masterkeyid; }
            set { Modifly(); masterkeyid = value; }
        }
       string uid;

       public string Uid
       {
           get { return uid; }
           set { Modifly(); uid = value; }
       }
       string firstpaymenttime;

       public string Firstpaymenttime
       {
           get { return firstpaymenttime; }
           set { Modifly(); firstpaymenttime = value; }
       }
       string urlsource;

       public string Urlsource
       {
           get { return urlsource; }
           set { Modifly(); urlsource = value; }
       }
       string channel;

       public string Channel
       {
           get { return channel; }
           set { Modifly(); channel = value; }
       }
       string firstregistrationtime;

       public string Firstregistrationtime
       {
           get { return firstregistrationtime; }
           set { Modifly(); firstregistrationtime = value; }
       }
        public TableName getTableName()
        {
            return TableName.rawdatatable;
        }

        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
