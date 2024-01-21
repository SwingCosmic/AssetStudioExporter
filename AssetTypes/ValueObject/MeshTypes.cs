using AssetsTools.NET;
using AssetStudio;
using AssetStudioExporter.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AssetStudioExporter.AssetTypes.ValueObject;

public struct MinMaxAABB
{
    public Vector3 m_Min;
    public Vector3 m_Max;

    public MinMaxAABB(Vector3 min, Vector3 max)
    {
        m_Min = min;
        m_Max = max;
    }
}
/*
public class CompressedMesh
{
    public PackedFloatVector m_Vertices;
    public PackedFloatVector m_UV;
    public PackedFloatVector m_BindPoses;
    public PackedFloatVector m_Normals;
    public PackedFloatVector m_Tangents;
    public PackedIntVector m_Weights;
    public PackedIntVector m_NormalSigns;
    public PackedIntVector m_TangentSigns;
    public PackedFloatVector m_FloatColors;
    public PackedIntVector m_BoneIndices;
    public PackedIntVector m_Triangles;
    public PackedIntVector m_Colors;
    public uint m_UVInfo;

    public CompressedMesh(ObjectReader reader)
    {
        var version = reader.version;

        m_Vertices = new PackedFloatVector(reader);
        m_UV = new PackedFloatVector(reader);
        if (version[0] < 5)
        {
            m_BindPoses = new PackedFloatVector(reader);
        }
        m_Normals = new PackedFloatVector(reader);
        m_Tangents = new PackedFloatVector(reader);
        m_Weights = new PackedIntVector(reader);
        m_NormalSigns = new PackedIntVector(reader);
        m_TangentSigns = new PackedIntVector(reader);
        if (version[0] >= 5)
        {
            m_FloatColors = new PackedFloatVector(reader);
        }
        m_BoneIndices = new PackedIntVector(reader);
        m_Triangles = new PackedIntVector(reader);
        if (version[0] > 3 || (version[0] == 3 && version[1] >= 5)) //3.5 and up
        {
            if (version[0] < 5)
            {
                m_Colors = new PackedIntVector(reader);
            }
            else
            {
                m_UVInfo = reader.ReadUInt32();
            }
        }
    }
}
*/
public class StreamInfo : IAssetTypeReader<StreamInfo>
{
    public uint channelMask;
    public uint offset;
    public uint stride;
    public uint align;
    public byte dividerOp;
    public ushort frequency;

    public StreamInfo() { }

    public static StreamInfo Read(AssetTypeValueField value)
    {
        var stream = new StreamInfo();

        stream.channelMask = value["channelMask"].AsUInt;
        stream.offset = value["offset"].AsUInt;
        //if (version[0] < 4) //4.0 down

        //}
        //else
        stream.stride = value["stride"].AsByte;
        stream.dividerOp = value["dividerOp"].AsByte;
        stream.frequency = value["frequency"].AsUShort;

        return stream;
    }
}

public class ChannelInfo : IAssetTypeReader<ChannelInfo>
{
    public byte stream;
    public byte offset;
    public byte format;
    public byte dimension;

    public static ChannelInfo Read(AssetTypeValueField value)
    {
        var channel = new ChannelInfo();

        channel.stream = value["stream"].AsByte;
        channel.offset = value["offset"].AsByte;
        channel.format = value["format"].AsByte;
        channel.dimension = (byte)(value["dimension"].AsByte & 0xF);
        return channel;
    }
}

public class VertexData : IAssetTypeReader<VertexData>
{
    //public uint m_CurrentChannels;
    public uint m_VertexCount;
    public List<ChannelInfo> m_Channels;
    public List<StreamInfo> m_Streams;
    public byte[] m_DataSize;


    public static VertexData Read(AssetTypeValueField value)
    {
        var vertex = new VertexData();

        //if (version[0] < 2018)//2018 down
        //{
        //  m_CurrentChannels = reader.ReadUInt32();
        //}

        vertex.m_VertexCount = value["m_VertexCount"].AsUInt;
        vertex.m_Channels = value["m_Channels"]
            .AsList(c => ChannelInfo.Read(c));

        //if (version[0] < 5) //5.0 down
        //{

        //}
        //else //5.0 and up
        //{
        vertex.GetStreams(new[] {2019});
        //}

        vertex.m_DataSize = value["m_DataSize"].AsByteArray;
        
        return vertex;
    }

    private void GetStreams(int[] version)
    {
        var streamCount = m_Channels.Max(x => x.stream) + 1;
        m_Streams = new List<StreamInfo>();
        uint offset = 0;
        for (int s = 0; s < streamCount; s++)
        {
            uint chnMask = 0;
            uint stride = 0;
            for (int chn = 0; chn < m_Channels.Count; chn++)
            {
                var m_Channel = m_Channels[chn];
                if (m_Channel.stream == s)
                {
                    if (m_Channel.dimension > 0)
                    {
                        chnMask |= 1u << chn;
                        stride += m_Channel.dimension * MeshHelper.GetFormatSize(MeshHelper.ToVertexFormat(m_Channel.format, version));
                    }
                }
            }
            m_Streams.Add(new StreamInfo
            {
                channelMask = chnMask,
                offset = offset,
                stride = stride,
                dividerOp = 0,
                frequency = 0
            });
            offset += m_VertexCount * stride;
            //static size_t AlignStreamSize (size_t size) { return (size + (kVertexStreamAlign-1)) & ~(kVertexStreamAlign-1); }
            offset = (offset + (16u - 1u)) & ~(16u - 1u);
        }
    }

    private void GetChannels(int[] version)
    {
        m_Channels = new List<ChannelInfo>(6);
        for (int i = 0; i < 6; i++)
        {
            m_Channels.Add(new ChannelInfo());
        }
        for (var s = 0; s < m_Streams.Count; s++)
        {
            var m_Stream = m_Streams[s];
            var channelMask = new BitArray(new[] { (int)m_Stream.channelMask });
            byte offset = 0;
            for (int i = 0; i < 6; i++)
            {
                if (channelMask.Get(i))
                {
                    var m_Channel = m_Channels[i];
                    m_Channel.stream = (byte)s;
                    m_Channel.offset = offset;
                    switch (i)
                    {
                        case 0: //kShaderChannelVertex
                        case 1: //kShaderChannelNormal
                            m_Channel.format = 0; //kChannelFormatFloat
                            m_Channel.dimension = 3;
                            break;
                        case 2: //kShaderChannelColor
                            m_Channel.format = 2; //kChannelFormatColor
                            m_Channel.dimension = 4;
                            break;
                        case 3: //kShaderChannelTexCoord0
                        case 4: //kShaderChannelTexCoord1
                            m_Channel.format = 0; //kChannelFormatFloat
                            m_Channel.dimension = 2;
                            break;
                        case 5: //kShaderChannelTangent
                            m_Channel.format = 0; //kChannelFormatFloat
                            m_Channel.dimension = 4;
                            break;
                    }
                    offset += (byte)(m_Channel.dimension * MeshHelper.GetFormatSize(MeshHelper.ToVertexFormat(m_Channel.format, version)));
                }
            }
        }
    }

}


public class BoneWeights4 : IAssetTypeReader<BoneWeights4>
{
    public float[] weight;
    public int[] boneIndex;

    public BoneWeights4()
    {
        weight = new float[4];
        boneIndex = new int[4];
    }


    public static BoneWeights4 Read(AssetTypeValueField value)
    {
        var weights = new BoneWeights4();

        weights.weight = value["weight"].AsArray(w => w.AsFloat);
        weights.boneIndex = value["boneIndex"].AsArray(i => i.AsInt);

        return weights;
    }
}

//public class BlendShapeVertex
//{
//    public Vector3 vertex;
//    public Vector3 normal;
//    public Vector3 tangent;
//    public uint index;

//    public BlendShapeVertex(ObjectReader reader)
//    {
//        vertex = reader.ReadVector3();
//        normal = reader.ReadVector3();
//        tangent = reader.ReadVector3();
//        index = reader.ReadUInt32();
//    }
//}

//public class MeshBlendShape
//{
//    public string name;
//    public uint firstVertex;
//    public uint vertexCount;
//    public bool hasNormals;
//    public bool hasTangents;

//    public MeshBlendShape(ObjectReader reader)
//    {
//        var version = reader.version;

//        if (version[0] == 4 && version[1] < 3) //4.3 down
//        {
//            name = reader.ReadAlignedString();
//        }
//        firstVertex = reader.ReadUInt32();
//        vertexCount = reader.ReadUInt32();
//        if (version[0] == 4 && version[1] < 3) //4.3 down
//        {
//            var aabbMinDelta = reader.ReadVector3();
//            var aabbMaxDelta = reader.ReadVector3();
//        }
//        hasNormals = reader.ReadBoolean();
//        hasTangents = reader.ReadBoolean();
//        if (version[0] > 4 || (version[0] == 4 && version[1] >= 3)) //4.3 and up
//        {
//            reader.AlignStream();
//        }
//    }
//}

//public class MeshBlendShapeChannel
//{
//    public string name;
//    public uint nameHash;
//    public int frameIndex;
//    public int frameCount;

//    public MeshBlendShapeChannel(ObjectReader reader)
//    {
//        name = reader.ReadAlignedString();
//        nameHash = reader.ReadUInt32();
//        frameIndex = reader.ReadInt32();
//        frameCount = reader.ReadInt32();
//    }
//}

//public class BlendShapeData
//{
//    public List<BlendShapeVertex> vertices;
//    public List<MeshBlendShape> shapes;
//    public List<MeshBlendShapeChannel> channels;
//    public float[] fullWeights;

//    public BlendShapeData(ObjectReader reader)
//    {
//        var version = reader.version;

//        if (version[0] > 4 || (version[0] == 4 && version[1] >= 3)) //4.3 and up
//        {
//            int numVerts = reader.ReadInt32();
//            vertices = new List<BlendShapeVertex>();
//            for (int i = 0; i < numVerts; i++)
//            {
//                vertices.Add(new BlendShapeVertex(reader));
//            }

//            int numShapes = reader.ReadInt32();
//            shapes = new List<MeshBlendShape>();
//            for (int i = 0; i < numShapes; i++)
//            {
//                shapes.Add(new MeshBlendShape(reader));
//            }

//            int numChannels = reader.ReadInt32();
//            channels = new List<MeshBlendShapeChannel>();
//            for (int i = 0; i < numChannels; i++)
//            {
//                channels.Add(new MeshBlendShapeChannel(reader));
//            }

//            fullWeights = reader.ReadSingleArray();
//        }
//        else
//        {
//            var m_ShapesSize = reader.ReadInt32();
//            var m_Shapes = new List<MeshBlendShape>();
//            for (int i = 0; i < m_ShapesSize; i++)
//            {
//                m_Shapes.Add(new MeshBlendShape(reader));
//            }
//            reader.AlignStream();
//            var m_ShapeVerticesSize = reader.ReadInt32();
//            var m_ShapeVertices = new List<BlendShapeVertex>(); //MeshBlendShapeVertex
//            for (int i = 0; i < m_ShapeVerticesSize; i++)
//            {
//                m_ShapeVertices.Add(new BlendShapeVertex(reader));
//            }
//        }
//    }
//}

public enum GfxPrimitiveType
{
    Triangles = 0,
    TriangleStrip = 1,
    Quads = 2,
    Lines = 3,
    LineStrip = 4,
    Points = 5
};

public class SubMesh : IAssetTypeReader<SubMesh>
{
    public uint firstByte;
    public uint indexCount;
    public GfxPrimitiveType topology;
    public uint triangleCount;
    public uint baseVertex;
    public uint firstVertex;
    public uint vertexCount;
    public AABB localAABB;

    public static SubMesh Read(AssetTypeValueField value)
    {
        var mesh = new SubMesh();

        mesh.firstByte = value["firstByte"].AsUInt;
        mesh.indexCount = value["indexCount"].AsUInt;
        mesh.topology = (GfxPrimitiveType)value["indexCount"].AsInt;

        //if (version[0] < 4) //4.0 down
        //{
        //    triangleCount = reader.ReadUInt32();
        //}

        var baseVertexValue = value["baseVertex"];
        if (!baseVertexValue.IsDummy)
        {
            mesh.baseVertex = baseVertexValue.AsUInt;
        }


        //if (version[0] >= 3) //3.0 and up
        //{
        mesh.firstVertex = value["firstVertex"].AsUInt;
        mesh.vertexCount = value["vertexCount"].AsUInt;
        mesh.localAABB = value["localAABB"].AsAABB();
        //}

        return mesh;
    }
}