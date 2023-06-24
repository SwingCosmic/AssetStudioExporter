using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public interface IAssetTypeReader<T>
    {
        /// <summary>
        /// 根据<see cref="AssetTypeValueField"/>创建<typeparamref name="T"/>类型的AssetType
        /// </summary>
        /// <param name="value">序列化的<see cref="AssetTypeValueField"/></param>
        /// <returns><typeparamref name="T"/>类型的AssetType</returns>
        static abstract T Read(AssetTypeValueField value);

        static abstract AssetClassID AssetClassID { get; }
    }    
}
