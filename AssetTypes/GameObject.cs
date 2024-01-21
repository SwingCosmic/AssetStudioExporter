using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace AssetStudioExporter.AssetTypes
{
    public class GameObject : Object, IAssetType, IAssetTypeReader<GameObject>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.GameObject;

        public string m_Name;
        public IList<PPtr<Component>> m_Components;
        



        public static GameObject Read(AssetTypeValueField value)
        {
            var gameObject = new GameObject();

            gameObject.m_Name = value["m_Name"].AsString;
            gameObject.m_Components = new List<PPtr<Component>>();

            var components = value["m_Component"]["Array"];
            foreach (var comp in components.Children)
            {
                var childPtr = comp["component"];
                gameObject.m_Components.Add(new PPtr<Component>(childPtr));
            }

            return gameObject;
        }

        public MonoBehaviourCollection GetAllMonoBehaviours(AssetsFileInstance inst, AssetsManager am)
        {
            var ret = new MonoBehaviourCollection();
            foreach (var componentPtr in m_Components)
            {
                var componentRef = componentPtr.GetAssetReference(inst, am, true);
                var childInfo = componentRef.info;

                if (childInfo.TypeId != (int)AssetClassID.MonoBehaviour) continue;

                var monoBehaviourPtr = componentPtr.Cast<MonoBehaviour>();
                monoBehaviourPtr.FindInstance<MonoBehaviour>(inst, am);

                var mb = monoBehaviourPtr.Instance;
                mb.AssetInfo = childInfo;

                mb.m_Script.FindInstance<MonoScript>(inst, am);

                ret.Add(mb);

            }
            return ret;
        }
    }
}
