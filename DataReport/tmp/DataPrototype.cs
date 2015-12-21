using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public class DataPrototype
    {
        //这个类需要包含所有数据类的汇总地

        DataTypeMasterKey dataTypeMasterKey = new DataTypeMasterKey();

        internal DataTypeMasterKey DataTypeMasterKey
        {
            get { return dataTypeMasterKey; }
            set { dataTypeMasterKey = value; }
        }
        DataTypeADGroupKey dataTypeADGroupKey = new DataTypeADGroupKey();

        internal DataTypeADGroupKey DataTypeADGroupKey
        {
            get { return dataTypeADGroupKey; }
            set { dataTypeADGroupKey = value; }
        }
        DataTypeADKeyTable dataTypeADKeyTable = new DataTypeADKeyTable();

        internal DataTypeADKeyTable DataTypeADKeyTable
        {
            get { return dataTypeADKeyTable; }
            set { dataTypeADKeyTable = value; }
        }
        DataTypeADSeriesKey dataTypeADSeriesKey = new DataTypeADSeriesKey();

        internal DataTypeADSeriesKey DataTypeADSeriesKey
        {
            get { return dataTypeADSeriesKey; }
            set { dataTypeADSeriesKey = value; }
        }
        DataTypeADSourceTable dataTypeADSourceTable = new DataTypeADSourceTable();

        internal DataTypeADSourceTable DataTypeADSourceTable
        {
            get { return dataTypeADSourceTable; }
            set { dataTypeADSourceTable = value; }
        }
        DataTypeDateKey dataTypeDateKey = new DataTypeDateKey();

        internal DataTypeDateKey DataTypeDateKey
        {
            get { return dataTypeDateKey; }
            set { dataTypeDateKey = value; }
        }
        DataTypeDeviceTypeTable dataTypeDeviceTypeTable = new DataTypeDeviceTypeTable();

        internal DataTypeDeviceTypeTable DataTypeDeviceTypeTable
        {
            get { return dataTypeDeviceTypeTable; }
            set { dataTypeDeviceTypeTable = value; }
        }
        DataTypeRawDataTable dataTypeRawDataTable = new DataTypeRawDataTable();

        internal DataTypeRawDataTable DataTypeRawDataTable
        {
            get { return dataTypeRawDataTable; }
            set { dataTypeRawDataTable = value; }
        }
        DataTypeSummaryTable dataTypeSummaryTable = new DataTypeSummaryTable();

        internal DataTypeSummaryTable DataTypeSummaryTable
        {
            get { return dataTypeSummaryTable; }
            set { dataTypeSummaryTable = value; }
        }
        DataTypeWDDataTable dataTypeWDDataTable = new DataTypeWDDataTable();

        internal DataTypeWDDataTable DataTypeWDDataTable
        {
            get { return dataTypeWDDataTable; }
            set { dataTypeWDDataTable = value; }
        }



    }
   


}

