using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudioExporter.AssetTypes.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes;


public class AssetInfo
{
    public int preloadIndex;
    public int preloadSize;
    public PPtr<Object> asset;

    public AssetInfo(int preloadIndex, int preloadSize, PPtr<Object> asset)
    {
        this.preloadIndex = preloadIndex;
        this.preloadSize = preloadSize;
        this.asset = asset;
    }
}

public class AssetBundle : INamedObject, IAssetType, IAssetTypeReader<AssetBundle>
{
    public static AssetClassID AssetClassID { get; } = AssetClassID.AssetBundle;
    public string Name
    {
        get => m_Name;
        set => m_Name = value;
    }

    public string m_Name;
    public Dictionary<string, AssetInfo> m_Container = new();

    public static AssetBundle Read(AssetTypeValueField value)
    {
        var ab = new AssetBundle();
        ab.m_Name = value["m_Name"].AsString;

        var m_Container = value.Get("m_Container").Get("Array");
        foreach (var container in m_Container.Children)
        {

            var path = container.Get("first").AsString;
            var assetInfo = container.Get("second");

            var preloadIndex = assetInfo.Get("preloadIndex").AsInt;
            var preloadSize = assetInfo.Get("preloadSize").AsInt;
            var pptr = new PPtr<Object>(assetInfo.Get("asset"));

            ab.m_Container[path] = new AssetInfo(preloadIndex, preloadSize, pptr);
        }

        return ab;
    }
}
