using System;
using System.Collections;
using System.Collections.Generic;
using CustomPhysics;
using CustomPhysics.Collision;
using CustomPhysics.Collision.Shape;
using CustomPhysics.Test;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class MonoWorld : MonoBehaviour {
    public PhysicsWorld world;

    // Start is called before the first frame update
    void Start() {
        world = new PhysicsWorld();
        world.Init();

        // Test0();
        // Test1();
        // Test2();
        Test3();
        // Test4();
    }

    void Update()
    {
        Profiler.BeginSample("[ViE] Tick");
        world.Tick(Time.deltaTime);
        Profiler.EndSample();
    }

    #region Test

    private void Test0() {
        int range = 2;
        for (int i = 0; i < range; i++) {
            CreateATestRect(float3.zero);
        }
    }

    private void Test1() {
        int range = 1;
        for (int i = 0; i < range; i++) {
            float3 spawnPos = new float3(
                Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

            CreateATestRect(new float3(
                Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f)));
            // CreateACustomShape(new float3[] {
            //     new float3(-2, 0, 0), new float3(0, 0, 1),
            //     new float3(2, 0, 0), new float3(0, 0, -1)}, spawnPos, i);
        }
    }

    private void Test2() {
        int range = 2;
        for (int i = 0; i < range; i++) {
            // float3 spawnPos = new float3(
                // Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            // CreateATestCircle(1, spawnPos);
            CreateATestCircle(1, float3.zero);
        }

        CreateATestRect(float3.zero);

        // CreateATestCircle(1, float3.zero);
        // CreateATestCircle(1, float3.right);
    }

    private void Test3() {
        int range = 1;
        for (int i = 0; i < range; i++) {
            float3 spawnPos = new float3(
                Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

            CreateATestRect(float3.zero);
            CreateATestCircle(1, float3.zero);
            float3[] localVertices = new float3[] {
                new float3(-2, 0, 0), new float3(0, 0, 1), new float3(1, 0, 1),
                new float3(2, 0, 0), new float3(2, 0, -2), new float3(-2, 0, -3)
            };
            CreateACustomShape(localVertices, float3.zero, 1);
            // spawnPos = new float3(
            //     Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            // CreateACustomShape(new float3[] {
            //     new float3(-2, 0, 0), new float3(0, 0, 1), new float3(1, 0, 1),
            //     new float3(2, 0, 0), new float3(2, 0, -2), new float3(-2, 0, -3),
            //     new float3(-2, 0, 0)}, spawnPos, 1);
        }
    }

    private void Test4() {
        // CreateACustomShape(new float3[] {float3.zero, new float3(-1.54f, 0, 4.75f),
        //     new float3(2.5f, 0, 7.69f), new float3(6.54f, 0, 4.75f), new float3(5, 0, 0),},
        //     float3.zero, 0);
        //
        // CreateACustomShape(new float3[] {float3.zero, new float3(-1.54f, 0, 4.75f),
        //     new float3(2.5f, 0, 7.69f), new float3(6.54f, 0, 4.75f), new float3(5, 0, 0),},
        //     float3.zero, 0);
    }

    public void CreateACustomShape(float3[] vertices, float3 pos, int level) {
        CollisionShape shape = new CustomPhysics.Collision.Shape.CustomShape(vertices);
        CollisionObject co = new CollisionObject(shape, null, pos, 0, level);
        world.AddCollisionObject(co);
        GameObject go = CreateMesh(co);
    }

    public GameObject CreateMesh(CollisionObject co) {
        GameObject go = new GameObject(co.shape.shapeType.ToString());
        Mesh m;
        go.AddComponent<MeshFilter>().mesh = m = new Mesh();
        m.name = Guid.NewGuid().ToString();
        int localVerticesCount = co.shape.localVertices.Length;
        Vector3[] vertices = new Vector3[localVerticesCount];
        for (int i = 0; i < localVerticesCount; i++) {
            vertices[i] = new Vector3(co.shape.localVertices[i].x, co.shape.localVertices[i].y,
                co.shape.localVertices[i].z);
        }
        m.vertices = vertices;

        int vertexCount = co.shape.vertices.Length;
        int triCount = vertexCount - 2;
        int[] triangles = new int[3 * triCount];
        for (int i = 0, triIndex = 0; i < triCount; triIndex = 3 * ++i) {
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = (i + 1) % vertexCount;
            triangles[triIndex + 2] = (i + 2) % vertexCount;
        }
        m.triangles = triangles;
        go.AddComponent<MeshRenderer>();
        go.AddComponent<CollisionObjectProxy>().target = co;

        return go;
    }

    public void CreateATestRect(float3 pos, int level = 0) {
        CollisionShape shape = new CustomPhysics.Collision.Shape.Rect(1, 1);
        CollisionObject co = new CollisionObject(shape, null, pos, 0, level);
        world.AddCollisionObject(co);
        GameObject go = CreateMesh(co);
    }

    public void CreateATestCircle(float radius, float3 pos) {
        CollisionShape shape = new CustomPhysics.Collision.Shape.Circle(radius);
        CollisionObject co = new CollisionObject(shape, null, pos);
        world.AddCollisionObject(co);
        GameObject go = CreateMesh(co);
    }

    #endregion
}