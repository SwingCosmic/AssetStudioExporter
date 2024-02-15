using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetStudioExporter.AssetTypes;
using AssetStudioExporter.AssetTypes.Feature;
using AssetStudioExporter.Export;
using AssetStudioExporter.Util;
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


        UnityVersion version;
        public byte[] GetAudioData(AssetsFileInstance assetsFile)
        {
            return assetsFile.GetAssetData(m_Source, (uint)m_Size, m_Offset);
        }

        public AudioClip()
        {

        }

        public static AudioClip Read(AssetTypeValueField value, UnityVersion version)
        {
            var ac = new AudioClip();
            ac.version = version;

            ac.m_Name = value["m_Name"].AsString;
            ac.m_LoadType = value["m_LoadType"].AsInt;
            ac.m_Channels = value["m_Channels"].AsInt;
            ac.m_Frequency = value["m_Frequency"].AsInt;
            ac.m_BitsPerSample = value["m_BitsPerSample"].AsInt;
            ac.m_Length = value["m_Length"].AsFloat;
            ac.m_IsTrackerFormat = value["m_IsTrackerFormat"].AsBool;
            //ac.m_Ambisonic = bf["m_Ambisonic"].AsBool;
            ac.m_SubsoundIndex = value["m_SubsoundIndex"].AsInt;
            ac.m_PreloadAudioData = value["m_PreloadAudioData"].AsBool;
            ac.m_LoadInBackground = value["m_LoadInBackground"].AsBool;
            ac.m_Legacy3D = value["m_Legacy3D"].AsBool;
            ac.m_CompressionFormat = (AudioCompressionFormat)value["m_CompressionFormat"].AsInt;

            var m_Resource = value["m_Resource"];
            if (!m_Resource.IsDummy)
            {
                ac.m_Source = m_Resource["m_Source"].AsString;
                ac.m_Offset = (long)m_Resource["m_Offset"].AsULong;
                ac.m_Size = (long)m_Resource["m_Size"].AsULong;
            }
            return ac;
        }

        public bool Export(AssetsFileInstance assetsFile, Stream stream)
        {
            var m_AudioData = GetAudioData(assetsFile);
            var converter = new AudioClipConverter(this, version.ToIntArray());
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
