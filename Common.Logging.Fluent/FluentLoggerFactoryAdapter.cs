using Common.Logging.Simple;
using Fluent;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.Fluent
{
    public sealed class FluentLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        private FluentSender _sender;

        public FluentLoggerFactoryAdapter()
            : base(null)
        {
        }

        public FluentLoggerFactoryAdapter(NameValueCollection properties)
            : base(properties)
        {

            var tag = properties["tag"];
            var hostname = properties["hostname"];
            var port = int.Parse(properties["port"]);
            var bufmax = int.Parse(properties["bufmax"]);
            var timeout = int.Parse(properties["timeout"]);
            var verbose = bool.Parse(properties["verbose"]);

            _sender = FluentSender.CreateSync(tag, hostname, port, bufmax,timeout, verbose).Result;

        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new FluentLogger(_sender, name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }
}
