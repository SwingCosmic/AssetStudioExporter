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
                    searchPath = searchPath.Substring(9);
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
    }
}
