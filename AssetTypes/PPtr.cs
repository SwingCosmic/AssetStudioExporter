using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public class PPtr<T> : IAssetTypeReader<PPtr<T>> where T : class
    {
        /// <summary>
        /// PPtr是一个特殊类型，不代表任何资产，但是会出现在任何资产的序列化里面
        /// </summary>
        public static AssetClassID AssetClassID { get; } = AssetClassID.Object;

        public int m_FileID;
        public long m_PathID;

        public T? Instance { get; set; }

        public PPtr(int fileID, long pathID)
        {
            m_FileID = fileID;
            m_PathID = pathID;
        }
        
        public PPtr(AssetTypeValueField value)
        {
            m_FileID = value["m_FileID"].GetValue().AsInt();
            m_PathID = value["m_PathID"].GetValue().AsInt64();
        }

        public static implicit operator AssetPPtr(PPtr<T> self)
        {
            return new AssetPPtr(self.m_FileID, self.m_PathID);
        }
        
        public static explicit operator PPtr<T>(AssetPPtr pptr)
        {
            return new PPtr<T>(pptr.fileID, pptr.pathID);
        }
        
        public AssetTypeValueField FollowReference(AssetsFileInstance relativeFile, AssetsManager am, bool onlyGetInfo = false)
        {
            var assetExternal = am.GetExtAsset(relativeFile, m_FileID, m_PathID, onlyGetInfo);
            return assetExternal.instance.GetBaseField();
        }

        public static PPtr<T> Read(AssetTypeValueField value)
        {
            return new PPtr<T>(value);
        }
    }

}
