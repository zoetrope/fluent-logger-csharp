using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Fluent
{
    public class FluentSender : IDisposable
    {
        private string _host;
        private int _port;
        private int _bufmax;
        private int _timeout;
        private bool _verbose;
        private readonly SemaphoreSlim _lockObj = new SemaphoreSlim(1);
        private byte[] _pendings;
        private TcpClient _client;
        private MessagePacker _packer;
				private Timer _retrytimer;
				private int _autoretryinterval;

        public FluentSender(string tag, string host = "localhost", int port = 24224, int bufmax = 1024*1024, int timeout = 3000, bool verbose = false, int autoretryinterval = 10000)
        {
						_client = null;
						_pendings = null;

            _host = host;
            _port = port;
            _bufmax = bufmax;
            _timeout = timeout;
            _verbose = verbose;
						_autoretryinterval = autoretryinterval;

            _packer = new MessagePacker(tag);
        }

        public static async Task<FluentSender> CreateSync(string tag, string host = "localhost", int port = 24224, int bufmax = 1024*1024, int timeout = 3000, bool verbose = false, int autoretryinterval = 10000)
        {
            var sender = new FluentSender(tag, host, port, bufmax, timeout, verbose);

            await sender.InitializeAsync();

            return sender;
        }

				private async void RetryTimerTick(object sender)
				{
					await SendAsync(new byte[0]);
				}

        private async Task InitializeAsync()
        {
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

        public async Task EmitAsync(string label, params object[] obj)
        {
            var curTime = DateTime.Now;
            await EmitWithTimeAsync(label, curTime, obj);
        }

        public async Task EmitWithTimeAsync(string label, DateTime timestamp, params object[] obj)
        {
            var bytes = _packer.MakePacket(label, timestamp, obj);
            await SendAsync(bytes);
        }


        private async Task SendAsync(byte[] bytes)
        {
            await _lockObj.WaitAsync();

            try
            {
                await SendInternalAsync(bytes);
            }
            finally
            {
                _lockObj.Release();
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

						if (bytes.Length == 0) {
							return;
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

										if (_autoretryinterval > 0) {
											if (_retrytimer == null) {
												_retrytimer = new Timer(RetryTimerTick);
											}

											_retrytimer.Change(_autoretryinterval, 0);
										}
                }
            }
        }
    }
}
