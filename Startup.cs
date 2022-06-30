using ExternalAPIs.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace encryptionApis
{
    public class Startup
    {
        private readonly string corsePolicy = "allowAll";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy(corsePolicy, builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyOrigin().AllowAnyHeader().WithMethods("PUT", "DELETE", "GET", "POST");
                });
            });

            string sqlConnection = Configuration["ConnectionStrings:DefaultConnection"];
           // services.AddMvc(options =>
           // {
           //     options.RespectBrowserAcceptHeader = true; // false by default
           //     //options.InputFormatters.Insert(0,new XDocumentInputFormatter());
           // }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
           //.AddXmlSerializerFormatters()
           //.AddXmlDataContractSerializerFormatters();
           
            services.AddControllers().AddNewtonsoftJson();
            //services.AddControllers().AddXmlSerializerFormatters();
            services.AddSingleton<IOracleServerConnectionProvider>(new OracleServerConnectionProvider(sqlConnection));
            
            services.AddHttpContextAccessor();
            services.AddMvc()
           .AddXmlSerializerFormatters()
           .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
           .AddXmlDataContractSerializerFormatters();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(corsePolicy);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }

            app.UseRouting();
            // app.UseAuthentication();
            // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
