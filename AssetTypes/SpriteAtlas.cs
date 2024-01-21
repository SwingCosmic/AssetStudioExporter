using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudioExporter.AssetTypes.Feature;
using AssetStudioExporter.AssetTypes.ValueObject;
using AssetStudioExporter.Util;
using System.Runtime.InteropServices;

namespace AssetStudioExporter.AssetTypes;

public class SpriteAtlas : INamedObject, IAssetType, IAssetTypeReader<SpriteAtlas>
{
    public static AssetClassID AssetClassID { get; } = AssetClassID.SpriteAtlas;

    public string Name
    {
        get => m_Name;
        set => m_Name = value;
    }

    public string m_Name;
    public List<PPtr<Sprite>> m_PackedSprites;
    public List<string> m_PackedSpriteNamesToIndex;
    public Dictionary<KeyValuePair<Guid, long>, SpriteAtlasData> m_RenderDataMap;
    public string m_Tag;
    public bool m_IsVariant;

    public static SpriteAtlas Read(AssetTypeValueField value)
    {
        var sa = new SpriteAtlas();

        sa.m_Name = value["m_Name"].AsString;
        sa.m_PackedSprites = value["m_PackedSprites"]
            .AsList(sprite => new PPtr<Sprite>(sprite));
        sa.m_PackedSpriteNamesToIndex = value["m_PackedSpriteNamesToIndex"]
            .AsList(name => name.AsString);

        sa.m_RenderDataMap = new();
        var renderDataMap = value["m_RenderDataMap"]["Array"];
        foreach (var pair in renderDataMap.Children)
        {
            var key = pair["first"];
            var guid = key["first"].AsUnityGUID();
            var id = key["second"].AsLong;

            var v = SpriteAtlasData.Read(pair["second"]);

            sa.m_RenderDataMap[new(guid, id)] = v;
        }

        sa.m_Tag = value["m_Tag"].AsString;
        sa.m_IsVariant = value["m_IsVariant"].AsBool;

        return sa;
    }
}
