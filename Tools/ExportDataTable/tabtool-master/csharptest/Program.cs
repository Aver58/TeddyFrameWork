using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tabtool;

namespace csharptest
{
    class Program
    {
        static void Main(string[] args)
        {
            if(TableConfig.Instance.LoadTableConfig())
            {
                tbsAchiveItem item = cfgAchiveTable.Instance.GetTableItem(1);
                if(item != null)
                {
                    Console.WriteLine(item.name);
                }
            }
        }
    }
}
