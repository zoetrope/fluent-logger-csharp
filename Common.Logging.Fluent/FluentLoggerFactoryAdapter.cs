using Common.Logging.Simple;
using Fluent;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeplex.Web;
using System.Threading;
using System.Collections.Concurrent;

namespace Common.Logging.Fluent
{
    public sealed class FluentLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter, IDisposable
    {
        private FluentSender _sender;

        private BlockingCollection<Tuple<string, object>> _queue;
        private CancellationTokenSource _cancellation;

        public FluentLoggerFactoryAdapter()
            : base(null)
        {
        }

        public void Dispose()
        {
            _cancellation.Cancel();
        }

        public FluentLoggerFactoryAdapter(NameValueCollection properties)
            : base(properties)
        {

            string tag = (string)properties.ParseValue("tag") ?? "Fluent";
            string hostname = (string)properties.ParseValue("hostname") ?? "localhost";
            int port = properties.ParseValueOrDefault("port", 24224);
            int bufmax = properties.ParseValueOrDefault("bufmax", 1024 * 1024);
            int timeout = properties.ParseValueOrDefault("timeout", 3000);
            bool verbose = properties.ParseValueOrDefault("verbose", false);

            _sender = new FluentSender(tag, hostname, port, bufmax, timeout, verbose);

            _queue = new BlockingCollection<Tuple<string, object>>(new ConcurrentQueue<Tuple<string, object>>());
            _cancellation = new CancellationTokenSource();


            Task.Factory.StartNew(WriteLog, _cancellation);
        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new FluentLogger(_queue, name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }

        private void WriteLog(object obj)
        {
            while (true)
            {
                _cancellation.Token.ThrowIfCancellationRequested();

                var records = new List<Tuple<string,object>>();
                var item = _queue.Take(_cancellation.Token);

                records.Add(item);

                Tuple<string,object> record;
                while (_queue.TryTake(out record))
                {
                    records.Add(record);
                }

                var groupedRecords = records.GroupBy(x => x.Item1, x => x.Item2);

                foreach (var r in groupedRecords)
                {
                    _sender.EmitAsync(r.Key, r.ToArray()).Wait(_cancellation.Token);
                }
            }
        }
    }
}
