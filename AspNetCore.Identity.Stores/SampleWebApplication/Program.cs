#define AzureCosmosDB
//#define AzureStorageAccount

using AspNetCore.Identity.Stores;
using AspNetCore.Identity.Stores.AzureCosmosDB.Extensions;
using AspNetCore.Identity.Stores.AzureStorageAccount.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

#if AzureStorageAccount
//Configure identity repository connection
builder.Services.Configure<IdentityStoresOptions>(options => options
    .UseAzureStorageAccount(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddAzureStorageAccountStores(); //Add Identity stores
#elif AzureCosmosDB
//Configure identity repository connection
builder.Services.Configure<IdentityStoresOptions>(options => options
                .UseAzureCosmosDB(connectionString, databaseId: "MyDatabase"));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddAzureCosmosDbStores(); //Add Identity stores
#endif

//It's recommended to implement ILookupNormalizer to protect normalized user data
//builder.Services.AddScoped<ILookupNormalizer, LookupNormalizer>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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

app.MapRazorPages();

app.UseIdentitySeeding<IdentityUser, IdentityRole>(seeds =>
{
    seeds.AddUser(new("user@a.com") {Email= "user@a.com", EmailConfirmed = true }, "P@ssw0rd!");
    seeds.AddUser(new("admin1@a.com") { Email = "admin1@a.com", EmailConfirmed = true }, "P@ssw0rd!",new IdentityRole("Admin"));
    seeds.AddUser(new("admin2@a.com") { Email = "admin2@a.com", EmailConfirmed = true }, "P@ssw0rd!", new System.Security.Claims.Claim("AdminClaim", "true"));
    seeds.AddRole(new("Sales"), new System.Security.Claims.Claim("SalesManager", "true"));
    seeds.AddRole(new("Guest"));
});

app.Run();
