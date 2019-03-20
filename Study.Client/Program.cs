using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Study.Common;
using Study.Core.Attributes;
using Study.Core.Runtime.Client.Imp;
using Study.Core.ServiceId.Imp;
using Study.ProxyGenerator;
using Study.ProxyGenerator.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Study.Core;
using IServiceProxyFactory = Study.ProxyGenerator.IServiceProxyFactory;
using IServiceProxyGenerater = Study.ProxyGenerator.IServiceProxyGenerater;

namespace Study.Client
{
    class Program
    {
        #region MyRegion
        ///// <summary>
        ///// 服务代理生成器。
        ///// </summary>
        //private static readonly IServiceProxyGenerater ServiceProxyGenerater;

        ///// <summary>
        ///// 加载到的程序集。
        ///// </summary>
        //private static readonly Assembly[] Assemblies;

        ///// <summary>
        ///// 所获取到的程序集文件路径。
        ///// </summary>
        //private static readonly IEnumerable<string> AssemblyFiles;

        //static Program()
        //{
        //    ServiceProxyGenerater = new ServiceProxyGenerater(new ServiceIdGenerator(), new LoggerFactory().CreateLogger<ServiceProxyGenerater>());
        //    AssemblyFiles =
        //      Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "assemblies"), "*.dll").ToArray();

        //    Assemblies = AssemblyFiles.Select(i =>
        //    {
        //        using (var stream = File.OpenRead(i))
        //        {
        //            return AssemblyLoadContext.Default.LoadFromStream(stream);
        //        }
        //    }).ToArray();
        //}

        //private static IEnumerable<SyntaxTree> GetTrees()
        //{
        //    var services = Assemblies
        //        .SelectMany(assembly => assembly.GetExportedTypes())
        //        .Where(i => i.GetTypeInfo().IsInterface && i.GetTypeInfo().GetCustomAttribute<RpcServiceBundleAttribute>() != null);
        //    return services.Select(service => ServiceProxyGenerater.GenerateProxyTree(service));
        //}

        //private static void GenerateCodeFiles()
        //{
        //    foreach (var syntaxTree in GetTrees())
        //    {
        //        var compilationUnitSyntax = (CompilationUnitSyntax)syntaxTree.GetRoot();

        //        var classDeclarationSyntax = FindClassDeclaration(compilationUnitSyntax.Members);
        //        var className = classDeclarationSyntax.Identifier.ValueText;
        //        var code = syntaxTree.ToString();
        //        var fileName = Path.Combine("D:/", $"{className}.cs");
        //        File.WriteAllText(fileName, code, Encoding.UTF8);
        //        Console.WriteLine($"Generate successful path:{fileName}");
        //    }
        //}

        //private static ClassDeclarationSyntax FindClassDeclaration(IEnumerable<MemberDeclarationSyntax> members)
        //{
        //    foreach (var member in members)
        //    {
        //        var classDeclaration = member as ClassDeclarationSyntax;
        //        if (classDeclaration != null)
        //            return classDeclaration;

        //        var namespaceDeclaration = member as NamespaceDeclarationSyntax;
        //        if (namespaceDeclaration != null)
        //            return FindClassDeclaration(namespaceDeclaration.Members);
        //    }
        //    return null;
        //}

        //private static void GenerateAssembly()
        //{
        //    using (var stream = ProxyGenerator.Utilitys.CompilationUtilitys.CompileClientProxy(GetTrees(), AssemblyFiles.Select(file => MetadataReference.CreateFromFile(file))))
        //    {
        //        var fileName = Path.Combine("D:/", "Rabbit.Rpc.ClientProxys.dll");
        //        File.WriteAllBytes(fileName, stream.ToArray());
        //        Console.WriteLine($"Generate successful path:{ fileName}");
        //    }
        //}
        #endregion

        static void Main(string[] args)
        {
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

        }



    }
}
