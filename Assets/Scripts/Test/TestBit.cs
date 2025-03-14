using UnityEngine;

public class TestBit : MonoBehaviour {
    // 设置第 n 位为 1
    public static int SetBit(int number, int n) {
        return number | (1 << n);
    }

    // 获取第 n 位的值
    public static bool GetBit(int number, int n) {
        return (number & (1 << n)) != 0;
    }

    // 设置第 n 位为 0
    public static int ClearBit(int number, int n) {
        return number & ~(1 << n);
    }

    private void Start() {
        int number = 5; // 0101

        // 设置第三位为1
        number = SetBit(number, 2); // 结果是 0101 | 0100 = 0101
        Debug.LogError(number);
        // 获取第三位的值
        bool bit = GetBit(number, 2); // 结果是 true，因为第三位是1
        Debug.LogError(bit);

        // 清除第三位为0
        number = ClearBit(number, 2); // 结果是 1101 & 1011 = 1001
        Debug.LogError(number);
    }
}