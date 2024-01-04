#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ExportNavMesh {

    [MenuItem("Tools/Export NavMesh")]
    static void Export() {
        Debug.Log("ExportNavMesh");

        NavMeshTriangulation tmpNavMeshTriangulation = NavMesh.CalculateTriangulation();
        NavMesh.SamplePosition(Vector3.zero, out NavMeshHit tmpNavMeshHit, 100, NavMesh.AllAreas);
        //新建文件
        string tmpPath = Application.dataPath + "/" + SceneManager.GetActiveScene().name + ".obj";
        StreamWriter tmpStreamWriter = new StreamWriter(tmpPath);

        //顶点
        for (int i=0;i<tmpNavMeshTriangulation.vertices.Length;i++)
        {
            tmpStreamWriter.WriteLine($"v:{i} "+ tmpNavMeshTriangulation.vertices[i].x+" "+ tmpNavMeshTriangulation.vertices[i].y+" "+ tmpNavMeshTriangulation.vertices[i].z);
        }

        tmpStreamWriter.WriteLine("g pPlane1");

        //索引
        for (int i = 0; i < tmpNavMeshTriangulation.indices.Length;)
        {
            tmpStreamWriter.WriteLine($"i:{i} " + (tmpNavMeshTriangulation.indices[i]+1) + " " + (tmpNavMeshTriangulation.indices[i+1]+1) + " " + (tmpNavMeshTriangulation.indices[i+2]+1));
            i += 3;
        }

        tmpStreamWriter.Flush();
        tmpStreamWriter.Close();

        Debug.Log("ExportNavMesh Success");
    }
}

#endif