using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Test
{
    class DummyServer
    {

        public async Task<byte[]> Run(int port)
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();


            var ms = new MemoryStream();

            using (var stream = client.GetStream())
            {
                var data = new byte[4096];
                var x = await stream.ReadAsync(data, 0, data.Length);
                await ms.WriteAsync(data, 0, x);
            }
            return ms.ToArray();
        }
    }
}
