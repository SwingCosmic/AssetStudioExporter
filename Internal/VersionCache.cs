using AssetsTools.NET.Extra;
using System.Runtime.CompilerServices;

namespace AssetStudioExporter.Internal;

internal static class VersionCache
{
    static readonly ConditionalWeakTable<AssetsFileInstance, UnityVersion> cache = new();

    public static UnityVersion GetVersion(AssetsFileInstance instance)
    {
        if (!cache.TryGetValue(instance, out var version))
        {
            version = new UnityVersion(instance.file.Metadata.UnityVersion);
            cache.AddOrUpdate(instance, version);
        }
        return version;
    }
}
