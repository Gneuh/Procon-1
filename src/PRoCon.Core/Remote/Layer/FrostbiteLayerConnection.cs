using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PRoCon.Core.Remote.Layer {
    public class FrostbiteLayerConnection {
        protected const UInt32 MaxGarbageBytes = 4194304;

        /// <summary>
        ///     Lock used when aquiring a sequence #
        /// </summary>
        protected readonly Object AcquireSequenceNumberLock = new Object();

        protected TcpClient Client;
        protected byte[] PacketStream;
        protected byte[] ReceivedBuffer;
        protected UInt32 SequenceNumber;
        protected Object ShutdownConnectionLock = new Object();
        protected NetworkStream Stream;

        #region Events

        public delegate void EmptyParameterHandler(FrostbiteLayerConnection sender);

        public delegate void PacketDispatchHandler(FrostbiteLayerConnection sender, Packet packet);

        public event EmptyParameterHandler ConnectionClosed;

        public event PacketDispatchHandler PacketSent;
        public event PacketDispatchHandler PacketReceived;

        #endregion

        public FrostbiteLayerConnection(TcpClient acceptedConnection) {
            ReceivedBuffer = new byte[4096];
            PacketStream = null;

            Client = acceptedConnection;

            Stream = Client.GetStream();

            Stream.BeginRead(ReceivedBuffer, 0, ReceivedBuffer.Length, ReceiveCallback, this);
        }

        /// <summary>
        ///     The last packet that was receieved by this connection.
        /// </summary>
        public Packet LastPacketReceived { get; protected set; }

        /// <summary>
        ///     The last packet that was sent by this connection.
        /// </summary>
        public Packet LastPacketSent { get; protected set; }

        public UInt32 AcquireSequenceNumber {
            get {
                lock (AcquireSequenceNumberLock) {
                    return ++SequenceNumber;
                }
            }
        }

        //private string m_strClientIPPort = String.Empty;
        public string IPPort {
            get {
                string strClientIPPort = String.Empty;

                // However if the connection is open just get it straight from the horses mouth.
                if (Client != null && Client.Connected == true) {
                    strClientIPPort = ((IPEndPoint) Client.Client.RemoteEndPoint).Address + ":" + ((IPEndPoint) Client.Client.RemoteEndPoint).Port.ToString(CultureInfo.InvariantCulture);
                }

                return strClientIPPort;
            }
        }

        private void SendAsyncCallback(IAsyncResult ar) {
            try {
                Stream.EndWrite(ar);

                if (PacketSent != null) {
                    FrostbiteConnection.RaiseEvent(PacketSent.GetInvocationList(), this, (Packet) ar.AsyncState);
                }
            }
            catch (SocketException) {
                Shutdown();
            }
            catch (Exception) {
                Shutdown();
            }
        }

        public void SendAsync(Packet packet) {
            try {
                byte[] bytePacket = packet.EncodePacket();

                LastPacketSent = packet;

                Stream.BeginWrite(bytePacket, 0, bytePacket.Length, SendAsyncCallback, packet);
            }
            catch (SocketException) {
                // TO DO: Error reporting, possibly in a log file.
                Shutdown();
            }
            catch (Exception) {
                Shutdown();
            }
        }

        private void ReceiveCallback(IAsyncResult ar) {
            if (Stream != null) {
                try {
                    int iBytesRead = Stream.EndRead(ar);
                    iBytesRead = ServeCrossDomainPolicy(iBytesRead);

                    if (iBytesRead > 0) {
                        // Create or resize our packet stream to hold the new data.
                        if (PacketStream == null) {
                            PacketStream = new byte[iBytesRead];
                        }
                        else {
                            Array.Resize(ref PacketStream, PacketStream.Length + iBytesRead);
                        }

                        Array.Copy(ReceivedBuffer, 0, PacketStream, PacketStream.Length - iBytesRead, iBytesRead);

                        UInt32 packetSize = Packet.DecodePacketSize(PacketStream);

                        while (PacketStream.Length >= packetSize && PacketStream.Length > Packet.PacketHeaderSize) {
                            // Copy the complete packet from the beginning of the stream.
                            var completePacket = new byte[packetSize];
                            Array.Copy(PacketStream, completePacket, packetSize);

                            var deserializedPacket = new Packet(completePacket);
                            SequenceNumber = Math.Max(SequenceNumber, deserializedPacket.SequenceNumber);

                            LastPacketReceived = deserializedPacket;

                            // Dispatch the completed packet.
                            if (PacketReceived != null) {
                                FrostbiteConnection.RaiseEvent(PacketReceived.GetInvocationList(), this, deserializedPacket);
                            }
                            //this.DispatchPacket(cpCompletePacket);

                            // Now remove the completed packet from the beginning of the stream
                            var updatedSteam = new byte[PacketStream.Length - packetSize];
                            Array.Copy(PacketStream, packetSize, updatedSteam, 0, PacketStream.Length - packetSize);
                            PacketStream = updatedSteam;

                            packetSize = Packet.DecodePacketSize(PacketStream);
                        }

                        // If we've recieved 16 kb's and still don't have a full command then shutdown the connection.
                        if (ReceivedBuffer.Length >= MaxGarbageBytes) {
                            ReceivedBuffer = null;
                            Shutdown();
                        }
                        else {
                            Stream.BeginRead(ReceivedBuffer, 0, ReceivedBuffer.Length, ReceiveCallback, null);
                        }
                    }
                    else {
                        Shutdown();
                    }
                }
                catch (Exception) {
                    Shutdown();
                }
            }
        }

        /// <summary>
        ///     Pokes the connection, ensuring that the connection is still alive. If
        ///     this method determines that the connection is dead then it will call for
        ///     a shutdown.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method is a final check to make sure communications are proceeding in both directions in
        ///         the last five minutes. If nothing has been sent and received in the last five minutes then the connection is assumed
        ///         dead and a shutdown is initiated.
        ///     </para>
        /// </remarks>
        public virtual void Poke() {
            bool downstreamDead = LastPacketReceived != null && LastPacketReceived.Stamp < DateTime.Now.AddMinutes(-5);
            bool upstreamDead = LastPacketSent != null && LastPacketSent.Stamp < DateTime.Now.AddMinutes(-5);

            if (downstreamDead && upstreamDead) {
                Shutdown();
            }
        }

        // TO DO: Better error reporting on this method.
        public void Shutdown() {
            try {
                if (ConnectionClosed != null) {
                    FrostbiteConnection.RaiseEvent(ConnectionClosed.GetInvocationList(), this);
                }

                if (Client != null) {
                    lock (ShutdownConnectionLock) {
                        if (Stream != null) {
                            Stream.Close();
                            Stream.Dispose();
                            Stream = null;
                        }

                        if (Client != null) {
                            Client.Close();
                            Client = null;
                        }
                    }
                }
            }
            catch (SocketException) {
                // TO DO: Error reporting, possibly in a log file.
            }
            catch (Exception) {
            }
        }

        #region CrossDomainCode

        // Represents NULL-terminated "<policy-file-request/>"  
        protected static readonly byte[] PolicyRequest = new byte[] {0x3c, 0x70, 0x6f, 0x6c, 0x69, 0x63, 0x79, 0x2d, 0x66, 0x69, 0x6c, 0x65, 0x2d, 0x72, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x2f, 0x3e, 0x00};

        protected int ServeCrossDomainPolicy(int iBytesRead) {
            // Cross domain policy is only served once, at the begining of the TCP connection
            if (PacketStream != null)
                return iBytesRead;

            if (iBytesRead >= PolicyRequest.Length) {
                // Compare buffers, to see if policy request was received
                int i = 0;
                for (; i < PolicyRequest.Length; i++)
                    if (ReceivedBuffer[i] != PolicyRequest[i])
                        break;

                if (i == PolicyRequest.Length) {
                    // Comparison succeeded
                    int iLocalPort = ((IPEndPoint) Client.Client.LocalEndPoint).Port;

                    String sPolicyResponse = "<?xml version=\"1.0\"?>" + "<!DOCTYPE cross-domain-policy " + "SYSTEM \"http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd\">" + "<cross-domain-policy>" + "<allow-access-from domain=\"*\" to-ports=\"" + iLocalPort + "\" />" + "</cross-domain-policy>";

                    byte[] response = Encoding.GetEncoding(1252).GetBytes(sPolicyResponse + Convert.ToChar(0x00));
                    Stream.Write(response, 0, response.Length);

                    // Remove the policy request from the begining of the receive buffer
                    iBytesRead -= PolicyRequest.Length;
                    Array.Copy(ReceivedBuffer, PolicyRequest.Length, ReceivedBuffer, 0, iBytesRead);
                }
            }

            return iBytesRead;
        }

        #endregion
    }
}