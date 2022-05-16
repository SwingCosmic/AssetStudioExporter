using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public sealed class AudioClip
    {
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
        public byte[] m_AudioData;

        public AudioClip()
        {

        }

        public static AudioClip ReadAudioClip(AssetTypeValueField baseField)
        {
            var ac = new AudioClip();

            ac.m_Name = baseField.Get("m_Name").GetValue().AsString();
            ac.m_LoadType = baseField.Get("m_LoadType").GetValue().AsInt();
            ac.m_Channels = baseField.Get("m_Channels").GetValue().AsInt();
            ac.m_Frequency = baseField.Get("m_Frequency").GetValue().AsInt();
            ac.m_BitsPerSample = baseField.Get("m_BitsPerSample").GetValue().AsInt();
            ac.m_Length = baseField.Get("m_Length").GetValue().AsFloat();
            ac.m_IsTrackerFormat = baseField.Get("m_IsTrackerFormat").GetValue().AsBool();
            //ac.m_Ambisonic = bf.Get("m_Ambisonic").GetValue().AsBool();
            ac.m_SubsoundIndex = baseField.Get("m_SubsoundIndex").GetValue().AsInt();
            ac.m_PreloadAudioData = baseField.Get("m_PreloadAudioData").GetValue().AsBool();
            ac.m_LoadInBackground = baseField.Get("m_LoadInBackground").GetValue().AsBool();
            ac.m_Legacy3D = baseField.Get("m_Legacy3D").GetValue().AsBool();
            ac.m_CompressionFormat = (AudioCompressionFormat)baseField.Get("m_CompressionFormat").GetValue().AsInt();

            var m_Resource = baseField.Get("m_Resource");
            if (!m_Resource.IsDummy())
            {
                ac.m_Source = m_Resource.Get("m_Source").GetValue().AsString();
                ac.m_Offset = (long)m_Resource.Get("m_Offset").GetValue().AsUInt64();
                ac.m_Size = (long)m_Resource.Get("m_Size").GetValue().AsUInt64();
            }
            return ac;
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
