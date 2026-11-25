# Hướng dẫn Cấu hình Bảo mật & Môi trường Máy chấm (STT 3 / Task T06)

Tài liệu này hướng dẫn cấu hình thành phần lõi bảo mật (`BCrypt.Net-Next`) và nạp cấu hình CSDL ngoài từ `appsettings.json` cho hệ thống **Quản lý Trung tâm Thương mại (OOP_TTTM)**.

---

## 1. Yêu cầu Môi trường Máy chấm (Clean-Room)

- **.NET SDK / Runtime**: `.NET 8.0` (Windows Desktop Runtime 8.0 cho WinForms)
- **CSDL**: Microsoft SQL Server 2019/2022 hoặc SQL Server LocalDB (`(localdb)\MSSQLLocalDB`)
- **Thư viện NuGet trong `TTTM.Core`**:
  - `BCrypt.Net-Next` (phiên bản `4.2.0`, Cost factor mặc định `11`)
  - `Microsoft.Extensions.Configuration.Json` (phiên bản `8.0.1`)

---

## 2. Cấu hình Tệp Ngoài `appsettings.json` (Không cần biên dịch lại)

Hệ thống sử dụng một tệp cấu hình JSON duy nhất đặt ngoài mã nguồn đã biên dịch (`src/TTTM.GUI/appsettings.json`, được sao chép ra thư mục chạy ứng dụng khi build). Tệp mẫu nằm ở thư mục gốc: `appsettings.example.json`.

### Cách thiết lập trên máy mới
1. Nếu chưa có `appsettings.json`, sao chép `appsettings.example.json` sang `src/TTTM.GUI/appsettings.json` (hoặc đặt trực tiếp cạnh tệp `.exe` trong thư mục phát hành).
2. Mở `appsettings.json` bằng trình soạn thảo văn bản bất kỳ và chỉnh sửa mục `ConnectionStrings:DefaultConnection`:
   - **Dùng SQL Server LocalDB (mặc định với Visual Studio):**
     ```json
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TTTM_Management;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
     ```
   - **Dùng SQL Server Express (Windows Authentication):**
     ```json
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TTTM_Management;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
     ```
   - **Dùng SQL Server Authentication:**
     ```json
     "DefaultConnection": "Server=localhost,1433;Database=TTTM_Management;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=True;MultipleActiveResultSets=True;"
     ```
3. **Không cần biên dịch lại**: `DatabaseConfig.Load()` đọc trực tiếp `appsettings.json` tại `AppDomain.CurrentDomain.BaseDirectory` trước (hoặc tự dò về `src/TTTM.GUI/appsettings.json` theo đường dẫn tương đối), tuyệt đối không hard-code đường dẫn máy cá nhân (`C:\Users\...`).

### Tham số hệ thống trong `appsettings.json`
| Nhóm | Khóa | Mặc định | Ý nghĩa |
| --- | --- | --- | --- |
| `ConnectionStrings` | `DefaultConnection` | LocalDB `TTTM_Management` | Chuỗi kết nối SQL Server chính |
| `DatabaseSettings` | `CommandTimeoutSeconds` | `30` | Thời gian chờ tối đa cho truy vấn SQL (giây) |
| `SecuritySettings` | `BCryptWorkFactor` | `11` | Chi phí băm mật khẩu BCrypt (4–31, chuẩn Rubric = 11) |
| `SecuritySettings` | `MaxFailedLoginAttempts` | `5` | Số lần đăng nhập sai tối đa |
| `SecuritySettings` | `LockoutMinutes` | `15` | Thời gian khóa tạm sau khi vượt ngưỡng sai |
| `ApplicationSettings` | `CurrencyFormat` | `#,##0 VNĐ` | Định dạng hiển thị tiền tệ chuẩn toàn hệ thống |
| `ApplicationSettings` | `DateFormat` | `dd/MM/yyyy` | Định dạng hiển thị ngày chuẩn toàn hệ thống |

---

## 3. Thành phần Mã nguồn trong `src/TTTM.Core`

- `src/TTTM.Core/Security/IPasswordHasher.cs` & `PasswordHasher.cs`:
  - Sử dụng `BCrypt.Net.BCrypt.HashPassword` với Salt ngẫu nhiên cho từng mật khẩu và WorkFactor mặc định `11`.
  - Hỗ trợ `VerifyPassword` (an toàn khi gặp hash rỗng hoặc sai định dạng `SaltParseException`) và `NeedsRehash` để nâng cấp hash cũ.
- `src/TTTM.Core/Config/IDatabaseConfig.cs` & `DatabaseConfig.cs`:
  - Nạp và kiểm tra hợp lệ `appsettings.json` (từ chối chuỗi kết nối rỗng hoặc còn placeholder mẫu).
  - Cung cấp `GetMaskedConnectionString()` để che mật khẩu khi hiển thị thông tin kết nối trên giao diện/log.

---

## 4. Ghi chú Tích hợp với Solution `TTTM.sln` (Sau khi PR #1, #2 hợp nhất vào `develop`)

1. Thêm project `src/TTTM.Core/TTTM.Core.csproj` vào `TTTM.sln`.
2. Thêm ProjectReference tới `TTTM.Core` từ `TTTM.DAL`, `TTTM.BLL`, `TTTM.GUI`, và `TTTM.Tests`.
3. Trong `src/TTTM.GUI/TTTM.GUI.csproj`, cấu hình sao chép `appsettings.json` ra thư mục output:
   ```xml
   <ItemGroup>
     <None Update="appsettings.json">
       <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
     </None>
   </ItemGroup>
   ```
