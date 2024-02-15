using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes
{
    public class Component : Object, IAssetType, IAssetTypeReader<Component>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Component;

        public PPtr<Object> m_GameObject;

        public Component(PPtr<Object> gameObject)
        {
            m_GameObject = gameObject;
        }

        public static Component Read(AssetTypeValueField value, UnityVersion version)
        {
            var pptr = new PPtr<Object>(value);
            return new Component(pptr);
        }
    }   
}
