using Xunit;
using Engine.Network;
using System.Net;
using System.Diagnostics;
using System.Net.WebSockets;
namespace EngineTests
{
    /// <summary>
    /// Packet class to use for testing.
    /// </summary>
    internal class TestPacket : Packet {
        /// <summary>
        /// 
        /// </summary>
        public const PacketType PACKET_TYPE = PacketType.NUM_TYPES + 1;
        /// <summary>
        /// 
        /// </summary>
        public override PacketType PacketType {
            get {
                return PACKET_TYPE;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TestPacket() {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NetworkTests
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestPacketPeerInfoSerialization() {
            PacketPeerInfo peerInfo = new PacketPeerInfo();
            peerInfo.IPAddress = [127, 0, 0, 1];
            peerInfo.Port = 21;
            peerInfo.Sequence = 1;

            Memory<byte> buffer = peerInfo.ToBinary();

            PacketPeerInfo peerInfoFromBuffer = new PacketPeerInfo();
            peerInfoFromBuffer.FromBinary(buffer);

            Assert.Equal(
                peerInfo.IPAddress,
                peerInfoFromBuffer.IPAddress
            );
            Assert.Equal(peerInfo.Port, peerInfoFromBuffer.Port);
            Assert.Equal(peerInfo.Sequence, peerInfoFromBuffer.Sequence);
        }
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestPacketAcknowledgeSerialization() {
            PacketAcknowledge ack = new();
            ack.PacketIDToAck = 2;
            ack.Sequence = 3;

            Memory<byte> buffer = ack.ToBinary();

            PacketAcknowledge ackFromBinary = new PacketAcknowledge();
            ackFromBinary.FromBinary(buffer);

            Assert.Equal(ack.PacketIDToAck, ackFromBinary.PacketIDToAck);
            Assert.Equal(ack.Sequence, ackFromBinary.Sequence);
        }
        [Fact]
        public void TestP2PClentPeerManagementMethods() {
            P2PClient client = new P2PClient();

            IPEndPoint lanEndPoint = new IPEndPoint(new IPAddress(new byte[] {
                192, 162, 0, 69
            }), 23231);
            IPEndPoint wanEndpoint = new IPEndPoint(new IPAddress(new byte[] {
                1, 2, 3, 4
            }), 42069);
            Peer peerA = client.GetOrAddPeer(lanEndPoint);
            Peer? peerB = client.GetPeer(lanEndPoint);

            IPEndPoint lanEndPoint1 = new IPEndPoint(new IPAddress(new byte[] {
                0, 0, 0, 0
            }), 0);
            IPEndPoint wanEndpoint1 = new IPEndPoint(new IPAddress(new byte[] {
                0, 0, 0, 0
            }), 0);
            Peer? peerC = client.GetPeer(lanEndPoint1);

            Assert.Null(peerC);
        }
        /// <summary>
        /// Test sending and receiving packets over the local area network.
        /// </summary>
        [Fact]
        public async void TestP2PClientTestSendAndReceivePacket()
        {
            var socket0 = new SocketUdp();
            var socket1 = new SocketUdp();
            var serverSocket = new SocketUdp();
            bool client0PacketReceived = false;
            bool client1PacketReceived = false;
            var client0 = new P2PClient(socket0,
                async (endPoint, packetData) =>
                {
                    return await Task<Packet?>.Run(Packet? () =>
                    {
                        PacketType packetType = Packet.GetPacketType(
                            packetData
                        );
                        if (TestPacket.PACKET_TYPE == packetType) {
                            client0PacketReceived = true;
                            TestPacket packet = new TestPacket();
                            packet.FromBinary(packetData);
                            return packet;
                        } else {
                            return null;
                        }
                    });
                }
            );

            var client1 = new P2PClient(
                socket1,
                async (endPoint, packetData) => {
                    return await Task<Packet?>.Run(Packet? () => {
                        PacketType packetType = Packet.GetPacketType(
                            packetData
                        );
                        if(TestPacket.PACKET_TYPE == packetType) {
                            client1PacketReceived = true;
                            TestPacket packet = new TestPacket();
                            packet.FromBinary(packetData);
                            return packet;
                        } else { 
                            return null;
                        }
                    });
                }
            );

            var server = new P2PClient(serverSocket, async (
                    endpoint,
                    packetData
                ) => { return await Task<Packet?>.Run(Packet? () => {
                    PacketType packetType = Packet.GetPacketType(
                        packetData
                    );
                    if ( TestPacket.PACKET_TYPE == packetType ) {
                        client1PacketReceived = true;
                        TestPacket packet = new TestPacket();
                        packet.FromBinary(packetData);
                        return packet;
                    } else {
                        return null;
                    }
                }); },
                true
            );

            var serverTask = server.Start();
            var task0 = client0.Start();
            var task1 = client1.Start();

            Assert.NotNull(server.LanEndPoint);
            Assert.NotNull(client0.LanEndPoint);
            Assert.NotNull(client1.LanEndPoint);

            Stopwatch stopwatch = new();
            stopwatch.Start();

            const int findPeersTimeout = 2000; // 2 seconds in milliseconds.
            while (
                client0.Peers.Count < 2 && client1.Peers.Count < 2 &&
                stopwatch.ElapsedMilliseconds < findPeersTimeout
            ) {
                Thread.Sleep(10);
            }

            Assert.Equal(2, server.Peers.Count);
            Assert.Equal(2, client0.Peers.Count);
            Assert.Equal(2, client1.Peers.Count);

            TestPacket packet = new TestPacket();
            await client0.BroadcastPacketToPeers(packet, true);
            await client1.BroadcastPacketToPeers(packet, true);

            const int packetTestTimeout = 2000; // 2 seconds in milliseconds.
            stopwatch.Restart();
            while (
                (!client0PacketReceived || !client1PacketReceived) &&
                stopwatch.ElapsedMilliseconds < packetTestTimeout
            ) { Thread.Sleep(10); }

            Assert.True(client0PacketReceived);
            Assert.True(client1PacketReceived);

            const int acknowledgePacketsTestTimeout = 2000; // 2 seconds.
            stopwatch.Restart();
            bool client0AllPacketsAcknowledged = false;
            bool client1AllPacketsAcknowledged = false;
            bool serverAllPacketsAcknowledged = false;
            while ((
                    !client0AllPacketsAcknowledged ||
                    !client1AllPacketsAcknowledged ||
                    !serverAllPacketsAcknowledged
                ) &&
                stopwatch.ElapsedMilliseconds < acknowledgePacketsTestTimeout
            ) {
                client0AllPacketsAcknowledged = true;
                client1AllPacketsAcknowledged = true;
                serverAllPacketsAcknowledged = true;
                foreach (var peer in client0.Peers) {
                    client0AllPacketsAcknowledged =
                        client0AllPacketsAcknowledged &&
                        0 == peer.UnacknowledgedPackets.Count;
                }
                foreach (var peer in client1.Peers) {
                    client1AllPacketsAcknowledged =
                        client1AllPacketsAcknowledged &&
                        0 == peer.UnacknowledgedPackets.Count;
                }
                foreach (var peer in server.Peers ) {
                    serverAllPacketsAcknowledged =
                        serverAllPacketsAcknowledged &&
                        0 == peer.UnacknowledgedPackets.Count;
                }
                Thread.Sleep(10);
            }

            client0.Stop();
            client1.Stop();
            server.Stop();
 
            Assert.Equal(2, server.Peers.Count);
            Assert.Equal(2, client0.Peers.Count);
            Assert.Equal(2, client1.Peers.Count);

            foreach (var peer in client0.Peers) {
                Assert.Empty(peer.UnacknowledgedPackets);
                Assert.NotEqual(0, peer.ReliablePacketsSent);
                Assert.Equal(
                    peer.ReliablePacketsSent,
                    peer.ReliablePacketsAcknowledged
                );
            }
            foreach (var peer in client1.Peers) {
                Assert.Empty(peer.UnacknowledgedPackets);
                Assert.NotEqual(0, peer.ReliablePacketsSent);
                Assert.Equal(
                    peer.ReliablePacketsSent,
                    peer.ReliablePacketsAcknowledged
                );
            }
            foreach (var peer in server.Peers) {
                Assert.Empty(peer.UnacknowledgedPackets);
                Assert.Equal(0, peer.ReliablePacketsSent);
                Assert.Equal(
                    peer.ReliablePacketsSent,
                    peer.ReliablePacketsAcknowledged
                );
            }

            await task0;
            await task1;
            await serverTask;
        }
    }
}
