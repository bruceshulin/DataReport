using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
   public interface DataBaseInterface
    {
        //初始化本类实例
        void init();
        //初始化里面的创建表
        //bool Create();
        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <returns></returns>
       // bool SeaveData( List<DataPrototype> list);
        /// <summary>
        /// 从某个表中传入　key得到某个值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
       // string ReadTableId(TableName table, string  id);

    }
}
