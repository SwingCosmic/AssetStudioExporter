using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes
{
    public interface IAssetType
    {
        static abstract AssetClassID AssetClassID { get; }
    }
}
