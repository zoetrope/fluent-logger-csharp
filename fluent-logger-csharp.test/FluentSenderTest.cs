using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Codeplex.Web;
using System.Collections.Specialized;

namespace Fluent.test
{
    [TestClass]
    public class FluentSenderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var sender = FluentSender.CreateSync("app").Result)
            {
                sender.EmitAsync("hoge", new { message = "hoge" }).Wait();
            }
            
        }

        [TestMethod]
        public void TestMethod2()
        {
            var nvlist = new NameValueCollection();

            string x = (string)nvlist.ParseValue("x") ?? "hoge";

            x.Is("hoge");
        }
    }
}
