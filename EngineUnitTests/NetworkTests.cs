using Xunit;
using Engine.Network;
using System.Net;
using Moq;
using System.Net.Sockets;
namespace EngineTests {
    /// <summary>
    /// <c>TestPacket</c> class to be used in <c>NetworkTests</c> class Xunit
    /// test methods.
    /// </summary>
    public class TestPacket : Packet {
        public TestPacket() {
        }

        public TestPacket(int sequence) {
            Sequence = sequence;
        }

        public new int PacketSize {
            get {
                return Packet.PacketSize;
            }
        }

        public override PacketType PacketType {
            get {
                return (PacketType)4;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class NetworkTests {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void TestPacketPeerInfoSerialization() {
            PacketPeerInfo peerInfo = new() {
                IPAddress = [127, 0, 0, 1],
                Port = 21,
                Sequence = 1
            };

            Memory<byte> buffer = peerInfo.ToBinary();

            PacketPeerInfo peerInfoFromBuffer = new();
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
            PacketAcknowledge ack = new() {
                PacketIDToAck = 2,
                Sequence = 3
            };

            Memory<byte> buffer = ack.ToBinary();

            PacketAcknowledge ackFromBinary = new();
            ackFromBinary.FromBinary(buffer);

            Assert.Equal(ack.PacketIDToAck, ackFromBinary.PacketIDToAck);
            Assert.Equal(ack.Sequence, ackFromBinary.Sequence);
        }
        [Fact]
        public void TestP2PClentPeerManagementMethods() {
            P2PClient client = new();

            IPEndPoint lanEndPoint = new(new IPAddress(new byte[] {
                192, 162, 0, 69
            }), 23231);
            client.GetOrAddPeer(lanEndPoint);
            client.GetPeer(lanEndPoint);

            IPEndPoint lanEndPoint1 = new(new IPAddress(new byte[] {
                0, 0, 0, 0
            }), 0);
            Peer? peer = client.GetPeer(lanEndPoint1);

            Assert.Null(peer);
        }
        /// <summary>
        /// Test handling of reliable packets that are received out of order.
        /// </summary>
        [Fact]
        public async void TestOutOfOrderReliablePackets() {
            var socket = new Mock<ISocket>();
            IPEndPoint remoteEndPoint = new(
                new IPAddress(new byte[] { 192, 168, 10, 126 }),
                12346
            );
            PacketPeerInfo peerInfo = new(
                remoteEndPoint
            );
            TestPacket[] testPackets = [
                new TestPacket(1),
                new TestPacket(3),
                new TestPacket(2)
            ];
            int socketReceivedDataIndex = 0;
            Memory<byte>[] socketReceivedData = [
                peerInfo.ToBinary(),
                testPackets[0].ToBinary(),
                testPackets[1].ToBinary(),
                testPackets[2].ToBinary()
            ];
            socket.Setup(s => s.GetSubnetMask()).Returns(
                new IPAddress(new byte[] { 192, 168, 255, 255 }
            ));;
            socket.Setup(s => s.LocalEndPoint).Returns(new IPEndPoint(
                new IPAddress(new byte[] { 192, 168, 10, 125 }),
                12345
            ));
            socket.Setup(s => s.ReceiveFromAsync(
                It.IsAny<Memory<byte>>(),
                It.IsAny<IPEndPoint>(),
                It.IsAny<CancellationToken>()
            )).Callback((
                Memory<byte> data,
                IPEndPoint endPoint,
                CancellationToken token
            ) => {
                if (socketReceivedDataIndex >= socketReceivedData.Length) {
                    throw new OperationCanceledException();
                }
                socketReceivedData[socketReceivedDataIndex].CopyTo(data);
            }).ReturnsAsync(() => {
                var result =  new SocketReceiveFromResult() {
                    ReceivedBytes = socketReceivedData[
                        socketReceivedDataIndex
                    ].Length,
                    RemoteEndPoint = remoteEndPoint
                };
                ++socketReceivedDataIndex;
                return result;
           });

            var client = new P2PClient(socket.Object, (endPoint, data) => {
                TestPacket? packet = null;
                PacketType packetType = Packet.GetPacketType(data);
                if (4 == (int)packetType) {
                    packet = new TestPacket();
                    packet.FromBinary(data);
                }
                return packet;
            });
            var remotePeer = client.GetOrAddPeer(remoteEndPoint);
            var clientTask = client.Start();

            foreach (var peer in client.Peers) {
                Assert.Empty(peer.ReliablePacketInbox);
            }

            client.Stop();
            await clientTask;
        }
    }
}
