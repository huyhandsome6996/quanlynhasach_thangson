using Microsoft.Data.Sqlite;

namespace QuanLyThuVien.Data
{
    // ===== LỚP KHỞI TẠO CƠ SỞ DỮ LIỆU =====
    // Chịu trách nhiệm tạo bảng và chèn dữ liệu mẫu
    public static class DatabaseInitializer
    {
        private static readonly string _connectionString = "Data Source=library.db";

        public static string ConnectionString => _connectionString;

        // Khởi tạo Database: Tạo bảng + Dữ liệu mẫu
        public static void Initialize()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // ===== TẠO BẢNG NHAN_VIEN (Tài khoản đăng nhập) =====
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS NHAN_VIEN (
                        MaNV TEXT PRIMARY KEY,
                        HoTen TEXT NOT NULL,
                        SoDienThoai TEXT,
                        TenDangNhap TEXT UNIQUE NOT NULL,
                        MatKhau TEXT NOT NULL,
                        VaiTro TEXT DEFAULT 'NhanVien'
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== TẠO BẢNG THE_LOAI =====
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS THE_LOAI (
                        MaTheLoai TEXT PRIMARY KEY,
                        TenTheLoai TEXT NOT NULL,
                        MoTa TEXT
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== TẠO BẢNG SACH =====
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SACH (
                        MaSach TEXT PRIMARY KEY,
                        TenSach TEXT NOT NULL,
                        TacGia TEXT,
                        NhaXuatBan TEXT,
                        NamXuatBan INTEGER,
                        SoLuongTong INTEGER DEFAULT 0,
                        SoLuongHienTai INTEGER DEFAULT 0,
                        MaTheLoai TEXT,
                        TrangThai INTEGER DEFAULT 1,
                        FOREIGN KEY (MaTheLoai) REFERENCES THE_LOAI(MaTheLoai)
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== TẠO BẢNG DOC_GIA =====
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS DOC_GIA (
                        MaDG TEXT PRIMARY KEY,
                        TenDG TEXT NOT NULL,
                        CCCD TEXT UNIQUE,
                        SoDienThoai TEXT,
                        Email TEXT,
                        NgayDangKy TEXT,
                        NgayHetHan TEXT,
                        TrangThai TEXT DEFAULT 'Hoạt động'
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== TẠO BẢNG PHIEU_MUON =====
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS PHIEU_MUON (
                        MaPhieu TEXT PRIMARY KEY,
                        MaDG TEXT,
                        NgayMuon TEXT,
                        TrangThaiPhieu TEXT DEFAULT 'Đang mượn',
                        FOREIGN KEY (MaDG) REFERENCES DOC_GIA(MaDG)
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== TẠO BẢNG CHI_TIET_PHIEU =====
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS CHI_TIET_PHIEU (
                        MaPhieu TEXT,
                        MaSach TEXT,
                        NgayHenTra TEXT,
                        NgayTraThucTe TEXT,
                        TienPhat REAL DEFAULT 0,
                        TinhTrangSach TEXT DEFAULT 'Đang mượn',
                        PRIMARY KEY (MaPhieu, MaSach),
                        FOREIGN KEY (MaPhieu) REFERENCES PHIEU_MUON(MaPhieu),
                        FOREIGN KEY (MaSach) REFERENCES SACH(MaSach)
                    );
                ";
                cmd.ExecuteNonQuery();

                // ===== CHÈN DỮ LIỆU MẪU (NẾU CHƯA CÓ) =====
                InsertMockData(connection);

                connection.Close();
            }
        }

        // Chèn dữ liệu mẫu vào các bảng
        private static void InsertMockData(SqliteConnection connection)
        {
            var cmd = connection.CreateCommand();

            // Kiểm tra xem đã có dữ liệu chưa
            cmd.CommandText = "SELECT COUNT(*) FROM THE_LOAI";
            var count = Convert.ToInt64(cmd.ExecuteScalar());
            if (count > 0) return; // Đã có dữ liệu mẫu

            // ----- Tài khoản Admin mặc định -----
            cmd.CommandText = @"
                INSERT INTO NHAN_VIEN (MaNV, HoTen, SoDienThoai, TenDangNhap, MatKhau, VaiTro) VALUES
                ('NV-001', 'Quản trị viên', '0901234567', 'admin', 'admin123', 'Admin');
            ";
            cmd.ExecuteNonQuery();

            // ----- Dữ liệu Thể loại -----
            cmd.CommandText = @"
                INSERT INTO THE_LOAI (MaTheLoai, TenTheLoai, MoTa) VALUES
                ('TL-001', 'Văn học', 'Sách văn học Việt Nam và thế giới'),
                ('TL-002', 'Khoa học', 'Sách khoa học tự nhiên và ứng dụng'),
                ('TL-003', 'Lịch sử', 'Sách lịch sử Việt Nam và thế giới'),
                ('TL-004', 'Công nghệ', 'Sách công nghệ thông tin và lập trình'),
                ('TL-005', 'Kinh tế', 'Sách kinh tế, tài chính, quản trị');
            ";
            cmd.ExecuteNonQuery();

            // ----- Dữ liệu Sách -----
            cmd.CommandText = @"
                INSERT INTO SACH (MaSach, TenSach, TacGia, NhaXuatBan, NamXuatBan, SoLuongTong, SoLuongHienTai, MaTheLoai, TrangThai) VALUES
                ('S2026-001', 'Dế Mèn Phiêu Lưu Ký', 'Tô Hoài', 'NXB Kim Đồng', 2020, 10, 8, 'TL-001', 1),
                ('S2026-002', 'Số Đỏ', 'Vũ Trọng Phụng', 'NXB Văn Học', 2019, 5, 5, 'TL-001', 1),
                ('S2026-003', 'Lược Sử Thời Gian', 'Stephen Hawking', 'NXB Trẻ', 2021, 8, 7, 'TL-002', 1),
                ('S2026-004', 'Việt Nam Sử Lược', 'Trần Trọng Kim', 'NXB Tổng Hợp', 2018, 6, 6, 'TL-003', 1),
                ('S2026-005', 'Clean Code', 'Robert C. Martin', 'NXB Lao Động', 2022, 12, 10, 'TL-004', 1),
                ('S2026-006', 'Đắc Nhân Tâm', 'Dale Carnegie', 'NXB Tổng Hợp', 2023, 15, 13, 'TL-005', 1),
                ('S2026-007', 'Nhà Giả Kim', 'Paulo Coelho', 'NXB Hội Nhà Văn', 2020, 9, 9, 'TL-001', 1),
                ('S2026-008', 'Sapiens: Lược Sử Loài Người', 'Yuval Noah Harari', 'NXB Tri Thức', 2021, 7, 6, 'TL-003', 1);
            ";
            cmd.ExecuteNonQuery();

            // ----- Dữ liệu Độc giả -----
            cmd.CommandText = @"
                INSERT INTO DOC_GIA (MaDG, TenDG, CCCD, SoDienThoai, Email, NgayDangKy, NgayHetHan, TrangThai) VALUES
                ('DG-001', 'Nguyễn Văn An', '079201001234', '0912345678', 'an.nguyen@email.com', '2025-09-01', '2026-09-01', 'Hoạt động'),
                ('DG-002', 'Trần Thị Bình', '079201005678', '0923456789', 'binh.tran@email.com', '2025-10-15', '2026-10-15', 'Hoạt động'),
                ('DG-003', 'Lê Hoàng Cường', '079201009012', '0934567890', 'cuong.le@email.com', '2026-01-20', '2027-01-20', 'Hoạt động'),
                ('DG-004', 'Phạm Minh Dương', '079201003456', '0945678901', 'duong.pham@email.com', '2025-06-10', '2026-06-10', 'Hoạt động'),
                ('DG-005', 'Hoàng Thị Em', '079201007890', '0956789012', 'em.hoang@email.com', '2025-03-05', '2026-03-05', 'Hết hạn');
            ";
            cmd.ExecuteNonQuery();

            // ----- Dữ liệu Phiếu mượn mẫu -----
            cmd.CommandText = @"
                INSERT INTO PHIEU_MUON (MaPhieu, MaDG, NgayMuon, TrangThaiPhieu) VALUES
                ('PM-001', 'DG-001', '2026-05-20', 'Đang mượn'),
                ('PM-002', 'DG-002', '2026-05-25', 'Đang mượn');
            ";
            cmd.ExecuteNonQuery();

            // ----- Chi tiết phiếu mượn mẫu -----
            cmd.CommandText = @"
                INSERT INTO CHI_TIET_PHIEU (MaPhieu, MaSach, NgayHenTra, NgayTraThucTe, TienPhat, TinhTrangSach) VALUES
                ('PM-001', 'S2026-001', '2026-06-03', '', 0, 'Đang mượn'),
                ('PM-001', 'S2026-005', '2026-06-03', '', 0, 'Đang mượn'),
                ('PM-002', 'S2026-003', '2026-06-08', '', 0, 'Đang mượn'),
                ('PM-002', 'S2026-006', '2026-06-08', '', 0, 'Đang mượn');
            ";
            cmd.ExecuteNonQuery();
        }
    }
}
