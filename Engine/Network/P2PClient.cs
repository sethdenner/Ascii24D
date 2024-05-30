using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Buffers;
using System.Collections;
using SharpDX.Win32;
namespace Engine.Network {
    /// <summary>
    /// <c>DReceivedPacket</c> delegate declares a method intended to
    /// handle recieved network packets that have been sent to the server.
    /// </summary>
    /// <param name="endPoint">
    /// A reference of type <c>EndPoint</c> representing the ip addressInfo and port
    /// of the remote instance that the packet was recieved from.
    /// </param>
    /// <param name="packetData">
    /// A memory buffer containing the data sent from the remote instance.
    /// </param>
    /// <returns></returns>
    public delegate Packet? DReceivedPacket(
        EndPoint endPoint,
        Memory<byte> packetData
    );
    /// <summary>
    /// <c>P2PClient</c> is a class with functionality that handles opening and
    /// maintaining P2P network connections locally and over The Internet using
    /// the UDP protocol.
    /// </summary>
    public class P2PClient {
        /// <summary>
        /// Defines the default port number to use when port is not specified.
        /// </summary>
        public const int DEFAULT_PORT = 11230;
        /// <summary>
        /// Defines the maximum number of connections to allow.
        /// </summary>
        public const int MAX_CONNECTIONS = 10000;
        /// <summary>
        /// Defines the maximum number of queued connections to allow.
        /// </summary>
        public const int MAX_QUEUED_CONNECTIONS = 100;
        /// <summary>
        /// The maximum ethernet frame size. Used as the maximum size of a
        /// packet.
        /// </summary>
        public const int ETHERNET_FRAME_SIZE = 1380;
        /// <summary>
        /// The interval in milliseconds that should be observed before making
        /// another peer request.
        /// </summary>
        public const int PEER_SHARE_INTERVAL = 1000;
        /// <summary>
        /// <c>P2PClient</c> default constructor. 
        /// </summary>
        /// <param name="socket">
        /// <paramref name="socket"/> is an optional paramter. If a valid object
        /// reference implementing the <c>ISocket</c> interface is not set
        /// before calling the <c>Start</c> method an exception will be thrown.
        /// </param>
        public P2PClient(ISocket? socket = null) {
            _socket = socket;
            _gameCallback = (endPoint, dataBuffer) => {
                return null;
            };
            _peers = [];
            ReceivingData = false;
            IsDedicated = false;
            IsAlive = false;
            ReceiveFromCancellationSource = new CancellationTokenSource();
        }
        /// <summary>
        /// <c>P2PClient</c> constructor takes all parameters you might need
        /// to create and handle a P2P connection.
        /// </summary>
        /// <param name="socket">
        /// The initialized socket to use for communicating with the network.
        /// </param>
        /// <param name="gameCallback">
        /// A callback matching the declaration of <c>DRecievedPacket</c> which
        /// will be passed all data not handled by the base network system.
        /// </param>
        /// <param name="isDedicated">
        /// Flag this client as a dedicated server that is trusted to maintain
        /// game state.
        /// </param>
        public P2PClient(
            ISocket socket,
            DReceivedPacket gameCallback,
            bool isDedicated = false
        ) {
            _socket = socket;
            _gameCallback = gameCallback;
            _peers = [];
            ReceivingData = false;
            IsDedicated = isDedicated;
            IsAlive = false;
            ReceiveFromCancellationSource = new CancellationTokenSource();
        }
        /// <summary>
        /// Start the P2P client and begin searching for other clients by
        /// broadcasting packets to a known port on the LAN or optionally
        /// contacting a configured peer endpoint.
        /// </summary>
        /// <returns>
        /// An integer representing an error code.
        /// </returns>
        public async Task<int> Start() {
            if (null == _socket)
                throw new InvalidOperationException(nameof(_socket) + "is null.");
            string hostName = Dns.GetHostName();
            IPAddress bindAddress = Dns.GetHostAddresses(hostName).First(
                (address) => {
                    return address.AddressFamily == AddressFamily.InterNetwork;
                }
            );
            int port = IsDedicated ? DEFAULT_PORT : 0;
            _socket.Bind(new IPEndPoint(bindAddress, port));
            IsAlive = true;
            var task0 = StartReceivingData();
            var task1 = StartResendingUnacknowledgedPackets();
            var task3 = StartSharingPeers();
            return await task0 + await task1 + await task3;
        }
        /// <summary>
        /// <c>GetBroadcastAddress</c> calculates the subnet broadcast address
        /// for the currently configured socket.
        /// </summary>
        /// <returns>
        /// An instance of <c>IPAddress</c> that represents the calculated
        /// subnet broadcast address.
        /// </returns>
        private IPAddress GetBroadcastAddress() {
            if (null == _socket)
                throw new InvalidOperationException(
                   nameof(_socket) + " is null."
               );
            if (null == _socket.LocalEndPoint)
                throw new InvalidOperationException(
                    nameof(_socket.LocalEndPoint) + " is null."
                );
            // Subnet mask is required for calculating the subnet broadcast
            // address.
            IPAddress subnetMask = _socket.GetSubnetMask();
            byte[] ipAddressBytes =
                _socket.LocalEndPoint.Address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();
            // These both need to be IPV4 addresses.
            if (ipAddressBytes.Length != subnetMaskBytes.Length)
                throw new InvalidOperationException("Subnet mask is invalid.");
            // OR the address bytes with the XOR of the subnet mask bytes.
            byte[] broadcastAddressBytes = new byte[ipAddressBytes.Length];
            for (int i = 0; i < broadcastAddressBytes.Length; ++i) {
                broadcastAddressBytes[i] = (byte)(
                    ipAddressBytes[i] | (subnetMaskBytes[i] ^ 255)
                );
            }
            return new IPAddress(broadcastAddressBytes);
        }
        /// <summary>
        /// Start intermittently share peers by broadcasting to a known port on
        /// the local area network or optionally to a known peer at a configured
        /// <c>IPEndpoint</c>. Also shares all known peers with all other known
        /// peers.
        /// </summary>
        public async Task<int> StartSharingPeers() {
            int bytesSent = 0;
            SharingPeers = true;
            while (SharingPeers) {
                IPAddress broadcastAddress = GetBroadcastAddress();
                // Broadcast peer info over LAN to find local peer repository.
                bytesSent += await SendPeerInfo(
                    new IPEndPoint(broadcastAddress, DEFAULT_PORT)
                );
                if (null != DedicatedServerEndPoint) {
                    bytesSent += await SendPeerInfo(DedicatedServerEndPoint);
                }
                bytesSent += await BroadcastPeerInfo();
                Thread.Sleep(PEER_SHARE_INTERVAL);
            }
            return 0;
        }
        /// <summary>
        /// Stops peer sharing.
        /// </summary>
        public void StopSharingPeers() {
            SharingPeers = false;
        }
        /// <summary>
        /// Stop the P2P client.
        /// </summary>
        public void Stop() {
            StopResendingUnacknowledgedPackets();
            StopReceivingData();
            StopSharingPeers();
        }
        /// <summary>
        /// Stop the process of resending reliable packets that have not yet
        /// been acknowledged. Does not clear unacknowledged packet data.
        /// </summary>
        public void StopResendingUnacknowledgedPackets() {
            ResendingUnacknowledgedPackets = false;
        }
        /// <summary>
        /// Starts intermittently resending unacknowledged packets.
        /// </summary>
        /// <returns>
        /// An integer representing an error code (0 = no errors).
        /// </returns>
        private async Task<int> StartResendingUnacknowledgedPackets() {
            int errorCode = 0;
            int bytesSent = 0;
            ResendingUnacknowledgedPackets = true;
            return await Task<int>.Run(async () => {
                while (ResendingUnacknowledgedPackets) {
                    try {
                        for (int i = 0; i < _peers.Count; ++i) {
                            Peer peer = _peers[i];
                            int frameBytesUsed = 0;
                            int packetIndex = 0;
                            while (
                                packetIndex <
                                peer.UnacknowledgedPackets.Count
                            ) {
                                byte[] frameBytes = ArrayPool<byte>.Shared.Rent(
                                    ETHERNET_FRAME_SIZE
                                );
                                for (
                                    ;
                                    packetIndex <
                                    peer.UnacknowledgedPackets.Count;
                                    ++packetIndex
                                ) {
                                    Memory<byte> packetBytes =
                                        peer.UnacknowledgedPackets
                                        .Values.ElementAt(
                                            packetIndex
                                        );
                                    if (
                                        ETHERNET_FRAME_SIZE >
                                        frameBytesUsed + packetBytes.Length
                                    ) {
                                        packetBytes.Span.CopyTo(
                                            frameBytes.AsSpan()[
                                                frameBytesUsed..
                                            ]
                                        );
                                        frameBytesUsed += packetBytes.Length;
                                    } else {
                                        // The frame is full. break the loop.
                                        break;
                                    }
                                }
                                bytesSent += await SendData(
                                    frameBytes.AsMemory(
                                        0,
                                        frameBytesUsed
                                    ),
                                    peer.EndPoint
                                );
                                ArrayPool<byte>.Shared.Return(
                                    frameBytes
                                );
                            }
                            Thread.Sleep(20);
                        }
                    } catch (InvalidOperationException) {
                        //The peers for loop has become invalid.
                        //Break and let the loop continue execution on the new
                        //list.
                        break;
                    }
                }
                return errorCode;
            });
        }
        /// <summary>
        /// Terminates listening on the network via <c>CancellationToken</c>.
        /// </summary>
        public void StopReceivingData() {
            ReceivingData = false;
            ReceiveFromCancellationSource.Cancel();
        }
        /// <summary>
        /// Finds an existing peer matching the passed in endpoint or creates
        /// one to represent the passed in endpoint.
        /// </summary>
        /// <param name="endPoint">
        /// The endpoint of the remote client that will be used to find or if
        /// not found create a peer instance and save it to the list of known
        /// peers.
        /// </param>
        /// <returns>
        /// An instance of <c>Peer</c> that represents a peer that exists at the
        /// specified <c>EndPoint</c>.
        /// </returns>
        public Peer GetOrAddPeer(IPEndPoint endPoint) {
            Peer? peer = GetPeer(endPoint);
            if (null == peer) {
                peer = new Peer(endPoint);
                _peers.Add(peer);
            }
            return peer;
        }
        /// <summary>
        /// Returns a peer from the known peer list matching the IP address and
        /// port of the passed <c>IPEndPoint</c> instance.
        /// </summary>
        /// <param name="endPoint">
        /// The <c>IPEndPoint</c> of the peer to search for.
        /// </param>
        /// <returns>
        /// The peer instance matching the passed <c>IPEndPoint</c> or null if
        /// not found.
        /// </returns>
        public Peer? GetPeer(IPEndPoint endPoint) {
            string endPointIPString = endPoint.Address.ToString();
            int endPointIPPort = endPoint.Port;
            return _peers.FirstOrDefault((peer) => {
                if (null == peer)
                    return false;
                string peerIPAddressString = peer.EndPoint.Address.ToString();
                int peerPort = peer.EndPoint.Port;
                return (
                    endPointIPString == peerIPAddressString &&
                    endPointIPPort == peerPort
                );
            }, null);
        }
        /// <summary>
        /// Handles side effects of receiving peer info packet. If the peer is
        /// not currently known it will be added to the list of known peers.
        /// Will also add the sending end point as a peer if it is not currently
        /// known.
        /// </summary>
        /// <param name="endPoint">
        /// The <c>IPEndPoint</c> representing the sending client.
        /// </param>
        /// <param name="dataBuffer">
        /// The packet data recieved in binary format.
        /// </param>
        /// <returns>
        /// The deserialized <c>PacketPeerInfo</c> instance.
        /// </returns>
        private Packet HandlePeerInfo(
          IPEndPoint endPoint,
          Memory<byte> dataBuffer
        ) {
            PacketPeerInfo peerInfo = new();
            peerInfo.FromBinary(dataBuffer);
            if ((
                null != LanEndPoint &&
                new IPAddress(peerInfo.IPAddress).ToString() ==
                LanEndPoint.Address.ToString() &&
                peerInfo.Port == LanEndPoint.Port
            ) || (
                null != WanEndPoint &&
                new IPAddress(peerInfo.IPAddress).ToString() ==
                WanEndPoint.Address.ToString() &&
                peerInfo.Port == WanEndPoint.Port
            )) {
                // This peer is ourselves don't ad it.
                return peerInfo;
            }
            if (
                peerInfo.IPAddress.ToString() != endPoint.Address.ToString() ||
                peerInfo.Port != endPoint.Port
            ) {
                // This packet came from the internet! The IP/port in the packet
                // is the LAN but the sending IP address is their public
                // internet facting IP address. Like maybe we shouldn't even
                // save their LAN IP? Well we will anyway and clean that up by
                // some other means.
                GetOrAddPeer(endPoint);
            }
            GetOrAddPeer(peerInfo.EndPoint);
            return peerInfo;
        }
        /// <summary>
        /// <c>HandleReceivedData</c> method deserializes a data buffer into
        /// the appropriate packet type and handles the packet as appropriate or
        /// forwards it to the calling application via <c>_gameCallback</c>.
        /// </summary>
        /// <param name="endPoint">
        /// The IP end point that the data was received from.
        /// </param>
        /// <param name="dataBuffer">
        /// The buffer containing the received data representing one or more 
        /// packets.
        /// </param>
        /// <param name="bytesReceived">
        /// The size of the data buffer in bytes 
        /// </param>
        /// <returns>
        /// An integer representing an error id if any occur.
        /// </returns>
        public int HandleReceivedData(
           IPEndPoint endPoint,
           Memory<byte> dataBuffer,
           int bytesReceived
        ) {
            int errorCode = 0;
            Peer? peer = GetPeer(endPoint);
            int processedBytes = 0;
            Memory<byte> remainingBytes =
                dataBuffer[..bytesReceived];
            while (processedBytes < bytesReceived) {
                int packetSize = ProcessNextPacketInBuffer(
                    endPoint,
                    remainingBytes
                );
                if (0 == packetSize) {
                    // Data is corrupted or unexpected. Throw the rest away.
                    errorCode = 1;
                    break;
                }
                processedBytes += packetSize;
                remainingBytes = dataBuffer[processedBytes..];
            }
            return errorCode;
        }
        /// <summary>
        /// Processes a single packet from the front of the provided data
        /// buffer.
        /// </summary>
        /// <param name="endPoint">
        /// The end point of the remote client we received the data from.
        /// </param>
        /// <param name="packetBuffer">
        /// Data buffer containing serialized packet data we have received.
        /// </param>
        /// <returns>
        /// The number of bytes processed.
        /// </returns>
        public int ProcessNextPacketInBuffer(
            IPEndPoint endPoint,
            Memory<byte> packetBuffer
        ) {
            int processedBytes = 0;
            PacketType packetType = Packet.GetPacketType(packetBuffer);
            Packet? packet = null;
            switch (packetType) {
            case PacketType.PEER_INFO:
                packet = HandlePeerInfo(
                    endPoint,
                    packetBuffer
                );
                break;
            case PacketType.ACKNOWLEDGE:
                packet = HandleAcknowledge(
                    endPoint,
                    packetBuffer
                );
                break;
            default:
                Peer? peer = GetPeer(endPoint);
                if (null == peer) {
                    // Data from unknown peer that isn't a connection request.
                    // We don't allow these to go out to the rest of the app.
                    break;
                }
                // Guarantee reliable packet order.
                int sequence = Packet.GetPacketSequence(packetBuffer);
                if (
                    sequence > 0 &&
                    sequence != peer.NextInboundPacketSequence
                ) {
                    // Save the packet data into the reliable packet inbox. Just
                    // save all the rest of the buffer because we don't know
                    // the size and the whole frame is going to be out of order.
                    // Only save the key if we don't have it already. This can
                    // happen if we have recieved the packet but have it has not
                    // been acknowledged yet causing the remote client to
                    // resend.
                    if (!peer.ReliablePacketInbox.ContainsKey(sequence)) {
                        peer.ReliablePacketInbox.Add(
                            sequence,
                            packetBuffer
                        );
                    }
                    // Everything processed for now. Return the full length of
                    // the buffer.
                    processedBytes += packetBuffer.Length;
                    break;
                } else {
                    // Increament NextInboundPacketSequence and also pass packet
                    // data to the applicaiton callback.
                    packet = _gameCallback(
                        endPoint,
                        packetBuffer
                    );
                    if (null == packet) {
                        // Something bad happened. bail.
                        break;
                    }
                    ++peer.NextInboundPacketSequence;
                    _ = SendAcknowledge(peer, packet.Sequence);
                    processedBytes += packet.GetSize();
                    // Lets keep processing pending packet data if we
                    // if we have it.
                    ProcessPendingPackets(peer);
                }
                break;
            }
            return processedBytes;
        }
        /// <summary>
        /// Processes any pending packets in <c>ReliablePacketInbox</c> in order
        /// if all subsequent packets have been received.
        /// </summary>
        /// <param name="peer">
        /// The peer whose pending packets are to be processed.
        /// </param>
        /// <returns>
        /// Returns an integer representing an error has occured or 0 if all OK.
        /// </returns>
        public int ProcessPendingPackets(Peer peer) {
            int errorCode = 0; // OK

            Packet? pendingPacket = null;
            int processedPendingBytes = 0;
            Memory<byte> pendingPacketData;
            bool haveNextPacket = peer.ReliablePacketInbox.TryGetValue(
                peer.NextInboundPacketSequence,
                out pendingPacketData
            );

            Memory<byte> remainingPendingPacketData;
            while (haveNextPacket) {
                peer.ReliablePacketInbox.Remove(peer.NextInboundPacketSequence);
                peer.NextInboundPacketSequence++;
                while (processedPendingBytes < pendingPacketData.Length) {
                    remainingPendingPacketData = pendingPacketData[
                        processedPendingBytes..
                    ];

                    pendingPacket = _gameCallback(
                        peer.EndPoint,
                        remainingPendingPacketData
                    );

                    if (null == pendingPacket) {
                        // Bad packet data. bail!
                        errorCode = 1;
                        break;
                    }

                    _ = SendAcknowledge(peer, pendingPacket.Sequence);

                    processedPendingBytes += pendingPacket.GetSize();
                }

                haveNextPacket = peer.ReliablePacketInbox.TryGetValue(
                    peer.NextInboundPacketSequence,
                    out pendingPacketData
                );
            }

            return errorCode;
        }
        /// <summary>
        /// Handles side effects for a recieved acknowledge packet.
        /// </summary>
        /// <param name="endPoint">
        /// The <c>IPEndPoint</c> instance representing the IP information of
        /// the sending client.
        /// </param>
        /// <param name="dataBuffer">
        /// Data buffer containing serialized packet data.
        /// </param>
        /// <returns>
        /// A instance of <c>PacketAcknowledge</c> deserialized from the packet
        /// data.
        /// </returns>
        private Packet? HandleAcknowledge(
            IPEndPoint endPoint,
            Memory<byte> dataBuffer
        ) {
            PacketAcknowledge ack = new();
            Peer? peer = GetPeer(endPoint);
            if (null == peer)
                return ack;
            ack.FromBinary(dataBuffer);
            peer.ConfirmAcknowledge(ack);
            return ack;
        }
        /// <summary>
        /// Starts the process of receiving data over the network.
        /// </summary>
        /// <returns>
        /// The total number of bytes received.
        /// </returns>
        public async Task<int> StartReceivingData() {
            int errorCode = 0;
            ReceivingData = true;
            errorCode = await ReceiveData();
            return errorCode;
        }
        /// <summary>
        /// Method that handles the loop for receiving data.
        /// </summary>
        /// <returns>
        /// The total number of bytes received.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Exception is thrown if the <c>_socket</c> property is not a valid
        /// and initialized socket.
        /// </exception>
        public async Task<int> ReceiveData() {
            if (null == _socket)
                throw new InvalidOperationException(
                    nameof(_socket) + "is null."
                );

            int errorCode = 0; // OK
            // Pin the receive data buffer so the garbage collector doesn't
            // clean it up.
            byte[] buffer = GC.AllocateArray<byte>(ETHERNET_FRAME_SIZE, true);
            Memory<byte> memoryBuffer = buffer.AsMemory();
            SocketReceiveFromResult result;
            while (ReceivingData) {
                try {
                    result = await _socket.ReceiveFromAsync(
                        memoryBuffer,
                        new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort),
                        ReceiveFromCancellationSource.Token
                    );
                } catch (OperationCanceledException) {
                    // Receiving data has been stopped by consuming application.
                    break;
                }
                if (result.RemoteEndPoint is not IPEndPoint remoteEP)
                    continue;
                errorCode = HandleReceivedData(
                    remoteEP,
                    memoryBuffer,
                    result.ReceivedBytes
                );
            }
            ReceiveFromCancellationSource.Dispose();
            ReceiveFromCancellationSource = new CancellationTokenSource();
            return errorCode;
        }
        /// <summary>
        /// Sends binary data to a provided remote end point.
        /// </summary>
        /// <param name="data">
        /// A buffer containing the data to sent.
        /// </param>
        /// <param name="remoteEndPoint">
        /// The remote end point to send data to.
        /// </param>
        /// <returns>
        /// The total number of bytes sent.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Exception is thrown if the <c>_socket</c> property is not a valid
        /// and initialized socket.
        /// </exception>
        public async ValueTask<int> SendData(
            ReadOnlyMemory<byte> data,
            IPEndPoint remoteEndPoint
        ) {
            if (null == _socket)
                throw new InvalidOperationException(
                    nameof(_socket) + "is null."
                );
            return await _socket.SendToAsync(
                data,
                remoteEndPoint
            );
        }
        /// <summary>
        /// Sends all known peer information to a provided remote
        /// <c>IPEndPoint</c>
        /// </summary>
        /// <param name="remoteEndPoint">
        /// The remote end point to send the peer information to.
        /// </param>
        /// <returns>
        /// The total number of bytes sent.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Exception is thrown if the <c>_socket</c> property is not a valid
        /// and initialized socket.
        /// </exception>
        public async Task<int> SendPeerInfo(
            IPEndPoint remoteEndPoint
        ) {
            if (null == _socket)
                throw new InvalidOperationException(
                    nameof(_socket) + "is null."
                );
            return await Task<int>.Run(async () => {
                IPEndPoint? lanEndPoint = _socket.LocalEndPoint;
                // It's possible we don't know our WAN IP addressInfo and port so
                // don't throw an exception if WanEndPoint is null. We will send
                // 0.0.0.0/0 to represent null as we don't want to allow peers
                // to share broadcast/any ip endpoints.
                IPEndPoint? wanEndPoint = WanEndPoint;
                if (null == lanEndPoint) {
                    throw new InvalidOperationException(
                        $"Invalid socket. {nameof(_socket.LocalEndPoint)} " +
                        "is null!"
                    );
                }
                PacketPeerInfo packetPeerInfoLan = (
                    new PacketPeerInfo(lanEndPoint)
                );
                int bytesSent = await SendData(
                    packetPeerInfoLan.ToBinary(),
                    remoteEndPoint
                );
                if (null != wanEndPoint) {
                    PacketPeerInfo packetPeerInfoWan = (
                        new PacketPeerInfo(wanEndPoint)
                    );
                    bytesSent += await SendData(
                        packetPeerInfoWan.ToBinary(),
                        remoteEndPoint
                    );
                }
                return bytesSent;
            });
        }
        /// <summary>
        /// Sends an acknowledge packet to a known peer.
        /// </summary>
        /// <param name="peer">
        /// An instance of <c>Peer</c> representing the remote client to send
        /// the acknowledge to.
        /// </param>
        /// <param name="packetId">
        /// The sequence ID of the packet that requested to be acknowledged.
        /// </param>
        /// <returns>
        /// The total number of bytes sent.
        /// </returns>
        public async Task<int> SendAcknowledge(
            Peer peer,
            int packetId
        ) {
            PacketAcknowledge ackPacket = new();
            ackPacket.PacketIDToAck = packetId;
            return await SendPacket(ackPacket, peer);
        }
        /// <summary>
        /// Sends a packet to a known peer. If the packet has a sequence value
        /// of greater that 0 it will be added to the dictionary of
        /// unacknowledged packets and the <c>ReliablePacketsSent</c> counter
        /// will be incremented.
        /// </summary>
        /// <param name="packet">
        /// The packet to send.
        /// </param>
        /// <param name="peer">
        /// An instance of <c>Peer</c> representing the remote client to send
        /// the packet to.
        /// </param>
        public async Task<int> SendPacket(Packet packet, Peer peer) {
            int bytesSent = await SendData(packet.ToBinary(), peer.EndPoint);
            if (0 < packet.Sequence) {
                peer.AddUnacknowledgedPacket(packet);
                ++peer.ReliablePacketsSent;
            }
            return bytesSent;
        }
        /// <summary>
        /// Sends a packet to all known peers.
        /// </summary>
        /// <param name="packet">
        /// The packet to send to all known peers.
        /// </param>
        /// <returns>
        /// The total number of bytes sent.
        /// </returns>
        public async Task<int> BroadcastPacketToPeers(
            Packet packet,
            bool reliable = false
        ) {
            int bytesSent = 0;
            foreach (var peer in _peers) {
                if (reliable) {
                    packet.Sequence = peer.GetNextPacketSequence();
                }
                bytesSent += await SendPacket(packet, peer);
            }
            return bytesSent;
        }
        /// <summary>
        /// Sends all known peer infomation to all other known peers.
        /// </summary>
        /// <returns>
        /// The number of bytes sent.
        /// </returns>
        public async Task<int> BroadcastPeerInfo() {
            int bytesSent = 0;
            foreach (var peer in _peers) {
                PacketPeerInfo packetPeerInfo = new PacketPeerInfo(
                    peer.EndPoint
                );
                bytesSent += await BroadcastPacketToPeers(packetPeerInfo);
            }
            return bytesSent;
        }
        /// <summary>
        /// An instance that implements <c>ISocket</c> representing the
        /// platform provided network socket interface.
        /// </summary>
        private ISocket? _socket;
        /// <summary>
        /// A callback to be defined by the implementing application which is
        /// invoked with recieved network data.
        /// </summary>
        private DReceivedPacket _gameCallback;
        /// <summary>
        /// A list of <c>Peer</c> instances representing known remote clients.
        /// </summary>
        private List<Peer> _peers;
        /// <summary>
        /// 
        /// </summary>
        public List<Peer> Peers {
            get {
                return _peers;
            }
        }
        /// <summary>
        /// A boolean flag representing if the client is receiving data or not.
        /// Set to false to break the loop responsible for receiving data.
        /// </summary>
        public bool ReceivingData {
            get; set;
        }
        /// <summary>
        /// A boolean flag indicating if the client is trusted with maintaining
        /// and providing application state information.
        /// </summary>
        public bool IsDedicated {
            get; private set;
        }
        /// <summary>
        /// A boolean flag indicating if the client is currently running and
        /// communicating.
        /// </summary>
        public bool IsAlive {
            get; set;
        }
        /// <summary>
        /// If known this is the clients public internet IP address.
        /// </summary>
        public IPEndPoint? WanEndPoint {
            get; private set;
        }
        /// <summary>
        /// This is the clients ip address on the local network the client is
        /// running on.
        /// </summary>
        public IPEndPoint? LanEndPoint {
            get {
                return null == _socket ? null : _socket.LocalEndPoint;
            }
        }
        /// <summary>
        /// A cancellation token used to stop the socket from receiving data.
        /// </summary>
        public CancellationTokenSource ReceiveFromCancellationSource {
            get; private set;
        }
        /// <summary>
        /// The <c>IPEndPoint</c>
        /// </summary>
        public IPEndPoint? DedicatedServerEndPoint {
            get; private set;
        }
        /// <summary>
        /// Property used to control the peer search sub process. Set to false
        /// to terminate the search.
        /// </summary>
        public bool SharingPeers {
            get; private set;
        }
        /// <summary>
        /// A boolean flag indicating if the client should be resending
        /// unacknowledged packets.
        /// </summary>
        public bool ResendingUnacknowledgedPackets {
            get; private set;
        }
    }
}
