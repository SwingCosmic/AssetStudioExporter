using AssetsTools.NET;
using AssetsTools.NET.Texture;
using AssetStudio;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace AssetStudioExporter.Export
{
    public static class Texture2DExtensions
    {
        public static Image<Bgra32> ConvertToImage(this TextureFile m_Texture2D, byte[] data, bool flip = true)
        {
            var converter = new Texture2DConverter(m_Texture2D, new[] { 2019 });
            var buff = BigArrayPool<byte>.Shared.Rent(m_Texture2D.m_Width * m_Texture2D.m_Height * 4);
            try
            {
                if (converter.DecodeTexture2D(data, buff))
                {
                    var image = Image.LoadPixelData<Bgra32>(buff, m_Texture2D.m_Width, m_Texture2D.m_Height);
                    if (flip)
                    {
                        image.Mutate(x => x.Flip(FlipMode.Vertical));
                    }
                    return image;
                }
                return null;
            }
            finally
            {
                BigArrayPool<byte>.Shared.Return(buff);
            }
        }

    }
}
