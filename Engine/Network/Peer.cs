using System.Net;

namespace Engine.Network {
    /// <summary>
    /// <c>Peer</c> is a class that represents a session with a remote client.
    /// Maintains information required to communicate with the client as well
    /// as managing incoming and outgoing sequence guaranteed (reliable)
    /// packets.
    /// </summary>
    public class Peer {
        /// <summary>
        /// An instance of <c>IPEndPoint</c> representing the remote IP address
        /// and port of the peer.
        /// </summary>
        public IPEndPoint EndPoint {
            get; set;
        }
        /// <summary>
        /// Stores the next integer representing the sequence of reliable
        /// packets.
        /// </summary>
        public int NextPacketSequence {
            get; set;
        }
        /// <summary>
        /// A dictionary with an integer key representing the sequence number of
        /// reliably sent packets and the corresponding packet binary data as
        /// the value.
        /// </summary>
        public Dictionary<int, Memory<byte>> UnacknowledgedPackets {
            get; set;
        }
        /// <summary>
        /// A counter keeping track of the number of reliable (sequenced
        /// guaranteed) packets that have been sent.
        /// </summary>
        public int ReliablePacketsSent {
            get; set;
        }
        /// <summary>
        /// A counter keeping track of the number of reliable (sequenced
        /// guaranteed) packets that have been acknowledged.
        /// </summary>
        public int ReliablePacketsAcknowledged {
            get; set;
        }
        /// <summary>
        /// The number of reliable (sequence and delivery guaranteed packets
        /// that were not acknowledged within the timeout period.
        /// </summary>
        public int PacketsLost {
            get; set;
        }
        /// <summary>
        /// A ratio of the amount of packets lost and the total number of
        /// reliable packets sent.
        /// </summary>
        public float PacketLoss {
            get {
                return
                    (float)PacketsLost /
                    (float)ReliablePacketsSent;
            }
        }
        /// <summary>
        /// <c>Peer</c> default constructor.
        /// </summary>
        public Peer() {
            EndPoint = new(IPAddress.Any, 0);
            // Start at 1 as 0 is reserved for unsequenced.
            NextPacketSequence = 1;
            UnacknowledgedPackets = [];
        }
        /// <summary>
        /// <c>Peer</c> constructor overload that takes an <c>IPEndPint</c> for
        /// initialization.
        /// </summary>
        /// <param name="endPoint">
        /// An instance of <c>IPEndPoint</c> representing the IP address and
        /// port of a remote client.
        /// </param>
        public Peer(
            IPEndPoint endPoint
        ) {
            EndPoint = endPoint;
            // Start at 1 as 0 is reserved for unsequenced.
            NextPacketSequence = 1;
            UnacknowledgedPackets = [];
        }
        /// <summary>
        /// Finds an unacknowledged packet matching the <c>PacketIDToAck</c>
        /// property in the provided <c>PacketAcknowledge</c> packet. If found
        /// removes the packet from the dictionary and increments the
        /// <c>ReliablePacketsAcknowledged</c> counter.
        /// </summary>
        /// <param name="ack">
        /// An instance of <c>PacketAcknowledge</c> that has been recieved from
        /// a remote client.
        /// </param>
        public void ConfirmAcknowledge(PacketAcknowledge ack) {
            Memory<byte> packetBytes;
            bool exists = UnacknowledgedPackets.TryGetValue(
                ack.PacketIDToAck,
                out packetBytes
            );
            // If the packet exists in the dictionary remove the packet from the
            // UnacknowledgedPackets dictionary and increment the
            // ReliablePacketsAcknowledged counter.
            if (exists) {
                UnacknowledgedPackets.Remove(ack.PacketIDToAck);
                ++ReliablePacketsAcknowledged;
            }
        }
        /// <summary>
        /// Returns the value representing the next sequence number to use for
        /// reliable packets and increments the <c>NexPacketSequence</c>
        /// counter.
        /// </summary>
        /// <returns>
        /// An integer representing the next sequence value to use.
        /// </returns>
        public int GetNextPacketSequence() {
            return NextPacketSequence++;
        }
        /// <summary>
        /// Add sent reliable packet to the UnacknowledgedPackets dictionary for
        /// resending until it's acknowledged.
        /// </summary>
        /// <param name="packet">
        /// The reliable packet that should be saved.
        /// </param>
        public void AddUnacknowledgedPacket(Packet packet) {
            UnacknowledgedPackets.Add(packet.Sequence, packet.ToBinary());
        }
    }
}
