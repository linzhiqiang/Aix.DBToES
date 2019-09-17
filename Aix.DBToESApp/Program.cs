using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Aix.DBToESApp
{
    //Microsoft.Extensions.Hosting
    //Microsoft.Extensions.Configuration.EnvironmentVariables
    //Microsoft.Extensions.Configuration.Json
    //Microsoft.Extensions.Logging.Console


    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                   .ConfigureHostConfiguration(builder =>
                   {
                   })
                    .ConfigureAppConfiguration((hostContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                    })
                   .ConfigureLogging((context, factory) =>
                   {
                       factory.AddConsole();
                       factory.SetMinimumLevel(LogLevel.Trace);

                   })
                   .ConfigureServices(Startup.ConfigureServices);

            host.RunConsoleAsync().Wait();
            Console.WriteLine("服务已退出");
        }
    }
}
