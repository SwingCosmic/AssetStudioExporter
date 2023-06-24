using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes
{
    public class MonoScript : IAssetTypeReader<MonoScript>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.MonoScript;

        public string m_ClassName;
         
        public string m_Namespace;

        public string m_AssemblyName;

        public MonoScript(string assembly, string className, string @namespace = "")
        {
            m_AssemblyName = assembly;
            m_ClassName = className;
            m_Namespace = @namespace;
        }

        public static MonoScript Read(AssetTypeValueField value)
        {
            var className = value["m_ClassName"].GetValue().AsString();
            var ns = value["m_Namespace"].GetValue().AsString() ?? "";
            var asm = value["m_AssemblyName"].GetValue().AsString();
            return new MonoScript(asm, className, ns);
        }
    }
}
