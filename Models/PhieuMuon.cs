namespace QuanLyThuVien.Models
{
    // ===== LỚP PHIẾU MƯỢN =====
    public class PhieuMuon
    {
        private string _maPhieu = string.Empty;
        private string _maDG = string.Empty;
        private string _ngayMuon = string.Empty;
        private string _trangThaiPhieu = "Đang mượn"; // Đang mượn, Hoàn tất

        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; }
        }

        public string MaDG
        {
            get { return _maDG; }
            set { _maDG = value; }
        }

        public string NgayMuon
        {
            get { return _ngayMuon; }
            set { _ngayMuon = value; }
        }

        public string TrangThaiPhieu
        {
            get { return _trangThaiPhieu; }
            set { _trangThaiPhieu = value; }
        }

        public PhieuMuon() { }

        public PhieuMuon(string maPhieu, string maDG, string ngayMuon, string trangThaiPhieu)
        {
            _maPhieu = maPhieu;
            _maDG = maDG;
            _ngayMuon = ngayMuon;
            _trangThaiPhieu = trangThaiPhieu;
        }
    }
}
