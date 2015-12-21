using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReport
{
    public class Field
    {
        string chname = "";

        public string Chname
        {
            get { return chname; }
            set { chname = value; }
        }
        string enname = "";

        public string Enname
        {
            get { return enname; }
            set { enname = value; }
        }
        string value = "";

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
