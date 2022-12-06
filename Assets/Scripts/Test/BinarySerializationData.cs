using UnityEngine;

// Custom asset type that prefers binary serialization.
//
// Create a new asset file by going to "Asset/Create/BinarySerializationData".
// If you open this new asset in a text editor, you can see how it
// is not affected by changing the project asset serialization mode.
//

// 适用于大批量数据的序列化
[CreateAssetMenu(menuName = "Tools/BinarySerializationData")]
[PreferBinarySerialization]
public class BinarySerializationData : ScriptableObject {
    public float[] lotsOfFloatData = new[] { 1f, 2f, 3f };
    public byte[] lotsOfByteData = new byte[] { 4, 5, 6 };
}