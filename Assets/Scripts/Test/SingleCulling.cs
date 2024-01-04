using UnityEngine;

public class SingleCulling : MonoBehaviour {
    public float cullingRadius = 1;
    public Vector3 offest;
    public Camera targetCamera;

    private CullingGroup mCullingGroup;

    void Awake() {
        mCullingGroup = new CullingGroup();
        mCullingGroup.SetBoundingSphereCount(1);
        mCullingGroup.onStateChanged += OnStateChanged;
    }

    void Start() {
        UpdateCullingGroup();
    }

    public bool IsVisible() {
        return mCullingGroup.IsVisible(0);
    }

    public void UpdateCullingGroup() {
        if (mCullingGroup == null)
            return;

        mCullingGroup.targetCamera = targetCamera? targetCamera : Camera.main;
        mCullingGroup.SetBoundingSpheres(new BoundingSphere[] { new BoundingSphere(transform.TransformPoint(offest), cullingRadius) });
    }

    void OnStateChanged(CullingGroupEvent sphere) {
        if (sphere.isVisible)
            OnBecameVisible();
        else
            OnBecameInvisible();
    }

    protected virtual void OnBecameVisible() {
    }

    protected virtual void OnBecameInvisible() {
    }

    void OnDestroy() {
        if (mCullingGroup != null)
            mCullingGroup.Dispose();
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.TransformPoint(offest), cullingRadius);
    }

    private void OnValidate() {
        UpdateCullingGroup();
    }
#endif
}