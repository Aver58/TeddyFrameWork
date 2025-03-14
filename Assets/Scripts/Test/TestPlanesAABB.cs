using UnityEngine;

public class TestPlanesAABB : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject go;

    // 检查物体是否在视锥体内
    public bool IsInViewFrustum(GameObject obj)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    // 检查物体是否在视锥体内并且没有被遮挡
    public bool IsVisible(GameObject obj)
    {
        if (IsInViewFrustum(obj))
        {
            Vector3 direction = obj.transform.position - mainCamera.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, direction, out hit))
            {
                if (hit.transform.gameObject == obj)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Update() {
        if (IsVisible(go)) {
            Debug.LogError("可见");
        }
    }
}