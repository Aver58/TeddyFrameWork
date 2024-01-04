using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestCollider : MonoBehaviour {
    private List<float> sortedXBounds;
    private List<float> sortedYBounds;
    private List<float> sortedZBounds;
    // 维护一个可能碰撞的列表，碰撞检测只检测这个列表
    private List<object> overlapSet;
    
    private void Start() {
        sortedXBounds = new List<float>();
        sortedYBounds = new List<float>();
        sortedZBounds = new List<float>();
        
        overlapSet = new List<object>();
        // 创建 Rect
        // 创建 Circle
        // 创建 自定义形状
        
        // 初始化排序
    }

    private void Update() {
        Profiler.BeginSample("[ViE] UpdateBounds");
        UpdateBounds();
        Profiler.EndSample();
        
        CollisionDetection();
    }
    
    // 碰撞检测
    private void CollisionDetection() {
        Profiler.BeginSample("[ViE] BroadPhase");
        BroadPhase();
        Profiler.EndSample();
        
        Profiler.BeginSample("[ViE] NarrowPhase");
        NarrowPhase();
        Profiler.EndSample();
    }

    #region 碰撞检测：粗筛
    // 碰撞检测：粗筛
    private void BroadPhase() {
        SweepAndPrune();
        // DynamicBVH();
    }
    
    // 遍历和裁剪
    private void SweepAndPrune() {
        // 插入排序

    }
    
    #endregion

    #region 碰撞检测：细筛

    private void NarrowPhase() {
        
    }

    #endregion

    #region Private

    // 交换边缘
    private void UpdateBounds() {
        
    }
    
    // 新增碰撞检测对象
    private void AddCollisionDetection() {
        
    }
    
    private void CreateRect() {
        
    }
    
    // public GameObject CreateMesh(CollisionObject co) {
    //     GameObject go = new GameObject(co.shape.shapeType.ToString());
    //     Mesh m;
    //     go.AddComponent<MeshFilter>().mesh = m = new Mesh();
    //     m.name = Guid.NewGuid().ToString();
    //     int localVerticesCount = co.shape.localVertices.Length;
    //     Vector3[] vertices = new Vector3[localVerticesCount];
    //     for (int i = 0; i < localVerticesCount; i++) {
    //         vertices[i] = new Vector3(co.shape.localVertices[i].x, co.shape.localVertices[i].y, co.shape.localVertices[i].z);
    //     }
    //     m.vertices = vertices;
    //
    //     int vertexCount = co.shape.vertices.Length;
    //     int triCount = vertexCount - 2;
    //     int[] triangles = new int[3 * triCount];
    //     for (int i = 0, triIndex = 0; i < triCount; triIndex = 3 * ++i) {
    //         triangles[triIndex] = 0;
    //         triangles[triIndex + 1] = (i + 1) % vertexCount;
    //         triangles[triIndex + 2] = (i + 2) % vertexCount;
    //     }
    //     m.triangles = triangles;
    //     go.AddComponent<MeshRenderer>();
    //     // go.AddComponent<CollisionObjectProxy>().target = co;
    //
    //     return go;
    // }

    #endregion
}
