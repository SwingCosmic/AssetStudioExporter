using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public class Object : IAssetType, IAssetTypeReader<Object>
    {
        public static AssetClassID AssetClassID { get; } = AssetClassID.Object;

        public static Object Read(AssetTypeValueField value)
        {
            return new Object();
        }
    }
}
