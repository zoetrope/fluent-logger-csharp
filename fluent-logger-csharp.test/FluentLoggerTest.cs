using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using Common.Logging;
using Common.Logging.Fluent;

namespace Fluent.Test
{
    [TestClass]
    public class FluentLoggerTest
    {

        [TestMethod]
        public void LoggingTest()
        {
            var server = new DummyServer();
            var task = server.Run(24224);

            var properties = new NameValueCollection();
            LogManager.Adapter = new FluentLoggerFactoryAdapter(properties);
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("test");
            
            var ret = task.Result;
            Utils.Unpack(ret).Is(@"[ ""Fluent.Fluent.Test.FluentLoggerTest"", [ [ 1360421509.813, { ""Level"" : ""DEBUG"", ""Message"" : ""test"" } ] ] ]");
            
        }
    }
}
