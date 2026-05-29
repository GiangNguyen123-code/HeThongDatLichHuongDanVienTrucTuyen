using Microsoft.EntityFrameworkCore;
using HeThongDatLich.Data;
var builder = WebApplication.CreateBuilder(args);

// 1. Lấy chuỗi kết nối từ file appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký AppDbContext vào Container của ứng dụng
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
// Thêm dịch vụ lưu cache bộ nhớ (bắt buộc phải có để chạy Session)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tự hủy sau 30 phút không hoạt động

    // Thêm .Cookie vào trước HttpOnly và IsEssential để trỏ đúng cấu trúc đối tượng
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); 

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=KhachHangHome}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
