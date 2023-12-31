using AssetsTools.NET;
using AssetsTools.NET.Extra;
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

public class AssetBundle : IAssetType, IAssetTypeReader<AssetBundle>
{
    public static AssetClassID AssetClassID { get; } = AssetClassID.AssetBundle;


    public Dictionary<string, AssetInfo> m_Container = new();

    public static AssetBundle Read(AssetTypeValueField value)
    {
        var ab = new AssetBundle();

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
