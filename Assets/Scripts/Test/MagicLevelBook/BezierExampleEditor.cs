using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierExample))]
public class BezierExampleEditor : Editor
{
    void OnSceneGUI()
    {
        BezierExample be = target as BezierExample;

        // be.startPoint = Handles.PositionHandle(be.startPoint, Quaternion.identity);
        // be.endPoint = Handles.PositionHandle(be.endPoint, Quaternion.identity);
        // be.startTangent = Handles.PositionHandle(be.startTangent, Quaternion.identity);
        // be.endTangent = Handles.PositionHandle(be.endTangent, Quaternion.identity);

        // Visualize the tangent lines
        // Handles.DrawDottedLine(be.startPoint, be.startTangent, 5);
        // Handles.DrawDottedLine(be.endPoint, be.endTangent, 5);

        // Handles.DrawBezier(be.startPoint, be.endPoint, be.startTangent, be.endTangent, Color.red, null, 5f);
        Handles.DrawLine(be.startPoint, be.endPoint);
    }
}