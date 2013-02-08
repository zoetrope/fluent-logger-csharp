using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
    }
}
