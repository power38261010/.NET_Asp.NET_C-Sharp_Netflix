using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetflixClone.Data;
using NetflixClone.Middleware;
using NetflixClone.Services;
using NetflixClone.Services.Contracts;
using System.Text.Json.Serialization;

namespace NetflixClone
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
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            // Configuración de serialización JSON
            services.AddControllers().AddJsonOptions(options => {
                // Desactiva la preservación de referencias para evitar $id y $values
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
                // MercadoPagoConfigs.Initialize(Configuration);
            });

            services.AddMemoryCache();

            services.AddScoped<ISubscriptionService,SubscriptionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IPayService, PayService>();

            services.AddAuthorization(options => {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin","super_admin"));
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("super_admin"));
            });

            // Configuración de CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            // Configuración de autenticación
            services.AddAuthentication();

            //MP
            // MercadoPagoConfigs.Initialize(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Aplicar la configuración de CORS
            app.UseCors("AllowAll");

            // Middleware JWT
            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
