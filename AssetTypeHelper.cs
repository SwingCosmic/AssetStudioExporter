using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using AssetStudioExporter.AssetTypes;
using AssetStudioExporter.AssetTypes.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = AssetStudioExporter.AssetTypes.Object;

namespace AssetStudioExporter;

public class AssetTypeHelper
{
    public static IAssetType ReadAsset(AssetFileInfo asset, AssetTypeValueField valueField)
    {
        return (AssetClassID)asset.TypeId switch
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

            _ => Object.EmptyObject.Read(valueField),
        };
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
