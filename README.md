# Hệ thống Quản lý Trung tâm Thương mại (Commercial Shopping Mall Management System)

> **Bài tập lớn Lập trình Hướng đối tượng (MI4090)** — Nhóm 02 Sinh viên (ĐHBK Hà Nội)

## 1. Tổng quan dự án

Hệ thống Thông tin Quản lý Trung tâm Thương mại (TTTM) được xây dựng theo kiến trúc **3-Layer + Shared Domain** trên nền tảng **C# / .NET 8.0 WinForms** và **Microsoft SQL Server**, nhằm số hóa toàn bộ quy trình vận hành cốt lõi của TTTM:

- **Phân hệ 1 (TV1 — Core Architect): Quản lý Gian hàng, Mặt bằng & Hợp đồng Thuê**
  - Quy hoạch phân khu, vị trí ki-ốt/gian hàng theo tầng, diện tích, đơn giá cơ sở.
  - Quản lý hồ sơ đối tác/thương hiệu thuê (F&B, Thời trang, Công nghệ, Rạp phim...).
  - Lập & quản lý Hợp đồng thuê mặt bằng (Master-Detail), Biên bản bàn giao hiện trạng, thanh lý hợp đồng.
- **Phân hệ 2 (TV2 — Core Developer & Quality Lead): Quản lý Dịch vụ Tiện ích, Hóa đơn & Công nợ**
  - Quản lý danh mục dịch vụ tiện ích (điện, nước, điều hòa, phí quản lý chung) & bảng giá lũy tiến bậc thang.
  - Chốt chỉ số điện/nước định kỳ, lập Hóa đơn tổng hợp hàng tháng (kết nối tiền thuê từ Phân hệ 1), thu tiền & theo dõi công nợ.
- **Phần Hợp nhất chung:**
  - Giao diện hợp nhất `frm_main_shell` + đăng nhập `frm000_dang_nhap` (BCrypt, phân quyền RBAC 3 vai trò: `Admin`, `LeasingStaff`, `BillingStaff`).
  - CSDL 3NF 12 bảng thống nhất, giao tiếp liên phân hệ qua Interface (`ILeasingContractService`, `IBillingService`) và Domain Events.

## 2. Yêu cầu môi trường

- **Hệ điều hành:** Windows 10 / 11
- **SDK:** .NET 8.0 SDK (hoặc mới hơn hỗ trợ target `net8.0` / `net8.0-windows`)
- **IDE:** Visual Studio 2022 (khuyến nghị) hoặc VS Code / JetBrains Rider
- **CSDL:** Microsoft SQL Server 2019/2022 hoặc SQL Server LocalDB
- **Kiểm thử:** xUnit, Moq

## 3. Cấu trúc thư mục Solution

```text
OOP_TTTM/
├── src/
│   ├── TTTM.GUI/        # Tầng Giao diện (WinForms - frm000_, frm001_dm_, frm002_nv_)
│   ├── TTTM.BLL/        # Tầng Nghiệp vụ (Services, Business Rules, GoF Patterns)
│   ├── TTTM.DAL/        # Tầng Truy cập Dữ liệu (Repositories, Parameterized SQL)
│   ├── TTTM.Domain/     # Tầng Miền dùng chung (Entities, Enums, Contracts, Events)
│   └── TTTM.DTO/        # Data Transfer Objects
├── tests/
│   └── TTTM.Tests/      # Unit Tests (xUnit + Moq) & Integration Tests
├── database/            # Script tạo Schema 3NF & Seed Data
├── docs/                # Tài liệu khảo sát, BFD, Use Case, UML, Quy chuẩn
├── TTTM.sln
└── README.md
```

## 4. Hướng dẫn Build & Test nhanh

```bash
# Khôi phục gói và build toàn bộ Solution
dotnet restore TTTM.sln
dotnet build TTTM.sln --configuration Release

# Chạy bộ kiểm thử tự động
dotnet test TTTM.sln
```

## 5. Tài liệu liên quan

- [Quy chuẩn Lập trình & Kiến trúc 3-Layer](docs/CodingConventions.md)
