﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using ProductCatalog.Models;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Runtime;

namespace ProductCatalog
{
    public class Startup
    {

        public IConfiguration Configuration { get; private set; }
        public Startup(IApplicationEnvironment env)
        {
            var builder = new ConfigurationBuilder(env.ApplicationBasePath)
                        .AddJsonFile("config.json")
                        .AddEnvironmentVariables(); //All environment variables in the process's context flow in as configuration values.

            Configuration = builder.Build();

        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();

            services.AddScoped<IConfigurationElasticClientSettings>(s =>
            {
                return new ConfigurationElasticClientSettings(Configuration.GetConfigurationSection("ElasticSearch"));
            });
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");

            //Add Sample Data to Elastic Search
            var service = app.ApplicationServices.GetService<IConfigurationElasticClientSettings>();
           

            var helper = new ElasticSearchDataHelper(service);
            helper.AddTestData().Wait();

        }
    }
}
