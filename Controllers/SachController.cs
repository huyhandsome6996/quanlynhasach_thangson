using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using QuanLyThuVien.Data;
using QuanLyThuVien.Models;

namespace QuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SachController : ControllerBase
    {
        [HttpGet]
        public IActionResult LayTatCa()
        {
            try
            {
                var ds = new List<object>();
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT s.MaSach,s.TenSach,s.TacGia,s.NhaXuatBan,s.NamXuatBan,
                    s.SoLuongTong,s.SoLuongHienTai,s.MaTheLoai,s.TrangThai,COALESCE(tl.TenTheLoai,'') 
                    FROM SACH s LEFT JOIN THE_LOAI tl ON s.MaTheLoai=tl.MaTheLoai WHERE s.TrangThai=1 ORDER BY s.MaSach";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ds.Add(new { maSach=r.GetString(0),tenSach=r.GetString(1),
                        tacGia=r.IsDBNull(2)?"":r.GetString(2),nhaXuatBan=r.IsDBNull(3)?"":r.GetString(3),
                        namXuatBan=r.GetInt32(4),soLuongTong=r.GetInt32(5),soLuongHienTai=r.GetInt32(6),
                        maTheLoai=r.IsDBNull(7)?"":r.GetString(7),trangThai=r.GetInt32(8),tenTheLoai=r.GetString(9) });
                }
                return Ok(ds);
            }
            catch (Exception ex) { return StatusCode(500, new { message = ex.Message }); }
        }

        [HttpPost]
        public IActionResult ThemSach([FromBody] Sach sach)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var c = conn.CreateCommand();
                c.CommandText = "SELECT COUNT(*) FROM SACH";
                string maSach = $"S2026-{(Convert.ToInt64(c.ExecuteScalar())+1):D3}";
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO SACH (MaSach,TenSach,TacGia,NhaXuatBan,NamXuatBan,SoLuongTong,SoLuongHienTai,MaTheLoai,TrangThai)
                    VALUES(@ma,@ten,@tg,@nxb,@nam,@tong,@ht,@loai,1)";
                cmd.Parameters.AddWithValue("@ma",maSach);
                cmd.Parameters.AddWithValue("@ten",sach.TenSach);
                cmd.Parameters.AddWithValue("@tg",sach.TacGia??"");
                cmd.Parameters.AddWithValue("@nxb",sach.NhaXuatBan??"");
                cmd.Parameters.AddWithValue("@nam",sach.NamXuatBan);
                cmd.Parameters.AddWithValue("@tong",sach.SoLuongTong);
                cmd.Parameters.AddWithValue("@ht",sach.SoLuongTong);
                cmd.Parameters.AddWithValue("@loai",sach.MaTheLoai??"");
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message=$"Thêm sách thành công! Mã: {maSach}", maSach });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        [HttpPut("{ma}")]
        public IActionResult CapNhat(string ma, [FromBody] Sach sach)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE SACH SET TenSach=@ten,TacGia=@tg,NhaXuatBan=@nxb,NamXuatBan=@nam,SoLuongTong=@tong,MaTheLoai=@loai WHERE MaSach=@ma";
                cmd.Parameters.AddWithValue("@ma",ma);
                cmd.Parameters.AddWithValue("@ten",sach.TenSach);
                cmd.Parameters.AddWithValue("@tg",sach.TacGia??"");
                cmd.Parameters.AddWithValue("@nxb",sach.NhaXuatBan??"");
                cmd.Parameters.AddWithValue("@nam",sach.NamXuatBan);
                cmd.Parameters.AddWithValue("@tong",sach.SoLuongTong);
                cmd.Parameters.AddWithValue("@loai",sach.MaTheLoai??"");
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message="Cập nhật sách thành công!" });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        [HttpDelete("{ma}")]
        public IActionResult XoaMem(string ma)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var ck = conn.CreateCommand();
                ck.CommandText = "SELECT COUNT(*) FROM CHI_TIET_PHIEU WHERE MaSach=@ma AND TinhTrangSach='Đang mượn'";
                ck.Parameters.AddWithValue("@ma",ma);
                if (Convert.ToInt64(ck.ExecuteScalar()) > 0)
                    return Ok(new { success=false, message="Không thể xóa! Sách đang được mượn." });
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE SACH SET TrangThai=0 WHERE MaSach=@ma";
                cmd.Parameters.AddWithValue("@ma",ma);
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message="Đã ngừng lưu hành sách!" });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        [HttpGet("search")]
        public IActionResult TimKiem([FromQuery] string q)
        {
            try
            {
                var ds = new List<object>();
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT s.MaSach,s.TenSach,s.TacGia,s.NhaXuatBan,s.NamXuatBan,
                    s.SoLuongTong,s.SoLuongHienTai,s.MaTheLoai,s.TrangThai,COALESCE(tl.TenTheLoai,'')
                    FROM SACH s LEFT JOIN THE_LOAI tl ON s.MaTheLoai=tl.MaTheLoai
                    WHERE s.TrangThai=1 AND (s.MaSach LIKE @q OR s.TenSach LIKE @q OR s.TacGia LIKE @q) ORDER BY s.MaSach";
                cmd.Parameters.AddWithValue("@q",$"%{q}%");
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ds.Add(new { maSach=r.GetString(0),tenSach=r.GetString(1),
                        tacGia=r.IsDBNull(2)?"":r.GetString(2),nhaXuatBan=r.IsDBNull(3)?"":r.GetString(3),
                        namXuatBan=r.GetInt32(4),soLuongTong=r.GetInt32(5),soLuongHienTai=r.GetInt32(6),
                        maTheLoai=r.IsDBNull(7)?"":r.GetString(7),trangThai=r.GetInt32(8),tenTheLoai=r.GetString(9) });
                }
                return Ok(ds);
            }
            catch (Exception ex) { return StatusCode(500, new { message=ex.Message }); }
        }
    }
}
