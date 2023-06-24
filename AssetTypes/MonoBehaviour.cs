using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{

    public class MonoBehaviour : IAssetTypeReader<MonoBehaviour>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.MonoBehaviour;

        public PPtr<MonoScript> m_Script;

        public string m_Name;

        public AssetFileInfoEx? assetFile;

        public MonoBehaviour(string name, PPtr<MonoScript> monoScript)
        {
            m_Name = name;
            m_Script = monoScript;
        }

        public void FindPPtrInstance(AssetsFileInstance inst, AssetsManager am)
        {
            try
            {
                var monoScript = m_Script.FollowReference(inst, am);
                m_Script.Instance = MonoScript.Read(monoScript);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot read PPtr {{ FileID = {m_Script.m_FileID}, PathID = {m_Script.m_PathID} }}\n");
                Console.WriteLine(ex);
            }
            
        }

        public static MonoBehaviour Read(AssetTypeValueField value)
        {
            var name = value["m_Name"].GetValue().AsString();
            var script = new PPtr<MonoScript>(value["m_Script"]);
            return new MonoBehaviour(name, script);
        }

    }

    public static  class MonoBehaviourExtensions
    {
        public static IList<MonoBehaviour> ReadAllMonoBehaviours(this AssetFileInfoEx gameObject, AssetsFileInstance inst, AssetsManager am)
        {
            var type = am.GetTypeInstance(inst.file, gameObject);
            var components = type.GetBaseField()["m_Component"]["Array"];

            var ret = new List<MonoBehaviour>();
            foreach (var comp in components.children)
            {
                var childPtr = comp["component"];
                var childExternal = am.GetExtAsset(inst, childPtr, true);
                var childInfo = childExternal.info;

                if (childInfo.curFileType != (uint)AssetClassID.MonoBehaviour)
                {
                    continue;
                }

                var mbPtr = new PPtr<MonoBehaviour>(childPtr);
                var monoBehaviour = mbPtr.FollowReference(inst, am);
                var mb = MonoBehaviour.Read(monoBehaviour);
                mb.assetFile = childInfo;
                mb.FindPPtrInstance(inst, am);

                ret.Add(mb);

            }
            return ret;
        }
    }
}
