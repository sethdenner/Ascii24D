using System.Runtime.InteropServices;

namespace Engine.Network {
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public abstract class Packet {
        /// <summary>
        /// 
        /// </summary>
        public abstract PacketType PacketType {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Sequence {
            get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public static int PacketSize {
            get {
                return sizeof(int) +
                    sizeof(PacketType);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int GetSize() {
            return PacketSize;
        }
        public Packet() {
            Sequence = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Memory<byte> ToBinary() {
            Memory<byte> packet = new byte[PacketSize];
            Memory<byte> packetTypeBytes =
                BitConverter.GetBytes((int)PacketType);
            Memory<byte> sequenceBytes = BitConverter.GetBytes(Sequence);
            int spanStart = 0;
            packetTypeBytes.Span.CopyTo(
                packet[spanStart..(spanStart += sizeof(PacketType))].Span
            );
            sequenceBytes.Span.CopyTo(
                packet[spanStart..(spanStart += sizeof(int))].Span
            );
            return packet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public virtual void FromBinary(Memory<byte> data) {
            int spanStart = sizeof(PacketType);
            Sequence = BitConverter.ToInt32(
                data[spanStart..(spanStart += sizeof(int))].Span
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PacketType GetPacketType(Memory<byte> data) {
            return (PacketType)BitConverter.ToInt32(
                data[..sizeof(PacketType)].Span
            );
        }
        /// <summary>
        /// Deserializes the sequence value in a binaray serialized packet
        /// buffer.
        /// </summary>
        /// <param name="data">
        /// A buffer containing serialized packet data.
        /// </param>
        /// <returns>
        /// returns an integer representing the packets sequence value.
        /// </returns>
        public static int GetPacketSequence(Memory<byte> data) {
            return BitConverter.ToInt32(data[
                sizeof(PacketType)..(sizeof(PacketType) + sizeof(int))
            ].Span);
        }
    }
}
