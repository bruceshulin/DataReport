using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{

    class DataBaseSqlServe:DataBaseInterface
    {
      

        private Lv.Database.OleDB dataDB = null;
         
        public void init()
        {
            dataDB = new Lv.Database.OleDB("Provider=SQLOLEDB;Data Source=192.168.0.111;User ID=sa;Password=lewell;Initial Catalog=baoshulin;Connect Timeout=15");
            throw new NotImplementedException();
        }

        public bool Create()
        {
            throw new NotImplementedException();
        }




    }



}
