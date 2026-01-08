using Microsoft.EntityFrameworkCore;
using UcakBiletiRezervasyonSistemi.Data;
using UcakBiletiRezervasyonSistemi.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC servislerini ekle
builder.Services.AddControllersWithViews();

// 1. Veritabanı Bağlantısı (MySQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Gerekli Servis ve Repository Kayıtları (DI)
builder.Services.AddScoped<UcusRepository>();
builder.Services.AddScoped<KullaniciRepository>();
builder.Services.AddScoped<RezervasyonRepository>();
builder.Services.AddScoped<FiyatlandirmaServisi>();

// 3. Session Ayarları
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor(); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Session middleware'i eklenmeli
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Kullanici}/{action=Login}/{id?}");

app.Run();