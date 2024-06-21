using System.Diagnostics;
using UnityEngine;

public class TestGridMeshRenderer : MonoBehaviour
{
    public float gridSize = 1.0f;
    public int gridCount = 10;
    public Material lineMaterial;

    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        CreateGridMesh();
        stopwatch.Stop();
        UnityEngine.Debug.Log("代码执行耗时：" + stopwatch.ElapsedMilliseconds + " 毫秒");
    }

    void CreateGridMesh()
    {
        GameObject grid = new GameObject("GridMesh");
        grid.transform.SetParent(transform);
        MeshFilter meshFilter = grid.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = grid.AddComponent<MeshRenderer>();
        meshRenderer.material = lineMaterial;

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        int lineCount = (gridCount * 2 + 1) * 2;
        Vector3[] vertices = new Vector3[lineCount * 2];
        int[] indices = new int[lineCount * 2];

        int index = 0;
        for (int x = -gridCount; x <= gridCount; x++)
        {
            vertices[index * 2] = new Vector3(x * gridSize, 0, -gridCount * gridSize);
            vertices[index * 2 + 1] = new Vector3(x * gridSize, 0, gridCount * gridSize);
            indices[index * 2] = index * 2;
            indices[index * 2 + 1] = index * 2 + 1;
            index++;
        }

        for (int z = -gridCount; z <= gridCount; z++)
        {
            vertices[index * 2] = new Vector3(-gridCount * gridSize, 0, z * gridSize);
            vertices[index * 2 + 1] = new Vector3(gridCount * gridSize, 0, z * gridSize);
            indices[index * 2] = index * 2;
            indices[index * 2 + 1] = index * 2 + 1;
            index++;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
    }
}