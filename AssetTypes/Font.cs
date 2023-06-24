using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public class Font : IAssetTypeReader<Font>, IAssetTypeExporter
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Font;

        public string m_Name;
        public byte[] m_FontData = { };

        public static Font Read(AssetTypeValueField value)
        {
            var f = new Font();

            f.m_Name = value["m_Name"].GetValue().AsString();
            // 其它字段省略，因为整个字体文件都在m_FontData里面

            // WARN: 高内存占用警告！因为Unity的蜜汁类型定义不能直接返回byte[]，产生巨量包装类
            var arr = value["m_FontData"].Get("Array");
            f.m_FontData = new byte[arr.childrenCount];
            for (int i = 0; i < arr.childrenCount; i++)
            {
                f.m_FontData[i] = arr.children[i].value.value.asUInt8;
            }
            return f;
        }

        public string GetFileExtension(string name)
        {
            if (m_FontData.Length == 0)
            {
                return "bin";
            }

            if (m_FontData[0..4].SequenceEqual(Encoding.ASCII.GetBytes("OTTO")))
            {
                return "otf";
            } else
            {
                return "ttf";
            }
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream)
        {
            if (m_FontData.Length == 0)
            {
                Console.WriteLine($"[WARN] Font '{m_Name}' doesn't have any data, skipped");
                return false;
            }
            stream.Write(m_FontData);
            return true;
        }
    }
}
