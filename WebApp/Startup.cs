using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace WebApp
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
            services.AddCors();

            services.AddDbContext<ShortcutUrlApp.Data.ShortcutUrlContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            var shortUrlRoutingConstraint = new RegexNamedGroupRoutingConstraint(@"\b[a-z,A-Z,0-9]{6}\b$");

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Urls}/{action=Index}/{id?}");


                routes.MapRoute(
                name: "nav-by-codes",
                template: "{*url}",
                constraints: new { url = shortUrlRoutingConstraint },
                defaults: new { controller = "Urls", action = "ConversionCount" });
            });
        }


        public class RegexNamedGroupRoutingConstraint : IRouteConstraint
        {
            private readonly List<Regex> regexes = new List<Regex>();

            public RegexNamedGroupRoutingConstraint(params string[] regexes)
            {
                foreach (var regex in regexes)
                {
                    this.regexes.Add(new Regex(regex));
                }
            }

            public bool Match(HttpContext httpContext,
                              IRouter route,
                              string routeKey,
                              RouteValueDictionary values,
                              RouteDirection routeDirection)
            {
                if (values[routeKey] == null)
                {
                    return false;
                }

                var url = values[routeKey].ToString();

                foreach (var regex in regexes)
                {
                    var match = regex.Match(url);
                    if (match.Success)
                    {
                        foreach (Group group in match.Groups)
                        {
                            values.Add(group.Name, group.Value);
                        }

                        return true;
                    }
                }

                return false;
            }
        }


    }
}
