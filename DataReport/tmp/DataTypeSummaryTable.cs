using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeSummaryTable:DataTypeInterface
    {
        string summaryid;

        public string Summaryid
        {
            get { return summaryid; }
            set { Modifly(); summaryid = value; }
        }
        string dateid;

        public string Dateid
        {
            get { return dateid; }
            set { Modifly(); dateid = value; }
        }
       string weekdate;

       public string Weekdate
       {
           get { return weekdate; }
           set { Modifly(); weekdate = value; }
       }
       string adsourceid;

       public string Adsourceid
       {
           get { return adsourceid; }
           set { Modifly(); adsourceid = value; }
       }
       string devicetypeid;

       public string Devicetypeid
       {
           get { return devicetypeid; }
           set { Modifly(); devicetypeid = value; }
       }
       string adseriesid;

       public string Adseriesid
       {
           get { return adseriesid; }
           set { Modifly(); adseriesid = value; }
       }
       string adgroupid;

       public string Adgroupid
       {
           get { return adgroupid; }
           set { Modifly(); adgroupid = value; }
       }
       string adkeyid;

       public string Adkeyid
       {
           get { return adkeyid; }
           set { Modifly(); adkeyid = value; }
       }
       string two;

       public string Two
       {
           get { return two; }
           set { Modifly(); two = value; }
       }
       string three;

       public string Three
       {
           get { return three; }
           set { Modifly(); three = value; }
       }
       string shownum;

       public string Shownum
       {
           get { return shownum; }
           set { Modifly(); shownum = value; }
       }
       string clicknum;

       public string Clicknum
       {
           get { return clicknum; }
           set { Modifly(); clicknum = value; }
       }
       string consumption;

       public string Consumption
       {
           get { return consumption; }
           set { Modifly(); consumption = value; }
       }
       string clickratenum;

       public string Clickratenum
       {
           get { return clickratenum; }
           set { Modifly(); clickratenum = value; }
       }
       string avgclickprice;

       public string Avgclickprice
       {
           get { return avgclickprice; }
           set { Modifly(); avgclickprice = value; }
       }
       string landingnum;

       public string Landingnum
       {
           get { return landingnum; }
           set { Modifly(); landingnum = value; }
       }
       string registrationnum;

       public string Registrationnum
       {
           get { return registrationnum; }
           set { Modifly(); registrationnum = value; }
       }
       string appnum;

       public string Appnum
       {
           get { return appnum; }
           set { Modifly(); appnum = value; }
       }
       string regsuccessdate;

       public string Regsuccessdate
       {
           get { return regsuccessdate; }
           set { Modifly(); regsuccessdate = value; }
       }
       string firstpaytime;

       public string Firstpaytime
       {
           get { return firstpaytime; }
           set { Modifly(); firstpaytime = value; }
       }
        public TableName getTableName()
        {
            return TableName.summarytable;
        }

        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
