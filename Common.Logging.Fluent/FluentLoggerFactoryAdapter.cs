using Common.Logging.Simple;
using Fluent;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Web;

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

            string tag = (string)properties.ParseValue("tag") ?? "Fluent";
            string hostname = (string)properties.ParseValue("hostname") ?? "localhost";
            int port = properties.ParseValueOrDefault("port", 22434);
            int bufmax = properties.ParseValueOrDefault("bufmax", 1024 * 1024);
            int timeout = properties.ParseValueOrDefault("timeout", 3000);
            bool verbose = properties.ParseValueOrDefault("verbose", false);

            _sender = FluentSender.CreateSync(tag, hostname, port, bufmax, timeout, verbose).Result;

        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new FluentLogger(_sender, name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }
}
