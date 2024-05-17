using System.Net;
using System.Net.Sockets;

namespace Engine.Network
{
    /// <summary>
    /// <c>ISocket</c> is an interface that declares the methods and properties
    /// required for sending and receiving data over a network end point. 
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// <c>Bind</c> is a method that is expected to reserve and configure
        /// the specified end point (IP address and port) for UDP data
        /// transmission for the host platform.
        /// </summary>
        /// <param name="endPoint">
        /// <paramref name="endPoint"/> is a reference to an instance of type
        /// <c>IPEndPoint</c> containing the IP address and port that should
        /// be bound by the application.
        /// </param>
        public void Bind(IPEndPoint endPoint);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remoteEndPoint"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<SocketReceiveFromResult> ReceiveFromAsync(
            Memory<byte> buffer,
            IPEndPoint remoteEndPoint,
            CancellationToken token
       );
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public Task<int> SendToAsync(
            ReadOnlyMemory<byte> buffer,
            IPEndPoint remoteEP
        ); 
        /// <summary>
        /// 
        /// </summary>
        public Socket Socket { get; }
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint? LocalEndPoint { get; }
    }
}

