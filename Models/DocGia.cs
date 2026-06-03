namespace QuanLyThuVien.Models
{
    // ===== LỚP ĐỘC GIẢ - KẾ THỪA TỪ NGUOI (Tính Kế thừa) =====
    public class DocGia : Nguoi
    {
        private string _cccd = string.Empty;
        private string _email = string.Empty;
        private string _ngayDangKy = string.Empty;
        private string _ngayHetHan = string.Empty;
        private string _trangThai = "Hoạt động"; // Hoạt động, Hết hạn, Bị khóa

        // ===== GETTER / SETTER =====
        public string CCCD
        {
            get { return _cccd; }
            set { _cccd = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string NgayDangKy
        {
            get { return _ngayDangKy; }
            set { _ngayDangKy = value; }
        }

        public string NgayHetHan
        {
            get { return _ngayHetHan; }
            set { _ngayHetHan = value; }
        }

        public string TrangThai
        {
            get { return _trangThai; }
            set { _trangThai = value; }
        }

        // ===== CONSTRUCTOR =====
        public DocGia() { }

        public DocGia(string maDG, string tenDG, string soDienThoai, string cccd,
                      string email, string ngayDangKy, string ngayHetHan, string trangThai)
            : base(maDG, tenDG, soDienThoai)
        {
            _cccd = cccd;
            _email = email;
            _ngayDangKy = ngayDangKy;
            _ngayHetHan = ngayHetHan;
            _trangThai = trangThai;
        }

        // ===== TÍNH ĐA HÌNH: Override phương thức trừu tượng từ lớp cha =====
        public override string LayThongTin()
        {
            return $"Độc giả: {HoTen} - Mã: {MaSo} - Trạng thái: {TrangThai}";
        }
    }
}
