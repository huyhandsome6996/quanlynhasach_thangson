namespace QuanLyThuVien.Models
{
    // ===== LỚP SÁCH =====
    public class Sach
    {
        private string _maSach = string.Empty;
        private string _tenSach = string.Empty;
        private string _tacGia = string.Empty;
        private string _nhaXuatBan = string.Empty;
        private int _namXuatBan;
        private int _soLuongTong;
        private int _soLuongHienTai;
        private string _maTheLoai = string.Empty;
        private bool _trangThai = true; // true = Đang lưu hành, false = Ngừng lưu hành

        // ===== GETTER / SETTER =====
        public string MaSach
        {
            get { return _maSach; }
            set { _maSach = value; }
        }

        public string TenSach
        {
            get { return _tenSach; }
            set { _tenSach = value; }
        }

        public string TacGia
        {
            get { return _tacGia; }
            set { _tacGia = value; }
        }

        public string NhaXuatBan
        {
            get { return _nhaXuatBan; }
            set { _nhaXuatBan = value; }
        }

        public int NamXuatBan
        {
            get { return _namXuatBan; }
            set { _namXuatBan = value; }
        }

        public int SoLuongTong
        {
            get { return _soLuongTong; }
            set { _soLuongTong = value; }
        }

        public int SoLuongHienTai
        {
            get { return _soLuongHienTai; }
            set { _soLuongHienTai = value; }
        }

        public string MaTheLoai
        {
            get { return _maTheLoai; }
            set { _maTheLoai = value; }
        }

        public bool TrangThai
        {
            get { return _trangThai; }
            set { _trangThai = value; }
        }

        // ===== CONSTRUCTOR =====
        public Sach() { }

        public Sach(string maSach, string tenSach, string tacGia, string nhaXuatBan,
                    int namXuatBan, int soLuongTong, int soLuongHienTai, string maTheLoai, bool trangThai)
        {
            _maSach = maSach;
            _tenSach = tenSach;
            _tacGia = tacGia;
            _nhaXuatBan = nhaXuatBan;
            _namXuatBan = namXuatBan;
            _soLuongTong = soLuongTong;
            _soLuongHienTai = soLuongHienTai;
            _maTheLoai = maTheLoai;
            _trangThai = trangThai;
        }
    }
}
