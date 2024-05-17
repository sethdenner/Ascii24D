using System.Net;

namespace Engine.Network
{
    /// <summary>
    /// <c>PacketPeerInfo</c> is a <c>class</c> that represents the IP end point
    /// information that can be used to connect to a peer over LAN or Internet.
    /// </summary>
    public class PacketPeerInfo : Packet
    {
        /// <summary>
        /// Returns the size of the packet in bytes.
        /// </summary>
        public new static int PacketSize {
            get {
                return Packet.PacketSize +
                    sizeof(int) +
                    sizeof(byte) * 4;
            }
        }
        /// <summary>
        /// This is <c>PacketPeerInfo</c>. Returns <c>PacketType.PEER_INFO</c>.
        /// </summary>
        public override PacketType PacketType {
            get { return PacketType.PEER_INFO; }
        }
        /// <summary>
        /// Property that gets or sets the port of the peer being shared.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Property that gets or sets the IP address of the peer being
        /// shared.
        /// </summary>
        public byte[] IPAddress { get; set; }
        public IPEndPoint EndPoint
        {
            get {
                return new IPEndPoint(
                    new IPAddress(IPAddress),
                    Port
                );
            }
        }
        public override int GetSize() {
            return PacketSize;
        }
        /// <summary>
        /// <c>PacketPeerInfo</c> default constructor.
        /// </summary>
        public PacketPeerInfo()
        {
            Port = 0;
            IPAddress = System.Net.IPAddress.Any.GetAddressBytes();
        }
        /// <summary>
        /// Constructor that takes an endpoint to initialize the class with.
        /// </summary>
        /// <param name="endPoint">
        /// The LAN address of the peer being shared.
        /// </param>
        public PacketPeerInfo(IPEndPoint endPoint)
        {
            Port = endPoint.Port;
            IPAddress = endPoint.Address.GetAddressBytes();
        }
        /// <summary>
        /// Serializes the packet data to binary.
        /// </summary>
        /// <returns>
        /// A <c>Memory</c> object containing a binary representation of the
        /// packet.
        /// </returns>
        public override Memory<byte> ToBinary()
        {
            Memory<byte> packet = new byte[PacketSize];
            base.ToBinary().Span.CopyTo(packet.Span);
            Memory<byte> port = BitConverter.GetBytes(Port);
            Memory<byte> ipAddress = IPAddress;

            int startSpan = Packet.PacketSize;
            port.CopyTo(packet[startSpan..(startSpan += sizeof(int))]);
            ipAddress.CopyTo(packet[
                startSpan..(startSpan += sizeof(byte) * 4)
            ]);
            return packet;
        }
        /// <summary>
        /// Populates the class by deserializing data from the provided
        /// <c>Memory</c> buffer.
        /// </summary>
        /// <param name="data"></param>
        public override void FromBinary(Memory<byte> data)
        {
            base.FromBinary(data);
            int spanStart = Packet.PacketSize;
            Port = BitConverter.ToInt32(data[
                spanStart..(spanStart += sizeof(int))
            ].Span);
            IPAddress = data[
                spanStart..(spanStart += sizeof(byte) * 4)
            ].ToArray();
        }
    }
}
