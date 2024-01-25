using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using AssetStudioExporter.AssetTypes;
using AssetStudioExporter.AssetTypes.Feature;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Object = AssetStudioExporter.AssetTypes.Object;

namespace AssetStudioExporter;

public class AssetTypeManager
{
    public string UnityVersion { get;}
    public int[] Version { get; }

    private static readonly Regex regex = new(@"(\d+)\.(\d+)\.(\d+)");
    public AssetTypeManager(string version)
    {
        UnityVersion = version;
        Version = regex
            .Matches(version)[0]
            .Groups.Values
            .Skip(1)
            .Select(v => int.Parse(v.Value))
            .ToArray();
    }

    public T ReadAsset<T>(AssetTypeValueField valueField) where T : IAssetType
    {
        var type = T.AssetClassID;
        return (T)ReadAsset(type, valueField);
    }

    public IAssetType ReadAsset(AssetClassID type, AssetTypeValueField valueField)
    {
        return type switch
        {
            AssetClassID.Object => Object.EmptyObject.Read(valueField),//0
            AssetClassID.GameObject => GameObject.Read(valueField),//1
            AssetClassID.Component => Component.Read(valueField),//2

            AssetClassID.Texture2D => Texture2D.Read(valueField),//28
            AssetClassID.TextAsset => TextAsset.Read(valueField),//49
            AssetClassID.AudioClip => AudioClip.Read(valueField),//83
            AssetClassID.MonoBehaviour => MonoBehaviour.Read(valueField),//114
            AssetClassID.MonoScript => MonoScript.Read(valueField),//115
            AssetClassID.Font => Font.Read(valueField),//128

            AssetClassID.AssetBundle => AssetBundle.Read(valueField),//142
            AssetClassID.ResourceManager => ResourceManager.Read(valueField),//147

            AssetClassID.SpriteAtlas => SpriteAtlas.Read(valueField),//687078895

            _ => Object.EmptyObject.Read(valueField),
        };
    }

    public T ReadObject<T>(AssetTypeValueField valueField) where T : IAssetTypeReader<T>
    {
        return T.Read(valueField);
    }

    public static bool TryGetExporter(IAssetType asset, [NotNullWhen(true)]out IAssetTypeExporter? exporter)
    {
        if (CanExport(asset))
        {
            exporter = (asset as IAssetTypeExporter)!;
            return true;
        }
        exporter = null;
        return false;
    }

    public static bool CanExport<T>()
    {
        return typeof(T).IsAssignableTo(typeof(IAssetTypeExporter));
    } 
    
    public static bool CanExport(IAssetType asset)
    {
        return asset.GetType().IsAssignableTo(typeof(IAssetTypeExporter));
    }

}
