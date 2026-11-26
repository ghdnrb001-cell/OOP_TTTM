# Quy chuẩn Lập trình & Kiến trúc 3-Layer (Coding Conventions)

Tài liệu này quy định các tiêu chuẩn bắt buộc cho cả 2 thành viên (TV1 & TV2) trong quá trình phát triển hệ thống **Quản lý Trung tâm Thương mại (TTTM)** trên nền tảng **C# / .NET 8.0 WinForms**.

---

## 1. Kiến trúc Phân tầng & Quy tắc Tham chiếu 1 chiều

Hệ thống áp dụng kiến trúc **3-Layer + Shared Domain** kết hợp **Dependency Inversion Principle (DIP)**:

```text
  TTTM.GUI  ──▶  TTTM.BLL  ──▶  TTTM.Domain  ◀──  TTTM.DAL
     │              │                                 ▲
     │              └─────────── (Interfaces) ────────┘
     ▼
  TTTM.DTO  (được dùng làm dữ liệu truyền tải giữa GUI ↔ BLL)
```

### Quy tắc bắt buộc (Chống Circular Dependency & Layer Violation)
1. **Tham chiếu 1 chiều tuyệt đối:**
   - `TTTM.GUI` → `TTTM.BLL`, `TTTM.DTO`, `TTTM.Domain` (cho phép tham chiếu `TTTM.DAL` **chỉ** tại Composition Root nếu đăng ký DI).
   - `TTTM.BLL` → `TTTM.Domain`, `TTTM.DTO`, `TTTM.DAL` (giao tiếp qua Repository Interfaces).
   - `TTTM.DAL` → `TTTM.Domain`, `TTTM.DTO`.
   - `TTTM.Domain` → **Không tham chiếu** bất kỳ project nào khác trong Solution.
   - `TTTM.DTO` → **Không tham chiếu** `GUI`, `BLL`, `DAL`.
   - `TTTM.Tests` → `TTTM.BLL`, `TTTM.Domain`, `TTTM.DTO`, `TTTM.DAL`.
2. **Cách ly công nghệ giữa các tầng:**
   - **Tầng `GUI`:** Tuyệt đối **không** chứa câu lệnh SQL, `SqlConnection`, `SqlCommand` hay logic tính toán nghiệp vụ.
   - **Tầng `Domain` & `BLL`:** Tuyệt đối **không** `using System.Windows.Forms;` hoặc `System.Data.SqlClient;`.
   - **Tầng `DAL`:** Sử dụng **100% Parameterized SQL** (`@Param`), cấm tuyệt đối nối chuỗi SQL.

---

## 2. Quy ước Đặt tên (Naming Conventions)

| Đối tượng | Quy tắc | Ví dụ chuẩn |
| :--- | :--- | :--- |
| **Class / Struct / Record / Enum** | `PascalCase` | `LeaseContract`, `LeasingService`, `ContractStatus` |
| **Interface** | `I` + `PascalCase` | `ILeasingContractService`, `IBillingService` |
| **Method / Property** | `PascalCase` | `CalculateDeposit()`, `TotalAmount` |
| **Tham số & Biến cục bộ** | `camelCase` | `contractId`, `monthlyRent` |
| **Biến `private` / `protected` field** | Tiền tố `_` + `camelCase` | `_leasingRepository`, `_details` |
| **Hằng số (`const` / `static readonly`)** | `PascalCase` hoặc `UPPER_SNAKE_CASE` | `MinDepositMonths`, `MAX_RETRY_COUNT` |
| **Form Hệ thống / Đăng nhập** | Tiền tố `frm000_` hoặc `frm_main_shell` | `frm000_dang_nhap`, `frm_main_shell` |
| **Form Danh mục** | Tiền tố `frm001_dm_` + `snake_case` | `frm001_dm_vi_tri_gian_hang`, `frm001_dm_khach_thue` |
| **Form Nghiệp vụ / Master-Detail** | Tiền tố `frm002_nv_` + `snake_case` | `frm002_nv_hop_dong_thue`, `frm002_nv_ban_giao_mat_bang` |

---

## 3. Quy chuẩn Thiết kế Domain & Kiểm thử

- **Chống Anemic Domain Model:** Các Entity trong `TTTM.Domain` sử dụng `private set`, validate bất biến qua Constructor và phương thức nghiệp vụ, các tập hợp con công khai qua `IReadOnlyList<T>`.
- **Validation 2 lớp:**
  - Lớp 1 (UI): Sử dụng `ErrorProvider` trên WinForms kiểm tra rỗng, định dạng, biên cơ bản.
  - Lớp 2 (Domain/BLL): Kiểm tra lại toàn bộ quy tắc nghiệp vụ và ném `CustomException` (`LeasingException`, `BillingException`).
- **Định dạng hiển thị UI thống nhất:**
  - Font chữ: `Segoe UI`, 9–10pt.
  - Tiền tệ: `#,##0 VNĐ` (căn phải).
  - Ngày tháng: `dd/MM/yyyy` (căn giữa).
- **Kiểm thử Unit Test (xUnit + Moq):**
  - Tuân thủ mẫu **AAA (Arrange - Act - Assert)**.
  - Giao tiếp liên phân hệ giữa TV1 và TV2 được Mock chéo qua `ILeasingContractService` và `IBillingService`.

---

## 4. Quy trình Git & Conventional Commits

- Nhánh gốc tích hợp: `develop` (nhánh `main` chỉ dùng cho release chính thức).
- Nhánh tính năng: `feature/<ten-chuc-nang>` hoặc `feature/tv1-<ten>`, `feature/tv2-<ten>`.
- Chuẩn Commit Message (**Conventional Commits**):
  - `feat(scope): ...` — Thêm tính năng mới
  - `fix(scope): ...` — Sửa lỗi
  - `docs(scope): ...` — Tài liệu, sơ đồ phân tích
  - `test(scope): ...` — Bổ sung/sửa Unit & Integration Tests
  - `refactor(scope): ...` — Tái cấu trúc mã nguồn
  - `chore(scope): ...` — Cấu hình repo, build, packaging
