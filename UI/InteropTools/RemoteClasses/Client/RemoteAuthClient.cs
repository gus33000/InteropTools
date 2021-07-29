using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace InteropTools.RemoteClasses.Client
{
    internal class RemoteAuthClient
    {
        public delegate void Authentificated();

        public delegate void Connected();

        public delegate void Error(string message);

        private DataReader _reader;
        private StreamSocket _socket;
        private DataWriter _writer;

        public RemoteAuthClient(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        private string Ip { get; }
        private int Port { get; }
        public event Error OnError;
        public event Connected OnConnected;
        public event Authentificated OnAuthentificated;
        public event Authentificated OnNotAuthentificated;

        public async void Connect()
        {
            try
            {
                HostName hostName = new(Ip);
                _socket = new StreamSocket();
                await _socket.ConnectAsync(hostName, Port.ToString());
                OnConnected?.Invoke();
                _writer = new DataWriter(_socket.OutputStream);
                Read();
                RootObject jsonObject = new()
                {
                    SessionID = SessionManager.SessionId,
                    Operation = "Authentificate"
                };
                string json = JsonConvert.SerializeObject(jsonObject);
                Send(json);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }

        private async void Send(string message)
        {
            _writer.WriteUInt32(_writer.MeasureString(message));
            _writer.WriteString(message);

            try
            {
                await _writer.StoreAsync();
                await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }

        private async void Read()
        {
            _reader = new DataReader(_socket.InputStream);

            try
            {
                while (true)
                {
                    uint sizeFieldCount = await _reader.LoadAsync(sizeof(uint));

                    if (sizeFieldCount != sizeof(uint))
                    {
                        return;
                    }

                    uint stringLength = _reader.ReadUInt32();

                    if (stringLength == 0)
                    {
                        return;
                    }

                    uint actualStringLength = await _reader.LoadAsync(stringLength);

                    if (stringLength != actualStringLength)
                    {
                        return;
                    }

                    string reply = _reader.ReadString(actualStringLength);
                    RootObject obj = JsonConvert.DeserializeObject<RootObject>(reply);

                    if (obj.Result.Status == "SUCCESS")
                    {
                        OnAuthentificated?.Invoke();
                    }
                    else
                    {
                        OnNotAuthentificated?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }

        private class Item
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Hive { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
        }

        private class Result
        {
            public string Error { get; set; }
            public bool Exists { get; set; }
            public string ValueData { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
            public string Status { get; set; }

            public List<Item> Items { get; set; }

            public string AppInstallationPath { get; set; }

            public DateTime LastModifiedTime { get; set; }
        }

        private class RootObject
        {
            public string SessionID { get; set; }
            public string Operation { get; set; }
            public string Path { get; set; }
            public string Hive { get; set; }
            public string Key { get; set; }
            public string ValueName { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
            public string ValueData { get; set; }
            public bool Recursive { get; set; }
            public string NewName { get; set; }
            public Result Result { get; set; }
            public string FilePath { get; set; }
            public string mountpoint { get; set; }
            public bool inUser { get; set; }
        }
    }
}