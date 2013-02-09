using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Codeplex.Web;
using System.Collections.Specialized;
using Common.Logging.Fluent;
using Common.Logging;

namespace Fluent.Test
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


        [TestMethod]
        public void TestMethod3()
        {
            var properties = new NameValueCollection();
            LogManager.Adapter = new FluentLoggerFactoryAdapter(properties);

            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("test");
            
        }
    }
}
