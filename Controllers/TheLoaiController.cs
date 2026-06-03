using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    // ===== CONTROLLER QUẢN LÝ THỂ LOẠI =====
    [ApiController]
    [Route("api/[controller]")]
    public class TheLoaiController : ControllerBase
    {
        // GET api/theloai - Lấy tất cả thể loại
        [HttpGet]
        public IActionResult LayTatCa()
        {
            try
            {
                var danhSach = new List<TheLoai>();
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MaTheLoai, TenTheLoai, MoTa FROM THE_LOAI ORDER BY MaTheLoai";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    danhSach.Add(new TheLoai
                    {
                        MaTheLoai = reader.GetString(0),
                        TenTheLoai = reader.GetString(1),
                        MoTa = reader.IsDBNull(2) ? "" : reader.GetString(2)
                    });
                }

                return Ok(danhSach);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST api/theloai - Thêm thể loại mới
        [HttpPost]
        public IActionResult ThemMoi([FromBody] TheLoai theLoai)
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO THE_LOAI (MaTheLoai, TenTheLoai, MoTa)
                                    VALUES (@ma, @ten, @mota)";
                cmd.Parameters.AddWithValue("@ma", theLoai.MaTheLoai);
                cmd.Parameters.AddWithValue("@ten", theLoai.TenTheLoai);
                cmd.Parameters.AddWithValue("@mota", theLoai.MoTa ?? "");
                cmd.ExecuteNonQuery();

                return Ok(new { success = true, message = "Thêm thể loại thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT api/theloai/{ma} - Cập nhật thể loại
        [HttpPut("{ma}")]
        public IActionResult CapNhat(string ma, [FromBody] TheLoai theLoai)
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"UPDATE THE_LOAI SET TenTheLoai = @ten, MoTa = @mota
                                    WHERE MaTheLoai = @ma";
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.Parameters.AddWithValue("@ten", theLoai.TenTheLoai);
                cmd.Parameters.AddWithValue("@mota", theLoai.MoTa ?? "");
                cmd.ExecuteNonQuery();

                return Ok(new { success = true, message = "Cập nhật thể loại thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE api/theloai/{ma} - Xóa thể loại
        [HttpDelete("{ma}")]
        public IActionResult Xoa(string ma)
        {
            try
            {
                using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
                connection.Open();

                // Kiểm tra còn sách thuộc thể loại này không
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM SACH WHERE MaTheLoai = @ma AND TrangThai = 1";
                checkCmd.Parameters.AddWithValue("@ma", ma);
                var count = Convert.ToInt64(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    return Ok(new { success = false, message = $"Không thể xóa! Còn {count} cuốn sách thuộc thể loại này." });
                }

                var cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM THE_LOAI WHERE MaTheLoai = @ma";
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return Ok(new { success = true, message = "Xóa thể loại thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
