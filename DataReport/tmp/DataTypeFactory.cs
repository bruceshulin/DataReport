using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public enum TableName
    {
        masterkey,
        datekey,
        adserieskey,
        adgroupkey,
        adkeytable,
        devicetypetable,
        adsourcetable,
        rawdatatable,
        wddatatable,
        summarytable,
    }
   public  class DataTypeFactory
    {
       /// <summary>
       /// 返回相应表的插入语句
       /// </summary>
       /// <param name="data"></param>
       /// <returns></returns>
       public string GetTableInsertSql(DataPrototype data)
       {
           StringBuilder strSql = new StringBuilder();
            List<DataTypeInterface> interdata = new List<DataTypeInterface>();
            interdata.Add(data.DataTypeADGroupKey);
            interdata.Add(data.DataTypeADKeyTable);
            interdata.Add(data.DataTypeADSeriesKey);
            interdata.Add(data.DataTypeADSourceTable);
            interdata.Add(data.DataTypeDateKey);
            interdata.Add(data.DataTypeDeviceTypeTable);
            interdata.Add(data.DataTypeMasterKey);
            interdata.Add(data.DataTypeRawDataTable);
            interdata.Add(data.DataTypeSummaryTable);
            interdata.Add(data.DataTypeWDDataTable);
            foreach (var item in interdata)
            {
                TableName table = item.getTableName();
                if (item.Modifly() == false)
                {
                    continue;
                }
                switch (table)
                {
                    case TableName.masterkey:
                        //查找　4　个　ＩＤ　后才能保存
                        strSql.AppendFormat("insert into masterkey (`dateid`,`adseriesid`,`adgroup`,`adkey`)values('{0}','{1}','{2}','{3}');", data.DataTypeMasterKey.Dateid, data.DataTypeMasterKey.Adseriesid, data.DataTypeMasterKey.Adgroup, data.DataTypeMasterKey.Adkey);
                        break;
                    case TableName.adgroupkey:
                        //这里如果不执行的话后面的就没有ＩＤ了
                        strSql.AppendFormat("insert into adgroupkey (`adgroup`)values('{0}');", data.DataTypeADGroupKey.Adgroup);
                        break;
                    case TableName.adkeytable:
                        strSql.AppendFormat("insert into adkeytable (`adkey`)values('{0}');", data.DataTypeADKeyTable.Adkey);
                        break;
                    case TableName.adserieskey:
                        strSql.AppendFormat("insert into adserieskey (`adseries`)values('{0}');", data.DataTypeADSeriesKey.Adseries);
                        break;
                    case TableName.adsourcetable:
                        strSql.AppendFormat(" insert into adsourcetable (`adsource`)values('{0}');;", data.DataTypeADSourceTable.Adsource);
                        break;
                    case TableName.datekey:
                        strSql.AppendFormat("insert into datekey (`date`)values('{0}');", data.DataTypeDateKey.Date);
                        break;
                    case TableName.devicetypetable:
                        strSql.AppendFormat("insert into devicetypetable (`devicetype`)values('{0}');", data.DataTypeDeviceTypeTable.Devicetype);
                        break;
                    case TableName.rawdatatable:
                        strSql.AppendFormat("insert into rawdatatable (`masterkeyid`,`uid`,`firstpaymenttime`,`urlsource`,`channel`,`firstregistrationtime`)values('{0}','{1}','{2}','{3}','{4}','{5}');", data.DataTypeRawDataTable.Masterkeyid, data.DataTypeRawDataTable.Uid, data.DataTypeRawDataTable.Firstpaymenttime, data.DataTypeRawDataTable.Urlsource, data.DataTypeRawDataTable.Channel, data.DataTypeRawDataTable.Firstregistrationtime);
                        break;
                    case TableName.summarytable:
                        strSql.AppendFormat("insert into summarytable (`dateid`,`Weekdate`,`adsourceid`,`devicetypeid`,`adseriesid`,`adgroupid`,`adkeyid`,`two`,`three`,`shownum`,`clicknum`,`clicknum`,`Consumption`,`clickratenum`,`avgclickprice`,`landingnum`,`registrationnum`,`appnum`,`regsuccessdate`,`firstpaytime`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}');", data.DataTypeSummaryTable.Dateid, data.DataTypeSummaryTable.Weekdate, data.DataTypeSummaryTable.Adsourceid, data.DataTypeSummaryTable.Devicetypeid, data.DataTypeSummaryTable.Adseriesid, data.DataTypeSummaryTable.Adgroupid, data.DataTypeSummaryTable.Adkeyid, data.DataTypeSummaryTable.Two, data.DataTypeSummaryTable.Three, data.DataTypeSummaryTable.Shownum, data.DataTypeSummaryTable.Clicknum, data.DataTypeSummaryTable.Clicknum, data.DataTypeSummaryTable.Consumption, data.DataTypeSummaryTable.Clickratenum, data.DataTypeSummaryTable.Avgclickprice, data.DataTypeSummaryTable.Landingnum, data.DataTypeSummaryTable.Registrationnum, data.DataTypeSummaryTable.Appnum, data.DataTypeSummaryTable.Regsuccessdate, data.DataTypeSummaryTable.Firstpaytime);
                        break;
                    case TableName.wddatatable:
                        strSql.AppendFormat("insert into wddatatable (`masterkeyid`,`uid`,`devicetypeid`,`adsourceid`,`HomeClickFinish`,`HomeLogionFinish`,`HomeOpenFinish`,`FulcrumlandingFinish`,`FulcrumRegistrationFinish`,`MobileAPPDownloadFinish`)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}');", data.DataTypeWDDataTable.Masterkeyid, data.DataTypeWDDataTable.Uid, data.DataTypeWDDataTable.Devicetypeid, data.DataTypeWDDataTable.Adsourceid, data.DataTypeWDDataTable.HomeClickFinish, data.DataTypeWDDataTable.HomeLogionFinish, data.DataTypeWDDataTable.HomeOpenFinish, data.DataTypeWDDataTable.FulcrumlandingFinish, data.DataTypeWDDataTable.FulcrumRegistrationFinish, data.DataTypeWDDataTable.FobileAPPDownloadFinish);
                        break;
                    default:
                        return null;
                }
            }

           return strSql.ToString();
       }
       /// <summary>
       /// 返回相应的表名
       /// </summary>
       /// <param name="table"></param>
       /// <returns></returns>
        public string GetTableName(TableName table)
        {
            DataTypeInterface inter = GetCreateClassProcess(table);
            return inter.getTableName().ToString();
        }
        public DataTypeInterface GetCreateClassProcess(TableName table)
        {
            switch (table)
            {
                case TableName.masterkey: return new DataTypeMasterKey();
                    break;
                case TableName.adgroupkey: return new DataTypeADGroupKey();
                    break;
                case TableName.adkeytable: return new DataTypeADKeyTable();
                    break;
                case TableName.adserieskey: return new DataTypeADSeriesKey();
                    break;
                case TableName.adsourcetable: return new DataTypeADSourceTable();
                    break;
                case TableName.datekey: return new DataTypeDateKey();
                    break;
                case TableName.devicetypetable: return new DataTypeDeviceTypeTable();
                    break;
                case TableName.rawdatatable: return new DataTypeRawDataTable();
                    break;
                case TableName.summarytable: return new DataTypeSummaryTable();
                    break;
                case TableName.wddatatable: return new DataTypeWDDataTable();
                    break;
                default:
                    return null;
            }
            return null;
        }
        public DataTypeInterface GetDataClassProcess(TableName table,DataPrototype data)
        {
            DataTypeInterface datainter;
            switch (table)
            {
                case TableName.masterkey:
                    datainter = data.DataTypeMasterKey;
                    break;
                case TableName.adgroupkey:
                    datainter = data.DataTypeADGroupKey;
                    break;
                case TableName.adkeytable:
                    datainter = data.DataTypeADKeyTable;
                    break;
                case TableName.adserieskey:
                    datainter = data.DataTypeADSeriesKey;
                    break;
                case TableName.adsourcetable:
                    datainter = data.DataTypeADSourceTable;
                    break;
                case TableName.datekey:
                    datainter = data.DataTypeDateKey;
                    break;
                case TableName.devicetypetable:
                    datainter = data.DataTypeDeviceTypeTable;
                    break;
                case TableName.rawdatatable:
                    datainter = data.DataTypeRawDataTable;
                    break;
                case TableName.summarytable:
                    datainter = data.DataTypeSummaryTable;
                    break;
                case TableName.wddatatable:
                    datainter = data.DataTypeWDDataTable;
                    break;
                default:
                    return null;
            }
            return datainter;
        }
            
    }
}
