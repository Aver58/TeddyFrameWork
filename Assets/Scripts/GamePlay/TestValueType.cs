using UnityEngine;

public class TestValueType : MonoBehaviour {
    private Vector3[] transformList = new Vector3[2];
    private Vector3[] v3;
    
    private Vector3 position0 = Vector3.zero;
    private Vector3 position1 = Vector3.one;
    private Vector3 position2 = Vector3.back;
    private Vector3 position3 = Vector3.forward;
    private class MyClass {
        public Vector3 Position;
        public Vector3 Rotation;
    }
    
    void Start() {
        // v3 = position0;
        // var v0 = TestValue(v3);
        // print(v0.Position);
        //
        // v3 = position1;
        // var v1 = TestValue(v3);
        // print(v0.Position);
        // print(v1.Position);
        
        transformList = new Vector3[2];
        transformList[0] = position0;
        transformList[1] = position1;
        var v0 = TestValue(transformList);
        print(v0.Position);
        
        // transformList = new Vector3[2];
        transformList[0] = position2;
        transformList[1] = position3;
        var v1 = TestValue(transformList);
        
        print(v0.Position);
        print(v1.Position);
    }

    MyClass TestValue(Vector3[] v) {
        MyClass m = new MyClass();
        m.Position = v[0];
        m.Rotation = v[1];
        return m;
    }

    private void Update() {
        Debug.LogError("111");
    }
}