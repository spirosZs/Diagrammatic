using Exercises.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Exercises.Data.DbContext;
using Exercises.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Exercises.Core.Hubs;

namespace Exercises
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
            services.AddHttpContextAccessor();
            services.AddServices();
            services.AddSwaggerGen(c => c.Init());

            services.AddDbContext<ExercisesContext>(options =>
            {
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                options.UseSqlServer(
                    Configuration.GetConnectionString("ExercisesContext"),
                    // Configuration.GetConnectionString("ExercisesContextOld"),
                    sql => sql.MigrationsAssembly(migrationsAssembly)
                );
            });

            services
                .AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ExercisesContext>();

            services.AddAuthSettings(Configuration);

            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddNewtonsoftJson();

            services
                .AddControllers();

            services.AddRazorPages();
            services.AddSignalR();

            Mapper.Initialize(cfg => cfg.Init());

            //var mapperConfiguration = new MapperConfiguration(cfg => cfg.Init());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = JsonExceptionMiddleware.Invoke
            });

            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjgyMDY3OEAzMTM3MmUzMTJlMzBIN2Jsc1orbk55eXBzSFp2Z1NqRUZrbmtyc0oyeUdNZGR6aitJRTlmQnc0PQ==;MjgyMDY3OUAzMTM3MmUzMTJlMzBuL2NxVWJOV3VpSlVZMWlpU1BQYkhCcUltRysvWDZUeXdOZXhMRVJER3JNPQ==;MjgyMDY4MEAzMTM3MmUzMTJlMzBRUWo4b2FIUVArdUsxTUh1dmFTdWZXOENHamgxeXhKRFQ4VFFqdW9BdDVNPQ==;MjgyMDY4MUAzMTM3MmUzMTJlMzBWR0tCVmJYb0VkbHZsd1hSV1VyU2FMQzBZZ05BZnYybTVnQk9IdG9qb2hrPQ==;MjgyMDY4MkAzMTM3MmUzMTJlMzBMOFExYVhCNGloTEFoWnB3enY0Y2R0enAvRUh6ZDI2eU16ZDh1Y3RNbXFjPQ==;MjgyMDY4M0AzMTM3MmUzMTJlMzBnN0FYdjQ5amVsOWJncERBTjVXRFg0a0FhRGpzZ1JVQ0JPdVROdkpTLzJFPQ==;MjgyMDY4NEAzMTM3MmUzMTJlMzBFK0YwVEIzeXFOdDNyVU04NlQ0UTJYemVVdmQzdENCcUVFdVRvTkdOU2QwPQ==;MjgyMDY4NUAzMTM3MmUzMTJlMzBISk9vNWF1b0JRRmNTWDEwSDhhejlQTmF5MHVIZkFCREE0TG1helhjdHBVPQ==;MjgyMDY4NkAzMTM3MmUzMTJlMzBRWlJQbzg2VTJSL25mK3lhc1E4c3N5ZjVRZFlGVVRPWXZncVdIV2R3Y3hzPQ==;MjgyMDY4N0AzMTM3MmUzMTJlMzBnamlBWjJiS2RoWmw2SU02UVMyTHEzSHFzM282K2hGcWtlTlRONjBNTzRnPQ==");

            app.UseStaticFiles();
            // app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gameHub");
            });

            app.UseSwaggerDocumentation();
        }
    }
}