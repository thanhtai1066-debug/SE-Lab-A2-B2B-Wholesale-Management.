# Hướng dẫn chạy trên máy Windows (từng bước)

## 1. Chuẩn bị

- Cài Visual Studio 2022 kèm hai workload **ASP.NET and web development** và **.NET desktop development**.
- Cài .NET 8 SDK nếu Visual Studio chưa có.
- Cài SQL Server Express LocalDB bằng Visual Studio Installer (Individual Components / SQL Server Express LocalDB).

Mở PowerShell trong thư mục giải nén `SE-Lab-A2-B2B-Wholesale-Management`:

```powershell
dotnet --version
sqllocaldb info
```

Nếu không thấy `MSSQLLocalDB`, cài LocalDB và tạo instance theo tài liệu chính thức của SQL Server hoặc cấu hình lại chuỗi kết nối tới SQL Server cậu đang có.

## 2. Mở solution

Mở `B2BWholesaleManagement.sln` bằng Visual Studio 2022; Restore NuGet packages.
Các project được tách thành Core, Data, Web, Desktop và Tests.

## 3. Tạo database

Có hai cách, **chỉ cần một**:

- **Cách A (khuyên dùng):** Chạy `Wholesale.Web`; code EF Core `EnsureCreatedAsync()` tự tạo database và seed dữ liệu mẫu. Không cần import SQL thủ công.
- **Cách B:** Mở SSMS chạy file `database/SchemaAndSeed.sql` trên SQL Server LocalDB/Express trống. Sau đó chạy Web/Desktop; seeder phát hiện database đã có dữ liệu.

Lưu ý: LocalDB chỉ kết nối tới cùng một SQL Server instance. Nếu sửa connection string, phải sửa cả:

```
src/Wholesale.Web/appsettings.json
src/Wholesale.Desktop/App.config
```

## 4. Chạy Web Agent Portal

```powershell
dotnet restore B2BWholesaleManagement.sln
dotnet run --project src/Wholesale.Web/Wholesale.Web.csproj --launch-profile Wholesale.Web
```

Vào `https://localhost:7148` (hoặc `http://localhost:5148`). Đăng nhập bằng tài khoản demo: `agent01` / `Agent@12345!`.

Thử chọn số lượng ở ít nhất 2 sản phẩm rồi bấm **Place order**. Trang Invoice cho phép in và xuất PDF bằng trình duyệt. Xem mục **My Orders** để kiểm tra trạng thái và đơn cũ.

## 5. Chạy WinForms back-office

Trong Visual Studio, chuột phải project `Wholesale.Desktop` -> **Set as Startup Project** -> **Start** (F5), hoặc gõ:

```powershell
dotnet run --project src/Wholesale.Desktop/Wholesale.Desktop.csproj
```

Đăng nhập `admin` / `Admin@12345!`. Kiểm tra lần lượt các tab **Items & Stock**, **Agents**, **Order Management**, **Shipping Manifest**, **Sales Dashboard**.

## 6. Chạy unit tests

```powershell
dotnet test B2BWholesaleManagement.sln
```

Tests chạy SQLite trong bộ nhớ nên không cần mở SQL Server. Chỉ ghi kết quả pass/fail lên báo cáo sau khi xem kết quả thực tế ở Terminal/Test Explorer.

## 7. Nộp bài

- Báo cáo Word đã có nội dung code và đúng form, nhưng cần bổ sung **MSSV, họ tên, phân công, ảnh thật và link GitHub/video**.
- `SourceCode.zip` có solution và `.sql`, không có thư mục `bin`/`obj`.
- `Readme.txt` có hướng dẫn DB và chỗ điền link / phần trăm đóng góp.
- Tạo video 3–5 phút quay màn hình chạy thật. Dùng file `VIDEO_DEMO_3_TO_5_MINUTES.md` làm dàn ý.
- Đổi tên file ZIP tổng thành định dạng giảng viên yêu cầu (MSSV và họ tên thành viên).

**Quan trọng:** Bộ source được tạo trên môi trường không có SDK .NET và không chạy được Windows Forms nên chưa thể khẳng định đã build và chạy thành công. Nếu xuất hiện lỗi cụ thể trên máy Windows, dùng thông báo từ Visual Studio để sửa đúng chỗ.
