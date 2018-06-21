using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace SwaggerVirtualPath
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            var virtualPath = Configuration["virtualPath"];

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.BasePath = virtualPath);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(virtualPath + "/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
