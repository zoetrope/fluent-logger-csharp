# fluent-logger-csharp / Common.Logging.Fluent

fluent-logger-csharp is a structured logger for Fluentd (C#).

Common.Logging.Fluent is a Common.Logging library bindings for fluent-logger-csharp.


## Requirements

.NET Framework 4.5

## Dependencies

* [MessagePack for CLI](http://cli.msgpack.org/)
* [Common.Logging](http://netcommon.sourceforge.net/)
* [ImplicitQueryString](http://implicitquerystring.codeplex.com/)
* [Chaining Assertion](http://chainingassertion.codeplex.com/) (for test)



## Installation (in the NuGet Package Manager Console)

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
        sender.EmitAsync("hoge", new { message = "test" }).Wait();
        //Output: [ app.hoge, 2013-02-19T23:01:00+09:00, { "message" : "test" } ]
    }

### Common.Logging.Fluent

using

    using Common.Logging;
    using Common.Logging.Fluent;

usage

    var properties = new NameValueCollection();
    properties["tag"] = "app";
    properties["hostname"] = "localhost";
    properties["port"] = "24224";
    LogManager.Adapter = new FluentLoggerFactoryAdapter(properties);

    var logger = LogManager.GetCurrentClassLogger();
    
    logger.Debug("test"); //Output: [ app.TypeName, 2013-02-19T23:01:00+09:00, { "Level" : "DEBUG", "Message" : "test" } ]

### Supported types

* Primitive type, string, DateTime
* class (public property only)
* struct
* Dictionary<string, object>
* ExpandoObject (public property only)
* Anonymous type
* List<>, array

* does not support cyclic reference
* does not support null reference


## Configuration

### Configuration for Fluentd daemon

    <source>
      type forward
      port 24224
    </source>

    <match app.**>
      type stdout
    </match>


### app.config/web.config for Common.Logging.Fluent

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
            <arg key="tag" value="app" />
            <arg key="hostname" value="localhost" />
            <arg key="port" value="24224" />
            <arg key="bufmax" value="1048576" />
            <arg key="timeout" value="3000" />
            <arg key="verbose" value="false" />
            <arg key="showLevel" value="true" />
          </factoryAdapter>
        </logging>
      </common>
    </configuration>

## License

[New BSD License](https://github.com/zoetrope/fluent-logger-csharp/blob/master/License.txt)


