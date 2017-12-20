using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gateway.Authorisation;
using Gateway.Controllers;
using Gateway.Services;
using Gateway.Services.Implementations;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StatisticServer.EventBus;
using StatisticServer;
using StatisticServer.EventsHandlers;
using Newtonsoft.Json.Serialization;

namespace Gateway
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

            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<IStockService, StockService>();
            services.AddSingleton<ITransferService, TransferService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddTransient<AggregationController>();
            services.AddLogging(lb => lb.AddConsole());
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(o =>
                {
                    o.Authority = "http://localhost:50539";
                    o.RequireHttpsMetadata = false;
                    o.ApiName = "api";
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowAnyHeader());
            });
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;
            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<EventsStorage>();
            services.AddSingleton<AckHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors("AllowAll");
            app.UseMiddleware<GatewayAuthorizationMiddleWare>();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
