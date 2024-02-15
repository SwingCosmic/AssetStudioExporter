using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using AssetStudioExporter.AssetTypes.Feature;
using AssetStudioExporter.AssetTypes.ValueObject;
using AssetStudioExporter.Util;
using System.Reflection.PortableExecutable;

namespace AssetStudioExporter.AssetTypes;

public class Sprite : INamedObject, IAssetType, IAssetTypeReader<Sprite>
{
    public static AssetClassID AssetClassID { get; } = AssetClassID.Sprite;

    public string Name
    {
        get => m_Name;
        set => m_Name = value;
    }

    public string m_Name;
    public Rectf m_Rect;
    public Vector2 m_Offset;
    public Vector4 m_Border;
    public float m_PixelsToUnits;
    public Vector2 m_Pivot = new Vector2(0.5f, 0.5f);
    public uint m_Extrude;
    public bool m_IsPolygon;
    public KeyValuePair<Guid, long> m_RenderDataKey;
    public string[] m_AtlasTags;
    public PPtr<SpriteAtlas> m_SpriteAtlas;
    public SpriteRenderData m_RD;
    public List<Vector2[]> m_PhysicsShape;
    /// <summary>
    /// 不知道是什么
    /// </summary>
    public List<object> m_Bones;


    public static Sprite Read(AssetTypeValueField value)
    {
        var s = new Sprite();
        s.m_Name = value["m_Name"].AsString;
        s.m_Rect = Rectf.Read(value["m_Rect"]);
        s.m_Border = value["m_Border"].AsVector4();
        s.m_PixelsToUnits = value["m_PixelsToUnits"].AsFloat;
        s.m_Pivot = value["m_Pivot"].AsVector2();
        s.m_Extrude = value["m_Extrude"].AsUInt;
        s.m_IsPolygon = value["m_IsPolygon"].AsBool;

        //if (version[0] >= 2017) //2017 and up
        var m_RenderDataKey = value["m_RenderDataKey"];
        var first = m_RenderDataKey["first"].AsUnityGUID();
        var second = m_RenderDataKey["second"].AsLong;
        s.m_RenderDataKey = new KeyValuePair<Guid, long>(first, second);

        s.m_AtlasTags = value["m_AtlasTags"]
            .AsArray(t => t.AsString);

        s.m_SpriteAtlas = PPtr<SpriteAtlas>.Read(value["m_SpriteAtlas"]);
        //}

        s.m_RD = SpriteRenderData.Read(value["m_RD"]);

        //if (version[0] >= 2017)
        s.m_PhysicsShape = value["m_PhysicsShape"]
            .AsList(s => s.AsArray(v => v.AsVector2()));
        //}

        //if (version[0] >= 2018) //2018 and up
        s.m_Bones = value["m_Bones"].AsList(b => (object)b.ToString());
        //}

        return s;
    }
}
