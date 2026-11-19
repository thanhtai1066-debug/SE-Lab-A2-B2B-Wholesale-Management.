# PHÂN CHIA SOURCE CODE CHO NHÓM 4 NGƯỜI

Dự án: **B2B Wholesale Management System** (Software Engineering Lab Assignment 2, HK1 2627)  
GitHub repo đề xuất: `SE-Lab-A2-B2B-Wholesale-Management`  
Giải pháp: ASP.NET Core MVC (Option C, Code First) + WinForms + SQL Server + xUnit, .NET 8.

> Đây là **chia tệp của source hiện có**, không phải bằng chứng từng bạn đã tự viết hoặc đã chạy thành công. Mỗi thành viên cần hiểu, sửa, kiểm tra, commit và trình bày trung thực phần mình thực sự làm. Cả nhóm cùng dùng **một repository và một solution**; không tạo bốn repository khác nhau.

## 0) Trưởng nhóm: bộ khung chung trên `main`

ZIP: `Starter_Main.zip`, gồm `B2BWholesaleManagement.sln`, 4 project `.csproj`, `.gitignore`, `Readme.txt`, tài liệu hướng dẫn trong `docs/`. Đây là scaffold, **chưa đủ mã nguồn để chạy**, vì 4 nhóm sẽ thêm các file còn lại qua PR.

## 1) Người 1 — Database, Seed Data, Authentication

- Branch: `feature/database-auth`
- ZIP: `Nguoi_1_Database_Auth.zip`
- Phần phụ trách:
  - `database/SchemaAndSeed.sql`, `database/IntegrityAndSeedChecks.sql`
  - `src/Wholesale.Data/Entities.cs`: 5 entity Item, Agent, SalesOrder, OrderDetail, User.
  - `src/Wholesale.Data/WholesaleDbContext.cs`: EF Core DbContext, key/relationship/rules.
  - `src/Wholesale.Data/DemoSeeder.cs`: tạo dữ liệu mẫu.
  - `src/Wholesale.Data/AuthService.cs`: xác thực, vai trò Admin/Agent, khóa đăng nhập.
- Thực tế cần kiểm tra: 15–30 dòng/bảng, primary/foreign key, CHECK/UNIQUE, seed, tài khoản mẫu, mật khẩu băm. Chụp ERD/SQL screenshot.
- Gợi ý commit thực sự sau khi kiểm tra/chỉnh sửa: `feat(db): define schema and relations`; `feat(auth): add role login and seed data`.

## 2) Người 2 — WinForms Admin Back-Office

- Branch: `feature/winforms-admin`
- ZIP: `Nguoi_2_WinForms_Admin.zip`
- Phần phụ trách: `src/Wholesale.Desktop/` ngoại trừ `.csproj` chung ở starter.
  - `Program.cs`: khởi tạo app, nạp DB config.
  - `App.config`: kết nối SQL Server ở cấu hình, tránh hardcode.
  - `LoginForm.cs`: form đăng nhập chỉ Admin.
  - `MainForm.cs`: CRUD Item/Agent, quản lý trạng thái đơn và Dashboard (top sellers, low stock, doanh thu theo agent).
  - `ManifestPdf.cs`: xuất Shipping Manifest PDF.
- Thực tế cần kiểm tra: thêm/sửa/xóa item và agent, cập nhật kho và trạng thái, báo cáo PDF, dashboard; chụp WinForms running screenshots.
- Gợi ý commit thực sự: `feat(desktop): implement admin login`; `feat(desktop): add inventory and order screens`; `feat(desktop): add manifest and dashboard`.

## 3) Người 3 — ASP.NET Core MVC Agent Portal

- Branch: `feature/agent-portal`
- ZIP: `Nguoi_3_ASPNET_AgentPortal.zip`
- Phần phụ trách: `src/Wholesale.Web/` ngoại trừ `.csproj` chung ở starter.
  - `Program.cs`, `appsettings.json`, `Properties/launchSettings.json`.
  - `Controllers/AccountController.cs`, `CatalogController.cs`, `OrdersController.cs`, `HomeController.cs`.
  - `Models/*`, `Views/*`, `wwwroot/css/site.css`.
- Thực tế cần kiểm tra: chỉ Agent vào được portal; catalog; tạo đơn nhiều dòng; My Orders chỉ của agent đang đăng nhập; invoice in được; desktop/mobile responsive.
- **Phụ thuộc người 1 và 4:** `Program.cs` đăng ký `WholesaleDbContext`, `AuthService`, `OrderService`. Không sửa chồng các file business logic do người 4 chịu trách nhiệm.
- Gợi ý commit thực sự: `feat(web): implement agent login and catalog`; `feat(web): build order workflow and invoice`; `style(web): add responsive layout`.

## 4) Người 4 — Order Transaction, Business Rules và Unit Tests

- Branch: `feature/order-rules-tests`
- ZIP: `Nguoi_4_OrderLogic_UnitTests.zip`
- Phần phụ trách:
  - `src/Wholesale.Core/OrderRules.cs`: validate dòng đơn, tính tiền, khấu trừ tồn kho, lockout rule.
  - `src/Wholesale.Data/OrderService.cs`: transaction tạo Order + OrderDetail + trừ tồn, rollback khi lỗi/thiếu hàng, giới hạn quyền xem đơn.
  - `tests/Wholesale.Tests/OrderRulesTests.cs` và `OrderServiceTests.cs`: bộ test xUnit.
- Thực tế cần kiểm tra: test order hợp lệ, thiếu hàng, rollback khi dòng thứ 2 lỗi, tính tổng, chỉ đọc order của chính agent; chạy `dotnet test` và chụp Test Explorer.
- **Phụ thuộc người 1:** OrderService dùng entities và DbContext của người 1. **Người 3** gọi OrderService, không tạo phiên bản khác.
- Gợi ý commit thực sự: `feat(order): implement transactional stock deduction`; `test(order): cover order validation and rollback`.

## Quy tắc không xung đột

- `Starter_Main.zip` chỉ được import **một lần** để tạo repository gốc trên `main`.
- Mỗi người giải nén **chỉ gói của mình** trên feature branch, giữ nguyên đường dẫn thư mục. Cấm tự tạo solution khác.
- Không giải nén tất cả 4 ZIP rồi commit lên `main` trước khi chia nhánh: như vậy các PR sẽ không còn khác biệt để review.
- Tất cả pull request target `main`; được đồng đội review trước khi merge. Người phụ trách sửa lỗi phần mình; đổi file thuộc người khác cần báo và thống nhất trước.
- Gợi ý thứ tự merge: 1 (database/auth) → 4 (transaction/tests) → 3 (web) → 2 (WinForms), rồi cả nhóm test tích hợp. Có thể làm song song các branch.
- Chưa nên kỳ vọng build được trên `main` sau PR đầu tiên vì các chức năng phụ thuộc nhiều module. Build/test tích hợp sau khi đủ cả bốn PR.
- Chỉ ghi tên, % đóng góp, URL PR, test pass trong Report.docx khi đã có thông tin thật; không tạo commit/PR giả.

## Sau khi merge: checklist nghiệm thu

1. Trên Windows + Visual Studio/.NET 8 + SQL Server LocalDB: `dotnet restore B2BWholesaleManagement.sln`.
2. `dotnet build B2BWholesaleManagement.sln` và sửa lỗi thật nếu có.
3. `dotnet test B2BWholesaleManagement.sln` và chụp kết quả; 13 test được **định nghĩa** trong source gốc, chưa có xác nhận pass.
4. Chạy WinForms và Web, kiểm tra demo users trong README, luồng đặt hàng, cập nhật kho, hóa đơn, PDF.
5. Cập nhật Report.docx, team workbook, phần đóng góp, ERD, ảnh chụp hai app, PR, Git commit graph và video 3–5 phút.
6. Xuất cuối cùng `SourceCode.zip` từ toàn bộ repository sau merge, **bỏ** `bin/`, `obj/`, `.git/`; gửi kèm Report.docx và Readme.txt.
