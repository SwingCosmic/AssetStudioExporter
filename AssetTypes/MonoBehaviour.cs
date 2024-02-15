using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{

    public class MonoBehaviour : IAssetType, IAssetTypeReader<MonoBehaviour>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.MonoBehaviour;

        public PPtr<MonoScript> m_Script;

        public string m_Name;

        public AssetFileInfo? AssetInfo { get; internal set; }

        public MonoBehaviour(string name, PPtr<MonoScript> monoScript)
        {
            m_Name = name;
            m_Script = monoScript;
        }

        public static MonoBehaviour Read(AssetTypeValueField value, UnityVersion Version)
        {
            var name = value["m_Name"].AsString;
            var script = new PPtr<MonoScript>(value["m_Script"]);
            return new MonoBehaviour(name, script);
        }

    }
}
