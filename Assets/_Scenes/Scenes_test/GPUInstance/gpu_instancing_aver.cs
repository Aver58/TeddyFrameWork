#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using UnityEditor;

//此脚本挂在任意物体(空物体也可) gpu_instancing 魔改版本
public class gpu_instancing_aver : MonoBehaviour {
    [Header("Required")]
    public SkinnedMeshRenderer skinmesh_render;
    public int gobj_line_count_ = 20;
    public int gobj_count_perline_ = 20;
    public GameObject gobj_gpu_instancing_;
    public Material mtrl_instance_;
    [Tooltip("主纹理")]public Texture2D tex_surface_;
    
    [Header("Private 可视化")]
    [SerializeField] private Texture2D tex_ani_tmp_;
    [SerializeField] private int tex_ani_side_length_ = 0;
    [SerializeField] private int ani_len_ = 0;
    [SerializeField] private int bone_count = 0;
    [SerializeField] private float ani_rate_ = 1.0f;
    [SerializeField] private Material mtrl_work_;
    // 一个矩阵用4个像素去记录
    private const int PIXEL_PER_MATRIX = 4;
    [SerializeField] private int ani_idx_ = 0;
    private AnimationClip ani_clip_;
    private Texture2D tex_ani_;
    private ComputeBuffer cb_color_;
    private List<float4> lst_color_;
    private List<float> lst_ani_matrix_index_;
    private MaterialPropertyBlock materialPropertyBlock;
    private float[] animMapIndexs;
    private bool isGenerated = false;
    
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
        
        lst_ani_matrix_index_ = new List<float>();
        mtrl_work_ = new Material(mtrl_instance_);
        mtrl_work_.mainTexture = tex_surface_;

        isGenerated = false;
        StartCoroutine(gen_ani_tex_by_AnimationClip());
    }

    private int frame = 0;
    private void FixedUpdate() {
        if (isGenerated == false) {
            
            frame++;
        }
    }

    //根据动画剪辑生成骨骼变换矩阵纹理,mesh的顶点属性应有bones index, bones weight属性
    IEnumerator gen_ani_tex_by_AnimationClip() {
        if (!gobj_gpu_instancing_)
            yield return null;

        var obj = GameObject.Instantiate(gobj_gpu_instancing_, new Vector3(0, 0, 0), Quaternion.identity);
        if (!skinmesh_render)
            yield return null;

        //从预制体中取出mesh与骨骼动画
        var skin_render = obj.GetComponentInChildren<SkinnedMeshRenderer>();
        var root_name = skin_render.rootBone.name;
        if (!skin_render)
            yield return null;
        var bindposes = skin_render.sharedMesh.bindposes;
        var bones = skin_render.bones;
        bone_count = bones.Length;

        var animator = obj.GetComponent<Animator>();
        if (!animator)
            yield return null;

        //从动画状态机里取出动画剪辑
        RuntimeAnimatorController ani_ctrl = animator.runtimeAnimatorController;
        if (!ani_ctrl)
            yield return null;
        if (ani_ctrl.animationClips.Length < 1)
            yield return null;
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        var tmp_ani_clip = clipInfo.clip;
        
        int frame_count = (int)(tmp_ani_clip.frameRate * tmp_ani_clip.length);
        ani_len_ = frame_count;
        //记录动画帧率
        ani_rate_ = 1.0f / tmp_ani_clip.frameRate;

        animator.applyRootMotion = false;//避免物体跑来跑去
        animator.Rebind();
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
     
        for (int j = 0; j < frame_count; j++) {
            float t = (j / (float)frame_count) * tmp_ani_clip.length;
            // 重置动画
            obj.SetActive(false);
            obj.SetActive(true);
            // 更新到指定时间
            animator.Update(t);

            for (int bone_idx = 0; bone_idx < bones.Length; ++bone_idx) {
                // 获取骨骼节点树
                Transform tmp_bone = bones[bone_idx];
                List<Transform> lst_parent = new List<Transform>();
                lst_parent.Add(tmp_bone);
                while (tmp_bone.name != root_name) {
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
                // Debug.Log(bone_idx + ":" + bones[bone_idx].name + "\r\n" + tmp_mtx);
            }
            // 直接animator.Update 动作可能在下一帧变化，改成了在协程，每帧变化
            yield return null;
        }
        
        Debug.Log($"总长度：{texel_index_identity} = 帧数：{frame_count} * 骨骼数：{bone_count} * 4");
        tex_ani_tmp_.SetPixels(tex_clr_identity);
        tex_ani_tmp_.Apply();
        
        DestroyImmediate(obj);

        isGenerated = true;
        yield return null;
    }

    void Update() {
        if (!gobj_gpu_instancing_) {
            Debug.Log("NO gpu game obj");
            return;
        }
        
        if (isGenerated == false) {
            return;
        }
        
        //绘制gpu instancing
        lst_color_.Clear();
        lst_ani_matrix_index_.Clear();
        Vector3 pos = transform.position;
        var matrices = new Matrix4x4[gobj_line_count_ * gobj_count_perline_];
        for (int i = 0; i < gobj_line_count_; i++) {
            for (int j = 0; j < gobj_count_perline_; j++) {
                Vector3 tmpPos = pos;
                // 位置
                tmpPos.z = pos.z + 5.0f * i;
                tmpPos.x = pos.x + 5.0f * j;
                var matrix = Matrix4x4.TRS(tmpPos, Quaternion.identity, Vector3.one);
                matrices[i * gobj_count_perline_ + j] = matrix;
        
                //每个实例颜色赋值
                lst_color_.Add(new float4((float)i / gobj_line_count_, (float)j / gobj_count_perline_, 0.0f, 1.0f));
                int a_idx = (ani_idx_ + i * 50 * gobj_line_count_ + j * 50 * gobj_count_perline_) % ani_len_;
                var pixelStartIndex = a_idx * bone_count * 4;
                animMapIndexs[i * gobj_count_perline_ + j] = ani_idx_ * bone_count * 4;
            }
        }
        
        // 当前播放帧
        ani_idx_ = (int)(Time.time / ani_rate_) % ani_len_;
        cb_color_.SetData(lst_color_);
        
        //将生成的动画矩阵纹理传给shader
        mtrl_work_.SetTexture(Shader.PropertyToID("_tex_ani"), tex_ani_tmp_);
        mtrl_work_.SetInt(Shader.PropertyToID("_tex_ani_side_length"), tex_ani_side_length_);
        //2种传递实例参数方式：
        //方式一：shader设置变量，传入变量，然后shader里用[unity_InstanceID] 获取实例变量
        //方式二：接口用 materialPropertyBlock 传入，shader中用 UNITY_ACCESS_INSTANCED_PROP(Props, _ani_matrix_index)获取实例变量
        mtrl_work_.SetBuffer(Shader.PropertyToID("_instancing_color"), cb_color_);
        materialPropertyBlock.SetFloatArray(Shader.PropertyToID("_ani_matrix_index"), animMapIndexs);
        
        if (mtrl_work_) {
            Graphics.DrawMeshInstanced(skinmesh_render.sharedMesh, 0, mtrl_work_, matrices, gobj_line_count_ * gobj_count_perline_, materialPropertyBlock);
        }
    }

    void OnDestroy() {
        cb_color_.Dispose();
    }
    
    [ContextMenu("export animation to texture")]
    private void write_ske_animation_to_texture() {
        if (tex_ani_tmp_ == null) {
            Debug.LogError("请在运行时使用！");
            return;
        }

        // 纹理
        var file_name = Application.dataPath + "/_ani_bake.exr";
        File.WriteAllBytes(file_name, tex_ani_tmp_.EncodeToEXR(Texture2D.EXRFlags.CompressZIP));
        AssetDatabase.Refresh();
        
        // 骨骼
        var bones = skinmesh_render.bones;
        var file_stream = File.CreateText(Application.dataPath + "/bone.txt");
        for (int k = 0; k < bones.Length; ++k) {
            Debug.Log(" " + k + " " + bones[k].name);
            file_stream.WriteLine(" " + k + " " + bones[k].name);
        }
        file_stream.Close();
    }
}

#endif