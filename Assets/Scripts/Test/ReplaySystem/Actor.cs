using UnityEngine;

public abstract class Actor : MonoBehaviour {
    [HideInInspector]
    public int ActorId;
    // 先用Json，后续改成二进制
    public abstract string Serialize();
    public abstract void Deserialize(string data);
}