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

            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXdecnRTRmZdVkJxWkdWYUo=");

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