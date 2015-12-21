using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class database
    {
        /*
         * 主关键字表
         CREATE TABLE  if not exists  masterkey([Id] integer PRIMARY KEY AUTOINCREMENT,
        [dateid] integer,
        [adseriesid]integer,
        [adgroup] integer,
        [adkey] integer
        );

        insert into masterkey (`dateid`,`adseriesid`,`adgroup`,`adkey`)values('1','2','3','4');
         
        日期
        CREATE TABLE  if not exists  datekey([Id] integer PRIMARY KEY AUTOINCREMENT,[date] Text);
        insert into datekey (`date`)values('2015-10-12');


        广告系列
        CREATE TABLE  if not exists  adserieskey([Id] integer PRIMARY KEY AUTOINCREMENT,[adseries] Text);
        insert into adserieskey (`adseries`)values('品牌词-微操盘-P');


        广告分组
        CREATE TABLE  if not exists  adgroupkey([Id] integer PRIMARY KEY AUTOINCREMENT,[adgroup] Text);
        insert into adgroupkey (`adgroup`)values('新浪-炒股_150120');


        广告关键词
        CREATE TABLE  if not exists  adkeytable([Id] integer PRIMARY KEY AUTOINCREMENT,[adkey] Text);
        insert into adkeytable (`adkey`)values('新浪 配资');
         */

        /*
        devicetype设备类型
        CREATE TABLE  if not exists  devicetypetable([Id] integer PRIMARY KEY AUTOINCREMENT,[devicetype] Text);
        insert into devicetypetable (`devicetype`)values('pc');

        adsource广告来源
        CREATE TABLE  if not exists  adsourcetable([Id] integer PRIMARY KEY AUTOINCREMENT,[adsource] Text);
        insert into adsourcetable (`adsource`)values('baidu');

         */
        /*
         * 原数据
        rawdata
        CREATE TABLE  if not exists  rawdatatable([Id] integer PRIMARY KEY AUTOINCREMENT,[masterkeyid] Text,[uid] Text,[firstpaymenttime] Text,[urlsource] Text,[channel] Text,[firstregistrationtime] Text);
        insert into rawdatatable (`masterkeyid`,`uid`,`firstpaymenttime`,`urlsource`,`channel`,`firstregistrationtime`)values('masterkeyid','uid','firstpaymenttime','urlsource','channel','firstregistrationtime');

         */
        /*  WD数据表
         * [HomeClickFinish] Text
        首页-操盘宝 完成数
        [HomeLogionFinish] Text
        首页-点此登录 完成数
        [HomeOpenFinish] Text
        首页-点此开通 完成数

        支点登陆 完成数
        [FulcrumlandingFinish] Text
        支点注册 完成数
        [FulcrumRegistrationFinish] Text
        Mobile端全部APP下载 完成数
        [MobileAPPDownloadFinish] Text
         * 
         CREATE TABLE  if not exists  wddatatable([Id] integer PRIMARY KEY AUTOINCREMENT,[masterkeyid] Text,[uid] Text,[devicetypeid] Text,[adsourceid] Text,[HomeClickFinish] Text,[HomeLogionFinish] Text,[HomeOpenFinish] Text,[FulcrumlandingFinish] Text,[FulcrumRegistrationFinish] Text,[MobileAPPDownloadFinish] Text);
        insert into wddatatable (`masterkeyid`,`uid`,`devicetypeid`,`adsourceid`,`HomeClickFinish`,`HomeLogionFinish`,`HomeOpenFinish`,`FulcrumlandingFinish`,`FulcrumRegistrationFinish`,`MobileAPPDownloadFinish`)values('masterkeyid','uid','devicetypeid','adsourceid','HomeClickFinish','HomeLogionFinish','HomeOpenFinish','FulcrumlandingFinish','FulcrumRegistrationFinish','MobileAPPDownloadFinish');

         */
        /*
         * 目标汇总表
         * 
            CREATE TABLE  if not exists  summarytable([Id] integer PRIMARY KEY AUTOINCREMENT,[dateid] Text,[Weekdate] Text,[adsourceid] Text,[devicetypeid] Text,[adseriesid] Text,[adgroupid] Text,[adkeyid] Text,[two] Text,[three] Text,[shownum] Text,[clicknum] Text,[Consumption] Text,[clickratenum] Text,[avgclickprice] Text,[landingnum] Text,[registrationnum] Text,[appnum] Text,[regsuccessdate] Text,[firstpaytime] Text);
            insert into summarytable (`dateid`,`Weekdate`,`adsourceid`,`devicetypeid`,`adseriesid`,`adgroupid`,`adkeyid`,`two`,`three`,`shownum`,`clicknum`,`clicknum`,`Consumption`,`clickratenum`,`avgclickprice`,`landingnum`,`registrationnum`,`appnum`,`regsuccessdate`,`firstpaytime`)values('dateid','Weekdate','adsourceid','devicetypeid','adseriesid','adgroupid','adkeyid','two','three','shownum','clicknum','clicknum','Consumption','clickratenum','avgclickprice','landingnum','registrationnum','appnum','regsuccessdate','firstpaytime');

         */
    }
}
