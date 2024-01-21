using AssetsTools.NET;
using AssetStudioExporter.AssetTypes;
using System.Runtime.InteropServices;
using System.Xml;

namespace AssetStudio;

// 该类型从Sprite.cs中剥离并扩充

[StructLayout(LayoutKind.Sequential)]
public struct Rectf : IEquatable<Rectf>, IAssetTypeReader<Rectf>
{
    public float x;
    public float y;
    public float width;
    public float height;

    public bool Equals(Rectf other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, width, height);
    }

    public override bool Equals(object obj)
    {
        return obj is Rectf rectf && Equals(rectf);
    }

    public static bool operator==(Rectf left, Rectf right)
    {
        return left.x == right.x
            && left.y == right.y
            && left.width == right.width
            && left.height == right.height;
    }        
    
    public static bool operator!=(Rectf left, Rectf right)
    {
        return !(left == right);
    }


    public static Rectf Read(AssetTypeValueField value)
    {
        return new Rectf()
        {
            x = value["x"].AsFloat,
            y = value["y"].AsFloat,
            width = value["width"].AsFloat,
            height = value["height"].AsFloat,
        };
    }

}
