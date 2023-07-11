using AssetStudioExporter.AssetTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter
{
    public class ExporterSetting
    {
        public static ExporterSetting Default { get; set; } = new ExporterSetting()
        {

        };

        /// <summary>
        /// 可导出为图片的资源（如<see cref="Texture2D"/>, Sprite等）的文件格式<br/>
        /// 部分生成多个文件的组合资源可能不会遵循这个值
        /// </summary>
        public ImageFormat ImageExportFormat { get; set; } = ImageFormat.Png;
    }
}
