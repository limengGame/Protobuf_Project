using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Net;
using Proto;
using ServerMessage;
using System.IO;

namespace Server_Chat
{
    class Program
    {
        static byte[] result = new byte[1024];
        static string address = "127.0.0.1";
        const int port = 8885;
        static Socket serverSocket = null;

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse(address);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, port));
            serverSocket.Listen(50);
            Console.WriteLine("Start Listen Successs ! ", serverSocket.LocalEndPoint.ToString());
            Thread startServer = new Thread(ListenClientConnect);
            startServer.Start();
            Console.ReadLine();
        }

        public static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("Client Connected : {0}",clientSocket.LocalEndPoint.ToString());
                SignUpResponse response = new SignUpResponse();
                response.version = 1.0f;
                SendMessage(response, clientSocket);
                Thread thread = new Thread(ReceiveMessage);
                thread.Start(clientSocket);
            }
        }

        private static void ReceiveMessage(object clientSocket)
        {
            Socket client = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    int receiveNumber = client.Receive(result);
                    Console.WriteLine("Receive from client : {0}, Length : {1}", client.LocalEndPoint.ToString(), receiveNumber);
                    ByteBuffer buffer = new ByteBuffer(result);
                    int length = buffer.ReadShort();
                    int protoId = buffer.ReadShort();
                    if (!ProtoDic.ContainProtoId(protoId))
                    {
                        Console.WriteLine("Unknown ProtoId", protoId);
                        break;
                    }
                    if (protoId == 1003)
                    {
                        TosChat tosChat = ProtoBuf.Serializer.Deserialize<TosChat>(new MemoryStream(buffer.ReadBytes()));
                        Console.WriteLine("Client Message: Name {0}, Content {1}, Time {2}", tosChat.name, tosChat.content, tosChat.time);
                        TocChat tocChat = new TocChat();
                        tocChat.name = "Server";
                        tocChat.content = "SendToClient";
                        tocChat.time = 4.4444444f;
                        SendMessage(tocChat, client);

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Receive Error : ", ex.Message);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    return;
                }
            }
        }

        private static void SendMessage(object obj, Socket clientSocket)
        {
            MemoryStream ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, obj);
            ByteBuffer buffer = new ByteBuffer();
            Type protoType = obj.GetType();
            int protoId = ProtoDic.GetProtoIdByType(protoType);
            buffer.WriteShort((ushort)protoId);
            buffer.WriteBytes(ms.ToArray());
            clientSocket.Send(WriteMessage(buffer.ToBytes()));
        }


        private static byte[] WriteMessage(byte[] message)
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
                return ms.ToArray();
            }
        }

    }
}
