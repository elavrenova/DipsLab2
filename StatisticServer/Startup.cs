using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StatisticServer.EventBus;
using StatisticServer.RabbitMQTools;
using Microsoft.EntityFrameworkCore;
using StatisticServer.EventsHandlers;

namespace StatisticServer
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
            services.AddMvc();
            services.AddLogging(ops => ops.SetMinimumLevel(LogLevel.Critical));
            services.AddDbContext<ApplicationDbContext>(ops =>
                ops.UseInMemoryDatabase("Statistics"));
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<EventsStorage>();
            services.AddSingleton<RabbitMQEventBus, RabbitMQEventBus>();
            services.AddSingleton<IEventsHandler, LoginHandler>();
            services.AddSingleton<IEventsHandler, RequestEventHandler>();
            services.AddSingleton<IEventsHandler, AddOrderEventHandler>();
            services.AddSingleton<IEventsHandler, GetInfoEventHandler>();
            services.AddSingleton<IEventsHandler, GetStocksEventHandler>();
            services.AddSingleton<IEventsHandler, GetTransfersEventHandler>();
            services.AddTransient<DBProxy>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.ApplicationServices.GetServices<IEventsHandler>();
            app.UseMvc();
        }
    }
}
