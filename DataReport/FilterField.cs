using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    class FilterField
    {
      
        List<string> listday = new List<string>();

        public List<string> Listday
        {
            get { return listday; }
            set { listday = value; }
        }
        List<string> listweek = new List<string>();

        public List<string> Listweek
        {
            get { return listweek; }
            set { listweek = value; }
        }
        List<string> listsource = new List<string>();

        public List<string> Listsource
        {
            get { return listsource; }
            set { listsource = value; }
        }
        List<string> listdevice = new List<string>();

        public List<string> Listdevice
        {
            get { return listdevice; }
            set { listdevice = value; }
        }
        List<string> listserice = new List<string>();

        public List<string> Listserice
        {
            get { return listserice; }
            set { listserice = value; }
        }
        List<string> listgroup = new List<string>();

        public List<string> Listgroup
        {
            get { return listgroup; }
            set { listgroup = value; }
        }
        List<string> listkey = new List<string>();

        public List<string> Listkey
        {
            get { return listkey; }
            set { listkey = value; }
        }

         public void GetDataTableList()
         {
             DataBaseSqlite.Instance.SelectField(ref listday, ref listweek, ref listsource, ref listdevice, ref listserice, ref listgroup, ref listkey);
         }

        //....
    }
}
