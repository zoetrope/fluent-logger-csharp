using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fluent
{
    internal class MessagePacker
    {
        private string _tag;
        private int _nestCount = 0;

        public int MaxNestCount { get; set; }

        public MessagePacker(string tag, int maxNestCount = 10)
        {
            _tag = tag;
            MaxNestCount = maxNestCount;
        }

        private static readonly DateTime _epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public byte[] MakePacket(string label, DateTime timestamp, params object[] records)
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
            foreach (var record in records)
            {
                xs.Add(timestamp.ToUniversalTime().Subtract(_epoc).TotalSeconds);
                _nestCount = 0;
                xs.Add(CreateTypedMessagePackObject(record.GetType(), record));
            }
            var x = new MessagePackObject(xs);

            var ms = new MemoryStream();
            var packer = Packer.Create(ms);
            packer.Pack(x);
            return ms.ToArray();
        }
        private MessagePackObject CreateTypedMessagePackObject(Type type, object obj)
        {
            if (_nestCount > MaxNestCount)
            {
                throw new InvalidOperationException("nest counter is over the maximum. counter = " + _nestCount);
            }

            _nestCount++;
            using (var d = new DisposeWithAction(() => _nestCount--))
            {
                if (obj == null)
                {
                    throw new ArgumentException("can't pack a null value. type = " + type.FullName, "type");
                }

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
                if (type == typeof(DateTime)) return new MessagePackObject(MessagePackConvert.FromDateTime((DateTime)obj));
                if (type == typeof(DateTimeOffset)) return new MessagePackObject(MessagePackConvert.FromDateTimeOffset((DateTimeOffset)obj));

                if (type.IsEnum) return new MessagePackObject(obj.ToString());

                if (type.IsArray)
                {
                    return new MessagePackObject((obj as object[]).Select(x => CreateTypedMessagePackObject(type.GetElementType(), x)).ToList());
                }

                if (obj is ExpandoObject || obj is IDictionary<string, object>)
                {
                    var tmp = obj as IDictionary<string, object>;
                    var dict = tmp.ToDictionary(x => new MessagePackObject(x.Key), x => CreateTypedMessagePackObject(x.Value.GetType(), x.Value));
                    return new MessagePackObject(new MessagePackObjectDictionary(dict));
                }
                else if (type.IsValueType)
                {
                    var dict = obj.GetType()
                        .GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .ToDictionary(fi => new MessagePackObject(fi.Name), fi => CreateTypedMessagePackObject(fi.FieldType, fi.GetValue(obj)));
                    return new MessagePackObject(new MessagePackObjectDictionary(dict));
                }
                else if (type.GetInterfaces().Any(i => i == typeof(System.Collections.IEnumerable)))
                {
                    var list = new List<MessagePackObject>();
                    foreach (var x in obj as System.Collections.IEnumerable)
                    {
                        list.Add(CreateTypedMessagePackObject(x.GetType(), x));
                    }
                    return new MessagePackObject(list);
                }
                else
                {
                    var dict = obj.GetType()
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .ToDictionary(pi => new MessagePackObject(pi.Name), pi => CreateTypedMessagePackObject(pi.PropertyType, pi.GetValue(obj, null)));
                    return new MessagePackObject(new MessagePackObjectDictionary(dict));
                }
            }
        }

        public class DisposeWithAction : IDisposable
        {
            private Action _action;
            public DisposeWithAction(Action action)
            {
                _action = action;
            }
            public void Dispose()
            {
                _action();
            }
        }
    }
}
