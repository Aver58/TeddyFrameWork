using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;

//此脚本挂在任意物体(空物体也可)
public class gpu_instancing : MonoBehaviour {
    [Header("Required")]
    public SkinnedMeshRenderer skinmesh_render;
    public int gobj_line_count_ = 20;
    public int gobj_count_perline_ = 20;
    public GameObject gobj_gpu_instancing_;
    public AnimationClip ani_clip_;
    public Material mtrl_instance_;
    [Tooltip("主纹理")]public Texture2D tex_surface_;
    
    [Header("Private")]
    [SerializeField] private Texture2D tex_ani_tmp_;
    private Texture2D tex_ani_;
    private int tex_ani_side_length_ = 0;
    private int ani_len_ = 0;
    private int bone_count = 0;
    private float ani_rate_ = 1.0f;
    //gpu instancing shader
    private Material mtrl_work_;
    // 一个矩阵用4个像素去记录
    private const int PIXEL_PER_MATRIX = 4;
    private int ani_idx_ = 0;
    private ComputeBuffer cb_color_;
    private List<float4> lst_color_;
    private ComputeBuffer cb_ani_matrix_index_;
    private List<float> lst_ani_matrix_index_;
    private MaterialPropertyBlock materialPropertyBlock;
    private float[] animMapIndexs;
    
    private void Start() {
        if (gobj_line_count_ * gobj_count_perline_ > 1023) {
            Debug.Log("too many instancing");
        }

        tex_ani_tmp_ = tex_ani_;
        var total_count = gobj_line_count_ * gobj_count_perline_;
        cb_color_ = new ComputeBuffer(total_count, sizeof(float) * 4);
        animMapIndexs = new float[total_count];
        materialPropertyBlock = new MaterialPropertyBlock();;
        lst_color_ = new List<float4>();

        cb_ani_matrix_index_ = new ComputeBuffer(total_count, sizeof(int));
        lst_ani_matrix_index_ = new List<float>();
        mtrl_work_ = new Material(mtrl_instance_);
        mtrl_work_.mainTexture = tex_surface_;

        gen_ani_tex_by_AnimationClip();
    }

    //根据动画剪辑生成骨骼变换矩阵纹理,mesh的顶点属性应有bones index, bones weight属性
    void gen_ani_tex_by_AnimationClip() {
        if (!gobj_gpu_instancing_)
            return;

        var tmp_gobj = GameObject.Instantiate(gobj_gpu_instancing_, new Vector3(0, 0, 0), Quaternion.identity);

        if (!skinmesh_render)
            return;

        //从预制体中取出mesh与骨骼动画
        var skin_render = skinmesh_render;
        var root_name = skin_render.rootBone.name;
        if (!skin_render)
            return;
        var bindposes = skin_render.sharedMesh.bindposes;
        var bones = skin_render.bones;
        bone_count = bones.Length;

        var animator = tmp_gobj.GetComponent<Animator>();
        if (!animator)
            return;

        //从动画状态机里取出动画剪辑
        RuntimeAnimatorController ani_ctrl = animator.runtimeAnimatorController;
        if (!ani_ctrl)
            return;
        if (ani_ctrl.animationClips.Length < 1)
            return;
        var tmp_ani_clip = ani_ctrl.animationClips[0];

        int frame_count = (int)(tmp_ani_clip.frameRate * tmp_ani_clip.length);
        ani_len_ = frame_count;
        //记录动画帧率
        ani_rate_ = 1.0f / tmp_ani_clip.frameRate;

        //录制动画
        {
            animator.applyRootMotion = false;//避免物体跑来跑去
            animator.Rebind();
            animator.recorderStartTime = 0;
            animator.StartRecording(frame_count);
            for (int i = 0; i < frame_count; ++i)
            {
                animator.Update(1.0f / tmp_ani_clip.frameRate);
            }
            animator.StopRecording();
            animator.StartPlayback();
        }

        // 需要保存的矩阵数量 = 帧数 * 骨骼数
        int max_matrix_count = frame_count * bones.Length;
        int pixel_count = max_matrix_count * PIXEL_PER_MATRIX;
        // 开根号，算出最小整数
        int tex_width = (int)Mathf.Ceil(Mathf.Sqrt(pixel_count));
        int tex_height = 1;
        // 左移算出最小2的N次方
        while (tex_height < tex_width)
            tex_height = tex_height << 1;
        tex_ani_side_length_ = tex_width = tex_height;

        //生成一个临时纹理,记录骨骼动画矩阵
        tex_ani_tmp_ = new Texture2D(tex_width, tex_height, TextureFormat.RGBAHalf, false);
        var tex_clr_identity = tex_ani_tmp_.GetPixels();
        int texel_index_identity = 0;

        animator.playbackTime = 0;
        for (int j = 0; j < frame_count; j++) {
            float t = (j / (float)frame_count) * tmp_ani_clip.length;

            //回放动画
            animator.playbackTime = t;
            animator.Update(0);

            for (int bone_idx = 0; bone_idx < bones.Length; ++bone_idx) {
                Transform tmp_bone = bones[bone_idx];
                List<Transform> lst_parent = new List<Transform>();
                lst_parent.Add(tmp_bone);
                while (tmp_bone.name != root_name)
                {
                    tmp_bone = tmp_bone.parent;
                    if (!tmp_bone)
                        break;
                    lst_parent.Add(tmp_bone);
                }

                // 骨骼有根节点, 每个骨骼节点有父子关系,子节点相对父节点有相对的旋转平移,记录为<矩阵M>
                // 根据矩阵的结合律,把子节点依次按父节点回溯,对应的<矩阵M>依次相乘,就可以得到该节点在世界空间中的新的变换矩阵
                Matrix4x4 tmp_mtx = bindposes[bone_idx];
                foreach (var tNode in lst_parent) {
                    var nodeTransform = tNode.transform;
                    Matrix4x4 mat = Matrix4x4.TRS(nodeTransform.localPosition, nodeTransform.localRotation, nodeTransform.localScale);
                    Matrix4x4 tm = tmp_mtx;
                    tmp_mtx = mat * tm;
                }

                // 分4个像素存入贴图
                for (int row_idx = 0; row_idx < 4; ++row_idx) {
                    var row = tmp_mtx.GetRow(row_idx);
                    tex_clr_identity[texel_index_identity++] = new Color(row.x, row.y, row.z, row.w);
                }
                Debug.Log(bone_idx + ":" + bones[bone_idx].name + " " + tmp_mtx);
            }
        }

        tex_ani_tmp_.SetPixels(tex_clr_identity);
        tex_ani_tmp_.Apply();

        DestroyImmediate(tmp_gobj);
    }

    void Update() {
        if (!gobj_gpu_instancing_) {
            Debug.Log("NO gpu game obj");
            return;
        }

        //绘制gpu instancing 100个小熊
        lst_color_.Clear();
        lst_ani_matrix_index_.Clear();
        Vector3 pos = transform.position;
        var matrices = new Matrix4x4[gobj_line_count_ * gobj_count_perline_];
        for (int i = 0; i < gobj_line_count_; i++) {
            for (int j = 0; j < gobj_count_perline_; j++) {
                Vector3 tmppos = pos;
                tmppos.z = pos.z + 1.0f * i;
                tmppos.x = pos.x + 1.0f * j;

                var scale = new Vector3(1.0f, 1.0f, 1.0f);
                Quaternion q = Quaternion.AngleAxis(30, Vector3.up);
                if (i == 0 && (j < 2))
                    q = Quaternion.identity;

                var matrix = Matrix4x4.TRS(tmppos, q, scale);
                matrices[i * gobj_count_perline_ + j] = matrix;

                //每个实例颜色赋值
                lst_color_.Add(new float4((float)i / gobj_line_count_, (float)j / gobj_count_perline_, 0.0f, 1.0f));
                int a_idx = (ani_idx_ + i * 50 * gobj_line_count_ + j * 50 * gobj_count_perline_) % ani_len_;
                lst_ani_matrix_index_.Add(a_idx * bone_count * 4);
                animMapIndexs[i * gobj_count_perline_ + j] = ani_idx_;
            }
        }
        
        ani_idx_ = (int)(Time.time / ani_rate_) % ani_len_;

        cb_color_.SetData(lst_color_);
        cb_ani_matrix_index_.SetData(lst_ani_matrix_index_);

        //将生成的动画矩阵纹理传给shader
        mtrl_work_.SetTexture(Shader.PropertyToID("_tex_ani"), tex_ani_tmp_);
        mtrl_work_.SetInt(Shader.PropertyToID("_tex_ani_side_length"), tex_ani_side_length_);
        mtrl_work_.SetFloat(Shader.PropertyToID("_tex_ani_unit_size"), 1.0f / tex_ani_side_length_);
        //每个实例的颜色数组
        mtrl_work_.SetBuffer(Shader.PropertyToID("_instancing_color"), cb_color_);
        mtrl_work_.SetBuffer(Shader.PropertyToID("_ani_matrix_index"), cb_ani_matrix_index_);
        materialPropertyBlock.SetFloatArray(Shader.PropertyToID("_ani_matrix_index"), animMapIndexs);

        if (mtrl_work_) {
            var instance_mesh = skinmesh_render.sharedMesh;
            Graphics.DrawMeshInstanced(instance_mesh, 0, mtrl_work_, matrices, gobj_line_count_ * gobj_count_perline_, materialPropertyBlock);
        }
    }


    [ContextMenu("export animation to texture")]
    private void write_ske_animation_to_texture() {
        if (!gobj_gpu_instancing_)
            return;

        var tmp_gobj = GameObject.Instantiate(gobj_gpu_instancing_, new Vector3(0, 0, 0), Quaternion.identity);

        if (!skinmesh_render)
            return;
        
        //GPUSkinningBone;
        var bindposes = skinmesh_render.sharedMesh.bindposes;
        var bones = skinmesh_render.bones;
        var bone_weight = skinmesh_render.sharedMesh.boneWeights;
        var root_name = skinmesh_render.rootBone.name;
        bone_count = bones.Length;

        int frame_count = (int)(ani_clip_.frameRate * ani_clip_.length);
        ani_len_ = frame_count;

        int max_matrix_count = frame_count * bones.Length;
        int pixel_count = max_matrix_count * PIXEL_PER_MATRIX;
        // 开根号，算出最小整数，左移算出最小2的N次方
        int tex_width = (int)Mathf.Ceil(Mathf.Sqrt(pixel_count)), tex_height = 1;
        while (tex_height < tex_width)
            tex_height = tex_height << 1;
        tex_ani_side_length_ = tex_width = tex_height;        

        var tex = new Texture2D(tex_width, tex_height, TextureFormat.RGBAFloat, false);
        var tex_data = tex.GetRawTextureData<float>();
        int texel_index = 0;

        var tex1 = new Texture2D(tex_width, tex_height, TextureFormat.RGBAFloat, false);
        var tex_data1 = tex1.GetRawTextureData<float>();
        int texel_index1 = 0;

        var mtx_self = tmp_gobj.transform.localToWorldMatrix;
        Debug.Log("gobj_gpu_instancing_ localToWorldMatrix: " + mtx_self);
        Debug.Log("bones count" + bones.Length + " animation frame " + frame_count + " tex_width" + tex_width);

        var m_d1 = new Dictionary<string, Matrix4x4>();
        var m_d2 = new Dictionary<string, Matrix4x4>();

        for (int i = 0; i < frame_count; i++) {
            float t = (i / (float)frame_count) * ani_clip_.length;
            ani_clip_.SampleAnimation(tmp_gobj, t);

            List<Matrix4x4> lst_mtx_1 = new List<Matrix4x4>();
            List<Matrix4x4> lst_mtx = new List<Matrix4x4>();
            m_d1.Clear();
            m_d2.Clear();
            for (int bone_idx = 0; bone_idx < bones.Length; ++bone_idx) {
                List<Matrix4x4> lst_mtx_tmp = new List<Matrix4x4>(); {
                    Transform tmp_bone = bones[bone_idx];
                    List<Transform> lst_parent = new List<Transform>();
                    lst_parent.Add(tmp_bone);
                    while (tmp_bone.name != root_name) {
                        tmp_bone = tmp_bone.parent;
                        if (!tmp_bone)
                            break;
                        lst_parent.Add(tmp_bone);
                    }

                    Matrix4x4 tmp_mtx = bindposes[bone_idx];
                    lst_mtx_tmp.Add(tmp_mtx);
                    foreach (var tnode in lst_parent) {
                        Matrix4x4 mat = Matrix4x4.TRS(tnode.transform.localPosition, tnode.transform.localRotation, tnode.transform.localScale);
                        lst_mtx_tmp.Add(mat);
                        Matrix4x4 tm = tmp_mtx;
                        tmp_mtx = mat * tm;
                    }

                    for (int row_idx = 0; row_idx < 4; ++row_idx) {
                        var row = tmp_mtx.GetRow(row_idx);

                        tex_data1[texel_index1] = row.x;
                        texel_index1++;
                        tex_data1[texel_index1] = row.y;
                        texel_index1++;
                        tex_data1[texel_index1] = row.z;
                        texel_index1++;
                        tex_data1[texel_index1] = row.w;
                        texel_index1++;
                    }


                    lst_mtx_1.Add(tmp_mtx);
                    m_d1[bones[bone_idx].name] = tmp_mtx;
                    Debug.Log("m_d1" + bone_idx + ":" + bones[bone_idx].name + " " + tmp_mtx);
                }

                {
                    Matrix4x4 matrix;

                    var bone_localToWorldMatrix = bones[bone_idx].localToWorldMatrix;
                    var bone_pos = bindposes[bone_idx];
                    matrix = bone_localToWorldMatrix * bone_pos;
                    for (int row_idx = 0; row_idx < 4; ++row_idx)
                    {
                        var row = matrix.GetRow(row_idx);

                        tex_data[texel_index] = row.x;
                        texel_index++;
                        tex_data[texel_index] = row.y;
                        texel_index++;
                        tex_data[texel_index] = row.z;
                        texel_index++;
                        tex_data[texel_index] = row.w;
                        texel_index++;
                    }

                    lst_mtx.Add(matrix);
                    m_d2[bones[bone_idx].name] = matrix;
                    Debug.Log("m_d2" + bone_idx + ":" + bones[bone_idx].name + " " + matrix);
                }
            }
        }


        Debug.Log("vertex count" + bone_weight.Length);
        for (int k = 0; k < bone_weight.Length; k++) {
            BoneWeight bw = bone_weight[k];

            if (bw.boneIndex0 > bones.Length || bw.boneIndex0 < 0)
                Debug.Log("bone weight out of range" + bw.boneIndex0);
            if (bw.boneIndex1 > bones.Length || bw.boneIndex1 < 0)
                Debug.Log("bone weight out of range" + bw.boneIndex0);
            if (bw.boneIndex2 > bones.Length || bw.boneIndex2 < 0)
                Debug.Log("bone weight out of range" + bw.boneIndex0);
            if (bw.boneIndex3 > bones.Length || bw.boneIndex3 < 0)
                Debug.Log("bone weight out of range" + bw.boneIndex0);
        }
        Debug.Log("end vertex count" + bone_weight.Length);

        var file_stream = File.CreateText(Application.dataPath + "/bone.txt");
        for (int k = 0; k < bones.Length; ++k) {
            Debug.Log(" " + k + " " + bones[k].name);
            file_stream.WriteLine(" " + k + " " + bones[k].name);
        }
        file_stream.Close();

        {
            var file_name = Application.dataPath + "/_ani_bake.exr";
            File.WriteAllBytes(file_name, tex.EncodeToEXR(Texture2D.EXRFlags.CompressZIP));
        }

        {
            var file_name = Application.dataPath + "/_ani_bake_1.exr";
            File.WriteAllBytes(file_name, tex1.EncodeToEXR(Texture2D.EXRFlags.CompressZIP));
        }

        DestroyImmediate(tmp_gobj);
        DestroyImmediate(tex);
        AssetDatabase.Refresh();
    }
}