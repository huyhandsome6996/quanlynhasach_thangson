using QuanLyThuVien.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Thêm Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Cấu hình Static Files (phục vụ HTML từ wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Cấu hình CORS cho phép gọi API từ frontend
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Map Controllers
app.MapControllers();

// Khởi tạo Database SQLite (tạo bảng + dữ liệu mẫu)
DatabaseInitializer.Initialize();

// ===== DESKTOP APP MODE =====
// Tự động mở trình duyệt ở chế độ App (ẩn thanh địa chỉ) khi server khởi động
var port = 5100;
app.Urls.Add($"http://localhost:{port}");

// app.Lifetime.ApplicationStarted.Register(() =>
// {
//     string url = $"http://localhost:{port}/login.html";
//     try
//     {
//         // Thử mở bằng Edge trước (--app mode)
//         Process.Start(new ProcessStartInfo
//         {
//             FileName = "msedge",
//             Arguments = $"--app={url} --window-size=1400,900",
//             UseShellExecute = true
//         });
//     }
//     catch
//     {
//         try
//         {
//             // Nếu không có Edge, thử Chrome
//             Process.Start(new ProcessStartInfo
//             {
//                 FileName = "chrome",
//                 Arguments = $"--app={url} --window-size=1400,900",
//                 UseShellExecute = true
//             });
//         }
//         catch
//         {
//             // Fallback: mở bằng trình duyệt mặc định
//             Process.Start(new ProcessStartInfo
//             {
//                 FileName = url,
//                 UseShellExecute = true
//             });
//         }
//     }
// });


app.Run();
