using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudioExporter.AssetTypes;
using AssetStudioExporter.AssetTypes.Feature;
using AssetStudioExporter.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public sealed class AudioClip : INamedObject, IAssetType, IAssetTypeReader<AudioClip>, IAssetTypeExporter
    {

        public static AssetClassID AssetClassID { get; } = AssetClassID.AudioClip;

        public string Name
        {
            get => m_Name; 
            set => m_Name = value;
        }

        public string m_Name;


        public int m_Format;
        public AudioType m_Type;
        public bool m_3D;
        public bool m_UseHardware;

        //version 5
        public int m_LoadType;
        public int m_Channels;
        public int m_Frequency;
        public int m_BitsPerSample;
        public float m_Length;
        public bool m_IsTrackerFormat;
        public int m_SubsoundIndex;
        public bool m_PreloadAudioData;
        public bool m_LoadInBackground;
        public bool m_Legacy3D;
        public AudioCompressionFormat m_CompressionFormat;

        public string m_Source;
        public long m_Offset; //ulong
        public long m_Size; //ulong

        [Obsolete("无法直接读取m_AudioData")]
        internal readonly byte[] m_AudioData = Array.Empty<byte>();


        public byte[] GetAudioData(AssetsFileInstance assetsFile)
        {
            return assetsFile.GetAssetData(m_Source, (uint)m_Size, m_Offset);
        }

        public AudioClip()
        {

        }

        public static AudioClip Read(AssetTypeValueField value)
        {
            var ac = new AudioClip();

            ac.m_Name = value.Get("m_Name").AsString;
            ac.m_LoadType = value.Get("m_LoadType").AsInt;
            ac.m_Channels = value.Get("m_Channels").AsInt;
            ac.m_Frequency = value.Get("m_Frequency").AsInt;
            ac.m_BitsPerSample = value.Get("m_BitsPerSample").AsInt;
            ac.m_Length = value.Get("m_Length").AsFloat;
            ac.m_IsTrackerFormat = value.Get("m_IsTrackerFormat").AsBool;
            //ac.m_Ambisonic = bf.Get("m_Ambisonic").AsBool;
            ac.m_SubsoundIndex = value.Get("m_SubsoundIndex").AsInt;
            ac.m_PreloadAudioData = value.Get("m_PreloadAudioData").AsBool;
            ac.m_LoadInBackground = value.Get("m_LoadInBackground").AsBool;
            ac.m_Legacy3D = value.Get("m_Legacy3D").AsBool;
            ac.m_CompressionFormat = (AudioCompressionFormat)value.Get("m_CompressionFormat").AsInt;

            var m_Resource = value.Get("m_Resource");
            if (!m_Resource.IsDummy)
            {
                ac.m_Source = m_Resource.Get("m_Source").AsString;
                ac.m_Offset = (long)m_Resource.Get("m_Offset").AsULong;
                ac.m_Size = (long)m_Resource.Get("m_Size").AsULong;
            }
            return ac;
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream)
        {
            var m_AudioData = GetAudioData(assetsFile);
            var converter = new AudioClipConverter(this, new[] { 2019 });
            var wav = converter.ConvertToWav(m_AudioData);
            if (wav is null)
            {
                Console.WriteLine($"Export audio clip '{m_Name}' failed");
                return false;
            }
            stream.Write(wav);
            return true;
        }

        public string GetFileExtension(string fileName)
        {
            return ".wav";
        }
    }

    public enum AudioType
    {
        UNKNOWN,
        ACC,
        AIFF,
        IT = 10,
        MOD = 12,
        MPEG,
        OGGVORBIS,
        S3M = 17,
        WAV = 20,
        XM,
        XMA,
        VAG,
        AUDIOQUEUE
    }

    public enum AudioCompressionFormat
    {
        PCM,
        Vorbis,
        ADPCM,
        MP3,
        VAG,
        HEVAG,
        XMA,
        AAC,
        GCADPCM,
        ATRAC9
    }
}
