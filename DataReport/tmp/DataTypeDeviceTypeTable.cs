using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class DataTypeDeviceTypeTable:DataTypeInterface
    {
        string devicetypeid;

        public string Devicetypeid
        {
            get { return devicetypeid; }
            set { Modifly();devicetypeid = value; }
        }
        string devicetype;

        public string Devicetype
        {
            get { return devicetype; }
            set { Modifly();devicetype = value; }
        }
        public TableName getTableName()
        {
            return TableName.devicetypetable;
        }


        bool modifly = false;
        public bool Modifly()
        {
            modifly = true;
            return modifly;
        }
    }
}
