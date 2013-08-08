using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Codeplex.Web;
using System.Collections.Specialized;
using Common.Logging.Fluent;
using Common.Logging;
using MsgPack;

namespace Fluent.Test
{
    [TestClass]
    public class FluentSenderTest
    {
        [TestMethod]
        public void SendMessage()
        {
            var server = new DummyServer();
            var task = server.Run(24224);
            var date = MessagePackConvert.ToDateTime(1360370224238);

            using (var sender = FluentSender.CreateSync("app").Result)
            {
                sender.EmitWithTimeAsync("hoge", date, new { message = "hoge" });
            }
            var ret = task.Result;

            Utils.Unpack(ret).Is(@"[ ""app.hoge"", [ [ 1360370224.238, { ""message"" : ""hoge"" } ] ] ]");
        }

    }
}
