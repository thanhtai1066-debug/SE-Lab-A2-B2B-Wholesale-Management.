# HƯỚNG DẪN PUSH GITHUB 4 NGƯỜI (WINDOWS / POWERSHELL)

**Đọc README_PHAN_CONG_4_NGUOI.md trước.** Mỗi người chỉ đẩy file mình thật sự phụ trách. Lệnh dưới đây giả định đã cài Git và có tài khoản GitHub.

## A. Trưởng nhóm tạo repository TRỐNG trên GitHub

Tên: `SE-Lab-A2-B2B-Wholesale-Management`.
Khi tạo repository, **KHÔNG** đánh dấu tạo sẵn README, .gitignore, license để tránh xung đột lúc push commit đầu.

Tạo thư mục trên máy tính, mở PowerShell trong thư mục đó và thực hiện (chỉnh đường dẫn ZIP phù hợp):

```powershell
mkdir SE-Lab-A2-B2B-Wholesale-Management
cd SE-Lab-A2-B2B-Wholesale-Management
Expand-Archive -Path "C:\Users\YOUR_NAME\Downloads\Starter_Main.zip" -DestinationPath . -Force
git init
git add .
git commit -m "chore: initialize solution and project references"
git branch -M main
git remote add origin https://github.com/GITHUB_OWNER/SE-Lab-A2-B2B-Wholesale-Management.git
git push -u origin main
```

Dùng URL repository của nhóm thay cho `GITHUB_OWNER`. Kiểm tra có `.sln`, `src`, `tests`, `docs`, `database`; `src` và `tests` chỉ có `.csproj` ở thời điểm này. Trưởng nhóm mời 3 bạn còn lại vào Settings → Collaborators.

## B. Mỗi người làm trên feature branch CỦA MÌNH

Mỗi bạn mở terminal riêng ở nơi muốn lưu source, chạy:

```powershell
git clone https://github.com/GITHUB_OWNER/SE-Lab-A2-B2B-Wholesale-Management.git
cd SE-Lab-A2-B2B-Wholesale-Management
```

Sau đó, **chỉ chọn một** nhóm lệnh bên dưới tương ứng với phần được giao. Đường dẫn ZIP phải trỏ đến file đã tải về.

### Người 1: Database + Auth

```powershell
git switch -c feature/database-auth
Expand-Archive -Path "C:\Users\YOUR_NAME\Downloads\Nguoi_1_Database_Auth.zip" -DestinationPath . -Force
git add database src/Wholesale.Data/Entities.cs src/Wholesale.Data/WholesaleDbContext.cs src/Wholesale.Data/DemoSeeder.cs src/Wholesale.Data/AuthService.cs
git status
git commit -m "feat(data): implement database schema seed and authentication"
git push -u origin feature/database-auth
```

### Người 2: WinForms

```powershell
git switch -c feature/winforms-admin
Expand-Archive -Path "C:\Users\YOUR_NAME\Downloads\Nguoi_2_WinForms_Admin.zip" -DestinationPath . -Force
git add src/Wholesale.Desktop
git status
git commit -m "feat(desktop): implement admin management and dashboard"
git push -u origin feature/winforms-admin
```

### Người 3: Agent Portal

```powershell
git switch -c feature/agent-portal
Expand-Archive -Path "C:\Users\YOUR_NAME\Downloads\Nguoi_3_ASPNET_AgentPortal.zip" -DestinationPath . -Force
git add src/Wholesale.Web
git status
git commit -m "feat(web): implement agent portal ordering and invoice"
git push -u origin feature/agent-portal
```

### Người 4: Order + Tests

```powershell
git switch -c feature/order-rules-tests
Expand-Archive -Path "C:\Users\YOUR_NAME\Downloads\Nguoi_4_OrderLogic_UnitTests.zip" -DestinationPath . -Force
git add src/Wholesale.Core/OrderRules.cs src/Wholesale.Data/OrderService.cs tests/Wholesale.Tests
git status
git commit -m "feat(order): implement transactional order service and tests"
git push -u origin feature/order-rules-tests
```

**Các commit mẫu chỉ nên thực hiện sau khi người phụ trách đã kiểm tra, hiểu và thực sự thay đổi/cải thiện phần code của mình.** Có thể chia thành nhiều commit hợp lý theo chức năng thật, thay vì 1 commit lớn.

## C. Tạo Pull Request đúng cách

1. Vào tab **Pull requests** trên repository GitHub → **New pull request**.
2. Base = `main`, Compare = branch của mình (ví dụ `feature/database-auth`).
3. Tiêu đề mô tả tính năng, mô tả file đã làm, cách đã test và ảnh minh chứng thực tế.
4. Mời thành viên khác review. Sau khi review, trưởng nhóm bấm **Merge pull request**.
5. Nếu báo conflict, fetch/pull mới nhất từ main rồi giải quyết trên branch riêng; không force push tùy tiện.

## D. Kiểm tra sau khi 4 nhánh được merge

```powershell
git switch main
git pull origin main
dotnet restore B2BWholesaleManagement.sln
dotnet build B2BWholesaleManagement.sln
dotnet test B2BWholesaleManagement.sln
dotnet run --project src/Wholesale.Web/Wholesale.Web.csproj
dotnet run --project src/Wholesale.Desktop/Wholesale.Desktop.csproj
```

**Lưu ý**: build và run WinForms cần Windows; SQL Server cần được cài/cấu hình. Hiện chưa có bằng chứng 13 test pass. Nếu có lỗi, sửa rồi push bằng commit thật và mở PR sửa tiếp.

## E. Đóng gói nộp bài

- Update README, report, workbook theo thông tin thật.
- Xuất 1 SourceCode.zip từ repo cuối cùng, loại bin/obj/.git/.
- Báo cáo phải có screenshot running app, test, ERD, Git PR và thành viên.
- Các ZIP chia việc là **để cộng tác**, không phải 4 SourceCode.zip để nộp tách rời.
