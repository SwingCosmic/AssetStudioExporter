using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public class TextAsset : IAssetTypeReader<TextAsset>, IAssetTypeExporter
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.TextAsset;

        public byte[] m_Script;

        public static TextAsset Read(AssetTypeValueField value)
        {
            var textAsset = new TextAsset();

            var str = value.Get("m_Script");
            textAsset.m_Script = str.AsByteArray;

            return textAsset;
        }

        public string GetFileExtension(string name)
        {
            var ext = Path.GetExtension(name);
            if (!string.IsNullOrEmpty(ext))
            {
                return ext;
            }
            return "txt";
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream)
        {
            stream.Write(m_Script);
            return true;
        }
    }
}
