# FINAL PROJECT: NỀN TẢNG QUẢN LÝ HỌC TẬP CHO TRUNG TÂM (LMS)

# 1. Giới thiệu

Hệ thống được xây dựng cho **mô hình trung tâm học tập** (không còn giới hạn phạm vi nội bộ trường học), hỗ trợ **học viên bên ngoài đăng ký tham gia, giáo viên tổ chức lớp học, phụ huynh theo dõi tiến độ** và trung tâm vận hành tập trung bởi **Admin**.

Phạm vi nghiệp vụ bao phủ **toàn bộ vòng đời học viên**: từ **đăng ký tài khoản**, **được duyệt/xếp lớp**, **học tập - nộp bài - nhận đánh giá**, đến **kết thúc khóa/lớp** với dữ liệu theo dõi đầy đủ.

**Các điểm nhấn công nghệ:**

- **AI phân tích dữ liệu học tập**: Tập trung vào thống kê tiến độ, phát hiện xu hướng học tập và đề xuất lời khuyên cho phụ huynh.
- **Realtime Notification (SignalR)**: Thông báo tức thời khi có bài mới, điểm mới hoặc thay đổi lớp học.
- **Bảo mật**: ASP.NET Core Identity + JWT Authentication kết hợp Role-Based Authorization.
- **Quản trị tập trung**: Admin điều phối toàn bộ danh mục Môn học, Lớp học và Người dùng.
- **An toàn dữ liệu**: Áp dụng cơ chế **Xóa mềm (Soft Delete)** cho toàn bộ hệ thống.

# 2. Actors (Người tham gia)

- **Giáo viên**: Tạo bài tập, chỉnh sửa/xóa bài tập, đặt deadline, xem danh sách học viên đã nộp bài, chấm điểm bài tập, xem dashboard hiệu suất lớp học do AI tổng hợp, gửi thông báo cho học viên.
- **Học viên**: Khách ngoài đăng ký học tại trung tâm, nhận bài tập, nộp bài và nhận kết quả.
- **Phụ huynh học viên**: Theo dõi kết quả học tập của con/em sau khi được Admin liên kết, nhận báo cáo và khuyến nghị từ AI.
- **Admin**: Quản lý người dùng (duyệt quyền, khóa tài khoản), quản lý khóa học/lớp học, xếp lớp cho giáo viên/học viên, liên kết phụ huynh - học viên, theo dõi Audit Log hệ thống.

# 3. Business Flow (Luồng nghiệp vụ tổng quát)

```
      Admin
        |
[Duyệt tài khoản Unknown -> Role chính thức]
        |
[Tạo Môn học & Lớp học]
        |
[Liên kết Phụ huynh - Học viên dựa trên thông tin định danh]
        |
[Xếp Giáo viên & Học viên vào lớp] ----> (SignalR: Notify Teacher/Learner)
        |
     Giáo viên
        |
[Tạo bài tập cho lớp] ------------------> (SignalR: Notify Learners)
        |
     Học viên
        |
[Nộp bài tập] --------------------------> (SignalR: Notify Teacher)
        |
     Hệ thống (AI)
        |
[AI tổng hợp thống kê tiến độ + phân tích hiệu suất lớp]
        |
     Giáo viên
        |
[Chấm điểm & Nhận xét] -----------------> (SignalR: Notify Learner & Parent)
```

# 4. System Architecture (Thiết kế hệ thống)

```
React Client (Frontend)
      |
      | REST API / SignalR
      v
ASP.NET Core Web API (Backend)
      |
      |-----------------------|-------------------|-------------------|
      v                       v                   v                   v
SQL Server Database     AI Analytics Service  Local Storage     Email Service (SMTP)
                                           (Server Disk)
```

# 5. Hệ thống thông báo Realtime (SignalR Hub)

| Sự kiện (Event) | Người nhận | Mô tả |
| --- | --- | --- |
| **UserApproved** | Người dùng | Thông báo khi Admin phê duyệt tài khoản và gán Role |
| **UserAddedToClass** | Giáo viên/Học viên | Thông báo khi Admin xếp vào lớp mới |
| **AccountLinked** | Phụ huynh | Thông báo khi Admin hoàn tất liên kết với tài khoản của con |
| **NewAssignment** | Học viên | Thông báo có bài tập mới trong lớp |
| **NewSubmission** | Giáo viên | Thông báo có học viên vừa nộp bài |
| **AssignmentGraded** | Học viên | Thông báo bài tập đã được chấm điểm |
| **GradePublished** | Phụ huynh | Thông báo con đã nhận được điểm mới |
| **DeadlineReminder** | Học viên | Thông báo nhắc nhở khi sắp đến hạn nộp bài (Hệ thống tự động) |

# 6. Functional Requirements (Yêu cầu chức năng chi tiết)

## FR-01: Đăng nhập (ASP.NET Core Identity + JWT Authentication)

### Mô tả:

Hệ thống cho phép người dùng đăng nhập từ tài khoản đã đăng ký. Sau khi xác nhận thành công, hệ thống sẽ tạo JSON Web Token (JWT).

### Actor: Giáo viên, Học viên, Phụ huynh, Admin.

### Preconditions:

- Người dùng đã có tài khoản.
- Tài khoản chưa bị khóa.

### Main Flow:

1. Người dùng truy cập trang Login.
2. Nhập Username/Email và Password.
3. Nhấn Login.
4. Hệ thống kiểm tra tính hợp lệ.
5. Nếu hợp lệ: Tạo JWT Token chứa UserId, Role, Expiration.
6. Hệ thống trả về Access Token và thông tin người dùng.
7. Client lưu token và chuyển hướng đến Dashboard.

### Alternative Flow:

- Sai mật khẩu: Trả về "Invalid username or password".
- Tài khoản không tồn tại: Trả về "User not found".
- Tài khoản bị khóa: Trả về "Account has been disabled".

## FR-02: Token Validation

### Mô tả:

Kiểm tra JWT Token trong mỗi request để xác thực.

### Main Flow:

1. Client gửi request kèm header `Authorization: Bearer <JWT Token>`.
2. Server kiểm tra tính hợp lệ và thời hạn.
3. Nếu hợp lệ: Xử lý request. Nếu không: Trả về **401 Unauthorized**.

## FR-03: Role-Based Authorization

### Mô tả:

Kiểm tra quyền truy cập theo Role sau khi đã xác thực.

### Main Flow:

1. Server đọc Role trong JWT.
2. Kiểm tra quyền (Ví dụ: Teacher -> Tạo bài; Learner -> Nộp bài).
3. Nếu không có quyền: Trả về **403 Forbidden**.

## FR-04: Login with Google (OAuth 2.0)

### Mô tả:

Đăng nhập qua Google. Nếu là lần đầu, yêu cầu hoàn thiện Profile.

### Main Flow:

1. Người dùng chọn "Login with Google".
2. Google xác thực và trả về Authorization Code.
3. Backend lấy thông tin từ Google (Email, Name).
4. Nếu tài khoản chưa tồn tại: Chuyển tới trang **Complete Profile** (Nhập Username, Password, SĐT).
5. Nếu tồn tại: Tạo JWT và đăng nhập.

## FR-05: User Registration (Đăng ký tài khoản)

### Mô tả:

Tạo tài khoản mới với các thông tin đối soát liên kết.

### Actor: Giáo viên, Học viên, Phụ huynh.

### Main Flow:

1. Người dùng nhập: Họ tên, Email, Username, Password, SĐT cá nhân.
2. **Nếu là Học viên**: Bắt buộc nhập thêm **SĐT Phụ huynh**.
3. **Nếu là Phụ huynh**: Nhập **SĐT cá nhân** (Dùng để Admin đối soát).
4. Hệ thống kiểm tra định dạng Email, độ mạnh Password.
5. Kiểm tra Username/Email/SĐT đã tồn tại chưa.
6. Hash password và lưu Database với Role mặc định là "Unknown".

## FR-17: User Management & Approval (Admin)

### Mô tả:

Admin xem danh sách các tài khoản mới đăng ký (Unknown) để phê duyệt vào hệ thống.

### Main Flow:

1. Admin xem danh sách tài khoản chờ duyệt.
2. Admin chọn tài khoản và gán Role chính thức (Teacher/Learner/Parent).
3. Hệ thống cập nhật Database và gửi SignalR thông báo cho người dùng.
4. Admin có quyền Khóa/Mở khóa tài khoản bất kỳ lúc nào.

## FR-19: Forgot Password (Quên mật khẩu)

### Mô tả:

Cho phép người dùng khôi phục lại mật khẩu thông qua mã xác thực (OTP) gửi qua Email.

### Actor: Tất cả người dùng.

### Main Flow:

1. Người dùng chọn "Forgot Password" tại trang Đăng nhập.
2. Người dùng nhập Email đã đăng ký.
3. Hệ thống kiểm tra Email tồn tại và gửi một mã Code (OTP) về Email đó.
4. Người dùng nhập mã Code và Mật khẩu mới.
5. Hệ thống xác thực mã Code có khớp và còn hiệu lực hay không.
6. Nếu hợp lệ, hệ thống tiến hành cập nhật mật khẩu mới (đã hash) vào Database.
7. Người dùng nhận thông báo thành công và quay lại trang Đăng nhập.

### Alternative Flow:

- Email không tồn tại: Thông báo "Email not registered".
- Mã Code sai hoặc hết hạn: Thông báo "Invalid or expired code".
- Mật khẩu mới và Nhập lại mật khẩu không khớp.

## FR-06: Create Assignment (Tạo bài tập)

### Actor: Giáo viên.

### Preconditions:

- Đã đăng nhập. Có quyền quản lý lớp.

### Main Flow:

1. Giáo viên chọn lớp và môn học.
2. Nhập Tiêu đề, Mô tả, Deadline.
3. **Giáo viên cấu hình tùy chọn: "Allow Late Submission" (Cho phép nộp quá hạn)**.
    - Nếu chọn **Bật**: Học viên có thể nộp sau deadline và bị đánh dấu "Late".
    - Nếu chọn **Tắt**: Hệ thống tự động đóng nút nộp bài khi hết hạn.
4. Tải lên tài liệu đính kèm (Lưu trữ cục bộ trên Server).
5. Nhấn Create.
6. Hệ thống lưu vào DB và gửi **SignalR Notification** tới học viên trong lớp.

### Postconditions:

- Bài tập hiển thị cho học viên với trạng thái nộp bài được cấu hình theo yêu cầu của giáo viên.

## FR-07: AI Learning Analytics (AI thống kê & khuyến nghị)

### Mô tả:

AI xử lý dữ liệu học tập để tạo thống kê tiến độ, đánh giá hiệu suất lớp và gợi ý lời khuyên cho phụ huynh.

### Main Flow:

1. Hệ thống tổng hợp dữ liệu bài tập, điểm số, trạng thái nộp bài, tỷ lệ hoàn thành của lớp.
2. AI Service phân tích và trả về: chỉ số hiệu suất lớp, cảnh báo học viên cần hỗ trợ, đề xuất hành động cho giáo viên/phụ huynh.
3. Hệ thống lưu kết quả vào Database và hiển thị trên dashboard theo từng lớp/học viên.

## FR-08: Submit Assignment (Nộp bài tập)

### Actor: Học viên.

### Main Flow:

1. Học viên chọn bài tập.
2. Hệ thống kiểm tra trạng thái bài tập:
    - Nếu hiện tại chưa quá Deadline: Cho phép nộp bình thường.
    - Nếu đã quá Deadline:
        - Trường hợp bài tập **Cho phép nộp muộn**: Cho phép nộp, bài nộp được đánh dấu trạng thái "Late Submission".
        - Trường hợp bài tập **Không cho phép nộp muộn**: Hệ thống hiển thị thông báo "Submission deadline has passed" và vô hiệu hóa chức năng nộp.
3. Nhập nội dung văn bản hoặc tải file.
4. Nhấn Submit.
5. **Hệ thống lưu trữ file vào thư mục vật lý trên Server**.
6. Hệ thống lưu bài nộp, kích hoạt AI analytics và gửi **SignalR Notification** cho Giáo viên.

## FR-09: View Assignment (Xem danh sách bài tập)

### Actor: Học viên.

### Main Flow:

1. Học viên xem danh sách bài theo lớp: Tiêu đề, Môn, GV, Deadline, Trạng thái nộp.
2. Chọn bài để xem chi tiết tài liệu và yêu cầu.

## FR-10: Grade Assignment (Chấm điểm)

### Actor: Giáo viên.

### Main Flow:

1. Giáo viên xem bài nộp và kết quả gợi ý từ AI.
2. Nhập Điểm và Nhận xét.
3. Hệ thống lưu điểm và gửi **SignalR Notification** cho Học viên & Phụ huynh.

## FR-11: Parent Monitor Progress (Theo dõi tiến độ)

### Actor: Phụ huynh.

### Preconditions:

- Tài khoản Phụ huynh đã được Admin liên kết với Học viên (FR-16).

### Main Flow:

1. Phụ huynh truy cập trang **Learner Progress**.
2. Xem danh sách bài tập của con: Trạng thái (On time/Late/Not submitted), Điểm, Nhận xét.

## FR-13: Quản lý Môn học (Admin)

Admin thêm/sửa/xóa danh mục các môn học chính thức trong hệ thống.

## FR-14: Quản lý Lớp học (Admin)

Admin tạo các thực thể Lớp học (Tên lớp, Niên khóa).

## FR-15: Xếp lớp (Admin)

Admin gán Giáo viên phụ trách và danh sách Học viên vào từng lớp. Sau khi lưu, SignalR sẽ gửi thông báo "Bạn đã được thêm vào lớp" cho các Actor.

## FR-16: Link Parent to Learner (Phụ huynh & Admin)

### Mô tả:

Quy trình liên kết an toàn không dựa trên thông tin cá nhân nhạy cảm.

### Main Flow:

1. Phụ huynh truy cập chức năng **Connect to Learner**.
2. Nhập **Link Code** do học viên cung cấp.
3. Hệ thống kiểm tra mã tồn tại và hiển thị thông tin tóm tắt của học viên (Họ tên, Lớp) để phụ huynh xác nhận.
4. Phụ huynh nhấn **Send Request**.
5. **Admin** nhận được yêu cầu, kiểm tra tính xác thực và nhấn **Approve**.
6. Hệ thống tạo bản ghi liên kết và gửi SignalR thông báo.

### Mô tả:

Admin thực hiện kết nối tài khoản Phụ huynh với Học viên để bảo mật thông tin dựa trên SĐT đối soát.

## FR-18: Audit Logging (Hệ thống)

### Mô tả:

Ghi lại mọi hoạt động quan trọng để Admin có thể truy vết khi xảy ra sự cố.

# 7. Thiết kế API (RESTful)

### Admin API

- `POST /api/v1/admin/users/approve`: Phê duyệt người dùng.
- `GET /api/v1/admin/audit-logs`: Xem lịch sử hệ thống.
- `POST /api/v1/admin/subjects`: Quản lý môn học.
- `POST /api/v1/admin/classes`: Tạo lớp học.
- `POST /api/v1/admin/classes/{id}/enroll`: Gán GV/Học viên vào lớp.
- `GET /api/v1/admin/linking/suggested`: Gợi ý liên kết dựa trên SĐT.
- `POST /api/v1/admin/linking/confirm`: Xác nhận liên kết Phụ huynh-Học viên.

### Authentication API

- `POST /api/v1/auth/login`: Đăng nhập.
- `POST /api/v1/auth/register`: Đăng ký (Gồm thông tin SĐT).
- `POST /api/v1/auth/forgot-password`: Yêu cầu gửi mã OTP về Email.
- `POST /api/v1/auth/reset-password`: Nhập mã OTP và Mật khẩu mới để đặt lại.

### Assignment & Grade API

- `POST /api/v1/assignments`: Tạo bài tập (Teacher).
- `POST /api/v1/submissions`: Nộp bài (Learner).
- `POST /api/v1/submissions/{id}/grade`: Chấm bài (Teacher).

# 8. Yêu cầu phi chức năng

- **Performance**: Xử lý 1000 requests/phút.
- **Security**: HTTPS, BCrypt hashing, JWT Validation cho mọi request.
- **Dữ liệu (Data - Soft Delete)**: Toàn bộ các bảng trong Database (Users, Classes, Subjects, Assignments, Submissions,...) **không được xóa vật lý**. Hệ thống sử dụng cờ `IsDeleted` hoặc `DeletedAt` để ẩn dữ liệu.
- **File Storage (Local Storage)**: Toàn bộ file PDF, MD, DOCX được lưu trữ trực tiếp trong thư mục vật lý trên server (ví dụ: `/uploads/assignments` và `/uploads/submissions`). Database chỉ lưu trữ đường dẫn (Path) tới file. Cần đảm bảo phân quyền truy cập thư mục để bảo mật.
- **Background Jobs**: Sử dụng Hangfire gửi thông báo nhắc deadline và dọn dẹp mã OTP hết hạn.

# 9. Technology Stack

- **Backend**: ASP.NET Core 8 Web API, Entity Framework Core (Global Query Filter cho Soft Delete), SignalR, Hangfire.
- **Frontend**: ReactJS.
- **Database**: SQL Server.
- **DevOps & Infrastructure**: GitHub Actions, Azure DevOps, Docker
- **Storage**: **Local File System (Server Disk)**.
- **Email Service**: SMTP (Gmail/SendGrid) để gửi OTP.
