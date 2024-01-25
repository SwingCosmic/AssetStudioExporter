using AssetsTools.NET;
using AssetStudio;
using AssetStudioExporter.Util;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.PortableExecutable;

namespace AssetStudioExporter.AssetTypes.ValueObject;

public class SecondarySpriteTexture
{
    public PPtr<Texture2D> texture;
    public string name;

    public SecondarySpriteTexture(AssetTypeValueField value)
    {
        texture = PPtr<Texture2D>.Read(value["texture"]);
        name = value["name"].AsString;
    }
}

public enum SpritePackingRotation
{
    None = 0,
    FlipHorizontal = 1,
    FlipVertical = 2,
    Rotate180 = 3,
    Rotate90 = 4
};

public enum SpritePackingMode
{
    Tight = 0,
    Rectangle
};

public enum SpriteMeshType
{
    FullRect,
    Tight
};

public struct SpriteSettings : IEquatable<SpriteSettings>
{
    public uint settingsRaw;

    public uint packed => settingsRaw & 1; //1
    public SpritePackingMode packingMode => (SpritePackingMode)(settingsRaw >> 1 & 1); //1
    public SpritePackingRotation packingRotation => (SpritePackingRotation)(settingsRaw >> 2 & 0xf); //4
    public SpriteMeshType meshType => (SpriteMeshType)(settingsRaw >> 6 & 1); //1

    //reserved

    public SpriteSettings(uint settingsRaw)
    {
        this.settingsRaw = settingsRaw;
    }

    public static implicit operator uint(SpriteSettings spriteSettings)
    {
        return spriteSettings.settingsRaw;
    }   
    
    public static explicit operator SpriteSettings(uint settingsRaw)
    {
        return new SpriteSettings(settingsRaw);
    }

    public bool Equals(SpriteSettings other)
    {
        return this == other;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is SpriteSettings settings && Equals(settings);
    }

    public override int GetHashCode()
    {
        return settingsRaw.GetHashCode();
    }

    public static bool operator ==(SpriteSettings left, SpriteSettings right)
    {
        return left.settingsRaw == right.settingsRaw;
    }

    public static bool operator !=(SpriteSettings left, SpriteSettings right)
    {
        return !(left == right);
    }
}

public class SpriteVertex : IAssetTypeReader<SpriteVertex>
{
    public Vector3 pos;
    public Vector2 uv;

    public static SpriteVertex Read(AssetTypeValueField value)
    {
        var vertex = new SpriteVertex();
        vertex.pos = value["pos"].AsVector3();
        //if (version[0] < 4 || (version[0] == 4 && version[1] <= 3)) //4.3 and down
        //{
        //    uv = reader.ReadVector2();
        //}

        return vertex;
    }
}

public class SpriteAtlasData : IAssetTypeReader<SpriteAtlasData>
{
    public PPtr<Texture2D> texture = null!;
    public PPtr<Texture2D> alphaTexture = null!;
    public Rectf textureRect;
    public Vector2 textureRectOffset;
    public Vector2 atlasRectOffset;
    public Vector4 uvTransform;
    public float downscaleMultiplier;
    public SpriteSettings settingsRaw;
    public List<SecondarySpriteTexture>? secondaryTextures;


    public static SpriteAtlasData Read(AssetTypeValueField value)
    {
        var data = new SpriteAtlasData();

        data.texture = new PPtr<Texture2D>(value["texture"]);
        data.alphaTexture = new PPtr<Texture2D>(value["alphaTexture"]);
        data.textureRect = Rectf.Read(value["textureRect"]);
        data.textureRectOffset = value["textureRectOffset"].AsVector2();

        var atlasRectOffsetValue = value["atlasRectOffset"];
        if (!atlasRectOffsetValue.IsDummy)
        {
            data.atlasRectOffset = atlasRectOffsetValue.AsVector2();
        }

        data.uvTransform = value["uvTransform"].AsVector4();
        data.downscaleMultiplier = value["downscaleMultiplier"].AsFloat;
        data.settingsRaw = new SpriteSettings(value["settingsRaw"].AsUInt);

        var secondaryTexturesValue = value["secondaryTextures"];
        if (!secondaryTexturesValue.IsDummy)
        {
            data.secondaryTextures = secondaryTexturesValue
                .AsList(texture => new SecondarySpriteTexture(texture));
        }

        return data;
    }
}

public class SpriteRenderData
{
    public PPtr<Texture2D> texture;
    public PPtr<Texture2D> alphaTexture;
    public List<SecondarySpriteTexture> secondaryTextures;
    public List<SubMesh> m_SubMeshes;
    public byte[] m_IndexBuffer;
    public VertexData m_VertexData;
    public List<SpriteVertex> vertices;
    public ushort[] indices;
    public Matrix4x4[] m_Bindpose;
    public List<BoneWeights4> m_SourceSkin;
    public Rectf textureRect;
    public Vector2 textureRectOffset;
    public Vector2 atlasRectOffset;
    public SpriteSettings settingsRaw;
    public Vector4 uvTransform;
    public float downscaleMultiplier;


}