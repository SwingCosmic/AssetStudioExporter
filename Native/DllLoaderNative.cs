using System.Reflection;
using System.Runtime.InteropServices;

namespace AssetStudioExporter.Native
{
    /// <summary>
    /// 使用原生方法加载的loader
    /// </summary>
    public static class DllLoaderNative
    {
        private static string GetDirectedDllDirectory()
        {
            var localPath = Assembly.GetExecutingAssembly().Location;
            var localDir = Path.GetDirectoryName(localPath);

            var subDir = Environment.Is64BitProcess ? "x64" : "x86";

            var directedDllDir = Path.Combine(localDir, subDir);

            return directedDllDir;
        }

        private static string GetFileName(string dllName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"{dllName}.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return $"lib{dllName}.so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"lib{dllName}.dylib";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static void PreloadDll(string dllName)
        {
            var name = Path.Combine(GetDirectedDllDirectory(), GetFileName(dllName));
            Console.WriteLine($"dll位置：{name}");
            NativeLibrary.Load(name);
        }
    }
}
