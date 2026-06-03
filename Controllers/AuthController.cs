using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Controllers
{
    // ===== CONTROLLER XÁC THỰC ĐĂNG NHẬP / ĐĂNG KÝ =====
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // POST api/auth/login - Đăng nhập
        [HttpPost("login")]
        public IActionResult DangNhap([FromBody] LoginRequest request)
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaNV, HoTen, VaiTro FROM NHAN_VIEN WHERE TenDangNhap = @user AND MatKhau = @pass";
                cmd.Parameters.AddWithValue("@user", request.TenDangNhap ?? "");
                cmd.Parameters.AddWithValue("@pass", request.MatKhau ?? "");

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Đăng nhập thành công!",
                        maNV = reader.GetString(0),
                        hoTen = reader.GetString(1),
                        vaiTro = reader.GetString(2)
                    });
                }

                return Ok(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST api/auth/register - Đăng ký tài khoản mới
        [HttpPost("register")]
        public IActionResult DangKy([FromBody] RegisterRequest request)
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                // Kiểm tra trùng tên đăng nhập
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM NHAN_VIEN WHERE TenDangNhap = @user";
                checkCmd.Parameters.AddWithValue("@user", request.TenDangNhap ?? "");
                var count = Convert.ToInt64(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    return Ok(new { success = false, message = "Tên đăng nhập đã tồn tại!" });
                }

                // Tạo mã nhân viên tự động
                var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(*) FROM NHAN_VIEN";
                var total = Convert.ToInt64(countCmd.ExecuteScalar());
                string maNV = $"NV-{(total + 1):D3}";

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO NHAN_VIEN (MaNV, HoTen, SoDienThoai, TenDangNhap, MatKhau, VaiTro)
                                    VALUES (@ma, @ten, @sdt, @user, @pass, 'NhanVien')";
                cmd.Parameters.AddWithValue("@ma", maNV);
                cmd.Parameters.AddWithValue("@ten", request.HoTen ?? "");
                cmd.Parameters.AddWithValue("@sdt", request.SoDienThoai ?? "");
                cmd.Parameters.AddWithValue("@user", request.TenDangNhap ?? "");
                cmd.Parameters.AddWithValue("@pass", request.MatKhau ?? "");
                cmd.ExecuteNonQuery();

                return Ok(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    // ===== DTO Classes cho Auth =====
    public class LoginRequest
    {
        public string? TenDangNhap { get; set; }
        public string? MatKhau { get; set; }
    }

    public class RegisterRequest
    {
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public string? TenDangNhap { get; set; }
        public string? MatKhau { get; set; }
    }
}
