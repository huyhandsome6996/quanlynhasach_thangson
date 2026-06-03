namespace QuanLyThuVien.Models
{
    // ===== LỚP CHI TIẾT PHIẾU MƯỢN =====
    // Bảng trung gian bẻ gãy quan hệ N-N giữa SACH và PHIEU_MUON
    public class ChiTietPhieuMuon
    {
        private string _maPhieu = string.Empty;
        private string _maSach = string.Empty;
        private string _ngayHenTra = string.Empty;
        private string _ngayTraThucTe = string.Empty;
        private double _tienPhat = 0;
        private string _tinhTrangSach = "Đang mượn"; // Đang mượn, Đã trả

        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; }
        }

        public string MaSach
        {
            get { return _maSach; }
            set { _maSach = value; }
        }

        public string NgayHenTra
        {
            get { return _ngayHenTra; }
            set { _ngayHenTra = value; }
        }

        public string NgayTraThucTe
        {
            get { return _ngayTraThucTe; }
            set { _ngayTraThucTe = value; }
        }

        public double TienPhat
        {
            get { return _tienPhat; }
            set { _tienPhat = value; }
        }

        public string TinhTrangSach
        {
            get { return _tinhTrangSach; }
            set { _tinhTrangSach = value; }
        }

        // Thuộc tính bổ sung để hiển thị trên giao diện
        public string? TenSach { get; set; }
        public string? TenDocGia { get; set; }

        public ChiTietPhieuMuon() { }

        public ChiTietPhieuMuon(string maPhieu, string maSach, string ngayHenTra,
                                 string ngayTraThucTe, double tienPhat, string tinhTrangSach)
        {
            _maPhieu = maPhieu;
            _maSach = maSach;
            _ngayHenTra = ngayHenTra;
            _ngayTraThucTe = ngayTraThucTe;
            _tienPhat = tienPhat;
            _tinhTrangSach = tinhTrangSach;
        }
    }
}
