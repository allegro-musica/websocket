using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using Quizzer.Domain.Events;
using Web.Socket.Consumers;
using Web.Socket.Hubs;
using Web.Socket.Services;

namespace Web.Socket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient<QuizService>();
            services.AddSignalR(hub =>
            {
                hub.KeepAliveInterval = TimeSpan.FromHours(1);
                hub.EnableDetailedErrors = true;
            });
            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetEntryAssembly());

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetValue<string>("RabbitMQConnection", "rabbitmq://localhost"));
                    
                    cfg.ConfigureEndpoints(context);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            var massTransit = provider.GetRequiredService<IBusControl>();

            massTransit.StartAsync();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<QuizzerGateway>("/gateway");
            });
        }
    }
}
