using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Study.Common;
using Study.Core;
using Study.Core.Consul;
using Study.Transport.DotNetty;
using System.IO;

namespace Study.Service
{
    class Program
    {
        static void Main(string[] args)
        {

            var host = new HostBuilder()
                .AddRpcRuntime()
                .AddRpcServer()
                .UseDotNettyServer()
                .UseConsul()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                 .ConfigureServices((context, services) =>
                 {
                     services.AddTransient<IUserService, UserService>();
                 })
                 .ConfigureLogging((context, logger) =>
                 {
                     logger.AddConfiguration(context.Configuration.GetSection("Logging"));
                     logger.AddConsole();
                 });

            host.RunConsoleAsync().Wait();

        }
    }
}
