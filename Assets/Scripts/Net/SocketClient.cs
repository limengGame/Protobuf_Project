using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using Net;

public enum DisType
{
    Exception = 1,
    Disconnect,
}

public class SocketClient {
    private TcpClient client = null;
    private NetworkStream networkStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    public SocketClient()
    {
    }

    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    public void OnRemove()
    {
        this.Close();
        reader.Close();
        memStream.Close();
    }

    public void SendConnect()
    {
        ConnectServer(AppConst.Address, AppConst.port);
    }

    public void ConnectServer(string host, int port)
    {
        client = null;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;

        try
        {
            client.BeginConnect(host, port, new System.AsyncCallback(OnConnect), null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    void OnConnect(IAsyncResult asr)
    {
        networkStream = client.GetStream();
        networkStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        NetManager.Instance.OnConnect();
    }

    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {
                OnDisconnected(DisType.Exception, "bytesRead < 1");
                return;
            }

            OnReceive(byteBuffer, bytesRead);

            lock (client.GetStream())
            {
                Array.Clear(byteBuffer, 0, byteBuffer.Length);
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            OnDisconnected(DisType.Disconnect, ex.Message);
        }

    }

    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {
            ushort messageLength = reader.ReadUInt16();
            if (RemainingBytes() >= messageLength)
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(reader.ReadBytes(messageLength));
                memoryStream.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(memoryStream);
            }
            else
            {
                memStream.Position -= 2;
                break;
            }
        }
        byte[] leftBytes = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);
        memStream.Write(leftBytes, 0, leftBytes.Length);
    }

    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader binaryReader = new BinaryReader(ms);
        byte[] message = binaryReader.ReadBytes((int)(ms.Length - ms.Position));
        ByteBuffer buffer = new ByteBuffer(message);
        int mainId = buffer.ReadShort();
        NetManager.Instance.DispatchProto(mainId, buffer);
    }

    public void SendMessage(ByteBuffer buffer)
    {
        SessionSend(buffer.ToBytes());
        buffer.Close();
    }

    void SessionSend(byte[] message)
    {
        WriteMessage(message);
    }

    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter binaryWriter = new BinaryWriter(ms);
            ushort length = (ushort)message.Length;
            binaryWriter.Write(length);
            binaryWriter.Write(message);
            binaryWriter.Flush();
            if (client != null && client.Connected)
            {
                byte[] byteArray = ms.ToArray();
                networkStream.BeginWrite(byteArray, 0, byteArray.Length, new AsyncCallback(EndWrite), null);
            }
            else
            {
                Debug.LogError("client disconnected!");
            }
        }
    }

    void EndWrite(IAsyncResult iar)
    {
        try
        {
            networkStream.EndWrite(iar);
        }
        catch (Exception ex)
        {
            Debug.LogError("write false : " + ex.Message);
        }
    }

    void OnDisconnected(DisType type, string msg)
    {
        Close();

    }

    public void Close()
    {
        if (client != null)
        {
            if(client.Connected) client.Close();
            client = null;
        }
    }

}
