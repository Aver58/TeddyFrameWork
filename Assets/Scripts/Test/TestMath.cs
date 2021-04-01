using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Quaternion rotation = Quaternion.Euler(0f, 30f, 0f) * transform.rotation;
        //Vector3 newPos = rotation * new Vector3(10f, 0f, 0f);

        //transform.position = newPos;
        //Debug.Log("newpos " + newPos + " nowpos " + transform.position + " distance " + Vector3.Distance(newPos, transform.position));
    }

	private float distance = 10f;
    void Update()
    {
        Quaternion right = transform.rotation * Quaternion.Euler(0f,30f,0f);
        Quaternion left = transform.rotation * Quaternion.Euler(0f, -30f, 0f);

        Vector3 n = transform.position + (Vector3.forward * distance);
        Vector3 leftPoint = left * n;
        Vector3 rightPoint = right * n;

        Debug.DrawLine(transform.position, leftPoint, Color.red);
        Debug.DrawLine(transform.position, rightPoint, Color.red);
        Debug.DrawLine(rightPoint, leftPoint, Color.red);
    }
}
