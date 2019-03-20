# Study.Microservices
一步一步，由浅入深，学习如何使用.net core搭建微服务框架。
<br />
## 第一步 使用dotnetty完成rpc
<br />

**启动服务端**

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

**启动客户端**

```
 var builder = new HostBuilder()
                .AddClientProxy()
                .AddRpcClient()
                .ConfigureLogging((context, logger) =>
                {
                    logger.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logger.AddConsole();
                });
                
            using (var host=builder.UseConsoleLifetime().Build())
            {
                host.Start();

                var provider = host.Services;
                var proxyGenerater = provider.GetService<IServiceProxyGenerater>();
                var remoteServices = proxyGenerater.GenerateProxys(new[] { typeof(IUserService) }).ToArray();
                var proxyFactory = provider.GetService<IServiceProxyFactory>();
                var userService = proxyFactory.CreateProxy<IUserService>(remoteServices.Single(typeof(IUserService).GetTypeInfo().IsAssignableFrom));

                Stopwatch sw = Stopwatch.StartNew();

                for (var i = 0; i < 100; i++)
                {
                    Stopwatch watch = Stopwatch.StartNew();
                    var result = userService.GetUserNameAsync(i).Result;
                    Console.WriteLine(result);
                    watch.Stop();

                    Console.WriteLine($"{watch.ElapsedMilliseconds}");
                }
                sw.Stop();
                Console.WriteLine($"调用结束,耗时：{sw.ElapsedMilliseconds} 毫秒");

                host.WaitForShutdown();
            }

```
