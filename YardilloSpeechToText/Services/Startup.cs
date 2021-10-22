using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MBADCases.Models;
using Microsoft.Extensions.Options;
using MBADCases.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace MBADCases
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
            services.AddMvc(setupAction =>
            {
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound ));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status409Conflict));
                setupAction.ReturnHttpNotAcceptable = true;
              

            });
            services.AddMvc()
                 .AddNewtonsoftJson(options =>
                options.SerializerSettings.ContractResolver =
                   new CamelCasePropertyNamesContractResolver());

            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time   
            });
            

            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1,0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });
            // requires using Microsoft.Extensions.Options
            services.Configure<CasesDatabaseSettings>(
            Configuration.GetSection(nameof(CasesDatabaseSettings)));

            services.AddSingleton<ICasesDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<CasesDatabaseSettings>>().Value);

            services.AddSingleton<CaseService>();
            services.AddSingleton<CaseTypeService>();
            services.AddSingleton<AdapterService>();
            services.AddSingleton<VaultService>();
            services.AddSingleton<TenantService>();
            services.AddSingleton<ScheduleService>();
            services.AddSingleton<SpeechtotextService>();
  
            


            //services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions);

            //services.AddAuthentication("Basic").AddScheme<AuthenticationSchemeOptions, Authentication.BasicAuthenticationHandler>("Basic", null);

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseStaticFiles();
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), "images")),
            //    RequestPath = "/images"
            //});

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
            });

            
        }
    }
}
