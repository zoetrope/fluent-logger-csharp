using Common.Logging.Simple;
using Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.Fluent
{
    internal class FluentLogger : AbstractSimpleLogger
    {
        private FluentSender _sender;
        private string _typeName;

        internal FluentLogger(FluentSender sender, string typeName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
            : base(typeName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
            _sender = sender;
            _typeName = typeName;
        }

        protected override void WriteInternal(LogLevel targetLevel, object message, Exception e)
        {
            var sb = new StringBuilder();
            FormatOutput(sb, targetLevel, message, e);

            _sender.EmitAsync(_typeName, sb.ToString()).Wait();
        }
    }
}
