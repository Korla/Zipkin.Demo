using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using Serilog.Extensions.Logging;

namespace zipkin
{
  class Program
  {
    static void Main(string[] args)
    {
      // Configure Serilog
      var log = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        {
          AutoRegisterTemplate = true,
          AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
          IndexFormat = "zipkin-{0:yyyy.MM.dd}"
        })
        .CreateLogger();
      Log.Logger = log;

      // Configure Microsoft.Logging
      var loggerFactory = new LoggerFactory(new[] { new SerilogLoggerProvider() });
      var logger = loggerFactory.CreateLogger<Program>();

      // Run application
      using (logger.BeginScope("{@scope}", new { value1 = "A value", value2 = "Another value" }))
      {
        logger.LogInformation("A log");
      }
      Log.CloseAndFlush();
    }
  }
}
