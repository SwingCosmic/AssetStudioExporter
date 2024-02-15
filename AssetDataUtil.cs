using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using AssetStudioExporter.AssetTypes;
using AssetStudioExporter.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudio;

public static class AssetDataUtil
{
    /// <summary>
    /// 根据资源路径、大小和偏移量读取AssetBundle中的数据
    /// </summary>
    /// <param name="inst">AssetsFileInstance</param>
    /// <param name="path">资源路径</param>
    /// <param name="size">大小</param>
    /// <param name="offset">偏移量</param>
    /// <returns>资源的原始二进制数据</returns>
    /// <exception cref="FileNotFoundException">找不到对应的资源</exception>
    public static byte[] GetAssetData(this AssetsFileInstance inst, string path, uint size, long offset = 0)
    {
        byte[] data = Array.Empty<byte>();
        if (path != null && inst.parentBundle != null)
        {
            string text = path;
            if (path.StartsWith("archive:/"))
            {
                text = text[9..];
            }
            text = Path.GetFileName(text);
            AssetBundleFile file = inst.parentBundle.file;
            AssetsFileReader dataReader = file.DataReader;
            var directoryInfos = file.BlockAndDirInfo.DirectoryInfos;
            bool foundFile = false;
            foreach (AssetBundleDirectoryInfo assetBundleDirectoryInfo in directoryInfos)
            {
                if (assetBundleDirectoryInfo.Name == text)
                {
                    dataReader.Position = assetBundleDirectoryInfo.Offset + offset;
                    data = dataReader.ReadBytes((int)size);
                    foundFile = true;
                    break;
                }
            }
            if (!foundFile)
            {
                throw new FileNotFoundException("resS was detected but no file was found in bundle");
            }
        }
        return data;
    }

    public static UnityVersion GetVersion(this AssetsFileInstance inst)
    {
        return VersionCache.GetVersion(inst);
    }

    /// <summary>
    /// 获取AssetBundle或ResourceManager的Container
    /// </summary>
    /// <param name="inst">AssetsFileInstance</param>
    /// <returns>Container字典</returns>
    public static Dictionary<string, AssetPPtr> GetContainers(this AssetsFileInstance inst, AssetsManager am)
    {
        var isAB = true;
        var ab = inst.file.AssetInfos.FirstOrDefault(f => f.TypeId == (uint)AssetClassID.AssetBundle);
        if (ab is null)
        {
            ab = inst.file.AssetInfos.FirstOrDefault(f => f.TypeId == (uint)AssetClassID.ResourceManager);
            isAB = false;
            if (ab is null)
            {
                return new Dictionary<string, AssetPPtr>();
            }
        }
        var bf = am.GetBaseField(inst, ab);
        var version = VersionCache.GetVersion(inst);
        if (isAB)
        {
            var assetBundle = AssetBundle.Read(bf, version);
            return assetBundle.m_Container
                .Select(p => new KeyValuePair<string, AssetPPtr>(p.Key, p.Value.asset))
                .ToDictionary(p => p.Key, p => p.Value);
        } else
        {
            var resourceManager = ResourceManager.Read(bf, version);
            return resourceManager.m_Container
                .Select(p => new KeyValuePair<string, AssetPPtr>(p.Key, p.Value))
                .ToDictionary(p => p.Key, p => p.Value);
        }
    }

    public static object? DeserializeMonoBehaviour(AssetTypeValueField monoBehaviour) =>
        monoBehaviour.Value switch
        {
            AssetTypeValue v => v.ValueType switch
            {
                AssetValueType.Bool => v.AsBool,
                AssetValueType.Int8 => v.AsByte,
                AssetValueType.UInt8 => v.AsSByte,
                AssetValueType.Int16 => v.AsShort,
                AssetValueType.UInt16 => v.AsUShort,
                AssetValueType.Int32 => v.AsInt,
                AssetValueType.UInt32 => v.AsUInt,
                AssetValueType.Int64 => v.AsLong,
                AssetValueType.UInt64 => v.AsULong,
                AssetValueType.Float => v.AsFloat,
                AssetValueType.Double => v.AsDouble,
                AssetValueType.String => v.AsString,
                AssetValueType.Array => monoBehaviour.Children
                    .Select(DeserializeMonoBehaviour)
                    .ToArray(),
                AssetValueType.ByteArray => v.AsByteArray,
                _ => null
            },
            null => monoBehaviour.Children
                .Aggregate(new Dictionary<string, dynamic?>(), (obj, c) =>
                {
                    var key = c.TemplateField.Name;
                    var value = DeserializeMonoBehaviour(c);
                    obj[key] = value;
                    return obj;
                })
        };

    public static AssetTypeValueField SerializeMonoBehaviour(object? obj, AssetTypeTemplateField templateField)
    {
        var ret = new AssetTypeValueField();
        ret.TemplateField = templateField;

        ret.Value = obj switch
        {
            null => new AssetTypeValue(AssetValueType.None, null),
            bool b => new AssetTypeValue(AssetValueType.Bool, b),
            sbyte int8 => new AssetTypeValue(AssetValueType.Int8, int8),
            byte uint8 => new AssetTypeValue(AssetValueType.UInt8, uint8),
            short int16 => new AssetTypeValue(AssetValueType.Int16, int16),
            ushort uint16 => new AssetTypeValue(AssetValueType.UInt16, uint16),
            int int32 => new AssetTypeValue(AssetValueType.Int32, int32),
            uint uint32 => new AssetTypeValue(AssetValueType.UInt32, uint32),
            long int64 => new AssetTypeValue(AssetValueType.Int64, int64),
            ulong uint64 => new AssetTypeValue(AssetValueType.UInt64, uint64),
            float f => new AssetTypeValue(AssetValueType.Float, f),
            double d => new AssetTypeValue(AssetValueType.Double, d),
            string s => new AssetTypeValue(AssetValueType.String, s),
            IEnumerable<byte> barr => new AssetTypeValue(
                AssetValueType.ByteArray,
                barr.ToArray()),
            _ => null
        };

        if (ret.Value is not null)
        {
            return ret;
        }


        if (obj is IDictionary<string, dynamic> dict)
        {
            ret.Value = null;
            ret.Children = dict.Select(p =>
            {
                var temp = templateField.Children.First(f => f.Name == p.Key);
                AssetTypeValueField child = SerializeMonoBehaviour(p.Value, temp);
                return child;
            }).ToList();
        }
        else if (obj is IList<dynamic> arr)
        {
            var array = new AssetTypeArrayInfo(arr.Count);
            ret.Children =arr
                .Select(x =>
                {
                    var temp = templateField.Children[1];// [0]为size
                    return (AssetTypeValueField)SerializeMonoBehaviour(x, temp);
                })
                .ToList();
            ret.Value = new AssetTypeValue(AssetValueType.Array, array);
        }
        else
        {
            ret.Value = null;
            Type type = obj!.GetType();
            ret.Children = type.GetFields().Select(f =>
            {
                var key = f.Name;
                var temp = templateField.Children.First(f => f.Name == key);
                AssetTypeValueField child = SerializeMonoBehaviour(f.GetValue(obj), temp);
                return child;
            }).ToList();
        }


        return ret;
    }
}
