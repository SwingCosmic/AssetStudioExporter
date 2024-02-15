using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public abstract class Object
    {
        public static Object Create() => new EmptyObject();

        internal class EmptyObject : Object, IAssetType, IAssetTypeReader<EmptyObject>
        {
            public static AssetClassID AssetClassID { get; } = AssetClassID.Object;
            public static EmptyObject Read(AssetTypeValueField value, UnityVersion version)
            {
                return new EmptyObject();
            }
        }
    }
}
