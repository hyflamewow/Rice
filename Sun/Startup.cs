using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog.Config;
using Sun.DB;

namespace Sun
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    // #維持屬性名稱大小寫
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                    );
                });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });
            ConfigFiles configFiles = Configuration.GetSection("ConfigFiles").Get<ConfigFiles>();
            // #目前不考慮整合ASP.NET的Log機制, 因為看不出優點, 單純用NLog就夠了。
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(configFiles.NLogConfig);
            var dbConfig = JsonConvert.DeserializeObject<Dictionary<string, DBHelperConfig>>(File.ReadAllText(configFiles.DBConfig));
            // #將DBHelper放入DI
            services.AddSingleton<DBHelper>(new DBHelper(dbConfig));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseMvc();
            app.UseCors("CorsPolicy");
            // app.UseDefaultFiles();
            // app.UseStaticFiles();
            // app.Run(async (context) =>
            // {
            //     if (!Path.HasExtension(context.Request.Path.Value))
            //     {
            //         await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            //     }
            // });
        }
    }
}
