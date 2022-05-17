//#define AzureCosmosDB
#define AzureStorageAccount

using AspNetCore.Identity.Stores;
using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace SampleWebApplication
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
            //Adds data protection services, needed to protect the identity personal data
            services.AddDataProtection();

#if AzureStorageAccount
            //Configure identity repository connection
            services.Configure<IdentityStoresOptions>(options => options
                .UseAzureStorageAccount(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddAzureStorageAccountStores(); //Add Identity stores
#elif AzureCosmosDB
            //Configure identity repository connection
            services.Configure<IdentityStoresOptions>(options => options
                .UseAzureCosmosDB(Configuration.GetConnectionString("DefaultConnection"), "MyDatabase"));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddAzureCosmosDbStores(); //Add Identity stores
#endif


            //It's recommended to implement ILookupNormalizer to protect normalized user data
            services.AddScoped<ILookupNormalizer, LookupNormalizer>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // Seed identity
            app.UseIdentitySeeding<IdentityUser, IdentityRole>(i =>
            {
                i.AddRole(new IdentityRole("Admin"))
                    .AddRole(new IdentityRole("User"))
                    .AddUser(new() { Email = "admin@sample.com", UserName = "admin@sample.com" }, "adminP@ssw0rd!", roles: new IdentityRole("Admin"))
                    .AddUser(new() { Email = "user@sample.com", UserName = "user@sample.com" }, "userP@ssw0rd!", roles: new IdentityRole("User"));

                //Note: Username should be provided as its a required field in identity framework, and password should meet identity password requirements
            });
        }
    }
}
