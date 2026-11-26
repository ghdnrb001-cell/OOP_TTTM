# Đặc tả 10 Use Case Cốt lõi & 2 Luồng Nghiệp vụ Liên phân hệ (TV1 ↔ TV2)

- **Môn học:** Lập trình Hướng đối tượng (MI4090)
- **Đề tài:** Hệ thống Quản lý Trung tâm Thương mại (TTTM)
- **Phạm vi:** Toàn hệ thống — Phân hệ 1 (TV1) & Phân hệ 2 (TV2)
- **Người thực hiện:** Thành viên 1 (TV1 — Core Architect) phối hợp thống nhất cùng Thành viên 2 (TV2)

---

## 1. Danh sách Tác nhân (Actors & Phân quyền RBAC)

| Tác nhân (Role) | Vai trò trong TTTM | Phạm vi quyền hạn tại tầng BLL |
| :--- | :--- | :--- |
| **Ban Quản lý (`Admin`)** | Quản trị viên / Giám đốc TTTM | Toàn quyền trên cả 2 phân hệ; **duy nhất** có quyền duyệt Hợp đồng > 500 triệu VNĐ, xóa/hủy hóa đơn đã phát hành, xem Dashboard tổng hợp BC5. |
| **Nhân viên Mặt bằng (`LeasingStaff`)** | Nhân viên Kinh doanh Mặt bằng (TV1) | Quản lý phân khu/gian hàng, hồ sơ khách thuê, lập hợp đồng (gửi duyệt nếu > 500tr), lập biên bản bàn giao, thanh lý hợp đồng, xem BC1 & BC2. |
| **Kế toán Thu phí (`BillingStaff`)** | Kế toán Dịch vụ & Công nợ (TV2) | Quản lý dịch vụ & bảng giá bậc thang, chốt chỉ số điện/nước, lập hóa đơn tháng, lập phiếu thu công nợ, xem BC3 & BC4. |

---

## 2. Bảng Tổng hợp 10 Use Case Cốt lõi

| Mã UC | Tên Use Case | Phân hệ | Loại UC | Actor Chính | Form / Service Đối ứng |
| :---: | :--- | :---: | :--- | :--- | :--- |
| **UC01** | Quản lý Phân khu & Vị trí Gian hàng (Lọc đa tiêu chí) | TV1 | Danh mục nâng cao | `LeasingStaff`, `Admin` | `frm001_dm_vi_tri_gian_hang` / `SpaceAllocationService` |
| **UC02** | Quản lý Hồ sơ Khách thuê & Tra cứu Sơ đồ Mặt bằng | TV1 | Danh mục + Tra cứu | `LeasingStaff`, `Admin` | `frm001_dm_khach_thue`, `frm002_nv_tra_cuu_mat_bang` |
| **UC03** | Lập & Phê duyệt Hợp đồng Thuê Mặt bằng | TV1 | **Phức hợp (M-D 1)** | `LeasingStaff`, `Admin` | `frm002_nv_hop_dong_thue` / `LeasingService` |
| **UC04** | Lập Biên bản Bàn giao Hiện trạng Nhận/Trả Mặt bằng | TV1 | **Phức hợp (M-D 2)** | `LeasingStaff`, `Admin` | `frm002_nv_ban_giao_mat_bang` / `LeasingService` |
| **UC05** | Thanh lý Hợp đồng & Giải phóng Mặt bằng | **TV1 → TV2** | **Liên phân hệ 1** | `LeasingStaff`, `Admin` | `frm002_nv_hop_dong_thue` / `LeasingService` + `IBillingService` |
| **UC06** | Quản lý Danh mục Dịch vụ, Bảng giá Bậc thang & PTTT | TV2 | Danh mục | `BillingStaff`, `Admin` | `frm001_dm_loai_dich_vu`, `frm001_dm_phuong_thuc_thanh_toan` |
| **UC07** | Chốt Chỉ số Điện/Nước Định kỳ Hàng tháng | TV2 | **Phức hợp (M-D 3)** | `BillingStaff`, `Admin` | `frm002_nv_ghi_chi_so_dich_vu` / `UtilityBillingService` |
| **UC08** | Lập Hóa đơn Tổng hợp Hàng tháng | **TV2 → TV1** | **Phức hợp (M-D 4) & Liên PH 2** | `BillingStaff`, `Admin` | `frm002_nv_lap_hoa_don_thang` / `UtilityBillingService` + `ILeasingContractService` |
| **UC09** | Thu tiền, Cấn trừ Công nợ & Tính Phạt Chậm nộp | TV2 | Nghiệp vụ giao dịch | `BillingStaff`, `Admin` | `frm002_nv_thu_tien_cong_no` / `PaymentDebtService` |
| **UC10** | Thống kê Báo cáo Phân hệ & Dashboard Doanh thu TTTM | Chung | Báo cáo Tổng hợp | `Admin`, `LeasingStaff`, `BillingStaff` | `BC1`–`BC5` (ClosedXML, QuestPDF, LiveCharts) |

---

## 3. Đặc tả Chi tiết 10 Use Case Cốt lõi

### UC01: Quản lý Phân khu & Vị trí Gian hàng (TV1)
- **Actor:** `LeasingStaff`, `Admin`.
- **Tiền điều kiện:** Người dùng đã đăng nhập thành công (`frm000_dang_nhap`) với vai trò hợp lệ.
- **Luồng chính:**
  1. Mở `frm001_dm_vi_tri_gian_hang`, hệ thống nạp danh sách khu vực (`KhuVuc`) và gian hàng (`ViTriGianHang`).
  2. Người dùng thêm/sửa/xóa thông tin lô (Mã lô, Khu vực/Tầng, Diện tích, Đơn giá cơ sở, Trạng thái).
  3. Người dùng sử dụng bộ lọc đa tiêu chí (Tầng, Diện tích từ–đến, Trạng thái) hoặc nhấn **Xóa lọc**.
  4. Hệ thống validate 2 lớp (`ErrorProvider` + Domain) và lưu vào CSDL qua `LeasingRepository`.
- **Ngoại lệ:** Diện tích $\le 0$ hoặc đơn giá $\le 0$ → ném `LeasingException`; không cho xóa gian hàng đang có Hợp đồng `Active`.

### UC02: Quản lý Hồ sơ Khách thuê & Tra cứu Sơ đồ Mặt bằng (TV1)
- **Actor:** `LeasingStaff`, `Admin`.
- **Tiền điều kiện:** Đã đăng nhập.
- **Luồng chính:**
  1. Tại `frm001_dm_khach_thue`, thêm/cập nhật đối tác thuê (Tên thương hiệu, MST 10–13 chữ số, Ngành hàng, Email, SĐT) và tra cứu lịch sử hợp đồng.
  2. Tại `frm002_nv_tra_cuu_mat_bang`, chọn tầng để xem trực quan trạng thái các lô (`Trong`: Xanh, `DangThue`: Đỏ, `DatCoc`: Vàng, `BaoTri`: Xám).
- **Ngoại lệ:** Trùng Mã số thuế (`UNIQUE`) hoặc sai định dạng Email/SĐT → hiển thị `ErrorProvider` và chặn lưu tại BLL.

### UC03: Lập & Phê duyệt Hợp đồng Thuê Mặt bằng (TV1 — Master-Detail)
- **Actor:** `LeasingStaff` (Lập HĐ), `Admin` (Phê duyệt HĐ > 500 triệu).
- **Tiền điều kiện:** Khách thuê đã tồn tại; các gian hàng được chọn đang ở trạng thái `Trong`.
- **Luồng chính:**
  1. Mở `frm002_nv_hop_dong_thue`, nhập thông tin Master (Khách thuê, Ngày ký, Ngày bắt đầu, Ngày kết thúc, Chiến lược giá `ILeasePricingStrategy`).
  2. Thêm các lô mặt bằng vào lưới Detail (`ChiTietHopDong_MatBang`). Hệ thống tự động áp dụng Strategy Pattern (`Standard`, `EarlyBird` -10%, `LongTerm` -15%) để tính đơn giá thỏa thuận, tổng tiền thuê và tiền cọc tối thiểu ($\ge 3$ tháng).
  3. Lưu hợp đồng trong `TransactionScope` (trạng thái `Draft`).
  4. Thực hiện phê duyệt (`ApproveContract`): chuyển hợp đồng sang `Active` và cập nhật trạng thái các lô liên quan sang `DangThue`.
- **Ngoại lệ:**
  - `NgayKetThuc <= NgayBatDau` → ném `InvalidLeaseTermException`.
  - Gian hàng đã có HĐ `Active` trùng lịch → ném `SpaceAlreadyOccupiedException`.
  - Diện tích không đạt chuẩn ngành hàng (F&B $< 50\text{ m}^2$, Kiosk $< 10\text{ m}^2$) → ném `LeasingException`.
  - `LeasingStaff` cố duyệt HĐ giá trị $> 500\text{ triệu}$ → ném `UnauthorizedAccessException`.

### UC04: Lập Biên bản Bàn giao Hiện trạng Nhận/Trả Mặt bằng (TV1 — Master-Detail)
- **Actor:** `LeasingStaff`, `Admin`.
- **Tiền điều kiện:** Hợp đồng thuê đang ở trạng thái `Active` (khi nhận) hoặc chuẩn bị thanh lý (khi trả).
- **Luồng chính:**
  1. Mở `frm002_nv_ban_giao_mat_bang`, chọn Hợp đồng và Loại biên bản (`NhanMatBang` hoặc `TraMatBang`).
  2. Ghi nhận chi tiết tình trạng hạ tầng/thiết bị từng lô thuê (điện, nước, PCCC, trần/sàn).
  3. Xác nhận biên bản và lưu đồng bộ bảng `BienBanBanGiao` + `ChiTietBanGiao`.
- **Ngoại lệ:** Ngày lập lớn hơn ngày hiện tại hoặc thiếu xác nhận 2 bên → chặn lưu.

### UC05: Thanh lý Hợp đồng & Giải phóng Mặt bằng (Liên phân hệ TV1 → TV2)
- **Actor:** `LeasingStaff`, `Admin`.
- **Tiền điều kiện:** Hợp đồng đang ở trạng thái `Active`.
- **Luồng chính:** Xem chi tiết tại **Mục 4.2 (Luồng Liên phân hệ 2)**.
- **Hậu điều kiện:** Hợp đồng chuyển sang `Terminated`, toàn bộ lô gian hàng thuộc hợp đồng chuyển sang `Trong`, phát sinh sự kiện `ContractTerminatedEvent` sang Phân hệ 2.

### UC06: Quản lý Danh mục Dịch vụ Tiện ích, Bảng giá Bậc thang & PTTT (TV2)
- **Actor:** `BillingStaff`, `Admin`.
- **Tiền điều kiện:** Đã đăng nhập với quyền phù hợp.
- **Luồng chính:**
  1. Quản lý danh mục dịch vụ (Điện, Nước, Điều hòa, Phí quản lý chung) và khai báo khung đơn giá lũy tiến (`TuChiSo`, `DenChiSo`, `DonGiaBacThang`) tại `frm001_dm_loai_dich_vu`.
  2. Quản lý các phương thức thanh toán (Tiền mặt, Chuyển khoản, Thẻ) tại `frm001_dm_phuong_thuc_thanh_toan`.
- **Ngoại lệ:** `DenChiSo <= TuChiSo` hoặc `DonGiaBacThang <= 0` → ném `BillingException`.

### UC07: Chốt Chỉ số Điện/Nước Định kỳ Hàng tháng (TV2 — Master-Detail)
- **Actor:** `BillingStaff`, `Admin`.
- **Tiền điều kiện:** Gian hàng đang hoạt động (`DangThue`).
- **Luồng chính:**
  1. Mở `frm002_nv_ghi_chi_so_dich_vu`, chọn Gian hàng, Kỳ chốt (`Thang`/`Nam`), Ngày ghi.
  2. Nhập `ChiSoMoi` cho từng dịch vụ đo đếm; hệ thống tự nạp `ChiSoCu` kỳ trước và tính sản lượng tiêu thụ (`ChiSoMoi - ChiSoCu`).
  3. Lưu bản ghi vào `ChiSoDichVu`.
- **Ngoại lệ:** `ChiSoMoi < ChiSoCu` → ném `InvalidMeterReadingException`; trùng kỳ chốt đã khóa → chặn ghi đè.

### UC08: Lập Hóa đơn Tổng hợp Hàng tháng (Liên phân hệ TV2 → TV1 — Master-Detail)
- **Actor:** `BillingStaff`, `Admin`.
- **Tiền điều kiện:** Gian hàng có Hợp đồng `Active` bên TV1 và đã chốt chỉ số tiện ích tháng hiện tại.
- **Luồng chính:** Xem chi tiết tại **Mục 4.1 (Luồng Liên phân hệ 1)**.
- **Hậu điều kiện:** Lưu đồng bộ `HoaDon` và `ChiTietHoaDon_DichVu` (kèm đơn giá lịch sử) trong `TransactionScope`, trạng thái `Unpaid`.

### UC09: Thu tiền, Cấn trừ Công nợ & Tính Phạt Chậm nộp (TV2)
- **Actor:** `BillingStaff`, `Admin`.
- **Tiền điều kiện:** Hóa đơn đang ở trạng thái `Unpaid`, `PartialPaid`, hoặc `Overdue`.
- **Luồng chính:**
  1. Mở `frm002_nv_thu_tien_cong_no`, chọn hóa đơn cần thu.
  2. Nếu quá hạn thanh toán, hệ thống tự động tính tiền phạt chậm nộp $0.05\%/\text{ngày}$ trên số dư nợ.
  3. Nhập số tiền thu và phương thức thanh toán; hệ thống lưu `PhieuThuTien`, cập nhật `DaThanhToan` và chuyển trạng thái hóa đơn (`PartialPaid` nếu nộp thiếu, `Paid` nếu nộp đủ).
- **Ngoại lệ:** Số tiền thu $\le 0$ hoặc lớn hơn số nợ còn lại → ném `BillingException`; cố sửa/thu tiền hóa đơn đã `Paid` hoặc `Cancelled` → State Guard ném `InvalidOperationException`.

### UC10: Thống kê Báo cáo Phân hệ & Dashboard Doanh thu Tổng hợp TTTM (Chung)
- **Actor:** `Admin` (toàn bộ BC1–BC5), `LeasingStaff` (BC1, BC2), `BillingStaff` (BC3, BC4).
- **Luồng chính:**
  1. Người dùng chọn loại báo cáo (BC1: Lấp đầy mặt bằng, BC2: Cảnh báo hạn HĐ 30/60 ngày, BC3: Tiêu thụ điện nước, BC4: Tuổi nợ loại trừ đơn `Cancelled`, BC5: Dashboard doanh thu tổng hợp TV1 + TV2) và điều kiện lọc.
  2. Hệ thống truy vấn tổng hợp (JOIN tối ưu), hiển thị biểu đồ (LiveCharts) và xuất tệp Excel (ClosedXML) / PDF (QuestPDF) sạch font Unicode tiếng Việt.

---

## 4. Đặc tả Chi tiết 2 Luồng Nghiệp vụ Liên phân hệ (TV1 ↔ TV2)

> Nguyên tắc kiến trúc: Hai phân hệ **tuyệt đối không** truy vấn trực tiếp bảng nội bộ hoặc khởi tạo cứng (`new`) lớp Concrete của nhau; 100% giao tiếp qua **Interface** trong `TTTM.Domain.Contracts` (tiêm bằng Constructor Injection) và **Domain Events** (`Observer Pattern`).

### 4.1. Luồng Liên phân hệ 1 (TV2 → TV1): Lập Hóa đơn Tổng hợp Hàng tháng (`UC08`)
1. `BillingStaff` chọn Gian hàng và Kỳ thanh toán (`Thang`, `Nam`) trên `frm002_nv_lap_hoa_don_thang`.
2. `UtilityBillingService` (TV2) gọi hợp đồng giao tiếp **`ILeasingContractService.GetActiveContractRent(slotId)`** do TV1 cung cấp.
3. **Kiểm tra điều kiện liên phân hệ:**
   - Nếu gian hàng không có Hợp đồng `Active` (hoặc đã `Expired`/`Terminated`), `ILeasingContractService` trả về `null` → TV2 ném `ContractNotFoundException` và dừng quy trình.
   - Nếu hợp lệ, TV1 trả về `MaHopDong`, `DonGiaThoaThuan` (tiền thuê cố định tháng) và chính sách cọc/ưu đãi.
4. `UtilityBillingService` kiểm tra đã có chỉ số chốt tháng (`ChiSoDichVu`) chưa; nếu chưa có → ném `BillingException`.
5. Dùng `IBillingItemFactory` (Factory Method) sinh các dòng `InvoiceDetail`: dòng tiền thuê (từ TV1), dòng điện/nước lũy tiến bậc thang (từ TV2), tính thuế VAT 10% và chiết khấu.
6. Bọc trong **`TransactionScope` (ACID)**: Lưu `HoaDon` (có FK `MaHopDong` nối sang `HopDongThue`) và toàn bộ `ChiTietHoaDon_DichVu`. Nếu lỗi ở bất kỳ dòng chi tiết nào → Rollback 100%.

### 4.2. Luồng Liên phân hệ 2 (TV1 → TV2): Thanh lý Hợp đồng & Tất toán Công nợ (`UC05`)
1. `LeasingStaff` hoặc `Admin` yêu cầu thanh lý Hợp đồng trên `frm002_nv_hop_dong_thue`.
2. `LeasingService` (TV1) kiểm tra State Guard của `LeaseContract`: chỉ cho phép thanh lý khi trạng thái là `Active` (nếu `Draft` hoặc đã `Terminated` → ném `InvalidOperationException`).
3. `LeasingService` gọi hợp đồng giao tiếp **`IBillingService.GetOutstandingDebt(contractId)`** do TV2 cung cấp để kiểm tra tổng công nợ còn tồn đọng của hợp đồng.
4. **Kiểm tra ràng buộc tất toán:**
   - Nếu `outstandingDebt > 0` → TV1 ném **`UnsettledDebtException`**, chặn thanh lý và yêu cầu khách hàng tất toán tại Phân hệ 2 (hoặc cấn trừ tiền cọc).
   - Nếu `outstandingDebt == 0` → tiến hành bước 5 trong **`TransactionScope` xuyên phân hệ**.
5. Trong `TransactionScope`:
   - TV1 cập nhật trạng thái `HopDongThue` sang `Terminated` và giải phóng toàn bộ lô mặt bằng trong `ChiTietHopDong_MatBang` sang trạng thái `Trong`.
   - TV1 phát sự kiện miền **`ContractTerminatedEvent`** qua `DomainEventBus` (Observer Pattern).
   - `BillingContractSyncListener` (TV2) lắng nghe sự kiện để tự động khóa sổ hóa đơn và chốt trạng thái công nợ của hợp đồng.
   - Nếu bất kỳ bước cập nhật nào của TV2 thất bại → `TransactionScope` tự động **Rollback toàn bộ**, khôi phục hợp đồng TV1 về `Active` và bảo toàn trạng thái lô mặt bằng.
