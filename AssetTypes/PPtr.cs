using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudio;
using AssetStudioExporter.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes;

public class PPtr<T> : IAssetTypeReader<PPtr<T>> where T : class
{

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
        m_FileID = value["m_FileID"].AsInt;
        m_PathID = value["m_PathID"].AsLong;
    }

    public static implicit operator AssetPPtr(PPtr<T> self)
    {
        return new AssetPPtr(self.m_FileID, self.m_PathID);
    }
    
    public static explicit operator PPtr<T>(AssetPPtr pptr)
    {
        return new PPtr<T>(pptr.FileId, pptr.PathId);
    }

    /// <summary>
    /// 追随<see cref="PPtr{T}"/>的引用，并返回目标类型的<see cref="AssetTypeValueField"/>
    /// </summary>
    /// <param name="relativeFile">相对的AssetsFile</param>
    /// <param name="am">所用的AssetsManager</param>
    /// <param name="onlyGetInfo">只获取信息</param>
    public AssetTypeValueField FollowReference(AssetsFileInstance relativeFile, AssetsManager am, bool onlyGetInfo = false)
    {
        return GetAssetReference(relativeFile, am).baseField;
    }

    /// <summary>
    /// 查找<see cref="PPtr{T}"/>的引用并将<see cref="Instance"/>设置为实例
    /// </summary>
    /// <typeparam name="R"><typeparamref name="T"/>类型的读取器</typeparam>
    /// <param name="relativeFile">相对的AssetsFile</param>
    /// <param name="am">所用的AssetsManager</param>
    [MemberNotNull(nameof(Instance))]
    public void FindInstance<R>(AssetsFileInstance relativeFile, AssetsManager am) where R : IAssetTypeReader<T>
    {
        try
        {
            var bf = FollowReference(relativeFile, am, false);
            var version = VersionCache.GetVersion(relativeFile);
            Instance = R.Read(bf, version);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Cannot read PPtr {{ FileID = {m_FileID}, PathID = {m_PathID} }}", ex);
        }

    }

    /// <summary>
    /// 类似<see cref="FollowReference"/>，但返回<see cref="AssetExternal"/>
    /// </summary>
    public AssetExternal GetAssetReference(AssetsFileInstance relativeFile, AssetsManager am, bool onlyGetInfo = false)
    {
        return am.GetExtAsset(relativeFile, m_FileID, m_PathID, onlyGetInfo);
    }

    /// <summary>
    /// 将引用的类型<typeparamref name="T"/>重新解读为类型<typeparamref name="U"/><br/>
    /// 如果<see cref="Instance"/>有值会丢弃
    /// </summary>
    /// <typeparam name="U">新的类型</typeparam>
    public PPtr<U> Cast<U>() where U : class
    {
        return new PPtr<U>(m_FileID, m_PathID);
    }

    public static PPtr<T> Read(AssetTypeValueField value, UnityVersion version)
    {
        return Read(value);
    }
    public static PPtr<T> Read(AssetTypeValueField value)
    {
        return new PPtr<T>(value);
    }
}


public class PPtr : PPtr<object>
{
    public PPtr(AssetTypeValueField value) : base(value)
    {
    }

    public PPtr(int fileID, long pathID) : base(fileID, pathID)
    {
    }
}
