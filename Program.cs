using IdentityLogin.Data;
using IdentityLogin.Models;
using IdentityLogin.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// -------------------------------------------------------
// CẤU HÌNH CHUỖI KẾT NỐI (CONNECTION STRING) CHO DB
// -------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
// Lấy chuỗi kết nối "DefaultConnection" từ file cấu hình (appsettings.json).
// Nếu null, gán chuỗi rỗng "" để tránh lỗi

// -------------------------------------------------------
// ĐĂNG KÝ DB CONTEXT SỬ DỤNG POSTGRESQL
// -------------------------------------------------------
builder.Services.AddDbContext<MyBlogContext>(options =>
    options.UseSqlServer(connectionString));
// Thêm MyBlogContext vào DI, dùng provider Npgsql (PostgreSQL)
// Mọi thao tác database sẽ qua MyBlogContext

// -------------------------------------------------------
// ĐĂNG KÝ IDENTITY DÙNG AddDefaultIdentity<AppUser>()
// -------------------------------------------------------
// Lệnh này tạo cấu hình mặc định cho Identity, nhưng không bao gồm Roles
// // Nếu bạn muốn quản lý Roles, hãy dùng AddIdentity<AppUser, IdentityRole>()
// builder.Services.AddDefaultIdentity<AppUser>()
//     .AddEntityFrameworkStores<MyBlogContext>()
//     .AddDefaultTokenProviders();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<MyBlogContext>()
    .AddDefaultTokenProviders();

// AppUser kế thừa IdentityUser, chứa thông tin user tùy biến (HomeAddress...)
// MyBlogContext: DbContext chứa các bảng Identity (Users, ...)
// DefaultTokenProviders: Token cho xác thực email, reset password, 2FA, v.v.

// -------------------------------------------------------
// TÙY CHỈNH CÁC THIẾT LẬP CHO IDENTITY (IdentityOptions)
// -------------------------------------------------------
builder.Services.Configure<IdentityOptions>(options =>
{
    // Cấu hình Password
    options.Password.RequireDigit = false; // Không bắt buộc có chữ số
    options.Password.RequireLowercase = false; // Không bắt buộc có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt buộc có ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc có chữ in hoa
    options.Password.RequiredLength = 3; // Mật khẩu tối thiểu 3 ký tự
    options.Password.RequiredUniqueChars = 1; // Yêu cầu ít nhất 1 ký tự khác nhau

    // Cấu hình Lockout - khóa user sau nhiều lần đăng nhập sai
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Sau 5 lần thất bại sẽ bị khóa
    options.Lockout.AllowedForNewUsers = true; // Áp dụng cho user mới

    // Cấu hình User
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    // Các ký tự được phép trong UserName
    options.User.RequireUniqueEmail = true; // Bắt buộc Email duy nhất

    // Cấu hình đăng nhập
    options.SignIn.RequireConfirmedEmail = true; // Yêu cầu xác thực email
    options.SignIn.RequireConfirmedPhoneNumber = false; // Không bắt buộc xác thực số điện thoại
});

// Register email
builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);
builder.Services.AddSingleton<IEmailSender,SendMailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();