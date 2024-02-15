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
    public string VersionString => Version.ToString();
    public UnityVersion Version { get; }

    readonly AssetsManager am;
    public AssetTypeManager(string version, AssetsManager manager)
    {
        Version = new UnityVersion(version);
        am = manager;
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
            AssetClassID.Object => Object.EmptyObject.Read(valueField, Version),//0
            AssetClassID.GameObject => GameObject.Read(valueField, Version),//1
            AssetClassID.Component => Component.Read(valueField, Version),//2

            AssetClassID.Texture2D => Texture2D.Read(valueField, Version),//28
            AssetClassID.TextAsset => TextAsset.Read(valueField, Version),//49
            AssetClassID.AudioClip => AudioClip.Read(valueField, Version),//83
            AssetClassID.MonoBehaviour => MonoBehaviour.Read(valueField, Version),//114
            AssetClassID.MonoScript => MonoScript.Read(valueField, Version),//115
            AssetClassID.Font => Font.Read(valueField, Version),//128

            AssetClassID.AssetBundle => AssetBundle.Read(valueField, Version),//142
            AssetClassID.ResourceManager => ResourceManager.Read(valueField, Version),//147

            AssetClassID.Sprite => Sprite.Read(valueField, Version),//213

            AssetClassID.SpriteAtlas => SpriteAtlas.Read(valueField, Version),//687078895

            _ => Object.EmptyObject.Read(valueField, Version),
        };
    }

    public T ReadObject<T>(AssetTypeValueField valueField) where T : IAssetTypeReader<T>
    {
        return T.Read(valueField, Version);
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
