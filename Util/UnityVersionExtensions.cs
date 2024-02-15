using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.Util;

public static class UnityVersionExtensions
{
    public static int[] ToIntArray(this UnityVersion version)
    {
        return new[]
        {
            version.major,
            version.minor,
            version.patch
        };
    }
}
