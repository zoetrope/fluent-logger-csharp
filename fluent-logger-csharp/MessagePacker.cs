using MsgPack;
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
        public MessagePacker(string tag)
        {
            _tag = tag;
        }

        public byte[] MakePacket(string label, DateTime timestamp, object obj)
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
            xs.Add(CreateTypedMessagePackObject(obj.GetType(), obj));
            var x = new MessagePackObject(xs);

            var ms = new MemoryStream();
            var packer = Packer.Create(ms);
            packer.Pack(x);
            return ms.ToArray();
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

            if (obj is ExpandoObject || obj is IDictionary<string, object>)
            {
                var tmp = obj as IDictionary<string, object>;
                var dict = tmp.ToDictionary(x => new MessagePackObject(x.Key), x => CreateTypedMessagePackObject(x.Value.GetType(), x.Value));
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
    }
}
