using System;
using Microsoft.EntityFrameworkCore;
using AirlineSendAgent.App;
using AirlineSendAgent.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AirlineSendAgent.Client;
using System.IO;

namespace AirlineSendAgent
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets(typeof(Program).Assembly, optional:true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            var config = builder.Build();
            
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>{
                  services.AddSingleton<IAppHost, AppHost>();
                  services.AddSingleton<IWebhookClient, WebhookClient>();
                  services.AddDbContext<SendAgentDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("AirlineConnection")));
                  //(config.GetConnectionString("AirlineConnection")));
                                                                
                  
                    
                  services.AddHttpClient();
                }).Build();

            host.Services.GetService<IAppHost>().Run();
        }
    }
}
