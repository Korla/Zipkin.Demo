using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        {
          AutoRegisterTemplate = true,
          AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
          IndexFormat = "zipkin-{0:yyyy.MM.dd}"
        })
        .CreateLogger();
      Log.Logger = log;
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddLogging(c => c.AddSerilog());
      var serviceProvider = serviceCollection.BuildServiceProvider();
      var logger = serviceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();
      using (logger.BeginScope("{@scope}", new { value1 = "A value", value2 = "Another value" }))
      {
        logger.LogInformation("A Microsoft log");
      }
      Log.CloseAndFlush();
    }
  }
}
