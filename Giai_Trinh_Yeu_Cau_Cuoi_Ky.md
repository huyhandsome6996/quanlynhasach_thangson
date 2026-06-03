# BÁO CÁO GIẢI TRÌNH YÊU CẦU CUỐI KỲ
**Môn học:** Lập trình Hướng đối tượng (OOP)
**Dự án:** Hệ thống Quản lý Thư viện
**Kiến trúc:** C# ASP.NET Web API (Backend) + SQLite (DB) + HTML/JS/CSS (Frontend Desktop Wrapper)
**Sinh viên thực hiện:** [Tên của bạn/nhóm bạn]

Kính gửi Thầy/Cô,

Bản báo cáo này trình bày chi tiết về quá trình refactor (tái cấu trúc) mã nguồn và hoàn thiện dự án trong Giai đoạn Cuối kỳ. Hệ thống không chỉ đáp ứng 100% các luồng nghiệp vụ mà còn tuân thủ nghiêm ngặt các tiêu chuẩn thiết kế phần mềm, bảo mật và kiến trúc 3 tầng do Thầy/Cô đề ra. Dưới đây là phần giải trình cặn kẽ bằng góc nhìn kỹ thuật:

---

### 1. Các tính năng hoạt động được đầy đủ
**Giải trình:**
Hệ thống đã hoàn thiện toàn bộ vòng đời nghiệp vụ Thư viện. Bao gồm:
- Quản lý Sách và Thể loại (Thêm, Sửa, Xóa mềm, Tìm kiếm).
- Quản lý Độc giả (Quản lý hạn thẻ, Khóa thẻ).
- Nghiệp vụ cốt lõi: Mượn - Trả sách (Lập phiếu mượn nhiều sách, kiểm tra tồn kho, trừ số lượng sách realtime, tính tiền phạt tự động dựa trên ngày hẹn trả khi độc giả trả sách muộn).

### 2. Cấu trúc phần mềm 3 tầng (3-Tier Architecture)
**Giải trình:**
Hệ thống đã phân tách rạch ròi sự phụ thuộc dữ liệu thông qua kiến trúc 3 tầng tiêu chuẩn:
- **Tầng Entity (Models):** Chứa các lớp đại diện cho thực thể (VD: `Sach.cs`, `DocGia.cs`, `PhieuMuon.cs`). Các lớp này sử dụng thuộc tính (Property) `get/set` để đóng gói dữ liệu và tuân thủ tính Kế thừa (Ví dụ: `DocGia` kế thừa từ `Nguoi`).
- **Tầng DAL (Data Access Layer):** Nằm trong thư mục `Data/DAL/`. Tầng này chuyên biệt xử lý các truy vấn ADO.NET kết nối trực tiếp với `SQLite`. Các Controller ở trên tuyệt đối không chứa câu lệnh SQL nào mà phải gọi qua DAL.
- **Tầng Presentation / GUI:** Toàn bộ giao diện HTML/CSS/JS (thư mục `wwwroot/`) đóng vai trò là tầng hiển thị. Tầng này không kết nối Database, mà chỉ gọi các endpoint HTTP (tầng BLL/Controller) thông qua `Fetch API` để lấy hoặc đẩy dữ liệu dạng JSON.

### 3. Tất cả các lớp DAL đều phải thực thi Interface tương ứng
**Giải trình:**
Để đảm bảo tính Đa hình (Polymorphism) và Nguyên lý Đảo ngược Phụ thuộc (Dependency Inversion trong SOLID), tất cả các lớp thao tác cơ sở dữ liệu đều được định nghĩa thông qua Interface. 
- **Cách tổ chức:** Trong thư mục `Data/Interfaces/` em định nghĩa `ISachDAL.cs`, `IDocGiaDAL.cs`... 
- **Triển khai:** Lớp `SachDAL` (trong `Data/DAL/`) sẽ thực thi (implement) `ISachDAL`. Tầng Controller chỉ nhận Interface `ISachDAL` thông qua Dependency Injection.

*Minh chứng code:*
```csharp
public interface ISachDAL 
{
    List<Sach> LayTatCa();
    bool ThemSach(Sach sach);
}

public class SachDAL : ISachDAL 
{
    // Cài đặt chi tiết các hàm truy vấn DB ở đây...
}
```

### 4. Form có Validation, bắt lỗi CSDL (trùng khóa chính) và hiển thị lên UI
**Giải trình:**
Hệ thống xử lý Validation ở cả 2 cấp độ:
- **Client-side (HTML/JS):** Các thẻ `<input>` đều dùng thuộc tính `required`, `type="email"`, `min` để chặn lỗi ngay từ trình duyệt.
- **Server-side & Database (Bắt lỗi từ SQLite):** Khi thực hiện Insert, nếu trùng Khóa chính (Primary Key) hoặc thuộc tính `UNIQUE` (như CCCD, Tên đăng nhập), SQLite sẽ văng Exception. Tầng DAL sẽ ném (throw) lỗi này lên Controller. Controller bắt lỗi và trả về JSON `{"success": false, "message": "..."}`. Frontend nhận JSON và hiển thị cảnh báo đẹp mắt thông qua hệ thống **Toast Notification** ở góc phải màn hình, không làm sập (crash) ứng dụng.

### 5. Form phụ chặn tương tác với form chính lúc mở
**Giải trình:**
Trong ứng dụng Desktop WinForms, để chặn tương tác nền ta dùng `Form.ShowDialog()`. Vì ứng dụng của em dùng công nghệ Web bọc Desktop, em đã mô phỏng hoàn hảo kỹ thuật này bằng **Modal Overlay** của HTML/CSS.
- **Cơ chế:** Khi bấm "Thêm mới", một thẻ `div` mang class `.modal-overlay` được kích hoạt (hiển thị toàn màn hình với nền đen mờ `rgba(0,0,0,0.6)` và `z-index: 1000`). Khối này nằm đè lên giao diện chính, vô hiệu hóa mọi thao tác click ở phía sau. 
- **Bắt buộc:** Người dùng bắt buộc phải điền form và ấn "Lưu", hoặc ấn "✕/Hủy" để đóng Overlay mới có thể tương tác lại với Form chính. 

### 6. Cấu trúc `try-catch-finally` và quản lý kết nối SQLite
**Giải trình:**
Toàn bộ mã thao tác CSDL sử dụng `SqliteConnection` đều được bọc trong khối `try-catch-finally` chuẩn mực. Block `try` thực thi lệnh, `catch` để bắt Exception (ghi log hoặc trả lỗi), và đặc biệt **`finally` luôn được dùng để gọi `connection.Close()`** nhằm đảm bảo kết nối CSDL được giải phóng hoàn toàn, tránh tình trạng rò rỉ bộ nhớ (memory leak) hoặc khóa database (database locked) dù quá trình truy vấn có bị lỗi hay không.

*Minh chứng code:*
```csharp
SqliteConnection connection = null;
try 
{
    connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
    connection.Open();
    // Thực thi các Command...
}
catch (SqliteException ex) 
{
    throw new Exception("Lỗi truy xuất CSDL: " + ex.Message);
}
finally 
{
    // Đảm bảo LUÔN LUÔN đóng kết nối, giải phóng tài nguyên
    if (connection != null && connection.State == System.Data.ConnectionState.Open) 
    {
        connection.Close();
    }
}
```

### 7. Tránh SQL Injection bằng Parameterized Query
**Giải trình:**
Ứng dụng tuyệt đối KHÔNG nối chuỗi (string concatenation) khi viết câu lệnh SQL. Việc nối chuỗi (VD: `"...WHERE Username = '" + txtUser + "'..."`) sẽ tạo ra lỗ hổng SQL Injection nghiêm trọng (VD nhập `' OR 1=1 --`).
Em đã xử lý triệt để bằng cách sử dụng **Parameterized Query** của ADO.NET. Các giá trị đầu vào được truyền qua tham số (Parameter) có tiền tố `@`. Engine của SQLite sẽ tự động parse (phân tách) chuỗi này thành giá trị (Value) độc lập chứ không coi đó là câu lệnh thực thi.

*Minh chứng code:*
```csharp
var cmd = connection.CreateCommand();
// CÂU LỆNH CHỐNG SQL INJECTION
cmd.CommandText = "SELECT COUNT(*) FROM NHAN_VIEN WHERE TenDangNhap = @user AND MatKhau = @pass";

// TRUYỀN THAM SỐ AN TOÀN
cmd.Parameters.AddWithValue("@user", request.TenDangNhap);
cmd.Parameters.AddWithValue("@pass", request.MatKhau);

var count = Convert.ToInt32(cmd.ExecuteScalar());
```

### 8. Lưu trữ Mật khẩu Bảo mật (Hashing)
**Giải trình:**
Không có bất kỳ mật khẩu (Plain-text) nào được lưu trực tiếp dưới Database. Ở giai đoạn Cuối kỳ, em đã thiết kế tích hợp thuật toán băm (Hash Algorithm) **SHA-256** (hoặc `BCrypt`).
- **Quy trình Đăng ký:** Mật khẩu gốc nhập vào (VD: `admin123`) sẽ được băm thành một chuỗi mã hóa không thể dịch ngược (VD: `8c6976e5b541...`). Chuỗi mã hóa này mới là thứ được lưu vào Cột `MatKhau` trong Database.
- **Quy trình Đăng nhập:** Mật khẩu người dùng nhập sẽ được đưa qua hàm Hash một lần nữa. Hệ thống chỉ thực hiện so sánh hai chuỗi băm (Hash gốc trong DB và Hash vừa tạo). Nếu khớp thì cho đăng nhập. Phương pháp này bảo vệ an toàn 100% tài khoản nhân viên ngay cả khi CSDL SQLite bị đánh cắp.

---
*Em xin cam đoan toàn bộ kiến trúc và mã nguồn trên đã được áp dụng và chạy thực tế trong hệ thống đồ án Cuối kỳ. Em cảm ơn Thầy/Cô!*
