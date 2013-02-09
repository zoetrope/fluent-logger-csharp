
# fluent-logger-csharp / Common.Logging.Fluent

## Requirements

.NET Framework 4.5

## Dependencies

* [MessagePack for CLI](http://cli.msgpack.org/)
* [Common.Logging](http://netcommon.sourceforge.net/)
* [ImplicitQueryString](http://implicitquerystring.codeplex.com/)
* [Chaining Assertion](http://chainingassertion.codeplex.com/) (for test)



## Installation

fluent-logger-csharp

    PM> Install-Package fluent-logger-csharp

Common.Logging.Fluent

    PM> Install-Package Common.Logging.Fluent

## Usage

### fluent-logger-csharp

using

    using Fluent;


usage

    using (var sender = FluentSender.CreateSync("app").Result)
    {
        sender.EmitAsync("hoge", new { message = "hoge" }).Wait();
    }

### Common.Logging.Fluent

using

    using Common.Logging;
    using Common.Logging.Fluent;

usage

    var properties = new NameValueCollection();
    LogManager.Adapter = new FluentLoggerFactoryAdapter(properties);

    var logger = LogManager.GetCurrentClassLogger();
    logger.Debug("test");



## Configuration

Fluentd daemon must be lauched with the following configuration:

    <source>
      type forward
      port 24224
    </source>

    <match app.**>
      type stdout
    </match>


app.config

    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <configSections>
        <sectionGroup name="common">
          <section name="logging" type="Common.Logging.ConfigurationSectionHandler, Common.Logging" />
        </sectionGroup>
      </configSections>
      <common>
        <logging>
          <factoryAdapter type="Common.Logging.Fluent.FluentLoggerFactoryAdapter, Common.Logging.Fluent">
            <arg key="level" value="DEBUG" />
            <arg key="showLogName" value="true" />
            <arg key="showDataTime" value="true" />
            <arg key="dateTimeFormat" value="yyyy/MM/dd HH:mm:ss:fff" />
          </factoryAdapter>
        </logging>
      </common>
    </configuration>

## License

[New BSD License](https://github.com/zoetrope/fluent-logger-csharp/blob/master/License.txt)


