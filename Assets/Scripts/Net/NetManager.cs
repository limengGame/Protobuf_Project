using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using System;
using Net;
using proto;
using System.IO;

public class NetManager : SingletonMonoBehaviour<NetManager>
{
    Dictionary<Type, TocHandler> _handlerDic = null;

    private SocketClient socketClient = null;
    public SocketClient SocketClient
    {
        get {
            if (socketClient == null)
                socketClient = new SocketClient();
            return socketClient;
        }
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _handlerDic = new Dictionary<Type, TocHandler>();
        SocketClient.OnRegister();
    }

    public void SendConnect()
    {
        SocketClient.SendConnect();
    }

    public void OnConnect()
    {
        Debug.Log("Connect successfully!");
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected!");
    }


    static Queue<KeyValuePair<Type, object>> sEvents = new Queue<KeyValuePair<Type, object>>();

    private void Update()
    {
        if (sEvents.Count > 0)
        {
            while (sEvents.Count > 0)
            {
                KeyValuePair<Type, object> _event = sEvents.Dequeue();
                if (_handlerDic.ContainsKey(_event.Key))
                {
                    _handlerDic[_event.Key](_event.Value);
                }
            }
        }
    }

    public void DispatchProto(int protoId, ByteBuffer buffer)
    {
        if (!ProtoDic.ContainProtoId(protoId))
        {
            Debug.LogError("Unknown ProtoId : " + protoId.ToString());
            return;
        }

        Type type = ProtoDic.GetProtoTypeById(protoId);
        object toClient = ProtoBuf.Serializer.Deserialize(type, new MemoryStream(buffer.ReadBytes()));
        sEvents.Enqueue(new KeyValuePair<Type, object>(type, toClient));
    }

    public void AddHandler(Type type, TocHandler handler)
    {
        if (_handlerDic.ContainsKey(type))
            _handlerDic[type] += handler;
        else
            _handlerDic.Add(type, handler);
    }

    private void SendMessage(ByteBuffer buffer)
    {
        this.SocketClient.SendMessage(buffer);
    }

    public void SendMessage(object obj)
    {
        if (!ProtoDic.ContainProtoType(obj.GetType()))
        {
            Debug.LogError("Unknown ProtoType : " + obj.GetType().ToString());
            return;
        }
        ByteBuffer buffer = new ByteBuffer();
        int protoId = ProtoDic.GetProtoIdByType(obj.GetType());
        buffer.WriteShort((ushort)protoId);
        MemoryStream ms = new MemoryStream();
        ProtoBuf.Serializer.Serialize(ms, obj);
        byte[] bytes = ms.ToArray();
        buffer.WriteBytes(bytes);
        SendMessage(buffer);
    }

}
