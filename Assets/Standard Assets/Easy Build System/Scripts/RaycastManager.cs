using UnityEngine;

public class RaycastManager {
    public static void RaycastNonAlloc(Ray ray, RaycastHit[] hits, float distance,
        int layerMask = -5, QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal) {
        Physics.RaycastNonAlloc(ray, hits, distance, layerMask, interaction);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);
    }
}