using Microsoft.EntityFrameworkCore;
using University_Admissions_Scoring_Engine.Data;
using University_Admissions_Scoring_Engine.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// SERVICES
// ==========================

builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Nasz silnik liczenia punktów
builder.Services.AddScoped<AdmissionScoringService>();

var app = builder.Build();

// ==========================
// DB INIT (migracje + seed)
// ==========================

await DbInitializer.InitializeAsync(app.Services);

// ==========================
// PIPELINE
// ==========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();