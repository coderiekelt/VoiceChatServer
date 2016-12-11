using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;

namespace Server
{
    class Program
    {
        static NetPeerConfiguration ServerConfiguration;
        static NetServer Server;

        static void Main(string[] args)
        {
            ServerConfiguration = new NetPeerConfiguration("Voice");
            ServerConfiguration.Port = 27000;

            Server = new NetServer(ServerConfiguration);

            Server.Start();

            while (true)
            {
                NetIncomingMessage Message = Server.ReadMessage();

                if (Message != null)
                {
                    switch (Message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            Console.WriteLine(Message.SenderEndPoint.Address + " status changed!");
                            break;

                        case NetIncomingMessageType.Data:

                            NetOutgoingMessage Broadcast = Server.CreateMessage();

                            List<NetConnection> Listeners = Server.Connections;
                            Listeners.Remove(Message.SenderConnection);

                            if (Listeners.Count() > 0)
                            {
                                Console.WriteLine("Broadcasting message from " + Message.SenderEndPoint.Address);

                                Broadcast.Write(Message.ReadBytes(Message.LengthBytes));
                                Server.SendMessage(Broadcast, Listeners, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            else
                            {
                                Console.WriteLine("No listeners for message from " + Message.SenderEndPoint.Address);
                            }

                            break;
                    }
                }

            }
        }
    }
}
