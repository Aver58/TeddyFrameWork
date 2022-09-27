using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRelativePosition : MonoBehaviour
{
    public Transform t1;
    public Transform t2;
    public Transform t3;
    // public Transform t4;

    // Start is called before the first frame update
    void Start()
    {
        t3.SetParent(t2, false);
        t2.SetParent(t1, false);

        // Debug.Log(t4.position);

        // Matrix4x4 m3 = Matrix4x4.TRS(t3.localPosition, t3.localRotation, t3.localScale);
        // Debug.Log(t1.parent.localToWorldMatrix.MultiplyPoint(m1.MultiplyPoint(m2.MultiplyPoint(m3.MultiplyPoint(t4.localPosition)))));
        // Debug.Log((t1.parent.localToWorldMatrix * m1 * m2 * m3).MultiplyPoint(t4.localPosition));
        
        // Debug.Log($"t2.localPosition:{t2.localPosition}");
        // Debug.Log($"t2.position:{t2.position}");
        // Debug.Log($"t3.position:{t3.position}");
        //
        // t3的localPosition转换为t2坐标下的localPosition(Unity API TransformPoint 实现)
        // 先转换世界坐标，再算相对坐标
        Debug.Log($"t3.position:{t3.position}");
        Debug.Log($"t3.TransformPoint:{t3.parent.parent.InverseTransformPoint(t3.position)}");
        
        // t3的localPosition转换为t2坐标下的localPosition(Unity API 矩阵实现)
        Matrix4x4 m1 = Matrix4x4.TRS(t1.localPosition, t1.localRotation, t1.localScale);
        Matrix4x4 m2 = Matrix4x4.TRS(t2.localPosition, t2.localRotation, t2.localScale);
        Debug.Log($"m1 :{m1}");
        Debug.Log($"m2 :{m2}");
        Debug.Log($"t1.localToWorldMatrix :{t1.localToWorldMatrix == m1}");
        
        Debug.Log($"m2.MultiplyPoint(t3.localPosition)) :{m2.MultiplyPoint(t3.localPosition)}");
        Debug.Log($"t1.localToWorldMatrix:{m1.MultiplyPoint(m2.MultiplyPoint(t3.localPosition))}");
    }
}
