using Engine.Network;
using System.Diagnostics;
using Xunit;

namespace EngineIntegrationTests {
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
    public class NetworkTests {
        /// <summary>
        /// Test sending and receiving packets over the local area network.
        /// </summary>
        [Fact]
        public async void TestP2PClientTestSendAndReceivePacket() {
            var socket0 = new SocketUdp();
            var socket1 = new SocketUdp();
            var serverSocket = new SocketUdp();
            bool client0PacketReceived = false;
            bool client1PacketReceived = false;
            var client0 = new P2PClient(socket0,
                (endPoint, packetData) => {
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
                }
            );

            var client1 = new P2PClient(
                socket1,
                (endPoint, packetData) => {
                    PacketType packetType = Packet.GetPacketType(
                        packetData
                    );
                    if (TestPacket.PACKET_TYPE == packetType) {
                        client1PacketReceived = true;
                        TestPacket packet = new TestPacket();
                        packet.FromBinary(packetData);
                        return packet;
                    } else {
                        return null;
                    }
                }
            );

            var server = new P2PClient(serverSocket, (
                    endpoint,
                    packetData
                ) => {
                    PacketType packetType = Packet.GetPacketType(
                        packetData
                    );
                    if (TestPacket.PACKET_TYPE == packetType) {
                        client1PacketReceived = true;
                        TestPacket packet = new TestPacket();
                        packet.FromBinary(packetData);
                        return packet;
                    } else {
                        return null;
                    }
                },
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
            ) {
                Thread.Sleep(10);
            }

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
                foreach (var peer in server.Peers) {
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