using System;
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
        TestIsRectIntersect();
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

    // 判断两个矩形是否相交
    // (x1,y1) (x2,y2)是第一个矩形左下和右上角的两个点
    // (x3,y3) (x4,y4)是第二个矩形左下角和右上角的两个点
    private bool IsRectIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
        return Math.Max(x1, x3) <= Math.Min(x2, x4) && Math.Max(y1, y3) <= Math.Min(y2, y4);
    }

    // AABB与圆是否相交
    // h = (rectLength/2, rectHeight/2)
    private bool IsBoxCircleIntersect(Vector2 rectCenter, Vector2 h, Vector2 point, float radius) {
        // 第一步：转换到第一象限
        var v = point - rectCenter;
        v.x = Math.Abs(v.x);
        v.y = Math.Abs(v.y);
        // 第二步：求圆心到矩形的最短距离矢量
        var u = v - h;
        u.x = Math.Max(u.x, 0);
        u.y = Math.Max(u.y, 0);
        // 第三步：长度平方与半径平方比较
        return u.sqrMagnitude <= radius * radius;
    }

    // 单元测试 判断两个矩形是否相交
    private void TestIsRectIntersect() {
        Debug.DrawLine(new Vector3(-100,-50, 0), new Vector3(-100,50, 0), Color.red, 1000);
        Debug.DrawLine(new Vector3(-100,-50, 0), new Vector3(100,-50, 0), Color.red, 1000);
        Debug.DrawLine(new Vector3(-100,50, 0), new Vector3(100,50, 0), Color.red, 1000);
        Debug.DrawLine(new Vector3(100,-50, 0), new Vector3(100,50, 0), Color.red, 1000);

        Debug.DrawLine(new Vector3(0,-100, 0), new Vector3(0,0, 0), Color.green, 1000);
        Debug.DrawLine(new Vector3(-150,0, 0), new Vector3(0,0, 0), Color.green, 1000);
        Debug.DrawLine(new Vector3(-150,-100, 0), new Vector3(-150,0, 0), Color.green, 1000);
        Debug.DrawLine(new Vector3(-150,-100, 0), new Vector3(0,-100, 0), Color.green, 1000);

        print(IsRectIntersect(-100, -50, 100, 50, -150, -100, 0, 0));
    }

    private void TestBoxCircleIntersect() {

        // print(IsBoxCircleIntersect(-100, -50, 100, 50, -150, -100, 0, 0));
    }
}
