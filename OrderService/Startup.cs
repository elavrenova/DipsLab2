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
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Data.SqlClient;
using Gateway.Authorisation;

namespace OrderService
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
            services.AddDbContext<OrderContext>(opt =>
                //opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                opt.UseInMemoryDatabase("Orders"));
            services.AddMvc();
            services.AddSingleton<TokensStore>();
            //services.BuildServiceProvider().GetRequiredService<OrderContext>().Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ServiceAuthorizationMiddleWare>();
            app.UseMvc();
        }
    }
}
