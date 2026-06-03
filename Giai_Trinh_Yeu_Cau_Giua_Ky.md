# BÁO CÁO GIẢI TRÌNH YÊU CẦU GIỮA KỲ
**Môn học:** Lập trình Hướng đối tượng (OOP)
**Dự án:** Hệ thống Quản lý Thư viện
**Sinh viên thực hiện:** [Tên của bạn/nhóm bạn]

Kính gửi Thầy/Cô,

Dưới đây là phần giải trình chi tiết về việc dự án "Hệ thống Quản lý Thư viện" của em đã đáp ứng đầy đủ và bám sát các yêu cầu chấm điểm Giữa kỳ mà Thầy/Cô đã đề ra. Nhóm em sử dụng kiến trúc ASP.NET Core Web API cho Backend kết hợp với HTML/CSS/JS thuần cho Frontend, và dùng thủ thuật cấu hình Process để chạy ứng dụng dưới dạng một Desktop App độc lập.

---

### 1. Tối thiểu 3-4 form ngoài form chính và form đăng nhập / đăng ký
**Giải trình:** 
Hệ thống của em đã phát triển tổng cộng **7 màn hình (form)** khác nhau, vượt mức yêu cầu tối thiểu. Bao gồm:
- **Form Đăng nhập & Đăng ký**: `login.html`, `register.html`.
- **Form Chính (Dashboard)**: `index.html`.
- **4 Form Quản lý nghiệp vụ (Các form ngoài)**:
  1. Form Quản lý Sách (`quanly_sach.html`)
  2. Form Quản lý Thể loại (`quanly_theloai.html`)
  3. Form Quản lý Độc giả (`quanly_docgia.html`)
  4. Form Mượn / Trả Sách (`quanly_muontra.html`)

### 2. Có ít nhất một form quản lý quan hệ giữa 2 đối tượng
**Giải trình:**
Hệ thống đã triển khai **Form Mượn / Trả sách (`quanly_muontra.html`)**. Đây là form cốt lõi thể hiện mối quan hệ nhiều-nhiều (N-N) giữa 2 đối tượng là **Độc Giả** và **Sách**.
- Trên giao diện `quanly_muontra.html`, em sử dụng `cboDocGia` (Combobox Độc giả) để chọn người mượn và `cboSach` (Combobox Sách) để chọn danh sách các cuốn sách được mượn.
- Ở tầng cơ sở dữ liệu và Model OOP, quan hệ này được bẻ gãy thông qua thực thể/bảng trung gian là `CHI_TIET_PHIEU` (Lớp `ChiTietPhieuMuon.cs`), lưu trữ chính xác Độc giả nào đang mượn Cuốn sách nào, ngày hẹn trả và tính toán tiền phạt độc lập cho từng cuốn.

### 3. Project chạy lên được (không lỗi) và chạy như một Desktop App (dù dùng HTML)
**Giải trình:**
- Hệ thống build và chạy hoàn toàn không có lỗi bằng lệnh `dotnet run`. DB SQLite tự động khởi tạo và chèn dữ liệu mẫu ngay lần chạy đầu tiên (file `Data/DatabaseInitializer.cs`).
- **Chạy như Desktop App**: Em đã cấu hình trong file `Program.cs` sử dụng `System.Diagnostics.Process.Start()` để gọi trực tiếp trình duyệt (Edge/Chrome) với tham số `--app=http://localhost:5100/login.html`. Nhờ tham số `--app`, giao diện HTML tự động bật lên trong một cửa sổ độc lập, không có thanh địa chỉ, không có tab, mang lại trải nghiệm 100% giống một phần mềm Desktop truyền thống.

### 4. Các form có giao diện hoàn chỉnh
**Giải trình:**
Mỗi form đều được thiết kế bố cục (layout) tiêu chuẩn bao gồm:
- Thanh Sidebar bên trái để điều hướng.
- Thanh Header chứa Tiêu đề chức năng và Nút hành động chính (vd: "Thêm sách mới").
- Vùng nội dung (Grid/Table) hiển thị danh sách dữ liệu trực quan.
- Các thao tác Thêm/Sửa được bọc trong các Modal Pop-up (cửa sổ nổi) với form nhập liệu đầy đủ nhãn (label) và trường (input), mang lại trải nghiệm người dùng hiện đại, không cần tải lại trang hay mở form lắt nhắt.

### 5. Từ form này có thể nhảy sang form khác
**Giải trình:**
Hệ thống có một thanh **Sidebar Navigation** được thiết kế cố định bên trái (CSS class `.sidebar-nav` trong `style.css`). 
Người dùng có thể dễ dàng bấm vào các mục "Trang chủ", "Quản lý Sách", "Quản lý Độc giả", "Mượn / Trả sách" để chuyển đổi linh hoạt và mượt mà giữa các form HTML mà không gặp bất kỳ cản trở nào. Mọi file HTML đều được liên kết chặt chẽ thông qua thẻ `<a>`.

### 6. Các control tuân theo quy tắc đặt tên đã học ở lớp
**Giải trình:**
Em đã tuân thủ nghiêm ngặt quy ước đặt tên tiền tố (Hungarian notation) cho toàn bộ các control HTML để phục vụ việc thao tác logic qua JavaScript. Minh chứng cụ thể trong code:
- **TextBox/Input**: `txtUsername`, `txtPassword` (ở `login.html`); `txtMaSach`, `txtTenSach`, `txtTacGia`, `txtNamXB` (ở `quanly_sach.html`).
- **ComboBox/Select**: `cboTheLoai` (chọn thể loại sách), `cboDocGia` (chọn người mượn), `cboSach` (chọn sách mượn trong `quanly_muontra.html`).
- **Button**: `btnSubmit` (đăng nhập), `btnLuuSach` (thêm sách), `btnLapPhieu` (tạo phiếu mượn).

### 7. Giao diện phải có trang trí, phải thay được biểu tượng chính của chương trình
**Giải trình:**
- **Về trang trí**: Giao diện được code CSS bài bản (file `wwwroot/css/style.css`) sử dụng phong cách thiết kế **Glassmorphism** (kính mờ) hiện đại. Tông màu chủ đạo là Xanh Navy (`#1a237e`) và Xanh Teal (`#00838f`). Các form có đổ bóng (box-shadow), bo góc (border-radius), và hiệu ứng micro-interactions (hover mượt mà). Bảng danh sách sử dụng Zebra-striping (phân màu dòng chẵn lẻ) và các Badge (nhãn) màu sắc để phân biệt trạng thái (vd: "Hoạt động", "Đã trả", "Đang mượn").
- **Về Biểu tượng (Icon)**: Em không sử dụng biểu tượng web mặc định. Thay vào đó, em đã thiết kế riêng một Favicon (`favicon.png`) và chèn vào tất cả các form qua thẻ `<link rel="icon" type="image/png" href="img/favicon.png">`. Khi ứng dụng bật lên ở chế độ Desktop, icon dưới thanh Taskbar của Windows và góc trên cửa sổ hiển thị chính xác icon Thư viện của phần mềm. Góc trái trên cùng Menu luôn có một `logo.png` tùy chỉnh mang tên "THƯ VIỆN".

---
*Em xin cảm ơn Thầy/Cô đã dành thời gian đánh giá và trải nghiệm dự án của em!*
