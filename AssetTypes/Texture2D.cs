using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using AssetStudio;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public class Texture2D : TextureFile, IAssetTypeReader<Texture2D>, IAssetTypeExporter
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Texture2D;

        public Texture2D() { }
        public Texture2D(TextureFile textureFile)
        {
            // 自动生成，复制构造函数
            m_Name = textureFile.m_Name;
            m_ForcedFallbackFormat = textureFile.m_ForcedFallbackFormat;
            m_DownscaleFallback = textureFile.m_DownscaleFallback;
            m_Width = textureFile.m_Width;
            m_Height = textureFile.m_Height;
            m_CompleteImageSize = textureFile.m_CompleteImageSize;
            m_TextureFormat = textureFile.m_TextureFormat;
            m_MipCount = textureFile.m_MipCount;
            m_MipMap = textureFile.m_MipMap;
            m_IsReadable = textureFile.m_IsReadable;
            m_ReadAllowed = textureFile.m_ReadAllowed;
            m_StreamingMipmaps = textureFile.m_StreamingMipmaps;
            m_StreamingMipmapsPriority = textureFile.m_StreamingMipmapsPriority;
            m_ImageCount = textureFile.m_ImageCount;
            m_TextureDimension = textureFile.m_TextureDimension;
            m_TextureSettings = textureFile.m_TextureSettings;
            m_LightmapFormat = textureFile.m_LightmapFormat;
            m_ColorSpace = textureFile.m_ColorSpace;
            m_StreamData = textureFile.m_StreamData;

            pictureData = textureFile.pictureData;
        }
        public static Texture2D Read(AssetTypeValueField value)
        {
            return new Texture2D(ReadTextureFile(value));
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream)
        {
            byte[] rawdata;
            if (pictureData?.Length > 0)
            {
                rawdata = pictureData;
            }
            else
            {
                var path = m_StreamData.path;
                var size = m_StreamData.size;
                var offset = (long)m_StreamData.offset;

                if (size == 0 || string.IsNullOrEmpty(path))
                {
                    Console.WriteLine($"[WARN] Texture2D '{m_Name}' doesn't have any data, skipped");
                    return false;
                }

                rawdata = assetsFile.GetAssetData(path, size, offset);
            }


            using var image = this.ConvertToImage(rawdata, true);
            image.SaveAsPng(stream);
            return true;
        }
    }
}
