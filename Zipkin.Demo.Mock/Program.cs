using Microsoft.Extensions.DependencyInjection;
using SEB.Logging;
using SEB.Logging.DotnetCore;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace zipkin
{
  class Program
  {
    static void Main(string[] args)
    {
      var log = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Information)
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        {
          AutoRegisterTemplate = true,
          AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
          IndexFormat = "test3-{0:yyyy.MM.dd}"
        })
        .CreateLogger();
      Log.Logger = log;
      log.Information("A Serilog log");
      var serviceCollection = new ServiceCollection();
      serviceCollection
        .AddLogging()
        .AddTransient<ILogService, LogService>();
      var serviceProvider = serviceCollection.BuildServiceProvider();
      var logger = serviceProvider.GetService<ILogService>();
      logger.WriteErrorLog("A Microsoft log");
      Log.CloseAndFlush();
    }
  }
}
