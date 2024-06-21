using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicLevelBookMono : MonoBehaviour {
    public MagicLevelBookSO MagicLevelBookSO;
    [SerializeField] private List<Transform> transforms;

#if UNITY_EDITOR
    [ContextMenu("预览 起始点")]
    private void ReadStartPoint() {
        var config = MagicLevelBookSO as MagicLevelBookSO;
        if (config == null) {
            return;
        }

        if (transforms.Count != config.books.Count) {
            Debug.LogError("MagicLevelBookSO books 的数量和 MagicLevelBookEditor 的 Transforms 的数量对不上！");
            return;
        }

        for (int i = 0; i < config.books.Count; i++) {
            var book = config.books[i];
            var bookIns = transforms[i];
            bookIns.position = book.StartPoint;
            bookIns.rotation = book.StartRotation;
        }
    }

    [ContextMenu("预览 终点")]
    private void ReadEndPoint() {
        var config = MagicLevelBookSO as MagicLevelBookSO;
        if (config == null) {
            return;
        }

        var bookGroup = config;
        if (transforms.Count != bookGroup.books.Count) {
            Debug.LogError("MagicLevelBookSO books 的数量和 MagicLevelBookEditor 的 Transforms 的数量对不上！");
            return;
        }

        for (int i = 0; i < bookGroup.books.Count; i++) {
            var book = bookGroup.books[i];
            var bookIns = transforms[i];
            bookIns.position = book.EndPoint;
            bookIns.rotation = book.EndRotation;
        }
    }

    [ContextMenu("保存 起始点")]
    private void SaveStartPoint() {
        var config = MagicLevelBookSO;
        if (config == null) {
            return;
        }

        config.StartPosition = transform.position;
        config.StartRotation = transform.rotation;

        for (int i = 0; i < transforms.Count; i++) {
            var book = transforms[i];

            if (i < config.books.Count) {
                config.books[i] = new Book {
                    StartPoint = book.position,
                    StartRotation = book.rotation,
                    EndPoint = config.books[i].EndPoint,
                    EndRotation = config.books[i].EndRotation,
                };
            } else {
                config.books.Add(new Book {
                    StartPoint = book.position,
                    StartRotation = book.rotation,
                });
            }

            var bezierExample = book.GetComponent<BezierExample>();
            if (bezierExample != null) {
                bezierExample.startPoint = config.books[i].StartPoint;
                bezierExample.endPoint = config.books[i].EndPoint;
            }
        }

        EditorUtility.SetDirty(MagicLevelBookSO);
        AssetDatabase.Refresh();
    }

    [ContextMenu("保存 终点")]
    private void SaveEndPoint() {
        var config = MagicLevelBookSO;
        if (config == null) {
            return;
        }

        for (int i = 0; i < transforms.Count; i++) {
            var book = transforms[i];
            if (i < config.books.Count) {
                config.books[i] = new Book {
                    StartPoint = config.books[i].StartPoint,
                    StartRotation = config.books[i].StartRotation,
                    EndPoint = book.position,
                    EndRotation = book.rotation,
                };
            } else {
                config.books.Add(new Book {
                    EndPoint = book.position,
                    EndRotation = book.rotation,
                });
            }

            var bezierExample = book.GetComponent<BezierExample>();
            if (bezierExample != null) {
                bezierExample.startPoint = config.books[i].StartPoint;
                bezierExample.endPoint = config.books[i].EndPoint;
            }
        }

        EditorUtility.SetDirty(MagicLevelBookSO);
        AssetDatabase.Refresh();
    }

    // void OnSceneGUI()
    // {
    //     if (MagicLevelBookSO == null) {
    //         return;
    //     }
    //
    //     for (int i = 0; i < MagicLevelBookSO.books.Count; i++) {
    //         var book = MagicLevelBookSO.books[i];
    //         Handles.DrawLine(book.StartPoint, book.EndPoint);
    //     }
    // }

#endif

    private float percent;
    private int bookCount;
    private void Start() {
        bookCount = transforms.Count;
        CalculateRandomControlPoint();
    }

    private void CalculateRandomControlPoint()
    {
        for (int i = 0; i < bookCount; i++) {
            var config = MagicLevelBookSO.books[i];
            var direction = config.EndPoint - config.StartPoint;
            var middlePoint = config.StartPoint + direction * MagicLevelBookSO.ControlPointDirectionOffset;
            MagicLevelBookSO.books[i].ControlPoint = middlePoint + Random.onUnitSphere * MagicLevelBookSO.ControlPointSphereRadius;
        }
    }

    private void Update() {
        if (transforms == null ||
            bookCount <= 0 ||
            MagicLevelBookSO == null) {
            return;
        }

        for (int i = 0; i < bookCount; i++) {
            var bookIns = transforms[i];
            var config = MagicLevelBookSO.books[i];
            if (config.StartPoint == config.EndPoint) {
                Debug.LogError("起始位置和终点位置一致！");
                continue;
            }

            var distance = Vector3.Distance(config.EndPoint, config.ControlPoint) + Vector3.Distance(config.StartPoint, config.ControlPoint);
            var percentSpeed = MagicLevelBookSO.FlySpeed / distance;
            percent += percentSpeed * Time.deltaTime;
            var curveValue = MagicLevelBookSO.AnimationCurve.Evaluate(percent);
            var pos = CalculateBezierPoint(curveValue, config.StartPoint, config.ControlPoint, config.EndPoint);
            bookIns.position = pos;

            var targetRotation = Quaternion.Lerp(config.StartRotation, config.EndRotation, percent);
            bookIns.rotation = targetRotation;
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0,Vector3 p1, Vector3 p2) {
        if (t > 1) {
            t = 1;
        }

        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
