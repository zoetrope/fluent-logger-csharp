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
            var pack = _packer.MakePacket("test1", date, new { Message = "hoge", Value = 1234 });
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"", ""Value"" : 1234 } ]");
        }
        [TestMethod]
        public void PackSimpleDictionary()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var msg = new Dictionary<string, object>() { { "Message", "hoge" }, { "Value", 1234 } };
            var pack = _packer.MakePacket("test1", date, msg);
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"", ""Value"" : 1234 } ]");
        }
        [TestMethod]
        public void PackSimpleDynamicObject()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            dynamic msg = new ExpandoObject();
            msg.Message = "hoge";
            msg.Value = 1234;
            var pack = _packer.MakePacket("test1", date, msg);
            ((string)Utils.Unpack(pack)).Is(@"[ ""test.test1"", 1360370224238, { ""Message"" : ""hoge"", ""Value"" : 1234 } ]");
        }

        [TestMethod]
        public void PackArray()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var pack = _packer.MakePacket("test1", date, new[] { "hoge", "1234" });
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, [ ""hoge"", ""1234"" ] ]");
        }
        [TestMethod]
        public void PackList()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var pack = _packer.MakePacket("test1", date, new List<string> { "hoge", "1234" });
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, [ ""hoge"", ""1234"" ] ]");
        }


        [TestMethod]
        public void PackStruct()
        {

            var date = MessagePackConvert.ToDateTime(1360370224238);
            var s = new StructData { Name = "Struct", Value = 112233445566778899};
            var pack = _packer.MakePacket("test1", date, s);
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Name"" : ""Struct"", ""Value"" : 112233445566778899 } ]");
        }

        public struct StructData
        {
            public string Name;
            public long Value;
        }

        [TestMethod]
        public void PackNestedData()
        {

            var date = MessagePackConvert.ToDateTime(1360370224238);

            var parent = new Parent()
            {
                Name = "Parent",
                Child1 = new Child { Name = "Child1", Value = 1 },
                Child2 = new Child { Name = "Child2", Value = 2 },
                Child3 = new Child { Name = "Child3", Value = 3 },
                Child4 = new Child { Name = "Child4", Value = 4 },
                Child5 = new Child { Name = "Child5", Value = 5 }
            };
            var pack = _packer.MakePacket("test1", date, parent);
            Utils.Unpack(pack).Is(@"[ ""test.test1"", 1360370224238, { ""Name"" : ""Parent"", ""Child1"" : { ""Name"" : ""Child1"", ""Value"" : 1 }, ""Child2"" : { ""Name"" : ""Child2"", ""Value"" : 2 }, ""Child3"" : { ""Name"" : ""Child3"", ""Value"" : 3 }, ""Child4"" : { ""Name"" : ""Child4"", ""Value"" : 4 }, ""Child5"" : { ""Name"" : ""Child5"", ""Value"" : 5 } } ]");
        }

        public class Parent
        {
            public string Name { get; set; }
            public Child Child1 { get; set; }
            public Child Child2 { get; set; }
            public Child Child3 { get; set; }
            public Child Child4 { get; set; }
            public Child Child5 { get; set; }
            
        }
        public class Child
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        [TestMethod]
        public void PackLoopRefType()
        {
            var date = MessagePackConvert.ToDateTime(1360370224238);
            var child = new Node { Name = "Child" };
            var node = new Node { Name = "Parent", Child = child };
            child.Child = node;

            var ex = AssertEx.Throws<InvalidOperationException>(() => _packer.MakePacket("test1", date, node));
            ex.Message.Is("nest counter is over the maximum. counter = 11");
        }

        public class Node
        {
            public Node Child { get; set; }
            public string Name { get; set; }
        }
    }
}
