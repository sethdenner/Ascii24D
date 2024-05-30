using System.Runtime.InteropServices;

namespace Engine.Network {
    /// <summary>
    /// Abstract <c>class</c> declaring fields common to all packets and
    /// defining base functionality for derived types.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public abstract class Packet {
        /// <summary>
        /// Implementation should return the type of the packet. Should be read
        /// only.
        /// </summary>
        public abstract PacketType PacketType {
            get;
        }
        /// <summary>
        /// Packet sequence number used to guarantee the order of the packet. 0
        /// indicates that the packet is not reliable and order is not
        /// guaranteed.
        /// </summary>
        public int Sequence = 0;
        /// <summary>
        /// The size of the packet in bytes. Should be overridden in base class
        /// using the <c>new</c> keyword. Unfortunatelly C# does not offer a
        /// better way to override static member methods.
        /// </summary>
        public static int PacketSize {
            get {
                return sizeof(int) +
                    sizeof(PacketType);
            }
        }
        /// <summary>
        /// Get the size of the packet in bytes. Generally does not need to be
        /// overridden unless implementing strange and arcane things.
        /// </summary>
        /// <returns>
        /// The size of the packet in bytes.
        /// </returns>
        public virtual int GetSize() {
            return PacketSize;
        }
        /// <summary>
        /// Serializes the base packet fields into binary data. Make sure to
        /// call this when overriding unless the method handles serializing
        /// these fields in some other way or order for some reason.
        /// </summary>
        /// <returns>
        /// A memory buffer containing the binary serialized data.
        /// </returns>
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
        /// Deserializes binary data serialized by the <c>ToBinary</c> method
        /// and populates the packet fields.
        /// </summary>
        /// <param name="data">
        /// A memory buffer containing binary serialized data.
        /// </param>
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
