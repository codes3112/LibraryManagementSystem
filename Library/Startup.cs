using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using LibraryData.Models;
using LibraryServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Library
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            

            services.AddSingleton(Configuration);
            //the following injects the libraryAssetService in the catalog and requesting the libraryAsset Interface
            services.AddScoped<ILibraryAsset, LibraryAssetService>();
            services.AddScoped<ICheckout, CheckoutService>();
            services.AddScoped<IPatron, PatronService>();
            services.AddScoped<ILibraryBranch, LibraryBranchService>();



            //add dbcontext
            services.AddDbContext<LibraryContext>(options
             => options.UseMySql(Configuration.GetConnectionString("LibraryConnection")));


            services.AddDbContext<ApplicationDbContext>(options
             => options.UseMySql(Configuration.GetConnectionString("LibraryConnection"),
             optionsBuilders=>
             optionsBuilders.MigrationsAssembly("LibraryData")));

            //services.AddDbContext<LibraryContext>(options
            // => options.UseSqlServer(Configuration.GetConnectionString("LibraryConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(
                 options => options.Stores.MaxLengthForKeys = 128)
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultUI()
                 .AddDefaultTokenProviders();
            //Add identity
            //services.AddIdentity<ApplicationUser, ApplicationRole>
            //(options =>
            //{
            //    options.Password.RequireDigit = false;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequiredUniqueChars = 0;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //})
            //    .AddEntityFrameworkStores<IdentityDbContext>()
            //    .AddDefaultTokenProviders();

            //adding policy claims
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                    policy =>
                    policy.RequireRole("Admin"));
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)

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
            //Add identity
            app.UseAuthentication();
            app.UseCookiePolicy();

            


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            SeedData.Initialize(context, userManager, roleManager).Wait();

        }
    }
}
