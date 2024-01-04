using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

// 将要进行浅度复制的对象,注意为引用类型
public class RefLine : ICloneable
{
    public RefPoint rPoint;
    public ValPoint vPoint;
    public RefLine(RefPoint rPoint, ValPoint vPoint)
    {
        this.rPoint = rPoint;
        this.vPoint = vPoint;
    }

    public object Clone()
    {
        return this.MemberwiseClone();//.net中实现浅拷贝的内置方法(System.Object的方法)
    }
}
// 定义一个引用类型成员
public class RefPoint
{
    public int x;
    public RefPoint(int x)
    {
        this.x = x;
    }
}
// 定义一个值类型成员
public struct ValPoint
{
    public int x;
    public ValPoint(int x)
    {
        this.x = x;
    }
}

public class TestClone : MonoBehaviour {
    private void Start() {
        demo2();
    }

    private static void demo2() {
        RefPoint rPoint = new RefPoint(1);
        ValPoint vPoint = new ValPoint(1);
        RefLine line = new RefLine(rPoint, vPoint);
        RefLine newLine = (RefLine)line.Clone();
        Debug.LogFormat("Original： line.rPoint.x = {0}, line.vPoint.x= {1} ", line.rPoint.x, line.vPoint.x);
        Debug.LogFormat("Cloned： newLine.rPoint.x = {0}, newLine.vPoint.x = {1} ", newLine.rPoint.x, newLine.vPoint.x);
        line.rPoint.x = 10;        // 修改原先的line的引用类型成员 rPoint
        line.vPoint.x = 10;        // 修改原先的line的值类型成员 vPoint
        Debug.LogFormat("Original： line.rPoint.x = {0}, line.vPoint.x= {1} ", line.rPoint.x, line.vPoint.x);
        Debug.LogFormat("Cloned： newLine.rPoint.x = {0}, newLine.vPoint.x = {1} ", newLine.rPoint.x, newLine.vPoint.x);
    }

    /// 深拷贝可以利用序列化反序列化对对象进行深度复制。
    public object Clone() {
        using(var ms = new MemoryStream())
        {
            var bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Seek(0, SeekOrigin.Begin);
            return (bf.Deserialize(ms));
        }

    }

    // 注：使用反射时必须引用类型必须有无参构造函数
    public static T DeepCopyByReflect<T>(T obj) {
        //如果是字符串或值类型则直接返回
        if (obj is string || obj.GetType().IsValueType) return obj;

        object retval = Activator.CreateInstance(obj.GetType());//若obj没有无参构造函数,此语句报错
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo field in fields) {
            try {
                field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj)));
            }//递归下去直到field为值类型或string给其赋值
            catch { }
        }
        return (T)retval;
    }
}
