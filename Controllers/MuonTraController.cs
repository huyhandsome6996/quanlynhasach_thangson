using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using QuanLyThuVien.Data;

namespace QuanLyThuVien.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MuonTraController : ControllerBase
    {
        // GET - Lấy danh sách phiếu mượn
        [HttpGet]
        public IActionResult LayDanhSachPhieu()
        {
            try
            {
                var ds = new List<object>();
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT pm.MaPhieu,pm.MaDG,dg.TenDG,pm.NgayMuon,pm.TrangThaiPhieu
                    FROM PHIEU_MUON pm LEFT JOIN DOC_GIA dg ON pm.MaDG=dg.MaDG ORDER BY pm.MaPhieu DESC";
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ds.Add(new { maPhieu=r.GetString(0),maDG=r.GetString(1),
                        tenDG=r.IsDBNull(2)?"":r.GetString(2),ngayMuon=r.GetString(3),
                        trangThaiPhieu=r.GetString(4) });
                }
                return Ok(ds);
            }
            catch (Exception ex) { return StatusCode(500, new { message=ex.Message }); }
        }

        // GET chi tiết phiếu
        [HttpGet("{maPhieu}/chitiet")]
        public IActionResult LayChiTiet(string maPhieu)
        {
            try
            {
                var ds = new List<object>();
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT ct.MaPhieu,ct.MaSach,s.TenSach,ct.NgayHenTra,
                    ct.NgayTraThucTe,ct.TienPhat,ct.TinhTrangSach
                    FROM CHI_TIET_PHIEU ct LEFT JOIN SACH s ON ct.MaSach=s.MaSach
                    WHERE ct.MaPhieu=@ma";
                cmd.Parameters.AddWithValue("@ma",maPhieu);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    ds.Add(new { maPhieu=r.GetString(0),maSach=r.GetString(1),
                        tenSach=r.IsDBNull(2)?"":r.GetString(2),ngayHenTra=r.GetString(3),
                        ngayTraThucTe=r.IsDBNull(4)?"":r.GetString(4),
                        tienPhat=r.GetDouble(5),tinhTrangSach=r.GetString(6) });
                }
                return Ok(ds);
            }
            catch (Exception ex) { return StatusCode(500, new { message=ex.Message }); }
        }

        // POST - Lập phiếu mượn mới
        [HttpPost]
        public IActionResult LapPhieuMuon([FromBody] PhieuMuonRequest req)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                using var transaction = conn.BeginTransaction();
                try
                {
                    // Tạo mã phiếu
                    var c = conn.CreateCommand();
                    c.CommandText = "SELECT COUNT(*) FROM PHIEU_MUON";
                    string maPhieu = $"PM-{(Convert.ToInt64(c.ExecuteScalar())+1):D3}";
                    string ngayMuon = DateTime.Now.ToString("yyyy-MM-dd");

                    // Lưu phiếu mượn
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"INSERT INTO PHIEU_MUON(MaPhieu,MaDG,NgayMuon,TrangThaiPhieu)
                        VALUES(@mp,@dg,@nm,'Đang mượn')";
                    cmd.Parameters.AddWithValue("@mp",maPhieu);
                    cmd.Parameters.AddWithValue("@dg",req.MaDG??"");
                    cmd.Parameters.AddWithValue("@nm",ngayMuon);
                    cmd.ExecuteNonQuery();

                    // Lưu chi tiết + cập nhật số lượng sách
                    if (req.DanhSachSach != null)
                    {
                        foreach (var maSach in req.DanhSachSach)
                        {
                            string ngayHenTra = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd");
                            var ct = conn.CreateCommand();
                            ct.CommandText = @"INSERT INTO CHI_TIET_PHIEU(MaPhieu,MaSach,NgayHenTra,NgayTraThucTe,TienPhat,TinhTrangSach)
                                VALUES(@mp,@ms,@nht,'',0,'Đang mượn')";
                            ct.Parameters.AddWithValue("@mp",maPhieu);
                            ct.Parameters.AddWithValue("@ms",maSach);
                            ct.Parameters.AddWithValue("@nht",ngayHenTra);
                            ct.ExecuteNonQuery();

                            // Trừ số lượng hiện tại
                            var upd = conn.CreateCommand();
                            upd.CommandText = "UPDATE SACH SET SoLuongHienTai=SoLuongHienTai-1 WHERE MaSach=@ms AND SoLuongHienTai>0";
                            upd.Parameters.AddWithValue("@ms",maSach);
                            upd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return Ok(new { success=true, message=$"Lập phiếu mượn thành công! Mã: {maPhieu}", maPhieu });
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }

        // POST - Trả sách
        [HttpPost("trasach")]
        public IActionResult TraSach([FromBody] TraSachRequest req)
        {
            try
            {
                using var conn = new SqliteConnection(DatabaseInitializer.ConnectionString);
                conn.Open();
                string ngayTra = DateTime.Now.ToString("yyyy-MM-dd");

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT NgayHenTra FROM CHI_TIET_PHIEU WHERE MaPhieu=@mp AND MaSach=@ms";
                cmd.Parameters.AddWithValue("@mp",req.MaPhieu??"");
                cmd.Parameters.AddWithValue("@ms",req.MaSach??"");
                var ngayHenTra = cmd.ExecuteScalar()?.ToString() ?? "";

                // Tính tiền phạt
                double tienPhat = 0;
                if (!string.IsNullOrEmpty(ngayHenTra))
                {
                    var soNgayTre = (DateTime.Parse(ngayTra) - DateTime.Parse(ngayHenTra)).Days;
                    if (soNgayTre > 0) tienPhat = soNgayTre * 5000; // 5000đ/ngày
                }

                // Cập nhật chi tiết phiếu
                var upd = conn.CreateCommand();
                upd.CommandText = @"UPDATE CHI_TIET_PHIEU SET NgayTraThucTe=@nt,TienPhat=@tp,TinhTrangSach='Đã trả'
                    WHERE MaPhieu=@mp AND MaSach=@ms";
                upd.Parameters.AddWithValue("@nt",ngayTra);
                upd.Parameters.AddWithValue("@tp",tienPhat);
                upd.Parameters.AddWithValue("@mp",req.MaPhieu??"");
                upd.Parameters.AddWithValue("@ms",req.MaSach??"");
                upd.ExecuteNonQuery();

                // Cộng lại số lượng sách
                var addBack = conn.CreateCommand();
                addBack.CommandText = "UPDATE SACH SET SoLuongHienTai=SoLuongHienTai+1 WHERE MaSach=@ms";
                addBack.Parameters.AddWithValue("@ms",req.MaSach??"");
                addBack.ExecuteNonQuery();

                // Kiểm tra nếu tất cả sách đã trả -> hoàn tất phiếu
                var ck = conn.CreateCommand();
                ck.CommandText = "SELECT COUNT(*) FROM CHI_TIET_PHIEU WHERE MaPhieu=@mp AND TinhTrangSach='Đang mượn'";
                ck.Parameters.AddWithValue("@mp",req.MaPhieu??"");
                if (Convert.ToInt64(ck.ExecuteScalar()) == 0)
                {
                    var done = conn.CreateCommand();
                    done.CommandText = "UPDATE PHIEU_MUON SET TrangThaiPhieu='Hoàn tất' WHERE MaPhieu=@mp";
                    done.Parameters.AddWithValue("@mp",req.MaPhieu??"");
                    done.ExecuteNonQuery();
                }

                return Ok(new { success=true, message="Trả sách thành công!", tienPhat });
            }
            catch (Exception ex) { return StatusCode(500, new { success=false, message=ex.Message }); }
        }
    }

    public class PhieuMuonRequest
    {
        public string? MaDG { get; set; }
        public List<string>? DanhSachSach { get; set; }
    }

    public class TraSachRequest
    {
        public string? MaPhieu { get; set; }
        public string? MaSach { get; set; }
    }
}
