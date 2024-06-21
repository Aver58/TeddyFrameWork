using UnityEngine;

public class TestGridLineRenderer : MonoBehaviour
{
    public float gridSize = 1.0f; // 每个网格的大小
    public int gridCount = 10; // 网格数量
    public Material lineMaterial; // 用于绘制网格线的材质

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        for (int x = -gridCount; x <= gridCount; x++)
        {
            CreateLine(new Vector3(x * gridSize, 0, -gridCount * gridSize), new Vector3(x * gridSize, 0, gridCount * gridSize));
        }
        for (int z = -gridCount; z <= gridCount; z++)
        {
            CreateLine(new Vector3(-gridCount * gridSize, 0, z * gridSize), new Vector3(gridCount * gridSize, 0, z * gridSize));
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.SetParent(transform);
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}