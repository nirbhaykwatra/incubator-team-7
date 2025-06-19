using UnityEngine;

public enum ColliderType
{
    Box,
    Sphere,
    Mesh,
    Capsule
}

[CreateAssetMenu(fileName = "ResourceData", menuName = "Resources/ResourceData")]
public class ResourceData : ScriptableObject
{
    public Mesh Mesh;
    public ColliderType ColliderType;
    public float DiggingTime;
    public float Hardness;
    public float Rarity;
    public float Health;
}
