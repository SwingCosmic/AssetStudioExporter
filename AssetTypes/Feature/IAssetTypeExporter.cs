using AssetsTools.NET.Extra;

namespace AssetStudioExporter.AssetTypes.Feature;

public interface IAssetTypeExporter<TSetting> where TSetting : class
{
    bool Export(AssetsFileInstance assetsFile, Stream stream, TSetting setting);
}

public interface IAssetTypeExporter
{
    /// <summary>
    /// 导出当前AssetType
    /// </summary>
    /// <param name="assetsFile">AssetsFile</param>
    /// <param name="stream">要输出的流</param>
    /// <returns>是否导出成功</returns>
    bool Export(AssetsFileInstance assetsFile, Stream stream);


    void Export(AssetsFileInstance assetsFile, string fileName)
    {
        using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            if (Export(assetsFile, fs))
            {
                return;
            }
        }
        // 未能导出，删掉空文件
        File.Delete(fileName);
    }

    string GetFileExtension(string fileName);
}


public interface IFormatAssetTypeExporter<TFormat>
{
    bool Export(AssetsFileInstance assetsFile, Stream stream, TFormat format);
}

public interface IMultipleFileAssetTypeExporter
{
    bool Export(AssetsFileInstance assetsFile, DirectoryInfo outputDir);
}
