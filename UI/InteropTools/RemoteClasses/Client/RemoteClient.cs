using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace InteropTools.RemoteClasses.Client
{
    internal class RemoteClient
    {
        private DataReader _reader;
        private StreamSocket _socket;
        private DataWriter _writer;

        public RemoteClient(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        private string Ip { get; }
        private int Port { get; }

        public async Task<string> GetData(string message)
        {
            try
            {
                HostName hostName = new(Ip);
                _socket = new StreamSocket();

                try
                {
                    await _socket.ConnectAsync(hostName, Port.ToString());
                    _writer = new DataWriter(_socket.OutputStream);
                    _writer.WriteUInt32(_writer.MeasureString(message));
                    _writer.WriteString(message);

                    try
                    {
                        await _writer.StoreAsync();
                        await _writer.FlushAsync();
                    }
                    catch
                    {
                        return null;
                    }

                    _reader = new DataReader(_socket.InputStream);

                    try
                    {
                        uint sizeFieldCount = await _reader.LoadAsync(sizeof(uint));

                        if (sizeFieldCount != sizeof(uint))
                        {
                            return null;
                        }

                        uint stringLength = _reader.ReadUInt32();
                        uint actualStringLength = await _reader.LoadAsync(stringLength);
                        return stringLength != actualStringLength ? null : _reader.ReadString(actualStringLength);
                    }
                    catch
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public void Close()
        {
            try
            {
                _writer.DetachStream();
                _writer.Dispose();
                _reader.DetachStream();
                _reader.Dispose();
                _socket.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}