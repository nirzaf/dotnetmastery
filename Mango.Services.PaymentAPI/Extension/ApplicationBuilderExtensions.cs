using System;
using Mango.Services.PaymentAPI.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mango.Services.PaymentAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            try
            {
                ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
                var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
                hostApplicationLife.ApplicationStarted.Register(OnStart);
                hostApplicationLife.ApplicationStopped.Register(OnStop);
                return app;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}