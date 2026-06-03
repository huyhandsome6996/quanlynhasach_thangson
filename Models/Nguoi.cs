namespace QuanLyThuVien.Models
{
    // ===== LỚP CHA: NGUOI (Tính Kế thừa - Inheritance) =====
    // Lớp trừu tượng đại diện cho một con người trong hệ thống
    public abstract class Nguoi
    {
        // Tính Đóng gói (Encapsulation): thuộc tính private, truy cập qua getter/setter
        private string _maSo = string.Empty;
        private string _hoTen = string.Empty;
        private string _soDienThoai = string.Empty;

        // ===== GETTER / SETTER =====
        public string MaSo
        {
            get { return _maSo; }
            set { _maSo = value; }
        }

        public string HoTen
        {
            get { return _hoTen; }
            set { _hoTen = value; }
        }

        public string SoDienThoai
        {
            get { return _soDienThoai; }
            set { _soDienThoai = value; }
        }

        // Constructor
        public Nguoi() { }

        public Nguoi(string maSo, string hoTen, string soDienThoai)
        {
            _maSo = maSo;
            _hoTen = hoTen;
            _soDienThoai = soDienThoai;
        }

        // Phương thức trừu tượng - bắt buộc lớp con override (Tính Đa hình)
        public abstract string LayThongTin();
    }
}
