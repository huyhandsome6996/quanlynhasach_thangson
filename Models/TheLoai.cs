namespace QuanLyThuVien.Models
{
    // ===== LỚP THỂ LOẠI =====
    public class TheLoai
    {
        private string _maTheLoai = string.Empty;
        private string _tenTheLoai = string.Empty;
        private string _moTa = string.Empty;

        public string MaTheLoai
        {
            get { return _maTheLoai; }
            set { _maTheLoai = value; }
        }

        public string TenTheLoai
        {
            get { return _tenTheLoai; }
            set { _tenTheLoai = value; }
        }

        public string MoTa
        {
            get { return _moTa; }
            set { _moTa = value; }
        }

        public TheLoai() { }

        public TheLoai(string maTheLoai, string tenTheLoai, string moTa)
        {
            _maTheLoai = maTheLoai;
            _tenTheLoai = tenTheLoai;
            _moTa = moTa;
        }
    }
}
