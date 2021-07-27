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
using Braintree;
using GraysPavers.Hubs;
using GraysPavers_DataAccess.Data;
using GraysPavers_DataAccess.Initializer;
using GraysPavers_DataAccess.Repository;
using GraysPavers_DataAccess.Repository.IRepository;
using GraysPavers_Utility;
using GraysPavers_Utility.BrainTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Environment = Braintree.Environment;
using Stripe;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Builder;

[assembly: OwinStartup(typeof(GraysPavers.Startup))]

namespace GraysPavers
{

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
            var config = _configuration.GetSection("BrainTree");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddSignalR();


            services.AddTransient<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddOptions();
            services.Configure<BrainTreeSettings>(config);
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
            
            // http context lets us access session, then we add session and configure
            //whatever options you want such as idle timeout, cookie http only and cookie required
            //along with any other options you may want
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAppTypeRepository, AppTypeRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailsRepository, InquiryDetailsRepository>();
            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddAuthentication().AddFacebook(opt =>
            {
                opt.AppId = "862755924337469";
                opt.AppSecret = "10c6997905c74c856d052f877396cba7";
            });


            services.AddAuthentication().AddGoogle(opt =>
            {
                opt.ClientId = "332666555749-g17p96mjq4il6kb4aq92d5caa88ms71e.apps.googleusercontent.com";
                opt.ClientSecret = "eqWMBQQyhiEvqkpkVpDls-HY";
            });


            #region Configuring DbContext Services/Creating Initial DB Cont'd


            /*dont have to understand all this just yet.
             basically services.adddbcontext to add a service, pass it our entity, then use a pointer
            to access useSQLserver method, and in that method supply the parameter of Configuration.GetConnectionString
            and pass that the name of your connection string*/

            #endregion
            services.AddControllersWithViews();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            //app.MapSignalR();
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
            dbInitializer.Initialize();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
