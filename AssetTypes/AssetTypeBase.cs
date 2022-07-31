using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public abstract class AssetTypeBase
    {
        protected AssetsManager am;

        public AssetTypeBase(AssetsManager assetsManager)
        {
            am = assetsManager;
        }
    }
}
