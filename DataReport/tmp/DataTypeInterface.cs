using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
   public interface DataTypeInterface
    {
        /// <summary>
        /// 返回当前类的表名
        /// </summary>
        /// <returns></returns>
        TableName getTableName();

       /*为了减少内存空间，决定不这么搞了
        string CreateTableSQL();
        string InsertSQL();
        */

        bool Modifly();
    }
}
