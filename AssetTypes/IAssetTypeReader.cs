using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes
{
    public interface IAssetTypeReader<T>
    {
        static abstract T Read(AssetTypeValueField value);
    }
}
