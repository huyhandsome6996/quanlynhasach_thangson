using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocGiaController : ControllerBase
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
                cmd.CommandText = "SELECT MaDG,TenDG,CCCD,SoDienThoai,Email,NgayDangKy,NgayHetHan,TrangThai FROM DOC_GIA ORDER BY MaDG";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ds.Add(new {
                        maDG=r.GetString(0), tenDG=r.GetString(1),
                        cccd=r.IsDBNull(2)?"":r.GetString(2), soDienThoai=r.IsDBNull(3)?"":r.GetString(3),
                        email=r.IsDBNull(4)?"":r.GetString(4), ngayDangKy=r.IsDBNull(5)?"":r.GetString(5),
                        ngayHetHan=r.IsDBNull(6)?"":r.GetString(6), trangThai=r.IsDBNull(7)?"":r.GetString(7)
                    });
                }
                return Ok(ds);
            }
            catch (Exception ex) { return StatusCode(500, new { message=ex.Message }); }
        }

        [HttpPost]
        public IActionResult ThemDocGia([FromBody] DocGiaRequest dg)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var c = conn.CreateCommand();
                c.CommandText = "SELECT COUNT(*) FROM DOC_GIA";
                string maDG = $"DG-{(Convert.ToInt64(c.ExecuteScalar())+1):D3}";
                string ngayDK = DateTime.Now.ToString("yyyy-MM-dd");
                string ngayHH = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO DOC_GIA(MaDG,TenDG,CCCD,SoDienThoai,Email,NgayDangKy,NgayHetHan,TrangThai)
                    VALUES(@ma,@ten,@cccd,@sdt,@email,@dk,@hh,'Hoạt động')";
                cmd.Parameters.AddWithValue("@ma",maDG);
                cmd.Parameters.AddWithValue("@ten",dg.TenDG??"");
                cmd.Parameters.AddWithValue("@cccd",dg.CCCD??"");
                cmd.Parameters.AddWithValue("@sdt",dg.SoDienThoai??"");
                cmd.Parameters.AddWithValue("@email",dg.Email??"");
                cmd.Parameters.AddWithValue("@dk",ngayDK);
                cmd.Parameters.AddWithValue("@hh",ngayHH);
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message=$"Thêm độc giả thành công! Mã: {maDG}", maDG });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        [HttpPut("{ma}")]
        public IActionResult CapNhat(string ma, [FromBody] DocGiaRequest dg)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE DOC_GIA SET TenDG=@ten,CCCD=@cccd,SoDienThoai=@sdt,Email=@email WHERE MaDG=@ma";
                cmd.Parameters.AddWithValue("@ma",ma);
                cmd.Parameters.AddWithValue("@ten",dg.TenDG??"");
                cmd.Parameters.AddWithValue("@cccd",dg.CCCD??"");
                cmd.Parameters.AddWithValue("@sdt",dg.SoDienThoai??"");
                cmd.Parameters.AddWithValue("@email",dg.Email??"");
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message="Cập nhật độc giả thành công!" });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        [HttpDelete("{ma}")]
        public IActionResult Xoa(string ma)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var ck = conn.CreateCommand();
                ck.CommandText = @"SELECT COUNT(*) FROM PHIEU_MUON pm JOIN CHI_TIET_PHIEU ct ON pm.MaPhieu=ct.MaPhieu
                    WHERE pm.MaDG=@ma AND ct.TinhTrangSach='Đang mượn'";
                ck.Parameters.AddWithValue("@ma",ma);
                if (Convert.ToInt64(ck.ExecuteScalar()) > 0)
                    return Ok(new { success=false, message="Không thể xóa! Độc giả đang có sách chưa trả." });
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM DOC_GIA WHERE MaDG=@ma";
                cmd.Parameters.AddWithValue("@ma",ma);
                cmd.ExecuteNonQuery();
                return Ok(new { success=true, message="Xóa độc giả thành công!" });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }
    }

    public class DocGiaRequest
    {
        public string? TenDG { get; set; }
        public string? CCCD { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
    }
}
