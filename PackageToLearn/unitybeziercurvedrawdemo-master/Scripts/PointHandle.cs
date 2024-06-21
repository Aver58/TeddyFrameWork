using UnityEngine;

/// <summary>
/// 控制点
/// </summary>
public class PointHandle : MonoBehaviour
{
    private Transform targetTrans;
    private Camera cam;
    private float posZ;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // 鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                // 缓存射线碰撞到的物体
                targetTrans = hit.transform;
                // 缓存物体与摄像机的距离
                posZ = targetTrans.position.z - cam.transform.position.z;
            }
        }
        // 鼠标左键抬起
        if (Input.GetMouseButtonUp(0))
        {
            // 释放碰撞体缓存
            targetTrans = null;
        }
        // 鼠标按住中
        if (null != targetTrans && Input.GetMouseButton(0))
        {
            // 鼠标的屏幕坐标转成世界坐标
            // 由于鼠标的屏幕坐标的z轴是0，所以需要使用物体距离摄像机的距离为z周的值
            var targetPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, posZ));

            targetTrans.position = targetPos;
        }
    }
}
