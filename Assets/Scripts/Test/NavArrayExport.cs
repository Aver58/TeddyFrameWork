#if UNITY_EDITOR

using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;

public class NavArrayExport: MonoBehaviour
{
    #region Public Attributes
    public Vector3 leftUpStart = Vector3.zero;
    public float accuracy = 1;
    public int height = 30;
    public int wide = 30;
    public float maxdistance = 0.2f;
    public float kmin = -10;
    public float kmax = 20;
    #endregion


    #region Public Methods

    public void Exp()
    {
        exportPoint(leftUpStart, wide, height, accuracy);
    }

    public float ConvertToNearestHalfNum(float num)
    {
        float numFloor = Mathf.Floor(num);
        float half = 0.5f;
        float d1 = num - numFloor;
        float d2 = Mathf.Abs(num - (numFloor + half));
        float d3 = numFloor + 1 - num;
        float d = Mathf.Min(d1, d2, d3);
        if(d == d1)
        {
            return numFloor;
        }
        else if(d == d2)
        {
            return numFloor + 0.5f;
        }
        else
        {
            return numFloor + 1f;
        }
    }

    public void exportPoint(Vector3 startPos, int x, int y, float accuracy)
    {
        StringBuilder str = new StringBuilder();
        int[,] list = new int[x, y];
        str.Append("[MAP_DESC]\r\n");
        str.Append("startpos=").Append(startPos).Append("\r\n");
        str.Append("height=").Append(y).Append("\r\nwide=").Append(x).Append("\r\naccuracy=").Append(accuracy).Append("\r\n");
        for (int j = 0; j < y; ++j)
        {
            for (int i = 0; i < x; ++i)
            {
                UnityEngine.AI.NavMeshHit hit;
                for (float k = kmin; k < kmax; k+=0.5f)
                {
                    Vector3 p = startPos + new Vector3(i * accuracy, k, -j * accuracy);
                    if (UnityEngine.AI.NavMesh.SamplePosition(p, out hit, maxdistance, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        //Debug.DrawRay(new Vector3(ConvertToNearestHalfNum(hit.position.x), hit.position.y, ConvertToNearestHalfNum(hit.position.z)), Vector3.up, Color.green);
                        list[(int)(ConvertToNearestHalfNum(hit.position.x) * 2), (int)(ConvertToNearestHalfNum(hit.position.z) * 2)] = 1;
                        break;
                    }
                }
            }
        }
        str.Append("map=");
        for (int j = 0; j < y; ++j)
        {
            str.Append("[");
            for (int i = 0; i < x; ++i)
            {
                str.Append(Mathf.Abs(1 - list[i, j])).Append(",");
            }
            str.Append("],");
        }
        //文件路径
        string path = Application.dataPath + SceneManager.GetActiveScene().name + ".ini";

        //新建文件
        StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.Write(str.ToString());
        streamWriter.Flush();
        streamWriter.Close();


        AssetDatabase.Refresh();
    }
    #endregion

}


[CustomEditor(typeof(NavArrayExport))]
public class NavArrayExportHelper: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Export"))
        {
            var exp = target as NavArrayExport;
            exp.Exp();
        }
    }
}

#endif