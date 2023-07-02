using AssetsTools.NET;

namespace AssetStudioExporter.AssetTypes
{
    public class MonoBehaviourCollection : List<MonoBehaviour>
    {
        public MonoBehaviour? GetMonoBehaviourByClassName(string fullClassName)
        {
            return this.FirstOrDefault(m =>
            {
                var script = m.m_Script.Instance ?? 
                    throw new InvalidOperationException("The PPtr doesn't have an instance");

                return script.FullName == fullClassName;
            });
        }

        public MonoBehaviour? GetMonoBehaviourByClassName(string @namespace, string className)
        {
            return GetMonoBehaviourByClassName(@namespace + "." + className);
        }
    }
}
