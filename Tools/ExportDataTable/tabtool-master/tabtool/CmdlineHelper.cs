using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tabtool
{
    class CmdlineHelper
    {
        public CmdlineHelper(string[] args)
        {
            m_args = args;
        }

        string[] m_args;

        public bool Has(string s)
        {
            return m_args.Count(p => p == s) > 0;
        }

        public string Get(string s)
        {
            for(int i = 0; i < m_args.Count(); i++)
            {
                if (m_args[i] == s && i + 1 < m_args.Count())
                {
                    return m_args[i + 1];
                }
            }
            return null;
        }
    }
}
