using System;
using System.Linq;
//using Texture2DDecoder;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace AssetStudio
{
	// Token: 0x0200028C RID: 652
	/// <summary>
	/// 魔改的AssetStudio.Texture2DConverter，适配AssetsTools.NET并注释了暂需要额外库的格式
	/// </summary>
	public class Texture2DConverter
	{
		
		// Token: 0x06000697 RID: 1687 RVA: 0x0001A9BC File Offset: 0x00018BBC
		public Texture2DConverter(TextureFile m_Texture2D, byte[] data)
		{
			this.image_data = data;
			this.image_data_size = image_data.Length;
			this.m_Width = m_Texture2D.m_Width;
			this.m_Height = m_Texture2D.m_Height;
			this.m_TextureFormat = (TextureFormat)m_Texture2D.m_TextureFormat;
			//this.version = m_Texture2D.version;
			//this.platform = m_Texture2D.platform;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0001AA2C File Offset: 0x00018C2C
		public byte[] DecodeTexture2D()
		{
			byte[] bytes = null!;
			switch (this.m_TextureFormat)
			{
				case TextureFormat.Alpha8:
					bytes = this.DecodeAlpha8();
					break;
				case TextureFormat.ARGB4444:
					this.SwapBytesForXbox();
					bytes = this.DecodeARGB4444();
					break;
				case TextureFormat.RGB24:
					bytes = this.DecodeRGB24();
					break;
				case TextureFormat.RGBA32:
					bytes = this.DecodeRGBA32();
					break;
				case TextureFormat.ARGB32:
					bytes = this.DecodeARGB32();
					break;
				case TextureFormat.RGB565:
					this.SwapBytesForXbox();
					bytes = this.DecodeRGB565();
					break;
				case TextureFormat.R16:
					bytes = this.DecodeR16();
					break;
				//case TextureFormat.DXT1:
				//	this.SwapBytesForXbox();
				//	bytes = this.DecodeDXT1();
				//	break;
				//case TextureFormat.DXT5:
				//	this.SwapBytesForXbox();
				//	bytes = this.DecodeDXT5();
				//	break;
				case TextureFormat.RGBA4444:
					bytes = this.DecodeRGBA4444();
					break;
                case TextureFormat.BGRA32New:
                    bytes = this.DecodeBGRA32();
                    break;
                //case TextureFormat.RHalf:
                //	bytes = this.DecodeRHalf();
                //	break;
                //case TextureFormat.RGHalf:
                //	bytes = this.DecodeRGHalf();
                //	break;
                //case TextureFormat.RGBAHalf:
                //	bytes = this.DecodeRGBAHalf();
                //	break;
                case TextureFormat.RFloat:
					bytes = this.DecodeRFloat();
					break;
				case TextureFormat.RGFloat:
					bytes = this.DecodeRGFloat();
					break;
				case TextureFormat.RGBAFloat:
					bytes = this.DecodeRGBAFloat();
					break;
				case TextureFormat.YUV2:
					bytes = this.DecodeYUY2();
					break;
				case TextureFormat.RGB9e5Float:
					bytes = this.DecodeRGB9e5Float();
					break;
				//case TextureFormat.BC6H:
				//	bytes = this.DecodeBC6H();
				//	break;
				//case TextureFormat.BC7:
				//	bytes = this.DecodeBC7();
				//	break;
				//case TextureFormat.BC4:
				//	bytes = this.DecodeBC4();
				//	break;
				//case TextureFormat.BC5:
				//	bytes = this.DecodeBC5();
				//	break;
				//case TextureFormat.DXT1Crunched:
				//	if (this.UnpackCrunch())
				//	{
				//		bytes = this.DecodeDXT1();
				//	}
				//	break;
				//case TextureFormat.DXT5Crunched:
				//	if (this.UnpackCrunch())
				//	{
				//		bytes = this.DecodeDXT5();
				//	}
				//	break;
				//case TextureFormat.PVRTC_RGB2:
				//case TextureFormat.PVRTC_RGBA2:
				//	bytes = this.DecodePVRTC(true);
				//	break;
				//case TextureFormat.PVRTC_RGB4:
				//case TextureFormat.PVRTC_RGBA4:
				//	bytes = this.DecodePVRTC(false);
				//	break;
				//case TextureFormat.ETC_RGB4:
				//case TextureFormat.ETC_RGB4_3DS:
				//	bytes = this.DecodeETC1();
				//	break;
				//case TextureFormat.ATC_RGB4:
				//	bytes = this.DecodeATCRGB4();
				//	break;
				//case TextureFormat.ATC_RGBA8:
				//	bytes = this.DecodeATCRGBA8();
				//	break;
				//case TextureFormat.EAC_R:
				//	bytes = this.DecodeEACR();
				//	break;
				//case TextureFormat.EAC_R_SIGNED:
				//	bytes = this.DecodeEACRSigned();
				//	break;
				//case TextureFormat.EAC_RG:
				//	bytes = this.DecodeEACRG();
				//	break;
				//case TextureFormat.EAC_RG_SIGNED:
				//	bytes = this.DecodeEACRGSigned();
				//	break;
				//case TextureFormat.ETC2_RGB:
				//	bytes = this.DecodeETC2();
				//	break;
				//case TextureFormat.ETC2_RGBA1:
				//	bytes = this.DecodeETC2A1();
				//	break;
				//case TextureFormat.ETC2_RGBA8:
				//case TextureFormat.ETC_RGBA8_3DS:
				//	bytes = this.DecodeETC2A8();
				//	break;
				//case TextureFormat.ASTC_RGB_4x4:
				//case TextureFormat.ASTC_RGBA_4x4:
				//case TextureFormat.ASTC_HDR_4x4:
				//	bytes = this.DecodeASTC(4);
				//	break;
				//case TextureFormat.ASTC_RGB_5x5:
				//case TextureFormat.ASTC_RGBA_5x5:
				//case TextureFormat.ASTC_HDR_5x5:
				//	bytes = this.DecodeASTC(5);
				//	break;
				//case TextureFormat.ASTC_RGB_6x6:
				//case TextureFormat.ASTC_RGBA_6x6:
				//case TextureFormat.ASTC_HDR_6x6:
				//	bytes = this.DecodeASTC(6);
				//	break;
				//case TextureFormat.ASTC_RGB_8x8:
				//case TextureFormat.ASTC_RGBA_8x8:
				//case TextureFormat.ASTC_HDR_8x8:
				//	bytes = this.DecodeASTC(8);
				//	break;
				//case TextureFormat.ASTC_RGB_10x10:
				//case TextureFormat.ASTC_RGBA_10x10:
				//case TextureFormat.ASTC_HDR_10x10:
				//	bytes = this.DecodeASTC(10);
				//	break;
				//case TextureFormat.ASTC_RGB_12x12:
				//case TextureFormat.ASTC_RGBA_12x12:
				//case TextureFormat.ASTC_HDR_12x12:
				//	bytes = this.DecodeASTC(12);
				//	break;
				case TextureFormat.RG16:
					bytes = this.DecodeRG16();
					break;
				case TextureFormat.R8:
					bytes = this.DecodeR8();
					break;
				//case TextureFormat.ETC_RGB4Crunched:
				//	if (this.UnpackCrunch())
				//	{
				//		bytes = this.DecodeETC1();
				//	}
				//	break;
				//case TextureFormat.ETC2_RGBA8Crunched:
				//	if (this.UnpackCrunch())
				//	{
				//		bytes = this.DecodeETC2A8();
				//	}
				//	break;
				default:
					throw new NotSupportedException($"不支持的图片格式 {m_TextureFormat}");
			}
			return bytes;
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0001ADC4 File Offset: 0x00018FC4
		private void SwapBytesForXbox()
		{
			//if (this.platform == BuildTarget.XBOX360)
			//{
			//	for (int i = 0; i < this.image_data_size / 2; i++)
			//	{
			//		byte b = this.image_data[i * 2];
			//		this.image_data[i * 2] = this.image_data[i * 2 + 1];
			//		this.image_data[i * 2 + 1] = b;
			//	}
			//}
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0001AE1C File Offset: 0x0001901C
		private byte[] DecodeAlpha8()
		{
			byte[] buff = Enumerable.Repeat<byte>(byte.MaxValue, this.m_Width * this.m_Height * 4).ToArray<byte>();
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				buff[i * 4 + 3] = this.image_data[i];
			}
			return buff;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0001AE70 File Offset: 0x00019070
		private byte[] DecodeARGB4444()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				byte[] pixelNew = new byte[4];
				ushort pixelOldShort = BitConverter.ToUInt16(this.image_data, i * 2);
				pixelNew[0] = (byte)(pixelOldShort & 15);
				pixelNew[1] = (byte)((pixelOldShort & 240) >> 4);
				pixelNew[2] = (byte)((pixelOldShort & 3840) >> 8);
				pixelNew[3] = (byte)((pixelOldShort & 61440) >> 12);
				for (int j = 0; j < 4; j++)
				{
					pixelNew[j] = (byte)((int)pixelNew[j] << 4 | (int)pixelNew[j]);
				}
				pixelNew.CopyTo(buff, i * 4);
			}
			return buff;
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0001AF20 File Offset: 0x00019120
		private byte[] DecodeRGB24()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				buff[i * 4] = this.image_data[i * 3 + 2];
				buff[i * 4 + 1] = this.image_data[i * 3 + 1];
				buff[i * 4 + 2] = this.image_data[i * 3];
				buff[i * 4 + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0001AF9C File Offset: 0x0001919C
		private byte[] DecodeRGBA32()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = this.image_data[i + 2];
				buff[i + 1] = this.image_data[i + 1];
				buff[i + 2] = this.image_data[i];
				buff[i + 3] = this.image_data[i + 3];
			}
			return buff;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0001B008 File Offset: 0x00019208
		private byte[] DecodeARGB32()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = this.image_data[i + 3];
				buff[i + 1] = this.image_data[i + 2];
				buff[i + 2] = this.image_data[i + 1];
				buff[i + 3] = this.image_data[i];
			}
			return buff;
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0001B074 File Offset: 0x00019274
		private byte[] DecodeRGB565()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				ushort p = BitConverter.ToUInt16(this.image_data, i * 2);
				buff[i * 4] = (byte)((int)p << 3 | (p >> 2 & 7));
				buff[i * 4 + 1] = (byte)((p >> 3 & 252) | (p >> 9 & 3));
				buff[i * 4 + 2] = (byte)((p >> 8 & 248) | p >> 13);
				buff[i * 4 + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0001B108 File Offset: 0x00019308
		private byte[] DecodeR16()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				buff[i * 4 + 2] = this.image_data[i * 2 + 1];
				buff[i * 4 + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0001B164 File Offset: 0x00019364
		//private byte[] DecodeDXT1()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeDXT1(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0001B1A4 File Offset: 0x000193A4
		//private byte[] DecodeDXT5()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeDXT5(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0001B1E4 File Offset: 0x000193E4
		private byte[] DecodeRGBA4444()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				byte[] pixelNew = new byte[4];
				ushort pixelOldShort = BitConverter.ToUInt16(this.image_data, i * 2);
				pixelNew[0] = (byte)((pixelOldShort & 240) >> 4);
				pixelNew[1] = (byte)((pixelOldShort & 3840) >> 8);
				pixelNew[2] = (byte)((pixelOldShort & 61440) >> 12);
				pixelNew[3] = (byte)(pixelOldShort & 15);
				for (int j = 0; j < 4; j++)
				{
					pixelNew[j] = (byte)((int)pixelNew[j] << 4 | (int)pixelNew[j]);
				}
				pixelNew.CopyTo(buff, i * 4);
			}
			return buff;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0001B294 File Offset: 0x00019494
		private byte[] DecodeBGRA32()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = this.image_data[i];
				buff[i + 1] = this.image_data[i + 1];
				buff[i + 2] = this.image_data[i + 2];
				buff[i + 3] = this.image_data[i + 3];
			}
			return buff;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0001B300 File Offset: 0x00019500
		//private byte[] DecodeRHalf()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	for (int i = 0; i < buff.Length; i += 4)
		//	{
		//		buff[i] = 0;
		//		buff[i + 1] = 0;
		//		buff[i + 2] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i / 2) * 255f));
		//		buff[i + 3] = byte.MaxValue;
		//	}
		//	return buff;
		//}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0001B36C File Offset: 0x0001956C
		//private byte[] DecodeRGHalf()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	for (int i = 0; i < buff.Length; i += 4)
		//	{
		//		buff[i] = 0;
		//		buff[i + 1] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i + 2) * 255f));
		//		buff[i + 2] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i) * 255f));
		//		buff[i + 3] = byte.MaxValue;
		//	}
		//	return buff;
		//}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0001B3F4 File Offset: 0x000195F4
		//private byte[] DecodeRGBAHalf()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	for (int i = 0; i < buff.Length; i += 4)
		//	{
		//		buff[i] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i * 2 + 4) * 255f));
		//		buff[i + 1] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i * 2 + 2) * 255f));
		//		buff[i + 2] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i * 2) * 255f));
		//		buff[i + 3] = (byte)Math.Round((double)(Half.ToHalf(this.image_data, i * 2 + 6) * 255f));
		//	}
		//	return buff;
		//}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0001B4C4 File Offset: 0x000196C4
		private byte[] DecodeRFloat()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = 0;
				buff[i + 1] = 0;
				buff[i + 2] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i) * 255f));
				buff[i + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0001B528 File Offset: 0x00019728
		private byte[] DecodeRGFloat()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = 0;
				buff[i + 1] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 2 + 4) * 255f));
				buff[i + 2] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 2) * 255f));
				buff[i + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0001B5AC File Offset: 0x000197AC
		private byte[] DecodeRGBAFloat()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				buff[i] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 4 + 8) * 255f));
				buff[i + 1] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 4 + 4) * 255f));
				buff[i + 2] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 4) * 255f));
				buff[i + 3] = (byte)Math.Round((double)(BitConverter.ToSingle(this.image_data, i * 4 + 12) * 255f));
			}
			return buff;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001B668 File Offset: 0x00019868
		private static byte ClampByte(int x)
		{
			return (byte)((255 < x) ? 255 : ((x > 0) ? x : 0));
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0001B684 File Offset: 0x00019884
		private byte[] DecodeYUY2()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			int p = 0;
			int o = 0;
			int halfWidth = this.m_Width / 2;
			for (int i = 0; i < this.m_Height; i++)
			{
				for (int j = 0; j < halfWidth; j++)
				{
					int y0 = (int)this.image_data[p++];
					int u0 = (int)this.image_data[p++];
					int y = (int)this.image_data[p++];
					int num = (int)this.image_data[p++];
					int c = y0 - 16;
					int d = u0 - 128;
					int e = num - 128;
					buff[o++] = Texture2DConverter.ClampByte(298 * c + 516 * d + 128 >> 8);
					buff[o++] = Texture2DConverter.ClampByte(298 * c - 100 * d - 208 * e + 128 >> 8);
					buff[o++] = Texture2DConverter.ClampByte(298 * c + 409 * e + 128 >> 8);
					buff[o++] = byte.MaxValue;
					c = y - 16;
					buff[o++] = Texture2DConverter.ClampByte(298 * c + 516 * d + 128 >> 8);
					buff[o++] = Texture2DConverter.ClampByte(298 * c - 100 * d - 208 * e + 128 >> 8);
					buff[o++] = Texture2DConverter.ClampByte(298 * c + 409 * e + 128 >> 8);
					buff[o++] = byte.MaxValue;
				}
			}
			return buff;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0001B840 File Offset: 0x00019A40
		private byte[] DecodeRGB9e5Float()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < buff.Length; i += 4)
			{
				int num = BitConverter.ToInt32(this.image_data, i);
				int scale = num >> 27 & 31;
				double scalef = Math.Pow(2.0, (double)(scale - 24));
				int b = num >> 18 & 511;
				int g = num >> 9 & 511;
				int r = num & 511;
				buff[i] = (byte)Math.Round((double)b * scalef * 255.0);
				buff[i + 1] = (byte)Math.Round((double)g * scalef * 255.0);
				buff[i + 2] = (byte)Math.Round((double)r * scalef * 255.0);
				buff[i + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0001B918 File Offset: 0x00019B18
		//private byte[] DecodeBC4()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeBC4(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006AF RID: 1711 RVA: 0x0001B958 File Offset: 0x00019B58
		//private byte[] DecodeBC5()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeBC5(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001B998 File Offset: 0x00019B98
		//private byte[] DecodeBC6H()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeBC6(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0001B9D8 File Offset: 0x00019BD8
		//private byte[] DecodeBC7()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeBC7(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B2 RID: 1714 RVA: 0x0001BA18 File Offset: 0x00019C18
		//private byte[] DecodePVRTC(bool is2bpp)
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodePVRTC(this.image_data, this.m_Width, this.m_Height, buff, is2bpp))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B3 RID: 1715 RVA: 0x0001BA58 File Offset: 0x00019C58
		//private byte[] DecodeETC1()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeETC1(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0001BA98 File Offset: 0x00019C98
		//private byte[] DecodeATCRGB4()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeATCRGB4(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0001BAD8 File Offset: 0x00019CD8
		//private byte[] DecodeATCRGBA8()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeATCRGBA8(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0001BB18 File Offset: 0x00019D18
		//private byte[] DecodeEACR()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeEACR(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001BB58 File Offset: 0x00019D58
		//private byte[] DecodeEACRSigned()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeEACRSigned(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0001BB98 File Offset: 0x00019D98
		//private byte[] DecodeEACRG()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeEACRG(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006B9 RID: 1721 RVA: 0x0001BBD8 File Offset: 0x00019DD8
		//private byte[] DecodeEACRGSigned()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeEACRGSigned(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006BA RID: 1722 RVA: 0x0001BC18 File Offset: 0x00019E18
		//private byte[] DecodeETC2()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeETC2(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006BB RID: 1723 RVA: 0x0001BC58 File Offset: 0x00019E58
		//private byte[] DecodeETC2A1()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeETC2A1(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006BC RID: 1724 RVA: 0x0001BC98 File Offset: 0x00019E98
		//private byte[] DecodeETC2A8()
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeETC2A8(this.image_data, this.m_Width, this.m_Height, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006BD RID: 1725 RVA: 0x0001BCD8 File Offset: 0x00019ED8
		//private byte[] DecodeASTC(int blocksize)
		//{
		//	byte[] buff = new byte[this.m_Width * this.m_Height * 4];
		//	if (!TextureDecoder.DecodeASTC(this.image_data, this.m_Width, this.m_Height, blocksize, blocksize, buff))
		//	{
		//		return null;
		//	}
		//	return buff;
		//}

		// Token: 0x060006BE RID: 1726 RVA: 0x0001BD1C File Offset: 0x00019F1C
		private byte[] DecodeRG16()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i += 2)
			{
				buff[i * 2 + 1] = this.image_data[i + 1];
				buff[i * 2 + 2] = this.image_data[i];
				buff[i * 2 + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0001BD84 File Offset: 0x00019F84
		private byte[] DecodeR8()
		{
			byte[] buff = new byte[this.m_Width * this.m_Height * 4];
			for (int i = 0; i < this.m_Width * this.m_Height; i++)
			{
				buff[i * 4 + 2] = this.image_data[i];
				buff[i * 4 + 3] = byte.MaxValue;
			}
			return buff;
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0001BDDC File Offset: 0x00019FDC
		//private bool UnpackCrunch()
		//{
		//	byte[] result;
		//	if (this.version[0] > 2017 || (this.version[0] == 2017 && this.version[1] >= 3) || this.m_TextureFormat == TextureFormat.ETC_RGB4Crunched || this.m_TextureFormat == TextureFormat.ETC2_RGBA8Crunched)
		//	{
		//		result = TextureDecoder.UnpackUnityCrunch(this.image_data);
		//	}
		//	else
		//	{
		//		result = TextureDecoder.UnpackCrunch(this.image_data);
		//	}
		//	if (result != null)
		//	{
		//		this.image_data = result;
		//		this.image_data_size = result.Length;
		//		return true;
		//	}
		//	return false;
		//}

		// Token: 0x040006F6 RID: 1782
		private int m_Width;

		// Token: 0x040006F7 RID: 1783
		private int m_Height;

		// Token: 0x040006F8 RID: 1784
		private TextureFormat m_TextureFormat;

		// Token: 0x040006F9 RID: 1785
		private int image_data_size;

		// Token: 0x040006FA RID: 1786
		private byte[] image_data;

		// Token: 0x040006FB RID: 1787
		//private int[] version;

		// Token: 0x040006FC RID: 1788
		//private BuildTarget platform;
	}
}
