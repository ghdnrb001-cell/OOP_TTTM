# Hồ sơ Khảo sát Hiện trạng 4W+1H — Phân hệ 1: Quản lý Gian hàng, Mặt bằng & Hợp đồng Thuê

- **Môn học:** Lập trình Hướng đối tượng (MI4090)
- **Đề tài:** Hệ thống Quản lý Trung tâm Thương mại (TTTM)
- **Phân hệ:** Phân hệ 1 — Mall Leasing & Tenant Management
- **Người thực hiện:** Thành viên 1 (TV1 — Core Architect)

---

## 1. Phân tích Khảo sát Hiện trạng theo Mô hình 4W+1H

### 1.1. WHO (Ai tham gia quy trình?)
- **Ban Quản lý TTTM (`Admin`):** Phê duyệt quy hoạch phân khu/tầng, duyệt Hợp đồng thuê giá trị lớn (> 500 triệu VNĐ), theo dõi báo cáo tỷ lệ lấp đầy (BC1) và cảnh báo hạn hợp đồng (BC2).
- **Nhân viên Kinh doanh Mặt bằng (`LeasingStaff`):** Trực tiếp quản lý danh mục khu vực/lô gian hàng, tiếp nhận và thẩm định hồ sơ đối tác/thương hiệu thuê, soạn thảo Hợp đồng thuê mặt bằng, lập Biên bản bàn giao/thu hồi hiện trạng mặt bằng.
- **Kế toán Thu phí (`BillingStaff` — TV2 phối hợp):** Nhận thông tin Hợp đồng hiệu lực (`Active`) từ Phân hệ 1 để lập Hóa đơn tháng và xác nhận trạng thái công nợ trước khi Phân hệ 1 thanh lý hợp đồng.
- **Đối tác / Khách thuê (`Tenant`):** Các thương hiệu F&B, Thời trang, Công nghệ, Rạp chiếu phim, Giải trí đăng ký thuê gian hàng/ki-ốt tại TTTM.

### 1.2. WHAT (Quản lý những đối tượng & nghiệp vụ nào?)
- **Mặt bằng & Vị trí:** Phân khu (`KhuVuc`: Khu A, B, C theo tầng 1–5) và từng lô vị trí gian hàng/ki-ốt (`ViTriGianHang`: mã lô, diện tích $m^2$, đơn giá niêm yết cơ sở, trạng thái `Trong` / `DatCoc` / `DangThue` / `BaoTri`).
- **Hồ sơ Khách thuê:** Thông tin pháp lý thương hiệu (`KhachThue`: tên thương hiệu, mã số thuế, ngành hàng, người đại diện, email, số điện thoại).
- **Hợp đồng Thuê & Chi tiết lô thuê:** Hợp đồng khung (`HopDongThue`: kỳ hạn, ngày ký, tiền đặt cọc, trạng thái `Draft` / `Active` / `Expired` / `Terminated`) và danh sách lô thuê kèm đơn giá thỏa thuận lịch sử (`ChiTietHopDong_MatBang` — quan hệ $n-n$).
- **Biên bản Bàn giao & Thanh lý:** Ghi nhận hiện trạng hạ tầng/thiết bị khi giao mặt bằng (`NhanMatBang`) và khi thu hồi/thanh lý (`TraMatBang`).

### 1.3. WHERE (Nghiệp vụ diễn ra ở đâu?)
- Tại **Văn phòng Ban Quản lý Trung tâm Thương mại** (bộ phận Kinh doanh Mặt bằng & Pháp chế hợp đồng) và trực tiếp tại **hiện trường các tầng/khu vực gian hàng** khi tiến hành khảo sát, đo đạc, bàn giao hoặc nghiệm thu thu hồi mặt bằng.

### 1.4. WHEN (Thực hiện vào thời điểm nào?)
- **Thường xuyên / Theo sự kiện:** Khi có thương hiệu mới đăng ký thuê, điều chỉnh hiện trạng gian hàng, hoặc bảo trì mặt bằng.
- **Đầu kỳ thuê:** Ký kết hợp đồng, thu tiền đặt cọc và lập Biên bản bàn giao nhận mặt bằng.
- **Định kỳ hàng tháng (30/60/90 ngày trước hạn):** Rà soát báo cáo tỷ lệ lấp đầy (BC1) và danh sách hợp đồng sắp hết hạn (BC2) để gia hạn hoặc tìm khách thay thế.
- **Cuối kỳ thuê / Thanh lý sớm:** Đối chiếu công nợ với Phân hệ 2 (TV2), lập Biên bản trả mặt bằng, hoàn trả cọc và giải phóng lô về trạng thái `Trong`.

### 1.5. HOW (Quy trình vận hành hiện tại & bài toán số hóa)
- **Thực trạng trước khi số hóa:** Quản lý danh sách lô bằng bảng tính Excel rời rạc, hợp đồng lưu bản Word riêng lẻ, dễ xảy ra trùng lịch đặt cọc/thuê cùng một gian hàng, khó theo dõi lịch sử đơn giá thỏa thuận khi TTTM điều chỉnh giá cơ sở, và dễ bỏ sót công nợ điện nước khi thanh lý hợp đồng.
- **Giải pháp hệ thống mới:**
  1. Chuẩn hóa quy trình khép kín trên ứng dụng WinForms 3-Layer + SQL Server 3NF.
  2. Kiểm soát **4 quy tắc nghiệp vụ cốt lõi** tại tầng BLL/Domain:
     - **Rule 1 (Chống trùng lịch/trùng vị trí):** Không cho phép lập hợp đồng mới cho gian hàng đang ở trạng thái `DangThue` hoặc trùng khoảng thời gian hiệu lực.
     - **Rule 2 (Tiền đặt cọc tối thiểu):** Tiền cọc bắt buộc $\ge 3$ tháng tiền thuê cơ sở.
     - **Rule 3 (Ràng buộc diện tích ngành hàng):** F&B $\ge 50\text{ m}^2$, Kiosk $\ge 10\text{ m}^2$, tránh xung đột quy hoạch ngành hàng.
     - **Rule 4 (Phân quyền duyệt HĐ giá trị lớn - RBAC):** Hợp đồng có tổng giá trị $> 500.000.000\text{ VNĐ}$ chỉ `Admin` mới có quyền phê duyệt tại tầng Service.
  3. **Quy tắc liên phân hệ (TV1 ↔ TV2):** Khi thanh lý hợp đồng, `LeasingService` gọi `IBillingService.GetOutstandingDebt(contractId)` của TV2; nếu còn công nợ $> 0$, hệ thống ném `UnsettledDebtException` và chặn giải phóng mặt bằng.

---

## 2. Biểu mẫu Thực tế Thu thập & Ánh xạ Dữ liệu

| STT | Tên Biểu mẫu Thực tế | Các trường thông tin cốt lõi trên biểu mẫu | Ánh xạ Bảng CSDL & Form WinForms |
| :---: | :--- | :--- | :--- |
| 1 | **Mẫu Hợp đồng Thuê Mặt bằng Thương mại** (`MauHopDongThue`) | Số HĐ, Ngày ký, Bên thuê (Tên, MST, Đại diện), Thời hạn thuê (`NgayBatDau` – `NgayKetThuc`), Tiền đặt cọc, Bảng danh sách lô thuê (Mã lô, Diện tích, **Đơn giá thỏa thuận**), Điều khoản thanh toán | `HopDongThue`, `ChiTietHopDong_MatBang` → `frm002_nv_hop_dong_thue` (Master-Detail) |
| 2 | **Mẫu Biên bản Bàn giao / Thu hồi Hiện trạng Mặt bằng** (`MauBienBanBanGiao`) | Số biên bản, Mã HĐ liên quan, Loại bàn giao (`NhanMatBang` / `TraMatBang`), Ngày lập, Danh mục hiện trạng hạ tầng (điện, nước, trần, sàn, PCCC) từng lô, Xác nhận hai bên | `BienBanBanGiao`, `ChiTietBanGiao`, `ViTriGianHang` → `frm002_nv_ban_giao_mat_bang` (Master-Detail) |

---

## 3. Bảng Ánh xạ Sơ đồ BFD 3 cấp (Phân hệ 1) sang Form & Báo cáo

| Mã BFD | Tên Chức năng (Cấp 2 / Cấp 3) | Form / Module Đối ứng | Loại |
| :--- | :--- | :--- | :--- |
| **F1.1** | **Quản lý Mặt bằng & Vị trí** | | Nhánh cấp 2 |
| F1.1.1 | Quản lý danh mục phân khu & vị trí gian hàng (lọc đa tiêu chí) | `frm001_dm_vi_tri_gian_hang` | Form Danh mục |
| F1.1.2 | Tra cứu trực quan sơ đồ mặt bằng theo tầng & trạng thái | `frm002_nv_tra_cuu_mat_bang` | Form Nghiệp vụ |
| F1.1.3 | Thống kê tỷ lệ lấp đầy & phân bổ diện tích mặt bằng (BC1) | `BC1` (Excel/PDF + Biểu đồ) | Báo cáo |
| **F1.2** | **Quản lý Khách thuê** | | Nhánh cấp 2 |
| F1.2.1 | Quản lý hồ sơ đối tác / thương hiệu thuê | `frm001_dm_khach_thue` | Form Danh mục |
| F1.2.2 | Tra cứu khách thuê theo thương hiệu, MST, ngành hàng & lịch sử thuê | `frm001_dm_khach_thue` | Form Danh mục |
| **F1.3** | **Quản lý Hợp đồng & Bàn giao** | | Nhánh cấp 2 |
| F1.3.1 | Lập & quản lý Hợp đồng thuê mặt bằng (áp dụng Strategy tính giá) | `frm002_nv_hop_dong_thue` | Form Master-Detail |
| F1.3.2 | Phê duyệt Hợp đồng thuê (kiểm soát RBAC hạn mức > 500tr) | `frm002_nv_hop_dong_thue` | Nghiệp vụ BLL |
| F1.3.3 | Lập Biên bản bàn giao nhận / trả hiện trạng mặt bằng | `frm002_nv_ban_giao_mat_bang` | Form Master-Detail |
| F1.3.4 | Thanh lý hợp đồng & kiểm tra tất toán công nợ (Liên PH TV2) | `frm002_nv_hop_dong_thue` | Liên phân hệ |
| F1.3.5 | Theo dõi hạn hợp đồng & cảnh báo sắp hết hạn 30/60 ngày (BC2) | `BC2` (Excel/PDF) | Báo cáo |
