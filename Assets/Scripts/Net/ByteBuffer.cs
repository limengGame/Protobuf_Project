using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Net
{
    public class ByteBuffer
    {
        MemoryStream stream = null;
        BinaryReader reader = null;
        BinaryWriter writer = null;

        public ByteBuffer()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data)
        {
            if (data != null)
            {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);
            }
            else
            {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        public void Close()
        {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();
            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public void WriteByte(byte v)
        {
            writer.Write(v);
        }
        public void WriteShort(ushort v)
        {
            writer.Write(v);
        }
        public void WriteInt(int v)
        {
            writer.Write(v);
        }
        public void WriteLong(long v)
        {
            writer.Write(v);
        }
        public void WriteFloat(float v)
        {
            writer.Write(v);
        }
        public void WriteDouble(double v)
        {
            writer.Write(v);
        }
        public void WriteString(string v)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(v);
            WriteShort((ushort)bytes.Length);
            writer.Write(v);
        }
        public void WriteBytes(byte[] v)
        {
            WriteInt(v.Length);
            writer.Write(v);
        }


        public byte ReadByte()
        {
            return reader.ReadByte();
        }
        public ushort ReadShort()
        {
            return (ushort)reader.ReadUInt16();
        }
        public int ReadInt()
        {
            return reader.ReadInt32();
        }
        public long ReadLong()
        {
            return reader.ReadInt64();
        }
        public float ReadFloat()
        {
            return reader.ReadSingle();
        }
        public double ReadDouble()
        {
            return reader.ReadDouble();
        }
        public string ReadString()
        {
            ushort length = ReadShort();
            byte[] buffer = new byte[length];
            buffer = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(buffer);
        }
        public byte[] ReadBytes()
        {
            int length = ReadInt();
            return reader.ReadBytes(length);
        }

        public byte[] ToBytes()
        {
            writer.Flush();
            return stream.ToArray();
        }
        public void Flush()
        {
            writer.Flush();
        }

    }

}