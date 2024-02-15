using AssetsTools.NET;
using AssetStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.Util;

public static class AssetTypeValueFieldExtensions
{
    /// <summary>
    /// Author: ChatGPT
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReverseBits(byte b)
    {
        // 获取高4位和低4位
        byte highNibble = (byte)(b >> 4);
        byte lowNibble = (byte)(b & 0x0F);

        // 交换高4位和低4位
        byte swappedByte = (byte)((lowNibble << 4) | highNibble);

        return swappedByte;
    }

    public static List<T> AsList<T>(this AssetTypeValueField value, Func<AssetTypeValueField, T> selector)
    {
        return value["Array"]
            .Children
            .Select(selector)
            .ToList();
    }    
    
    public static T[] AsArray<T>(this AssetTypeValueField value, Func<AssetTypeValueField, T> selector)
    {
        return value["Array"]
            .Children
            .Select(selector)
            .ToArray();
    }

    /// <summary>
    /// 将<paramref name="value"/>按照<c>UnityEditor.GUID</c>进行读取
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns><see cref="Guid"/></returns>
    public static Guid AsUnityGUID(this AssetTypeValueField value)
    {
        // 将uint[4]重新解释为byte[16]
        var uintArray = value.Children
            .Select(n => n.AsUInt)
            .ToArray();
        byte[] bytes = MemoryMarshal.AsBytes(uintArray.AsSpan()).ToArray();
        // 翻转每个字节的高低位 0x42ABCDEF => 0x24BADCFE
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = ReverseBits(bytes[i]);
        }

        return new Guid(bytes);
    }  
    
    public static Vector2 AsVector2(this AssetTypeValueField value)
    {
        return new Vector2
        (
            value["x"].AsFloat, 
            value["y"].AsFloat
        );
    }    
    
    public static Vector3 AsVector3(this AssetTypeValueField value)
    {
        return new Vector3
        (
            value["x"].AsFloat,
            value["y"].AsFloat,
            value["z"].AsFloat
        );
    }    
    
    public static Vector4 AsVector4(this AssetTypeValueField value)
    {
        return new Vector4
        (
            value["x"].AsFloat,
            value["y"].AsFloat,
            value["z"].AsFloat,
            value["w"].AsFloat
        );
    }
    
    public static AABB AsAABB(this AssetTypeValueField value)
    {
        return new AABB
        (
            value["m_Center"].AsVector3(),
            value["m_Extent"].AsVector3()
        );
    }
    
    public static Matrix4x4 AsMatrix4x4(this AssetTypeValueField value)
    {
        var values = value.AsArray(v => v.AsFloat);
        return new Matrix4x4(values);
    }
}
