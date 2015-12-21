using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeWDDataTable:DataTypeInterface
    {
        string wddataid;

        public string Wddataid
        {
            get { return wddataid; }
            set { Modifly(); wddataid = value; }
        }
        string consumption;

        public string Consumption
        {
            get { return consumption; }
            set { Modifly(); consumption = value; }
        }
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
       string devicetypeid;

       public string Devicetypeid
       {
           get { return devicetypeid; }
           set { Modifly(); devicetypeid = value; }
       }
       string adsourceid;

       public string Adsourceid
       {
           get { return adsourceid; }
           set { Modifly(); adsourceid = value; }
       }
       string homeClickFinish;

       public string HomeClickFinish
       {
           get { return homeClickFinish; }
           set { Modifly(); homeClickFinish = value; }
       }
       string homeLogionFinish;

       public string HomeLogionFinish
       {
           get { return homeLogionFinish; }
           set { Modifly(); homeLogionFinish = value; }
       }
       string homeOpenFinish;

       public string HomeOpenFinish
       {
           get { return homeOpenFinish; }
           set { Modifly(); homeOpenFinish = value; }
       }
       string fulcrumlandingFinish;

       public string FulcrumlandingFinish
       {
           get { return fulcrumlandingFinish; }
           set { Modifly(); fulcrumlandingFinish = value; }
       }
       string fulcrumRegistrationFinish;

       public string FulcrumRegistrationFinish
       {
           get { return fulcrumRegistrationFinish; }
           set { Modifly(); fulcrumRegistrationFinish = value; }
       }
       string fobileAPPDownloadFinish;

       public string FobileAPPDownloadFinish
       {
           get { return fobileAPPDownloadFinish; }
           set { Modifly(); fobileAPPDownloadFinish = value; }
       }
        public TableName getTableName()
        {
            return TableName.wddatatable;
        }

        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
