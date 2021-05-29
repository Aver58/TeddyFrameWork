using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public virtual void function()
    {
        GameLog.Log("执行了 父类 方法！");
    }
}

public class child:BaseClass
{
    public new void function()
    {
        GameLog.Log("执行了 子类 方法！");
    }
}

public class TestNew : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BaseClass t = new child();
        t.function();
    }
}
