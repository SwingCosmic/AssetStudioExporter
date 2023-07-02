using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes
{
    public class Component : IAssetType, IAssetTypeReader<Component>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Component;

        public PPtr<object> m_GameObject;

        public Component(PPtr<object> gameObject)
        {
            m_GameObject = gameObject;
        }

        public static Component Read(AssetTypeValueField value)
        {
            var pptr = new PPtr<object>(value);
            return new Component(pptr);
        }
    }   
}
