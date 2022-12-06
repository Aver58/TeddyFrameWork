using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
// 此示例从头开始创建四边形网格，创建骨骼并分配它们，
// 并根据简单的动画曲线设置骨骼动作的动画以使四边形网格生成动画。
public class TestBindPose : MonoBehaviour {
    private Vector3 bone0OriginPosition = Vector3.zero;
    private Vector3 bone1OriginPosition = new Vector3(0, 5, 0);
    private Vector3 v1 = new Vector3(-1, 0, 0);
    private Vector3 v2 = new Vector3(1, 0, 0);
    private Vector3 v3 = new Vector3(-1, 5, 0);
    private Vector3 v4 = new Vector3(1, 5, 0);
    private GameObject bone0;
    private GameObject bone1;
    private Mesh mesh;
    private SkinnedMeshRenderer render;
    private List<Transform> boneTransforms;
    private List<Vector3> boneOriginPositions;
    
    void Awake() {
        bone0 = new GameObject("Lower");
        bone1 = new GameObject("Upper");
        boneTransforms = new List<Transform>(2);
        boneTransforms.Add(bone0.transform);
        boneTransforms.Add(bone1.transform);
        boneOriginPositions = new List<Vector3>(2);
        boneOriginPositions.Add(bone0OriginPosition);
        boneOriginPositions.Add(bone1OriginPosition);
        
        gameObject.AddComponent<Animation>();
        gameObject.AddComponent<SkinnedMeshRenderer>();
        render = GetComponent<SkinnedMeshRenderer>();
        var anim = GetComponent<Animation>();
        mesh = new Mesh();
        mesh.vertices = new[] {v1, v2, v3, v4};
        mesh.uv = new[] {
            new Vector2(0, 0), 
            new Vector2(1, 0), 
            new Vector2(0, 1), 
            new Vector2(1, 1)
        };
        mesh.triangles = new[] { 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateNormals();
        render.material = new Material(Shader.Find("Diffuse"));
 
        // 将骨骼权重指定给网格
        // 可以用一个，两个或四个骨骼对每个顶点进行修饰，所有骨骼的权重总和为1
        // 下面的例子，我们创建了两个骨骼，一个在Mesh的底部，另一个在Mesh的顶部，
        // 由于每个顶点只受到一个骨骼影响，所以对应weight0都是1
        // 同时，BoneWeight数组与顶点数组一一对应
        // 附着在0，1索引顶点是第0个骨骼, 所以boneIndex0为0 
        // 附着在2索引顶点受2根骨骼影响，boneIndex0为0，权重为0.9f，boneIndex1为1，权重为0.1f
        // 附着在3索引顶点是第1个骨骼, 所以boneIndex0为1
        var weights = new BoneWeight[4];
        weights[0].boneIndex0 = 0;
        weights[0].weight0 = 1;
        weights[1].boneIndex0 = 0;
        weights[1].weight0 = 1;
        weights[2].boneIndex0 = 0;
        weights[2].weight0 = 0.1f;
        weights[2].boneIndex1 = 1;
        weights[2].weight1 = 0.9f;
        weights[3].boneIndex0 = 1;
        weights[3].weight0 = 1;
        mesh.boneWeights = weights;
 
        var bones = new Transform[2];
        var bindPoses = new Matrix4x4[2];
        bones[0] = bone0.transform;
        bones[0].parent = transform;
        bones[0].localRotation = Quaternion.identity;
        bones[0].localPosition = bone0OriginPosition;
        // 绑定姿势是骨骼的逆矩阵。在这种情况下，我们也相对于根生成这个矩阵。 这样我们就可以自由地移动根游戏对象
        bindPoses[0] = bones[0].worldToLocalMatrix * transform.localToWorldMatrix;
 
        bones[1] = bone1.transform;
        bones[1].parent = transform;
        bones[1].localRotation = Quaternion.identity;
        bones[1].localPosition = bone1OriginPosition;
        bindPoses[1] = bones[1].worldToLocalMatrix * transform.localToWorldMatrix;
 
        mesh.bindposes = bindPoses;
        render.bones = bones;
        render.sharedMesh = mesh;
 
        var curve = new AnimationCurve();
        curve.keys = new[] {
            new Keyframe(0, 0, 0, 0), 
            new Keyframe(1, 3, 0, 0), 
            new Keyframe(2, 0.0F, 0, 0)
        };
        var clip = new AnimationClip();
        clip.SetCurve("Lower", typeof(Transform), "m_LocalPosition.z", curve);
        clip.legacy = true;
        clip.wrapMode = WrapMode.Loop;
        anim.AddClip(clip, "test");
        anim.Play("test");
    }

    private void OnDrawGizmos() {
        if (mesh) {
            for (int i = 0; i < mesh.vertices.Length; i++) {
                var vertexOriginPosition = mesh.vertices[i];
                var boneIndex0 = mesh.boneWeights[i].boneIndex0;
                var boneIndex1 = mesh.boneWeights[i].boneIndex1;
                var boneIndex2 = mesh.boneWeights[i].boneIndex2;
                var boneIndex3 = mesh.boneWeights[i].boneIndex3;
                
                var weight0 = mesh.boneWeights[i].weight0;
                var weight1 = mesh.boneWeights[i].weight1;
                var weight2 = mesh.boneWeights[i].weight2;
                var weight3 = mesh.boneWeights[i].weight3;
                
                var matrix0 = boneTransforms[boneIndex0].localToWorldMatrix * mesh.bindposes[boneIndex0];
                var matrix1 = boneTransforms[boneIndex1].localToWorldMatrix * mesh.bindposes[boneIndex1];
                var matrix2 = boneTransforms[boneIndex2].localToWorldMatrix * mesh.bindposes[boneIndex2];
                var matrix3 = boneTransforms[boneIndex3].localToWorldMatrix * mesh.bindposes[boneIndex3];

                var vertexRuntimePosition0 = matrix0.MultiplyPoint(vertexOriginPosition) * weight0;
                var vertexRuntimePosition1 = matrix1.MultiplyPoint(vertexOriginPosition) * weight1;
                var vertexRuntimePosition2 = matrix2.MultiplyPoint(vertexOriginPosition) * weight2;
                var vertexRuntimePosition3 = matrix3.MultiplyPoint(vertexOriginPosition) * weight3;

                var vertexRuntimePosition = vertexRuntimePosition0 + vertexRuntimePosition1 + vertexRuntimePosition2 + vertexRuntimePosition3;
            }

            
            var vertexIndex = 0;
            var boneIndex = mesh.boneWeights[vertexIndex].boneIndex0;
            var v0Matrix = mesh.bindposes[boneIndex] * boneTransforms[boneIndex].localToWorldMatrix;
            var v0Position = v0Matrix.MultiplyPoint(mesh.vertices[vertexIndex]);
            v0Position *= mesh.boneWeights[vertexIndex].weight0;

            vertexIndex++;
            boneIndex = mesh.boneWeights[vertexIndex].boneIndex0;
            var v1Matrix = mesh.bindposes[boneIndex] * boneTransforms[boneIndex].localToWorldMatrix;
            var v1Position = v1Matrix.MultiplyPoint(mesh.vertices[vertexIndex]);
            v1Position *= mesh.boneWeights[vertexIndex].weight0;
            
            // 2根骨骼
            vertexIndex++;
            boneIndex = mesh.boneWeights[vertexIndex].boneIndex0;
            var v2Matrix1 = mesh.bindposes[boneIndex] * boneTransforms[boneIndex].localToWorldMatrix;
            var v2Position1 = v2Matrix1.MultiplyPoint(mesh.vertices[vertexIndex]);
            v2Position1 *= mesh.boneWeights[vertexIndex].weight0;
            boneIndex = mesh.boneWeights[vertexIndex].boneIndex1;
            var v2Matrix2 = mesh.bindposes[boneIndex] * boneTransforms[boneIndex].localToWorldMatrix;
            var v2Position2 = v2Matrix2.MultiplyPoint(mesh.vertices[vertexIndex]);
            v2Position2 *= mesh.boneWeights[vertexIndex].weight1;
            var v2Position = v2Position1 + v2Position2;
            
            Handles.Label(v0Position, "1");
            Handles.Label(v1Position, "2");
            Handles.Label(v2Position, "3");
            Handles.Label(mesh.vertices[3], "4");
        }

        if (bone0) {
            Handles.Label(bone0.transform.position, "bone0");
            Gizmos.DrawWireSphere(bone0.transform.position, 0.1f);
        }

        if (bone1) {
            Handles.Label(bone1.transform.position, "bone1");
            Gizmos.DrawWireSphere(bone1.transform.position, 0.1f);
        }
    }
}
