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

// Seed identity
app.UseIdentitySeeding<IdentityUser, IdentityRole>(seeds =>
{
    seeds
        .AddRole(role: new IdentityRole("Admin"))
        .AddRole(role: new IdentityRole("User"))
        .AddUser(user: new() { Email = "admin@sample.com", UserName = "admin@sample.com", EmailConfirmed = true }, password: "adminP@ssw0rd!", roles: new IdentityRole("Admin"))
        .AddUser(user: new() { Email = "user@sample.com", UserName = "user@sample.com", EmailConfirmed = true }, password: "userP@ssw0rd!", roles: new IdentityRole("User"));

    //Note: Username should be provided as its a required field in identity framework and email should be marked as confirmed to allow login, also password should meet identity password requirements
});

app.Run();
