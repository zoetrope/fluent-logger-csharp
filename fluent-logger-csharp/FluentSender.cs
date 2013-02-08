using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
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

        private FluentSender()
        {
        }

        public static async Task<FluentSender> CreateSync(string tag, string host = "localhost", int port = 24224, int bufmax = 1024*1024, int timeout = 3000, bool verbose = false)
        {
            var sender = new FluentSender();

            await sender.InitializeAsync(tag, host, port, bufmax, timeout, verbose);

            return sender;
        }

        private async Task InitializeAsync(string tag, string host, int port, int bufmax, int timeout, bool verbose)
        {

            _tag = tag;
            _host = host;
            _port = port;
            _bufmax = bufmax;
            _timeout = timeout;
            _verbose = verbose;

            try
            {
                await ReconnectAsync();
            }
            catch
            {
                Close();
            }
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
            var mpObj = CreateTypedMessagePackObject(obj.GetType(), obj);
            var bytes = MakePacket(label, timestamp, mpObj);
            await SendAsync(bytes);
        }

        private MessagePackObject CreateTypedMessagePackObject(Type type, object obj)
        {

            if (type == typeof(bool)) return new MessagePackObject((bool)obj);
            if (type == typeof(byte)) return new MessagePackObject((byte)obj);
            if (type == typeof(byte[])) return new MessagePackObject((byte[])obj);
            if (type == typeof(double)) return new MessagePackObject((double)obj);
            if (type == typeof(float)) return new MessagePackObject((float)obj);
            if (type == typeof(int)) return new MessagePackObject((int)obj);
            if (type == typeof(long)) return new MessagePackObject((long)obj);
            if (type == typeof(sbyte)) return new MessagePackObject((sbyte)obj);
            if (type == typeof(short)) return new MessagePackObject((short)obj);
            if (type == typeof(string)) return new MessagePackObject((string)obj);
            if (type == typeof(uint)) return new MessagePackObject((uint)obj);
            if (type == typeof(ulong)) return new MessagePackObject((ulong)obj);
            if (type == typeof(ushort)) return new MessagePackObject((ushort)obj);

            if (type.IsArray)
            {
                return new MessagePackObject((obj as object[]).Select(x => CreateTypedMessagePackObject(type.GetElementType(), x)).ToList());
            }

            if (obj is ExpandoObject || obj is Dictionary<string, object>)
            {
                var tmp = obj as Dictionary<string, object>;
                var dict = tmp.ToDictionary(x => new MessagePackObject(x.Key), x => CreateTypedMessagePackObject(x.GetType(), x.Value));
                return new MessagePackObject(new MessagePackObjectDictionary(dict));
            }
            else
            {
                var dict = obj.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .ToDictionary(pi => new MessagePackObject(pi.Name), pi => CreateTypedMessagePackObject(pi.PropertyType, pi.GetValue(obj, null)));
                return new MessagePackObject(new MessagePackObjectDictionary(dict));
            }

            throw new ArgumentException("invalid type", "type");
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
        private byte[] MakePacket(string label, DateTime timestamp, MessagePackObject data)
        {
            string tag;
            if (!string.IsNullOrEmpty(label))
            {
                tag = _tag + "." + label;
            }
            else
            {
                tag = _tag;
            }

            var xs = new List<MessagePackObject>();
            xs.Add(tag);
            xs.Add(MessagePackConvert.FromDateTime(timestamp));
            xs.Add(data);
            var x = new MessagePackObject(xs);

            if (_verbose)
            {
                Console.WriteLine(tag);
                Console.WriteLine(timestamp);
                Console.WriteLine(data.ToString());
            }

            var ms = new MemoryStream();
            var packer = Packer.Create(ms);
            packer.Pack(x);
            return ms.ToArray();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
