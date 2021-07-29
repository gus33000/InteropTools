using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace InteropTools.RemoteClasses.Server
{
    public class RemoteServer
    {
        public delegate void DataReceived(string data, StreamSocketListenerConnectionReceivedEventArgs args);

        public delegate void Error(string message);

        private StreamSocketListener _listener;

        public bool Started;
        public int Port { get; private set; }
        public event DataReceived OnDataReceived;
        public event Error OnError;

        public async void Start(int port)
        {
            Port = port;

            await new WebServer().Run();

            try
            {
                if (_listener != null)
                {
                    await _listener.CancelIOAsync();
                    _listener.Dispose();
                    _listener = null;
                }

                _listener = new StreamSocketListener();
                _listener.ConnectionReceived += Listener_ConnectionReceived;
                await _listener.BindServiceNameAsync(Port.ToString());
                Started = true;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Stop()
        {
            if (!Started)
            {
                return;
            }

            _listener.Dispose();
            Started = false;
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new(args.Socket.InputStream);

            try
            {
                while (true)
                {
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));

                    if (sizeFieldCount != sizeof(uint))
                    {
                        return;
                    }

                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);

                    if (stringLength != actualStringLength)
                    {
                        return;
                    }

                    if (OnDataReceived == null)
                    {
                        continue;
                    }

                    string data = reader.ReadString(actualStringLength);
                    OnDataReceived(data, args);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }
    }
}