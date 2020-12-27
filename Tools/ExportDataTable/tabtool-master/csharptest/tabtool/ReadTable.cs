using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace tabtool
{
    //tbs中的结构体都从这里继承
    interface ITableObject
    {
        bool FromString(string s);
    }

    //表字段读取
    class DataReader
    {
        public List<string> GetStringList(string s, char delim = ',')
        {
            string[] t = s.Split(delim);
            List<string> ret = new List<string>();
            ret.AddRange(t);
            return ret;
        }

        public int GetInt(string s)
        {
            return int.Parse(s);
        }

        public List<int> GetIntList(string s)
        {
            string[] vs = s.Split(',');
            List<int> ret = new List<int>();
            foreach (var ss in vs)
            {
                int x = int.Parse(ss);
                ret.Add(x);
            }
            return ret;
        }

        public float GetFloat(string s)
        {
            return float.Parse(s);
        }

        public List<float> GetFloatList(string s)
        {
            string[] vs = s.Split(',');
            List<float> ret = new List<float>();
            foreach (var ss in vs)
            {
                float x = float.Parse(ss);
                ret.Add(x);
            }
            return ret;
        }

        public T GetObject<T>(string s) where T : ITableObject, new()
        {
            T obj = new T();
            obj.FromString(s);
            return obj;
        }

        public List<T> GetObjectList<T>(string s) where T : ITableObject, new()
        {
            string[] vs = s.Split(';');
            List<T> ret = new List<T>();
            foreach (var ss in vs)
            {
                ret.Add(GetObject<T>(ss));
            }
            return ret;
        }
    };

    class TableReader
    {
        public DataTable ReadFile(string filepath)
        {
            DataTable dt = new DataTable();
            //首行是字段名 之后是字段值
            string[] lines = File.ReadAllLines(filepath);
            bool firstline = true;
            foreach (var line in lines)
            {
                string[] words = line.Split('\t');
                if (words == null || words.Length == 0)
                {
                    continue;
                }
                if (firstline)
                {
                    firstline = false;
                    foreach (var word in words)
                    {
                        dt.Columns.Add(word);
                    }
                    continue;
                }
                DataRow row = dt.NewRow();
                int i = 0;
                foreach (var word in words)
                {
                    row[i++] = word;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
