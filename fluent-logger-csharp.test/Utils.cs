using MsgPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Test
{
    static class Utils
    {
        public static string Unpack(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            var unpacker = Unpacker.Create(ms);
            var data = unpacker.ReadItem();

            return data.Value.ToString();
        }
    }
}
