# AssetStudioExporter

基于AssetStudio修改的适配AssetsTools.NET v3的AssetBundle导出工具

---

## 特性

* 同时支持AssetFile和AssetBundle文件
* 经过简单的封装，以简单的方式导出Texture2D, AudioClip, Font等各类资源
* 更好的Texture2D
  * 全面的图片纹理格式支持，能导出几乎所有种类的Texture2D。
  * 同时带有AssetRipper的解码器和AssetStudio自己的解码器，两者有各自的特点，可根据实际情况自由切换
* 支持反序列化MonoBehaviour，只需要提供由Il2CppDumper/Cpp2Il生成的dummy DLL，调一个简单方法即可导出JSON
* 直接的资源类型抽象，以及简单的导出参数配置，可以方便地实现新的导出方法，并灵活控制封装程度，不会对编写解包代码产生较大的侵入性和限制
* 简单的跨平台
  * 同时带有Windows/Mac/Linux的x86/x64第三方库二进制，无分平台包，无需判断平台和重新编译，编译一次到处运行