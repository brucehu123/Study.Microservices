# Study.Microservices
一步一步，由浅入深，学习如何使用.net core搭建微服务框架。
<br />
- 第一步 完成rpc
<br />
** 启动服务端 **
```
 var host = new HostBuilder()
                .AddRpcServer()
                .UseDotNettyServer()
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
```
