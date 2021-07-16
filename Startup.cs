using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Repository;
using GraysPavers_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace GraysPavers
{
    using Microsoft.EntityFrameworkCore;

    using GraysPavers_DataAccess.Data;

    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            


            //services.AddTransient<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            // http context lets us access session, then we add session and configure
            //whatever options you want such as idle timeout, cookie http only and cookie required
            //along with any other options you may want
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAppTypeRepository, AppTypeRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailsRepository, InquiryDetailsRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

            #region Configuring DbContext Services/Creating Initial DB Cont'd


            /*dont have to understand all this just yet.
             basically services.adddbcontext to add a service, pass it our entity, then use a pointer
            to access useSQLserver method, and in that method supply the parameter of Configuration.GetConnectionString
            and pass that the name of your connection string*/

            #endregion
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
