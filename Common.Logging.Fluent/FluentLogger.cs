using Common.Logging.Simple;
using Fluent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.Fluent
{
    internal class FluentLogger : AbstractSimpleLogger
    {
        private BlockingCollection<Tuple<string, object>> _sender;
        private string _typeName;

        internal FluentLogger(BlockingCollection<Tuple<string, object>> sender, string typeName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
            : base(typeName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
            _sender = sender;
            _typeName = typeName;
        }

        protected override void WriteInternal(LogLevel targetLevel, object message, Exception e)
        {
            object record = CreateLogRecord(targetLevel, message, e);
            _sender.TryAdd(Tuple.Create(_typeName, record));
        }


        private dynamic CreateLogRecord(LogLevel targetLevel, object message, Exception e)
        {
            dynamic record = new ExpandoObject();
            
            if (ShowLevel)
            {
                record.Level = targetLevel.ToString().ToUpper();
            }

            record.Message = message;

            if (e != null)
            {
                record.Exception = e.ToString();
            }

            return record;
        }
    }
}
