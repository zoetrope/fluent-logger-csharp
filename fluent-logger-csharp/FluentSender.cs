using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Fluent
{
    public class FluentSender : IDisposable
    {
        private string _tag;
        private string _host;
        private int _port;
        private int _bufmax;
        private int _timeout;
        private bool _verbose;
        private readonly object _lockObj = new object();
        private byte[] _pendings;
        private TcpClient _client;
        private MessagePacker _packer;

        public static async Task<FluentSender> CreateSync(string tag, string host = "localhost", int port = 24224, int bufmax = 1024*1024, int timeout = 3000, bool verbose = false)
        {
            var sender = new FluentSender();

            await sender.InitializeAsync(tag, host, port, bufmax, timeout, verbose);

            return sender;
        }
        private FluentSender()
        {
        }


        private async Task InitializeAsync(string tag, string host, int port, int bufmax, int timeout, bool verbose)
        {
            _host = host;
            _port = port;
            _bufmax = bufmax;
            _timeout = timeout;
            _verbose = verbose;
            _packer = new MessagePacker(tag);

            try
            {
                await ReconnectAsync();
            }
            catch
            {
                Close();
            }
        }

        public void Dispose()
        {
            Close();
        }

        private void Close()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        private async Task ReconnectAsync()
        {
            if (_client == null)
            {
                var client = new TcpClient();
                client.SendTimeout = _timeout;
                await client.ConnectAsync(_host, _port);
                _client = client;
            }
        }

        public async Task EmitAsync(string label, object obj)
        {
            var curTime = DateTime.UtcNow;
            await EmitWithTimeAsync(label, curTime, obj);
        }

        public async Task EmitWithTimeAsync(string label, DateTime timestamp, object obj)
        {
            var bytes = _packer.MakePacket(label, timestamp, obj);
            await SendAsync(bytes);
        }


        private async Task SendAsync(byte[] bytes)
        {
            while (!Monitor.TryEnter(_lockObj)) await Task.Yield();

            try
            {
                await SendInternalAsync(bytes);
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }


        private async Task SendInternalAsync(byte[] bytes)
        {
            if (_pendings != null)
            {
                var mergedArray = new byte[_pendings.Length + bytes.Length];
                Buffer.BlockCopy(_pendings, 0, mergedArray, 0, _pendings.Length);
                Buffer.BlockCopy(bytes, 0, mergedArray, _pendings.Length, bytes.Length);

                _pendings = mergedArray;
                bytes = mergedArray;
            }

            try
            {
                await ReconnectAsync();
                await _client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                await _client.GetStream().FlushAsync();
                _pendings = null;
            }
            catch
            {
                Close();

                if (_pendings != null && _pendings.Length > _bufmax)
                {
                    _pendings = null;
                }
                else
                {
                    _pendings = bytes;
                }
            }
        }
    }
}
