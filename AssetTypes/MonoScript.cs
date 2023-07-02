using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Globalization;

namespace AssetStudioExporter.AssetTypes
{
    public class MonoScript : IAssetType, IAssetTypeReader<MonoScript>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.MonoScript;

        public string m_ClassName;
         
        public string m_Namespace;

        public string m_AssemblyName;

        public string FullName
        {
            get
            {
                var ns = string.IsNullOrEmpty(m_Namespace) ? "" : (m_Namespace + ".");
                return ns + m_ClassName;
            }
        }

        public MonoScript(string assembly, string className, string @namespace = "")
        {
            m_AssemblyName = assembly;
            m_ClassName = className;
            m_Namespace = @namespace;
        }

        public static MonoScript Read(AssetTypeValueField value)
        {
            var className = value["m_ClassName"].AsString;
            var ns = value["m_Namespace"].AsString ?? "";
            var asm = value["m_AssemblyName"].AsString;
            return new MonoScript(asm, className, ns);
        }
    }
}
