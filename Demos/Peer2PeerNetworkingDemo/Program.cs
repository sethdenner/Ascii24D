using CommandLine;
using Engine.Network;
using System.Net;

namespace Peer2PeerNetworkingDemo
{
    internal class Program
    {
        public class MyPacket : Packet {
            public new static int PacketSize {
                get {
                    return Packet.PacketSize + sizeof(int);
                }
            }
            public override PacketType PacketType {
                get {
                    return PacketType.NUM_TYPES + 1;
                }
            }

            public int MyPacketData {
                get; set;
            }
            public MyPacket() {
                MyPacketData = 240;
            }
        }
        public class Options
        {
            [Option(
                'd',
                "dedicated",
                Required = false,
                HelpText = "Start the server in dedicated mode."
            )] public bool Dedicated {  get; set; }
        }
        static async Task<int> Main(string[] args)
        {
            bool isDedicated = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed(options => {
                isDedicated = options.Dedicated;
            });
            string mode = isDedicated ? "Server" : "Client";
            Console.WriteLine(
                $"Starting network in \"{mode}\" mode"
            );
            SocketUdp socket = new SocketUdp();
            P2PClient server = new P2PClient(socket, (endPoint, data) => {
                Packet? packet = null;
                IPEndPoint? ipEndPoint = endPoint as IPEndPoint;
                if (null == ipEndPoint) return packet;
                PacketType packetType = Packet.GetPacketType(data);
                if (PacketType.NUM_TYPES + 1 == packetType) {
                    packet = new MyPacket();
                    packet.FromBinary(data);
                }
                Console.WriteLine(
                    $"Packet received from {ipEndPoint.Address}:" +
                    $"{ipEndPoint.Port} " +
                    $"of type {packetType}."
                );
                return packet;
            }, isDedicated);
            Task<int> serverStart = server.Start();
            if (isDedicated)
            {
                Console.WriteLine(
                    $"Listening for connections."
                );
                while (server.IsAlive) ;
            }
            else
            {
                IPAddress serverAddress = new IPAddress(
                    new byte[] { 192, 168, 0, 27 }
                );
                IPEndPoint serverEndPoint = new IPEndPoint(
                    serverAddress,
                    P2PClient.DEFAULT_PORT
                );
                while (server.IsAlive)
                {
                    Console.WriteLine(
                        "Sending Client Announce Packet to " +
                        $"{serverEndPoint.Address}:{serverEndPoint.Port}..."
                    );
                    await server.SendPeerInfo(serverEndPoint);
                    Thread.Sleep(2000);
                }
            }

            return await serverStart;
        }
    }
}
