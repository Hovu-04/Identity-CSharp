using IdentityLogin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// Thư viện Identity cơ bản
// Thư viện Identity dùng với EF Core

// Thư viện Entity Framework Core

namespace IdentityLogin.Data;

// Kế thừa IdentityDbContext<IdentityUser>
// => Dùng lớp IdentityUser mặc định để tạo bảng Users (hoặc AspNetUsers)
public class MyBlogContext : IdentityDbContext<AppUser>
{
    // Constructor, nhận DbContextOptions<MyBlogContext> và gọi base
    public MyBlogContext(DbContextOptions<MyBlogContext> options)
        : base(options)
    {
    }

    // Nếu cần cấu hình thêm (chuỗi kết nối, logging...) có thể ghi đè OnConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Mặc định gọi base, không làm gì thêm
        base.OnConfiguring(optionsBuilder);
    }

    // Ghi đè OnModelCreating để tùy chỉnh mô hình (model) trước khi EF Core tạo migration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Gọi base.OnModelCreating để Identity thiết lập mặc định
        base.OnModelCreating(builder);

        // Vòng lặp qua mọi thực thể (entity) mà Identity tạo ra
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // Lấy tên bảng gốc
            var tableName = entityType.GetTableName();

            // Nếu tên bảng bắt đầu với "AspNet"
            // => Bỏ tiền tố "AspNet"
            //    "AspNetUsers" -> "Users"
            //    "AspNetRoles" -> "Roles"
            //    "AspNetUserRoles" -> "UserRoles"
            // v.v.
            if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith("AspNet"))
                entityType.SetTableName(tableName.Substring(6));
        }
    }
}