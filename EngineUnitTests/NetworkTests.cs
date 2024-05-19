using Xunit;
using Engine.Network;
using System.Net;
using System.Diagnostics;
using System.Net.WebSockets;
namespace EngineTests
{
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
    }
}
