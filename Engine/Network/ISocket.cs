using System.Net;
using System.Net.NetworkInformation;
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
        /// <c>GetSubnetMask</c> uses the <c>NetworkInterface</c> api to
        /// determine the subnet mask for the open P2P socket.
        /// </summary>
        /// <returns>
        /// An instance of <c>IPAddress</c> representing the subnet
        /// mask of the network the socket is currently on.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <c>InvalidOperationException</c> is thrown when the socket
        /// referenced by <c>Socket</c> is in an invalid state or if the
        /// interface corresponding to the socket cannot be found.
        /// </exception>
        public IPAddress GetSubnetMask() {
            if (null == LocalEndPoint)
                throw new InvalidOperationException(
                    nameof(LocalEndPoint) + " is null."
                );
            // Iterate through all known network interfaces and find the one
            // that matches the socket that the client is currently using.
            foreach (var iface in NetworkInterface.GetAllNetworkInterfaces()) {
                foreach (
                    var addressInfo in
                    iface.GetIPProperties().UnicastAddresses
                ) {
                    if (
                        AddressFamily.InterNetwork ==
                        addressInfo.Address.AddressFamily &&
                        addressInfo.Address.Equals(LocalEndPoint.Address)
                    ) {
                        return addressInfo.IPv4Mask;
                    }
                }
            }
            // If the subnet mask can not be obtained local peers can't be
            // found. Throw an exception in that case.
            throw new InvalidOperationException("No valid interface found.");
        }
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

