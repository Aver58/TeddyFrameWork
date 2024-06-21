using UnityEngine;
using UnityEngine.UI;

public class UICtrler : MonoBehaviour
{
    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    public GameObject bezierCurve2;
    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    public GameObject bezierCurve3;

    public Toggle toggleBezier3;

    void Start()
    {
        toggleBezier3.onValueChanged.AddListener((v) => 
        {
            bezierCurve3.SetActive(v);
            bezierCurve2.SetActive(!v);
        });

        // 默认显示三阶贝塞尔曲线
        bezierCurve3.SetActive(true);
        bezierCurve2.SetActive(false);
    }
}
