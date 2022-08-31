using System.Collections.Generic;
using System.IO;

//tbs中的结构体都从这里继承
interface ITableObject
{
    bool FromString(string s);
}

//表字段读取
class DataReader
{
    public List<string> GetStringList(string s, char delim=',')
    {
        string[] t = s.Split(delim);
        List<string> ret = new List<string>();
        ret.AddRange(t);
        return ret;
    }

    public int GetInt(string s)
    {
        if (string.IsNullOrEmpty(s)) {
            return 0;
        }

        return int.Parse(s);
    }

    public List<int> GetIntList(string s)
    {
        if (string.IsNullOrEmpty(s)) {
            return null;
        }

        string[] vs = s.Split(',');
        List<int> ret = new List<int>();
        foreach(var ss in vs)
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
        if (string.IsNullOrEmpty(s)) {
            return null;
        }

        string[] vs = s.Split(',');
        List<float> ret = new List<float>();
        foreach(var ss in vs)
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
        if (string.IsNullOrEmpty(s)) {
            return null;
        }

        string[] vs = s.Split(';');
        List<T> ret = new List<T>();
        foreach(var ss in vs)
        {
            ret.Add(GetObject<T>(ss));
        }
        return ret;
    }
};


public class MyDataRow
{
    private List<string> m_columns;
    public MyDataRow(List<string> columns)
    {
        m_columns = columns;
    }

    public Dictionary<string, string> m_valueMap = new Dictionary<string, string>();

    public string this[string columnName]
    {
        get
        {
            return m_valueMap[columnName];
        }
    }

    public string this[int index]
    {
        set
        {
            string columnName = m_columns[index];
            m_valueMap[columnName] = value;
        }
    }
}

public class MyDataTable
{
    public List<string> Columns = new List<string>(); //字段名
    public List<MyDataRow> Rows = new List<MyDataRow>();
    public MyDataRow GenOneRow()
    {
        return new MyDataRow(Columns);
    }
}

class TableReader
{
    // todo system.data.dll引用失败 ,2020.3.18自己写了个MyDataTable结构
    public MyDataTable ReadFile(string filepath)
    {
        MyDataTable dt = new MyDataTable();
        //首行是字段名 之后是字段值
        string[] lines = File.ReadAllLines(filepath);
        bool firstline = true;
        foreach(string line in lines)
        {
            string[] words = line.Split('\t');
            if(words == null || words.Length == 0)
                continue;

            if(firstline)
            {
                firstline = false;
                foreach(string word in words)
                {
                    dt.Columns.Add(word);
                }
                continue;
            }
            MyDataRow row = dt.GenOneRow();
            int i = 0;
            foreach(string word in words)
            {
                row[i++] = word;
            }
            dt.Rows.Add(row);
        }
        return dt;
    }
}
