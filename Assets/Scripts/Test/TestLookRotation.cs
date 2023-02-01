using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLookRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        CreatePrimitive(Vector3.zero, new Vector3(1,0,1), 0.1f, PrimitiveType.Cube);
    }

    public static GameObject CreatePrimitive(Vector3 origin, Vector3 target, float radius = 0.1f, PrimitiveType primitiveType = PrimitiveType.Cube) {
        var go = GameObject.CreatePrimitive(primitiveType);
        go.GetComponent<Collider>().enabled = false;
        var targetDir = target - origin;
        var distance = Vector3.Distance(origin, target);
        go.transform.position = (target + origin) / 2;
        if (primitiveType == PrimitiveType.Cylinder || primitiveType == PrimitiveType.Capsule) {
            distance /= 2;
        }
        go.transform.localScale = new Vector3(radius, distance, radius);
        go.transform.up = targetDir;

        return go;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
