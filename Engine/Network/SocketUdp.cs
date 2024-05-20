using SharpDX;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Engine.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class SocketUdp : ISocket
    {
        /// <summary>
        /// <c>SocketUdp</c> default constructor.
        /// </summary>
        public SocketUdp() {
            Socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            Socket.DontFragment = true;
            Socket.EnableBroadcast = true;
            Socket.MulticastLoopback = false;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="addressFamily"></param>
        public SocketUdp(AddressFamily addressFamily)
        {
            Socket = new Socket(
                addressFamily,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            Socket.DontFragment = true;
            Socket.EnableBroadcast = true;
            Socket.MulticastLoopback = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localEP"></param>
        public void Bind(IPEndPoint localEP)
        {
             Socket.Bind(localEP);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remoteEndPoint"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">
        /// <c>OperationCanceledException</c> is thrown when the
        /// <c>CancellationToken</c> has been canceled.
        /// </exception>"
        public async Task<SocketReceiveFromResult> ReceiveFromAsync(
            Memory<byte> buffer,
            IPEndPoint remoteEndPoint,
            CancellationToken token
        ) {
            return await Socket.ReceiveFromAsync(
                buffer,
                SocketFlags.None,
                remoteEndPoint,
                token
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public async Task<int> SendToAsync(
            ReadOnlyMemory<byte> buffer,
            IPEndPoint remoteEP
        ) {
            return await Socket.SendToAsync(
                buffer,
                SocketFlags.None,
                remoteEP
            );
        }
        /// <summary>
        /// 
        /// </summary>
        public Socket Socket { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint? LocalEndPoint
        {
            get
            {
                return Socket.LocalEndPoint as IPEndPoint;
            }
        }
    }
}
