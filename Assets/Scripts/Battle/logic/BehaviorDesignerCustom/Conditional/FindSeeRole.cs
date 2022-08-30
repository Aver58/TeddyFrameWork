namespace Battle.logic.BehaviorDesignerCustom.Conditional {
    
    using BehaviorDesigner.Runtime.Tasks;
    using UnityEngine;

    public sealed class FindSeeRole : Conditional {
        [SerializeField]
        private float fieldOfViewAngle = 60;
        [SerializeField]
        private float viewDistance = 5;

        public override void OnStart() {
            
        }
        
        public override void OnDrawGizmos() {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = Color.yellow;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;
            var halfFov = fieldOfViewAngle * 0.5f;
            Transform ownerTransform;
            var beginDirection = Quaternion.AngleAxis(-halfFov, Vector3.up) * (ownerTransform = Owner.transform).forward;
            UnityEditor.Handles.DrawSolidArc(ownerTransform.position, ownerTransform.up, beginDirection, fieldOfViewAngle, viewDistance);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}