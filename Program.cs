var builder = WebApplication.CreateBuilder(args);

// MVC servislerini ekle
builder.Services.AddControllersWithViews();

// --- Gerekli Servisler ---
// 1. Session: Kullanıcı oturum verilerini saklamak için
builder.Services.AddSession();

// 2. HttpContextAccessor: Razor'dan Session'a erişim (@inject) için
builder.Services.AddHttpContextAccessor(); 

var app = builder.Build();

// HTTP pipeline yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Hata sayfası
    app.UseHsts(); // Güvenli bağlantı zorunluluğu
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // CSS/JS dosyalarını sunmak için

app.UseRouting();

// 3. Session Middleware: Oturum verilerini okumak için
app.UseSession(); 

app.UseAuthorization();

// 4. Varsayılan rota: Proje Login ekranı ile başlar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Kullanici}/{action=Login}/{id?}");

app.Run();
