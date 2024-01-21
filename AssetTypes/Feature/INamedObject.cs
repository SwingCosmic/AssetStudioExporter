using System;

namespace AssetStudioExporter.AssetTypes.Feature;


/// <summary>
/// 有名称的对象
/// </summary>
public interface INamedObject
{
    /// <summary>
    /// 对象的名称
    /// </summary>
    public string Name { get; set; }

}


public interface IStructureExportable
{
    /// <summary>
    /// 返回一个可以用于序列化JSON的动态对象
    /// </summary>
    dynamic? DeserializeStructure();

    /// <summary>
    /// 序列化为指定<paramref name="type"/>的对象
    /// </summary>
    /// <param name="type">对象的类型</param>
    object DeserializeObject(Type type);
}