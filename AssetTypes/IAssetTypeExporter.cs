using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes
{
    public interface IAssetTypeExporter
    {
        /// <summary>
        /// 导出当前AssetType
        /// </summary>
        /// <param name="assetsFile">AssetsFile</param>
        /// <param name="stream">要输出的流</param>
        bool Export(AssetsFileInstance assetsFile, Stream stream);

        void Export(AssetsFileInstance assetsFile, string fileName)
        {
            using(var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                if (Export(assetsFile, fs))
                {
                    return;
                }
            }
            // 未能导出，删掉空文件
            File.Delete(fileName);
        }
    }
}
