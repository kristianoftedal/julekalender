using ChristmasCalendar.Data;
using ChristmasCalendar.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChristmasCalendar.Data.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton(new DbConnectionFactory(connectionString));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddAuthentication()
    .AddFacebook(o =>
    {
        o.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        o.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    })
    .AddGoogle(o =>
    {
        o.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        o.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        o.ClaimActions.Clear();
        o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        o.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        o.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
        o.ClaimActions.MapJsonKey("urn:google:profile", "link");
        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    });

builder.Services.AddRazorPages(o =>
{
    o.Conventions.AuthorizePage("/Doors/Today");
    o.Conventions.AuthorizePage("/Account/Manage");
    o.Conventions.AllowAnonymousToPage("/Doors/PreviousDoors");
    o.Conventions.AllowAnonymousToPage("/Highscore/Highscore");
});

builder.Services.AddScoped<IDatabaseQueries, DatabaseQueries>();
builder.Services.AddScoped<IDatabasePersister, DatabasePersister>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.Run();
