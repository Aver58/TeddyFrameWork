using UnityEngine;

public class RaycastCollisionDetector : MonoBehaviour
{
    public float rayLength = 10.0f;
    public float offset = 0.1f;
    public LayerMask layerMask;
    void Update()
    {
        // 从对象内部发射射线
        // 射线检测（Raycast）可能在某些情况下会忽略与射线起点重叠的碰撞体。
        // 调整射线起点
        Vector3 offsetPosition = transform.position + transform.forward * offset;
        Ray ray = new Ray(offsetPosition, transform.forward);
        RaycastHit hit;

        // 射线检测
        bool isHit = Physics.Raycast(ray, out hit, rayLength - offset);
        // 可视化射线
        Color debugColor = isHit ? Color.green : Color.red; // 碰撞到物体时变为绿色
        Debug.DrawRay(ray.origin, ray.direction * rayLength, debugColor);

        if (isHit)
        {
            Debug.Log("检测到碰撞: " + hit.collider.name);
        }
    }
}
