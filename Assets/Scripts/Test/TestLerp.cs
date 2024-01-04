using System.Collections.Generic;
using UnityEngine;

public class TestLerp : MonoBehaviour {
    public Transform targetPosition; // 目标位置
    public float duration = 2.0f; // 缓动持续时间

    private float startTime; // 缓动开始时间

    void Start() {
        startTime = Time.time;

        // 初始化List<int>，包含10个元素，每个元素都为0
        List<int> intList = new List<int>(new int[10]);

        // 打印List<int>的内容
        foreach (var item in intList) {
            // Debug.Log(item + " ");
        }
        // print(intList[2]);

        TestSwitchCase("Game");
        TestSwitchCase("Ballistic_Test");
    }

    void TestSwitchCase(string name) {
        switch (name) {
            case "Game":
            case "UnitTest":
            case "UnitTestInternal":
            case "UnitTestInternalForHouse":
            case "Ballistic_Test":
                Debug.LogError("xxxx");
                break;
        }
    }

    void Update() {
        // 计算当前时间相对于开始时间的比例
        float t = (Time.time - startTime) / duration;
        // 使用Vector3.Lerp实现插值缓动
        transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * duration);
    }
}