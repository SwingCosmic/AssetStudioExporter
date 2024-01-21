namespace AssetStudio;

// 该类型从AnimationClip.cs中剥离并扩充
public struct AABB
{
    public Vector3 m_Center;
    public Vector3 m_Extent;

    public AABB(Vector3 center, Vector3 extent)
    {
        m_Center = center;
        m_Extent = extent;
    }
}