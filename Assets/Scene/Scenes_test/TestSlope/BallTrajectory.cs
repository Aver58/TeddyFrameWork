using System.Collections.Generic;
using UnityEngine;

public class BallTrajectory : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();

    void Update()
    {
        positions.Add(transform.position);
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
