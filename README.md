# AssetStudioExporter

适配 [AssetsTools.NET](https://github.com/nesrak1/AssetsTools.NET) V3(AT3) 的AssetBundle导出工具，基于AssetStudio进行修改

> 本仓库是生成的是.NET库，用于简化使用AssetsTools.NET编写解包代码的逻辑，并不是一个独立的可执行文件，请先使用其他工具探索AssetBundle(AB)的结构并定位所需内容

---

## 特性

* 同时支持AssetFile和AssetBundle文件
* 经过简单的封装，以简单的方式导出`Texture2D`, `AudioClip`, `Font`等各类资源
* 更好的`Texture2D`
  * 全面的图片纹理格式支持，能导出几乎所有种类的`Texture2D`。
  * 同时带有AssetRipper的解码器和AssetStudio自己的解码器，两者有各自的特点，可根据实际情况自由切换
* 支持反序列化`MonoBehaviour`，只需要提供由Il2CppDumper/Cpp2Il生成的dummy DLL，调一个简单方法即可导出JSON
* 直接的资源类型抽象，以及简单的导出参数配置，可以方便地实现新的导出方法，并灵活控制封装程度，不会对编写解包代码产生较大的侵入性和限制
* 简单的跨平台
  * 同时带有Windows/Mac/Linux的x86/x64第三方库二进制，无分平台包，无需判断平台和重新编译，编译一次到处运行

## 开始使用

本仓库包含了对AssetsTools.NET V3(AT3)的封装，使用前先阅读AT3的开发文档

### 添加引用

1. 添加对`AssetsTools.NET`和`AssetsTools.NET.Texture`（如果需要处理图片）的nuget包
2. 克隆本仓库并作为项目依赖添加

### 定位Asset

具体参考AT3的文档

```csharp
var am = new AssetsManager();
am.LoadClassPackage(Path.Combine(AppContext.BaseDirectory, "classdata.tpk"));

var root = new DirectoryInfo("path/to/ab");
foreach (var file in root.EnumerateFiles("*.bundle", SearchOption.AllDirectories))
{
  var bf = am.LoadBundleFile(file.FullName, true);
  var inst = am.LoadAssetsFileFromBundle(bf, 0, true);

  foreach (var info in inst.file.AssetInfos) {
    var needExport = false;
    // 尝试定位到所需的资源
    // ...
    if(needExport) {
      // 使用Exporter进行导出
    }
  }
}


```

### 导出资源

* 如果只是想简单的导出符合要求的资源，可以根据asset的`AssetClassID`来使用不同的导出类。可以使用`ExporterSetting.Default`来设置全局默认导出设置。

```csharp
IAssetTypeExporter? exporter = null;
switch (info.TypeId)
{
  case (int)AssetClassID.Texture2D:
    exporter = Texture2D.Read(baseField);
    break;
  case (int)AssetClassID.AudioClip:
    exporter = AudioClip.Read(baseField);
    break;
  case (int)AssetClassID.TextAsset:
    exporter = TextAsset.Read(baseField);
    break;
  case (int)AssetClassID.Font:
    exporter = Font.Read(baseField);
    break;
  // 检查其他类型
  // ...
  default:
    break;
}

if (exporter is not null) 
{
  exporter.Export(inst, "path/to/exported/file");
}
```

* 对于需要更精细控制的asset，也可以手动传入参数

```csharp
if (info.TypeId == (int)AssetClassID.Texture2D) 
{
  var texture2D = Texture2D.Read(baseField);
  var fileName = Path.Combind("path/to/export", texture2D.m_Name + ".webp");
  using (var fs = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite)) 
  { 
    // 使用webp导出并写入流
    texture2D.Export(inst, fs, ImageFormat.Webp);
  }
}

```

### 反序列化MonoBehavoiur

读取MonoBehavoiur是一件十分复杂的操作，但如果只需要简单地读取指定`GameObject`中存储的数据的结构（例如反序列化并生成JSON），使用提供的方法可以进行很大程度的简化

```csharp

if (info.TypeId == (int)AssetClassID.GameObject) 
{
  // 确保加载了GameObject的依赖
  // ...

  // 读取该GameObject上包含的所有MonoBehaviour并筛选出需要的那个
  var assetFile = info
    .ReadAllMonoBehaviours(inst, am)
    .First(m => m.m_Script.m_ClassName == "YourClass" && m.m_Script.m_Namespace == "YourNamespace")
    .assetFile!;

  // dump出的dummy DLL所在目录
  var managedDir = "path/to/bin/Data/Managed";
  var monoBehaviour = MonoDeserializer.GetMonoBaseField(am, inst, assetFile, managedDir);

  // 将monoBehaviour的字段信息转换为对象，
  // 该对象和原始的类结构类似，且生成JSON的内容一致，
  // 但并不是原来的类的实例，而是由字典、数组和基本类型等组成的复杂结构
  dynamic? obj = AssetDataUtil.DeserializeMonoBehaviour(monoBehaviour);
  var json = JsonSerializer.Serialize(obj);
  Console.WriteLine(json);
}

```

## 一些工具方法


### 读取资产路径

部分资产在打包时包含路径信息，在AssetStudio中会显示为 "Container" 列，可以通过扩展方法`AssetsFileInstance.GetContainers`获取当前文件里的所有路径信息。

```csharp
Dictionary<string, AssetPPtr> containers = inst.GetContainers(am);
foreach (var (name, pptr) in containers) 
{
  Console.WriteLine($"{name}: FileId={pptr.FileId}, PathId={pptr.PathId}");
  // 根据PPtr找到对应的资产对象
}

```
