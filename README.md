# 📚 Hệ Thống Quản Lý Thư Viện

Hệ thống quản lý thư viện xây dựng bằng **ASP.NET Web API + HTML/CSS/JS + SQLite**, chạy như ứng dụng Desktop.

## 🚀 Cách chạy

```bash
# 1. Clone project
git clone <url-repo>

# 2. Restore packages
dotnet restore

# 3. Chạy ứng dụng (tự động mở giao diện Desktop)
dotnet run
```

## 🔑 Tài khoản mặc định

| Tên đăng nhập | Mật khẩu |
|---|---|
| `admin` | `admin123` |

## 🏗️ Công nghệ

- **Backend**: C# ASP.NET Core Web API
- **Frontend**: HTML, CSS, JavaScript (Vanilla)
- **Database**: SQLite
- **OOP**: Tính Đóng gói, Kế thừa, Đa hình

## 📂 Cấu trúc thư mục

```
├── Controllers/         # API Controllers (Auth, Sach, DocGia, TheLoai, MuonTra)
├── Models/              # Lớp thực thể OOP (Nguoi, Sach, DocGia, NhanVien, ...)
├── Data/                # Khởi tạo Database SQLite
├── wwwroot/             # Frontend (HTML, CSS, JS, Images)
│   ├── css/style.css
│   ├── img/
│   ├── login.html
│   ├── register.html
│   ├── index.html
│   ├── quanly_sach.html
│   ├── quanly_theloai.html
│   ├── quanly_docgia.html
│   └── quanly_muontra.html
└── Program.cs           # Entry point + Desktop Wrapper
```