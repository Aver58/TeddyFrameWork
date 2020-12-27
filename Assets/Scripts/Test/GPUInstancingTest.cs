using UnityEngine;

public class GPUInstancingTest : MonoBehaviour
{
    public Transform m_Transform;
    private MeshRenderer m_Renderer;
    private Mesh m_Mesh;
    private Matrix4x4[] m_Matrix;

    void Start()
    {
        m_Renderer = m_Transform.GetComponent<MeshRenderer>();
        var meshFilter = m_Transform.GetComponent<MeshFilter>();
        m_Mesh = meshFilter.sharedMesh;
        m_Matrix = new Matrix4x4[100];
        for(int i = 0; i < m_Matrix.Length; i++)
        {
            var position = Random.insideUnitSphere * 5;
            var rotation = Quaternion.LookRotation(Random.insideUnitSphere);
            var scale = Vector3.one * Random.Range(-2f, 2f);
            var matrix = Matrix4x4.TRS(position, rotation, scale);

            m_Matrix[i] = matrix;
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(m_Mesh, 0, m_Renderer.sharedMaterial, m_Matrix);
    }
}