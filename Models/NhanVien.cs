namespace QuanLyThuVien.Models
{
    // ===== LỚP NHÂN VIÊN THƯ VIỆN - KẾ THỪA TỪ NGUOI =====
    public class NhanVien : Nguoi
    {
        private string _tenDangNhap = string.Empty;
        private string _matKhau = string.Empty;
        private string _vaiTro = "NhanVien"; // Admin hoặc NhanVien

        public string TenDangNhap
        {
            get { return _tenDangNhap; }
            set { _tenDangNhap = value; }
        }

        public string MatKhau
        {
            get { return _matKhau; }
            set { _matKhau = value; }
        }

        public string VaiTro
        {
            get { return _vaiTro; }
            set { _vaiTro = value; }
        }

        public NhanVien() { }

        public NhanVien(string maNV, string hoTen, string soDienThoai,
                        string tenDangNhap, string matKhau, string vaiTro)
            : base(maNV, hoTen, soDienThoai)
        {
            _tenDangNhap = tenDangNhap;
            _matKhau = matKhau;
            _vaiTro = vaiTro;
        }

        // ===== TÍNH ĐA HÌNH: Override từ lớp cha =====
        public override string LayThongTin()
        {
            return $"Nhân viên: {HoTen} - Mã: {MaSo} - Vai trò: {VaiTro}";
        }
    }
}
