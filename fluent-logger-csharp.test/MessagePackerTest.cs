using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MsgPack;
using System.Collections.Generic;
using System.Dynamic;

namespace Fluent.Test
{
    [TestClass]
    public class MessagePackerTest
    {
        private MessagePacker _packer;

        [TestInitialize]
        public void Initialize()
        {
            _packer = new MessagePacker("test");
        }

        [TestMethod]
        public void PackSimpleAnonymousObject()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var pack = _packer.MakePacket("test1", date, new { Message = "hoge" });
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"" } ]");
        }
        [TestMethod]
        public void PackSimpleDictionary()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var msg = new Dictionary<string, object>() { { "Message","hoge"} };
            var pack = _packer.MakePacket("test1", date, msg);

            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"" } ]");
        }
        [TestMethod]
        public void PackSimpleDynamicObject()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            dynamic msg = new ExpandoObject();
            msg.Message = "hoge";
            var pack = _packer.MakePacket("test1", date, msg);
            ((string)Utils.Unpack(pack)).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"" } ]");
        }

    }
}
