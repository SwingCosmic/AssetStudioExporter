using AssetRipper.TextureDecoder.Astc;
using AssetRipper.TextureDecoder.Atc;
using AssetRipper.TextureDecoder.Bc;
using AssetRipper.TextureDecoder.Dxt;
using AssetRipper.TextureDecoder.Etc;
using AssetRipper.TextureDecoder.Pvrtc;
using AssetRipper.TextureDecoder.Rgb.Formats;
using AssetRipper.TextureDecoder.Rgb;
using AssetRipper.TextureDecoder.Yuy2;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using AssetStudio;
using AssetStudioExporter.Export;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Half = System.Half;

namespace AssetStudioExporter.AssetTypes
{
    public class Texture2D : TextureFile, IAssetType, IAssetTypeReader<Texture2D>, IAssetTypeExporter
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Texture2D;

        new TextureFormat m_TextureFormat;

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
            m_TextureFormat = (TextureFormat)textureFile.m_TextureFormat;
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
            return Export(assetsFile, stream, ExporterSetting.Default.ImageExportFormat);
        }        
        
        public byte[]? GetAssetData(AssetsFileInstance assetsFile)
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
                    return null;
                }

                rawdata = assetsFile.GetAssetData(path, size, offset);
            }
            return rawdata;
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream, ImageFormat format)
        {
            var rawdata = GetAssetData(assetsFile);
            if (rawdata is null || rawdata.Length == 0)
            {
                return false;
            }

            Image<Bgra32> image;
            if (ExporterSetting.Default.TextureDecoder == TextureDecoderType.AssetStudio)
            {
                image = ConvertToImage(rawdata, true);
            } else
            {
                var decoded = DecodeManagedCopy(rawdata, m_TextureFormat, m_Width, m_Height);
                image = ConvertToImageFromDecoded(decoded, true);
            }

            using(image)
            {
                switch (format)
                {
                    case ImageFormat.Jpeg:
                        image.SaveAsJpeg(stream);
                        break;
                    case ImageFormat.Webp:
                        image.SaveAsWebp(stream);
                        break;
                    case ImageFormat.Tga: 
                        image.SaveAsTga(stream);
                        break;
                    case ImageFormat.Tiff: 
                        image.SaveAsTiff(stream);
                        break;
                    case ImageFormat.Bmp: 
                        image.SaveAsBmp(stream); 
                        break;                
                    case ImageFormat.Gif: 
                        image.SaveAsGif(stream); 
                        break;
                    default:
                        image.SaveAsPng(stream);
                        break;
                }
            }
            return true;
        }


        public Image<Bgra32> ConvertToImage(byte[] data, bool flip = true)
        {
            var converter = new Texture2DConverter(this, new[] { 2019 });
            var buff = BigArrayPool<byte>.Shared.Rent(m_Width * m_Height * 4);
            try
            {
                if (converter.DecodeTexture2D(data, buff))
                {
                    var image = Image.LoadPixelData<Bgra32>(buff, m_Width, m_Height);
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
        
        public Image<Bgra32> ConvertToImageFromDecoded(byte[] decoded, bool flip = true)
        {
            var image = Image.LoadPixelData<Bgra32>(decoded, m_Width, m_Height);
            if (flip)
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
            }
            return image;
        }

        /// <summary>
        /// 等同于<see cref="TextureFile.DecodeManaged"/>。<br />
        /// 调用那个方法会导致 Could not load file or assembly 'System.Half',
        /// see <see href="https://github.com/nesrak1/AssetsTools.NET/issues/111"/>
        /// </summary>
        public static byte[] DecodeManagedCopy(byte[] data, TextureFormat format, int width, int height, bool useBgra = true)
        {
            byte[] output = Array.Empty<byte>();
            int num;
            switch (format)
            {
                case TextureFormat.Alpha8:
                    num = RgbConverter.Convert<ColorA8, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.ARGB4444:
                    num = RgbConverter.Convert<ColorARGB16, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGB24:
                    num = RgbConverter.Convert<ColorRGB24, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGBA32:
                    num = RgbConverter.Convert<ColorRGBA32, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.ARGB32:
                    num = RgbConverter.Convert<ColorARGB32, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.R16:
                    num = RgbConverter.Convert<ColorR16, ushort, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGBA4444:
                    num = RgbConverter.Convert<ColorRGBA16, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.BGRA32:
                    num = data.Length;
                    break;
                case TextureFormat.RG16:
                    num = RgbConverter.Convert<ColorRG16, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.R8:
                    num = RgbConverter.Convert<ColorR8, byte, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RHalf:
                    num = RgbConverter.Convert<ColorRHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGHalf:
                    num = RgbConverter.Convert<ColorRGHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGBAHalf:
                    num = RgbConverter.Convert<ColorRGBAHalf, Half, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RFloat:
                    num = RgbConverter.Convert<ColorRSingle, float, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGFloat:
                    num = RgbConverter.Convert<ColorRGSingle, float, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGBAFloat:
                    num = RgbConverter.Convert<ColorRGBASingle, float, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGB9e5Float:
                    num = RgbConverter.Convert<ColorRGB9e5, double, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RG32:
                    num = RgbConverter.Convert<ColorRG32, ushort, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGB48:
                    num = RgbConverter.Convert<ColorRGB48, ushort, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.RGBA64:
                    num = RgbConverter.Convert<ColorRGBA64, ushort, ColorBGRA32, byte>(data, width, height, out output);
                    break;
                case TextureFormat.DXT1:
                    num = DxtDecoder.DecompressDXT1(data, width, height, out output);
                    break;
                case TextureFormat.DXT3:
                    num = DxtDecoder.DecompressDXT3(data, width, height, out output);
                    break;
                case TextureFormat.DXT5:
                    num = DxtDecoder.DecompressDXT5(data, width, height, out output);
                    break;
                case TextureFormat.BC4:
                    num = BcDecoder.DecompressBC4(data, width, height, out output);
                    break;
                case TextureFormat.BC5:
                    num = BcDecoder.DecompressBC5(data, width, height, out output);
                    break;
                case TextureFormat.BC6H:
                    num = BcDecoder.DecompressBC6H(data, width, height, isSigned: false, out output);
                    break;
                case TextureFormat.BC7:
                    num = BcDecoder.DecompressBC7(data, width, height, out output);
                    break;
                case TextureFormat.ETC_RGB4:
                    num = EtcDecoder.DecompressETC(data, width, height, out output);
                    break;
                case TextureFormat.ETC2_RGB4:
                    num = EtcDecoder.DecompressETC2(data, width, height, out output);
                    break;
                case TextureFormat.ETC2_RGBA1:
                    num = EtcDecoder.DecompressETC2A1(data, width, height, out output);
                    break;
                case TextureFormat.ETC2_RGBA8:
                    num = EtcDecoder.DecompressETC2A8(data, width, height, out output);
                    break;
                case TextureFormat.EAC_R:
                    num = EtcDecoder.DecompressEACRUnsigned(data, width, height, out output);
                    break;
                case TextureFormat.EAC_R_SIGNED:
                    num = EtcDecoder.DecompressEACRSigned(data, width, height, out output);
                    break;
                case TextureFormat.EAC_RG:
                    num = EtcDecoder.DecompressEACRGUnsigned(data, width, height, out output);
                    break;
                case TextureFormat.EAC_RG_SIGNED:
                    num = EtcDecoder.DecompressEACRGSigned(data, width, height, out output);
                    break;
                case TextureFormat.ASTC_RGB_4x4:
                case TextureFormat.ASTC_RGBA_4x4:
                    num = AstcDecoder.DecodeASTC(data, width, height, 4, 4, out output);
                    break;
                case TextureFormat.ASTC_RGB_5x5:
                case TextureFormat.ASTC_RGBA_5x5:
                    num = AstcDecoder.DecodeASTC(data, width, height, 5, 5, out output);
                    break;
                case TextureFormat.ASTC_RGB_6x6:
                case TextureFormat.ASTC_RGBA_6x6:
                    num = AstcDecoder.DecodeASTC(data, width, height, 6, 6, out output);
                    break;
                case TextureFormat.ASTC_RGB_8x8:
                case TextureFormat.ASTC_RGBA_8x8:
                    num = AstcDecoder.DecodeASTC(data, width, height, 8, 8, out output);
                    break;
                case TextureFormat.ASTC_RGB_10x10:
                case TextureFormat.ASTC_RGBA_10x10:
                    num = AstcDecoder.DecodeASTC(data, width, height, 10, 10, out output);
                    break;
                case TextureFormat.ASTC_RGB_12x12:
                case TextureFormat.ASTC_RGBA_12x12:
                    num = AstcDecoder.DecodeASTC(data, width, height, 12, 12, out output);
                    break;
                case TextureFormat.ATC_RGB4:
                    num = AtcDecoder.DecompressAtcRgb4(data, width, height, out output);
                    break;
                case TextureFormat.ATC_RGBA8:
                    num = AtcDecoder.DecompressAtcRgba8(data, width, height, out output);
                    break;
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                    num = PvrtcDecoder.DecompressPVRTC(data, width, height, do2bitMode: true, out output);
                    break;
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                    num = PvrtcDecoder.DecompressPVRTC(data, width, height, do2bitMode: false, out output);
                    break;
                case TextureFormat.YUY2:
                    num = Yuy2Decoder.DecompressYUY2(data, width, height, out output);
                    break;
                default:
                    num = 0;
                    break;
            }

            if (num == 0)
            {
                return null;
            }

            return output;
        }

    }
}
