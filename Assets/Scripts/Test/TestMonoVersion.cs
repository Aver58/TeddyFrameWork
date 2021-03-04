using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TestMonoVersion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameLog.Log(Application.unityVersion);

        Type type = Type.GetType("Mono.Runtime");
        if(type != null)
        {
            MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if(displayName != null)
                GameLog.Log((displayName.Invoke(null, null)).ToString());

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for(int i = 0; i < methods.Length; i++)
            {
                MethodInfo m = methods[i];
                GameLog.Log((m.IsPublic ? "public " : (m.IsPrivate ? "private " : "")) + (m.IsStatic ? "static " : " ") + m.ReturnType.Name + " " + m.Name + " " + m.GetParameters().Length);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
