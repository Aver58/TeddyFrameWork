using UnityEngine;

public class TestGPUInstance : MonoBehaviour {
    public GameObject prefab;
    public int InstanceCount = 10;
    
    private Mesh mesh;
    private Material material;
    private Matrix4x4[] matrix;
    private MeshFilter[] meshFilters;
    private Renderer[] renders;

    private Vector4[] colors;
    private MaterialPropertyBlock materialPropertyBlock;
    
    void Awake() {
        TestSkinMesh();
    }
    
    void Update() {
        // 传入mesh、材质、矩阵
        // 可以使用 materialPropertyBlock 覆盖 material
        Graphics.DrawMeshInstanced(mesh, 0, material, matrix, matrix.Length, materialPropertyBlock);
    }

    private void TestSkinMesh() {
        if(prefab == null)
            return;
        var render = prefab.GetComponent<SkinnedMeshRenderer>();
        mesh = render.sharedMesh;
        material = prefab.GetComponent<Renderer>().sharedMaterial;
        if (mesh == null) {
            return;
        }

        if (material == null) {
            return;
        }

        // material.SetTexture("_tex_ani", ); // 现在就是缺动画纹理
        // material.SetInt("_tex_ani_side_length", );// 动画纹理宽度
        InitMatrix();
    }

    private void TestMeshFilter() {
        if(prefab == null)
            return;
        
        var meshFilter = prefab.GetComponent<MeshFilter>();
        if(meshFilter) {
            mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            material = prefab.GetComponent<Renderer>().sharedMaterial;
        }

        InitMatrix();
    }

    private void InitMatrix() {
        matrix = new Matrix4x4[InstanceCount];
        colors = new Vector4[InstanceCount];
        materialPropertyBlock = new MaterialPropertyBlock();
        
        for(int i = 0; i < InstanceCount; i++) {
            float x = Random.Range(-50, 50);
            float y = Random.Range(-3, 3);
            float z = Random.Range(-50, 50);
            matrix[i] = Matrix4x4.identity;  
            //设置位置
            matrix[i].SetColumn(3, new Vector4(x, y, z, 1));  
            //设置缩放，矩阵缩放
            matrix[i].m00   = Mathf.Max(1, 1);
            matrix[i].m11   = Mathf.Max(1, 1);
            matrix[i].m22   = Mathf.Max(1, 1);
            
            // 材质
            colors[i] = new Vector4(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                1);
            materialPropertyBlock.SetVectorArray("_Color", colors);
        }
    }
}