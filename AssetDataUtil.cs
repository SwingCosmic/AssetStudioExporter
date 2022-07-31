using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudio
{
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
                string searchPath = path;
                if (path.StartsWith("archive:/"))
                {
                    searchPath = searchPath[9..];
                }
                searchPath = Path.GetFileName(searchPath);
                AssetBundleFile bundle = inst.parentBundle.file;
                AssetsFileReader reader = bundle.reader;
                AssetBundleDirectoryInfo06[] dirInf = bundle.bundleInf6.dirInf;
                bool foundFile = false;
                foreach (AssetBundleDirectoryInfo06 info in dirInf)
                {
                    if (info.name == searchPath)
                    {
                        reader.Position = bundle.bundleHeader6.GetFileDataOffset() + info.offset + offset;
                        data = reader.ReadBytes((int)size);
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

        /// <summary>
        /// 获取AssetBundle Container
        /// </summary>
        /// <param name="inst">AssetsFileInstance</param>
        /// <returns>Container字典</returns>
        public static Dictionary<string, AssetPPtr> GetContainers(this AssetsFileInstance inst, AssetsManager am)
        {
            var ret = new Dictionary<string, AssetPPtr>();
            var isAB = true;
            var ab = inst.table.assetFileInfo.FirstOrDefault(f => f.curFileType == (uint)AssetClassID.AssetBundle);
            if (ab is null)
            {
                ab = inst.table.assetFileInfo.FirstOrDefault(f => f.curFileType == (uint)AssetClassID.ResourceManager);
                isAB = false;
                if (ab is null)
                {
                    return ret;
                }
            }
            var type = am.GetTypeInstance(inst.file, ab);
            var bf = type.GetBaseField();

            var m_Container = bf.Get("m_Container").Get("Array");
            foreach (var container in m_Container.children)
            {
                var path = container.Get("first").GetValue().AsString();
                var assetInfo = container.Get("second");
                AssetTypeValueField asset = assetInfo;
                if (isAB)
                {
                    var preloadIndex = assetInfo.Get("preloadIndex").GetValue().AsInt();
                    var preloadSize = assetInfo.Get("preloadSize").GetValue().AsInt();
                    asset = assetInfo.Get("asset"); 
                }
                var m_FileID = asset.Get("m_FileID").GetValue().AsInt();
                var m_PathID = asset.Get("m_PathID").GetValue().AsInt64();
                ret[path] = new AssetPPtr(m_FileID, m_PathID);
            }
            return ret;
        }

        public static object? DeserializeMonoBehaviour(AssetTypeValueField monoBehaviour) =>
            monoBehaviour.GetValue() switch
            {
                AssetTypeValue v => v.type switch
                {
                    EnumValueTypes.Bool => v.AsBool(),
                    EnumValueTypes.Int8 => v.value.asInt8,
                    EnumValueTypes.UInt8 => v.value.asUInt8,
                    EnumValueTypes.Int16 => v.value.asInt16,
                    EnumValueTypes.UInt16 => v.value.asUInt16,
                    EnumValueTypes.Int32 => v.AsInt(),
                    EnumValueTypes.UInt32 => v.AsUInt(),
                    EnumValueTypes.Int64 => v.AsInt64(),
                    EnumValueTypes.UInt64 => v.AsUInt64(),
                    EnumValueTypes.Float => v.AsFloat(),
                    EnumValueTypes.Double => v.AsDouble(),
                    EnumValueTypes.String => v.AsString(),
                    EnumValueTypes.Array => monoBehaviour.children
                        .Select(DeserializeMonoBehaviour)
                        .ToArray(),
                    EnumValueTypes.ByteArray => v.AsByteArray().data,
                    _ => null
                },
                null => monoBehaviour.children
                    .Aggregate(new Dictionary<string, dynamic?>(), (obj, c) =>
                    {
                        var key = c.templateField.name;
                        var value = DeserializeMonoBehaviour(c);
                        obj[key] = value;
                        return obj;
                    })
            };

        public static AssetTypeValueField SerializeMonoBehaviour(object? obj, AssetTypeTemplateField templateField)
        {
            var ret = new AssetTypeValueField();
            ret.templateField = templateField;

            ret.value = obj switch
            {
                null => new AssetTypeValue(EnumValueTypes.None, null),
                bool b => new AssetTypeValue(EnumValueTypes.Bool, b),
                sbyte int8 => new AssetTypeValue(EnumValueTypes.Int8, int8),
                byte uint8 => new AssetTypeValue(EnumValueTypes.UInt8, uint8),
                short int16 => new AssetTypeValue(EnumValueTypes.Int16, int16),
                ushort uint16 => new AssetTypeValue(EnumValueTypes.UInt16, uint16),
                int int32 => new AssetTypeValue(EnumValueTypes.Int32, int32),
                uint uint32 => new AssetTypeValue(EnumValueTypes.UInt32, uint32),
                long int64 => new AssetTypeValue(EnumValueTypes.Int64, int64),
                ulong uint64 => new AssetTypeValue(EnumValueTypes.UInt64, uint64),
                float f => new AssetTypeValue(EnumValueTypes.Float, f),
                double d => new AssetTypeValue(EnumValueTypes.Double, d),
                string s => new AssetTypeValue(EnumValueTypes.String, s),
                IEnumerable<byte> barr => new AssetTypeValue(
                    EnumValueTypes.ByteArray,
                    barr.ToArray()),
                _ => null
            };

            if (ret.value is not null)
            {
                return ret;
            }


            if (obj is IDictionary<string, dynamic> dict)
            {
                ret.value = null;
                ret.SetChildrenList(dict.Select(p =>
                {
                    var temp = templateField.children.First(f => f.name == p.Key);
                    AssetTypeValueField child = SerializeMonoBehaviour(p.Value, temp);
                    return child;
                }).ToArray());
            }
            else if (obj is IList<dynamic> arr)
            {
                var array = new AssetTypeArray(arr.Count);
                ret.SetChildrenList(arr
                    .Select(x =>
                    {
                        var temp = templateField.children[1];// [0]为size
                    return (AssetTypeValueField)SerializeMonoBehaviour(x, temp);
                    })
                    .ToArray());
                ret.value = new AssetTypeValue(EnumValueTypes.Array, array);
            }
            else
            {
                ret.value = null;
                Type type = obj!.GetType();
                ret.SetChildrenList(type.GetFields().Select(f =>
                {
                    var key = f.Name;
                    var temp = templateField.children.First(f => f.name == key);
                    AssetTypeValueField child = SerializeMonoBehaviour(f.GetValue(obj), temp);
                    return child;
                }).ToArray());
            }


            return ret;
        }
    }
}
