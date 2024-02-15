using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes;


public class ResourceManager : IAssetType, IAssetTypeReader<ResourceManager>
{
    public static AssetClassID AssetClassID { get; } = AssetClassID.ResourceManager;


    public Dictionary<string, PPtr<Object>> m_Container = new();

    public static ResourceManager Read(AssetTypeValueField value, UnityVersion version)
    {
        var rm = new ResourceManager();

        var m_Container = value.Get("m_Container").Get("Array");
        foreach (var container in m_Container.Children)
        {

            var path = container.Get("first").AsString;
            var assetInfo = container.Get("second");

            rm.m_Container[path] = new PPtr<Object>(assetInfo);
        }

        return rm;
    }
}
