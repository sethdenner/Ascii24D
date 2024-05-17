namespace Engine.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class PacketAcknowledge : Packet
    {
        /// <summary>
        /// 
        /// </summary>
        public new static int PacketSize {
            get { return Packet.PacketSize + sizeof(int); }
        }
        /// <summary>
        /// 
        /// </summary>
        public override PacketType PacketType {
            get { return PacketType.ACKNOWLEDGE; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int PacketIDToAck {  get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetSize() {
            return PacketSize;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Memory<byte> ToBinary()
        {
            Memory<byte> packet = new byte[PacketSize];
            base.ToBinary().Span.CopyTo(packet.Span);
            Memory<byte> packetIDToAckBytes = BitConverter.GetBytes(
                PacketIDToAck
            );
            packetIDToAckBytes.Span.CopyTo(
                packet[
                    Packet.PacketSize..(Packet.PacketSize + sizeof(int))
                ].Span
            );
            return packet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public override void FromBinary(Memory<byte> data)
        {
            base.FromBinary(data);
            PacketIDToAck = BitConverter.ToInt32(
                data[Packet.PacketSize..(Packet.PacketSize + sizeof(int))].Span
            );
        }
    }
}
