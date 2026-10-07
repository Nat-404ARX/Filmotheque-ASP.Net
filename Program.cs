using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Filmothèque.Data;
using Filmothèque.Services;
using Stripe;

DotNetEnv.Env.Load();


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' n'a pas été trouvée.");

var stripeSecretKey = Environment.GetEnvironmentVariable("KeyStripe")
                      ?? builder.Configuration["Stripe:SecretKey"];

Stripe.StripeConfiguration.ApiKey = stripeSecretKey;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; 
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole>() 
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IFilmService, FilmService>();
builder.Services.AddHttpClient<ITmdbService, TmdbService>();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Films}/{action=Index}/{id?}");

app.MapRazorPages(); 


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Une erreur est survenue lors du seeding de la BDD.");
    }
}

app.Run();
