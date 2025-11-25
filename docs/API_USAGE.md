# API Usage Guide

Hướng dẫn chi tiết cách sử dụng Equipment Management API

---

## 📋 Mục lục

- [Giới thiệu](#giới-thiệu)
- [Authentication & Authorization](#authentication--authorization)
- [Base URL & Headers](#base-url--headers)
- [Response Format](#response-format)
- [Error Handling](#error-handling)
- [Pagination](#pagination)
- [API Endpoints](#api-endpoints)
- [Equipments](#equipments)
- [Warehouses](#warehouses)
- [Assignments](#assignments)
- [Audits](#audits)
- [Maintenances](#maintenances)
- [Liquidations](#liquidations)
- [Use Cases](#use-cases)
- [Best Practices](#best-practices)

---

## 🎯 Giới thiệu

Equipment Management API cung cấp RESTful endpoints để quản lý trang thiết bị, kho, cấp phát, kiểm kê, bảo trì và thanh lý.

**API Version:** v1  
**Base URL (Docker):** `http://localhost:8080`  
**Base URL (Development):** `http://localhost:5000`

---

## 🔐 Authentication & Authorization

> **Lưu ý:** Hiện tại API **CHƯA** implement authentication/authorization. Bạn cần tự thêm middleware và attributes sau.

### Kế hoạch Authentication (Future)

```csharp
// Thêm vào Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
});

// Sử dụng trong Controller
[Authorize(Policy = "Admin")]
[HttpPost]
public async Task<ActionResult<Guid>> CreateEquipment(...)
```

### Roles dự kiến

| Role | Permissions |
|------|-------------|
| **Admin** | Toàn quyền |
| **Manager** | CRUD equipments, approve liquidations |
| **Technician** | View equipments, update maintenance |
| **User** | View assigned equipments |

### Policies dự kiến

- `CanCreateEquipment` - Tạo thiết bị mới
- `CanDeleteEquipment` - Xóa thiết bị
- `CanApproveLiquidation` - Duyệt thanh lý
- `CanAssignEquipment` - Cấp phát thiết bị
- `CanManageMaintenance` - Quản lý bảo trì

---

## 🌐 Base URL & Headers

### Base URLs

```
Development: http://localhost:5000
Docker:      http://localhost:8080
Production:  https://api.yourcompany.com
```

### Required Headers

```http
Content-Type: application/json
Accept: application/json
```

### Optional Headers (Future - với Authentication)

```http
Authorization: Bearer {your_jwt_token}
```

---

## 📦 Response Format

### Success Response

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "LAP001",
  "name": "Dell Latitude 7420",
  "type": "Laptop",
  "price": 25000000,
  "status": 1
}
```

### Paginated Response

```json
{
  "items": [
    {
      "id": "guid",
      "code": "LAP001",
      "name": "Dell Latitude 7420"
    }
  ],
  "totalCount": 100,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## ❌ Error Handling

### Error Response Format

```json
{
  "statusCode": 400,
  "message": "One or more validation errors occurred.",
  "details": {
    "Code": ["Code is required"],
    "Price": ["Price must be greater than or equal to 0"]
  }
}
```

### HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request successful |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request successful, no content returned |
| 400 | Bad Request | Validation error |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

### Common Error Scenarios

#### 1. Validation Error (400)

```json
{
  "statusCode": 400,
  "message": "One or more validation errors occurred.",
  "details": {
    "Name": ["Name is required"],
    "Code": ["Code must not exceed 50 characters"]
  }
}
```

#### 2. Not Found (404)

```json
{
  "statusCode": 404,
  "message": "Entity \"Equipment\" (3fa85f64-5717-4562-b3fc-2c963f66afa6) was not found.",
  "details": null
}
```

#### 3. Internal Server Error (500)

```json
{
  "statusCode": 500,
  "message": "An error occurred while processing your request.",
  "details": "Database connection failed"
}
```

---

## 📄 Pagination

Tất cả endpoints trả về danh sách đều hỗ trợ pagination.

### Query Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pageNumber` | int | 1 | Số trang (bắt đầu từ 1) |
| `pageSize` | int | 10 | Số items mỗi trang (max: 100) |

### Example Request

```http
GET /api/equipments?pageNumber=2&pageSize=20
```

### Example Response

```json
{
  "items": [...],
  "totalCount": 150,
  "pageNumber": 2,
  "pageSize": 20,
  "totalPages": 8,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

---

## 🔌 API Endpoints

### Equipments

#### 1. Get All Equipments (Paginated)

```http
GET /api/equipments
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Số trang (default: 1) |
| pageSize | int | No | Số items/trang (default: 10) |
| type | string | No | Lọc theo loại thiết bị |
| status | string | No | Lọc theo trạng thái (1-6) |
| keyword | string | No | Tìm kiếm trong Code, Name, Description |

**Example Request:**

```bash
curl -X GET "http://localhost:8080/api/equipments?pageNumber=1&pageSize=10&type=Laptop&status=1"
```

**Example Response:**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "code": "LAP001",
      "name": "Dell Latitude 7420",
      "type": "Laptop",
      "description": "Business laptop",
      "specification": "i7-11th, 16GB RAM, 512GB SSD",
      "purchaseDate": "2024-01-15T00:00:00Z",
      "supplier": "Dell Vietnam",
      "price": 25000000,
      "warrantyEndDate": "2027-01-15T00:00:00Z",
      "status": 1,
      "imageUrl": null,
      "qrCodeBase64": "iVBORw0KGgoAAAANSUhEUgAA...",
      "createdAt": "2024-11-25T10:30:00Z",
      "updatedAt": null
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Equipment Status Enum:**

| Value | Name | Description |
|-------|------|-------------|
| 1 | New | Thiết bị mới |
| 2 | InUse | Đang sử dụng |
| 3 | Broken | Hỏng |
| 4 | Repairing | Đang sửa chữa |
| 5 | Lost | Mất |
| 6 | Liquidated | Đã thanh lý |

---

#### 2. Get Equipment By ID

```http
GET /api/equipments/{id}
```

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | GUID | Yes | Equipment ID |

**Example Request:**

```bash
curl -X GET "http://localhost:8080/api/equipments/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Example Response:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "LAP001",
  "name": "Dell Latitude 7420",
  "type": "Laptop",
  "description": "Business laptop",
  "specification": "i7-11th, 16GB RAM, 512GB SSD",
  "purchaseDate": "2024-01-15T00:00:00Z",
  "supplier": "Dell Vietnam",
  "price": 25000000,
  "warrantyEndDate": "2027-01-15T00:00:00Z",
  "status": 1,
  "imageUrl": "/images/lap001.jpg",
  "qrCodeBase64": "iVBORw0KGgoAAAANSUhEUgAA...",
  "createdAt": "2024-11-25T10:30:00Z",
  "updatedAt": null
}
```

---

#### 3. Create Equipment

```http
POST /api/equipments
```

**Request Body:**

```json
{
  "code": "LAP001",
  "name": "Dell Latitude 7420",
  "type": "Laptop",
  "description": "Business laptop for developers",
  "specification": "Intel Core i7-11th Gen, 16GB RAM, 512GB SSD, 14 inch FHD",
  "purchaseDate": "2024-01-15T00:00:00Z",
  "supplier": "Dell Vietnam",
  "price": 25000000,
  "warrantyEndDate": "2027-01-15T00:00:00Z",
  "status": 1,
  "imageUrl": "/images/lap001.jpg"
}
```

**Field Validation:**

| Field | Required | Max Length | Constraints |
|-------|----------|------------|-------------|
| code | Yes | 50 | Unique |
| name | Yes | 200 | - |
| type | Yes | 100 | - |
| description | No | 1000 | - |
| specification | No | 2000 | - |
| price | Yes | - | >= 0 |
| purchaseDate | Yes | - | <= Today |
| status | Yes | - | 1-6 |
| imageUrl | No | 500 | Valid URL |

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/equipments" \
  -H "Content-Type: application/json" \
  -d '{
    "code": "LAP001",
    "name": "Dell Latitude 7420",
    "type": "Laptop",
    "price": 25000000,
    "purchaseDate": "2024-01-15T00:00:00Z",
    "status": 1
  }'
```

**Success Response (201 Created):**

```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Headers:**

```
Location: /api/equipments/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

---

#### 4. Update Equipment

```http
PUT /api/equipments/{id}
```

**Request Body:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "LAP001",
  "name": "Dell Latitude 7420 (Updated)",
  "type": "Laptop",
  "description": "Updated description",
  "specification": "Updated specs",
  "purchaseDate": "2024-01-15T00:00:00Z",
  "supplier": "Dell Vietnam",
  "price": 26000000,
  "warrantyEndDate": "2027-01-15T00:00:00Z",
  "status": 2,
  "imageUrl": "/images/lap001-new.jpg"
}
```

**Example Request:**

```bash
curl -X PUT "http://localhost:8080/api/equipments/3fa85f64-5717-4562-b3fc-2c963f66afa6" \
  -H "Content-Type: application/json" \
  -d '{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "code": "LAP001",
    "name": "Dell Latitude 7420",
    "type": "Laptop",
    "price": 26000000,
    "purchaseDate": "2024-01-15T00:00:00Z",
    "status": 2
  }'
```

**Success Response (204 No Content)**

---

#### 5. Delete Equipment (Soft Delete)

```http
DELETE /api/equipments/{id}
```

**Example Request:**

```bash
curl -X DELETE "http://localhost:8080/api/equipments/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Success Response (204 No Content)**

> **Note:** Đây là soft delete. Equipment vẫn tồn tại trong database với flag `IsDeleted = true`

---

### Warehouses

#### Create Warehouse Transaction (Import/Export)

```http
POST /api/warehouses/transactions
```

**Request Body:**

```json
{
  "warehouseItemId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "type": 1,
  "quantity": 10,
  "reason": "Nhập kho laptop mới",
  "performedBy": "admin"
}
```

**Transaction Types:**

| Value | Name | Description |
|-------|------|-------------|
| 1 | Import | Nhập kho |
| 2 | Export | Xuất kho |
| 3 | Adjustment | Điều chỉnh |

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/warehouses/transactions" \
  -H "Content-Type: application/json" \
  -d '{
    "warehouseItemId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "type": 1,
    "quantity": 10,
    "reason": "Nhập kho laptop mới"
  }'
```

**Success Response (201 Created):**

```json
"4gb96g75-6828-5673-c4gd-3d074g77bgb7"
```

---

### Assignments

#### Create Assignment

```http
POST /api/assignments
```

**Request Body:**

```json
{
  "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "assignedToUserId": "user123",
  "assignedToDepartment": "IT Department",
  "assignedDate": "2024-11-25T00:00:00Z",
  "notes": "Cấp phát laptop cho nhân viên mới",
  "assignedBy": "admin"
}
```

**Field Validation:**

- Either `assignedToUserId` OR `assignedToDepartment` must be provided
- `equipmentId` must exist and not deleted

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/assignments" \
  -H "Content-Type: application/json" \
  -d '{
    "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "assignedToUserId": "user123",
    "assignedDate": "2024-11-25T00:00:00Z",
    "notes": "Laptop for new developer"
  }'
```

**Success Response (201 Created):**

```json
"5hc07h86-7939-6784-d5he-4e185h88chc8"
```

**Side Effects:**
- Equipment status changed to `InUse` (2)
- Assignment status set to `Assigned` (1)

---

### Audits

#### Create Audit Record

```http
POST /api/audits
```

**Request Body:**

```json
{
  "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "checkDate": "2024-11-25T10:30:00Z",
  "checkedByUserId": "auditor01",
  "result": 1,
  "note": "Thiết bị trong tình trạng tốt",
  "location": "Phòng IT, tầng 3"
}
```

**Audit Results:**

| Value | Name | Description |
|-------|------|-------------|
| 1 | Match | Khớp với hồ sơ |
| 2 | NotMatch | Không khớp |
| 3 | Missing | Thiếu/Mất |

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/audits" \
  -H "Content-Type: application/json" \
  -d '{
    "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "checkDate": "2024-11-25T10:30:00Z",
    "result": 1,
    "location": "IT Room, Floor 3"
  }'
```

**Success Response (201 Created):**

```json
"6id18i97-8040-7895-e6if-5f296i99did9"
```

**Mobile App Integration:**

1. Mobile app quét QR code trên thiết bị
2. Parse Equipment Code từ QR
3. Lookup Equipment ID từ Code
4. POST audit record với Result

---

### Maintenances

#### Create Maintenance Request

```http
POST /api/maintenances
```

**Request Body:**

```json
{
  "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "requesterId": "user123",
  "description": "Màn hình bị nhấp nháy, cần kiểm tra",
  "notes": "Ưu tiên cao - thiết bị đang dùng"
}
```

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/maintenances" \
  -H "Content-Type: application/json" \
  -d '{
    "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "description": "Screen flickering issue",
    "requesterId": "user123"
  }'
```

**Success Response (201 Created):**

```json
"7je29j08-9151-8906-f7jg-6g307j00eje0"
```

**Side Effects:**
- Equipment status changed to `Repairing` (4)
- Maintenance status set to `Pending` (1)

**Maintenance Status Workflow:**

```
Pending (1) → InProgress (2) → Completed (3)
                            ↘ Cancelled (4)
```

---

### Liquidations

#### Create Liquidation Request

```http
POST /api/liquidations
```

**Request Body:**

```json
{
  "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "liquidationValue": 5000000,
  "note": "Thiết bị quá cũ, không còn giá trị sử dụng"
}
```

**Example Request:**

```bash
curl -X POST "http://localhost:8080/api/liquidations" \
  -H "Content-Type: application/json" \
  -d '{
    "equipmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "liquidationValue": 5000000,
    "note": "Equipment too old"
  }'
```

**Success Response (201 Created):**

```json
"8kf30k19-0262-9017-g8kh-7h418k11fkf1"
```

**Default State:**
- `IsApproved` = false
- Waiting for manager approval

---

## 💼 Use Cases

### Use Case 1: Nhập thiết bị mới vào hệ thống

```bash
# 1. Tạo thiết bị
POST /api/equipments
{
  "code": "LAP100",
  "name": "MacBook Pro 16\"",
  "type": "Laptop",
  "price": 60000000,
  "purchaseDate": "2024-11-25",
  "status": 1
}

# Response: equipment_id

# 2. QR Code tự động generate
# Lấy equipment detail để xem QR
GET /api/equipments/{equipment_id}

# Response chứa qrCodeBase64
# In QR và dán lên thiết bị
```

### Use Case 2: Cấp phát thiết bị cho nhân viên mới

```bash
# 1. Tìm thiết bị available
GET /api/equipments?status=1&type=Laptop

# 2. Cấp phát
POST /api/assignments
{
  "equipmentId": "{id}",
  "assignedToUserId": "newuser123",
  "assignedDate": "2024-11-25",
  "notes": "Onboarding equipment"
}

# Equipment status tự động chuyển sang InUse
```

### Use Case 3: Kiểm kê định kỳ với mobile app

```bash
# Mobile app flow:
# 1. User quét QR code
# QR data: "LAP100"

# 2. App lookup equipment
GET /api/equipments?keyword=LAP100

# 3. User xác nhận tình trạng
POST /api/audits
{
  "equipmentId": "{id}",
  "checkDate": "2024-11-25T14:30:00Z",
  "result": 1,
  "location": "Office Floor 5"
}

# 4. Sync to server (có thể offline)
# LastSyncDate được update
```

### Use Case 4: Quy trình sửa chữa

```bash
# 1. User báo hỏng
POST /api/maintenances
{
  "equipmentId": "{id}",
  "description": "Laptop không bật được",
  "requesterId": "user123"
}
# → Status: Pending, Equipment: Repairing

# 2. Admin gán technician (future feature)
PUT /api/maintenances/{id}
{
  "technicianId": "tech01",
  "status": 2  # InProgress
}

# 3. Technician update
PUT /api/maintenances/{id}
{
  "cost": 2000000,
  "notes": "Thay mainboard",
  "status": 3  # Completed
}
# → Equipment status: New hoặc InUse
```

### Use Case 5: Thanh lý thiết bị cũ

```bash
# 1. Tạo request thanh lý
POST /api/liquidations
{
  "equipmentId": "{id}",
  "liquidationValue": 3000000,
  "note": "Thiết bị quá 5 năm tuổi"
}
# → IsApproved: false

# 2. Manager approve (future API)
PUT /api/liquidations/{id}/approve
{
  "approvedBy": "manager01"
}
# → IsApproved: true
# → Equipment status: Liquidated
```

---

## ✅ Best Practices

### 1. Use Pagination

```bash
# BAD - Load all data
GET /api/equipments

# GOOD - Use pagination
GET /api/equipments?pageNumber=1&pageSize=20
```

### 2. Filter Data at Server

```bash
# BAD - Get all then filter client-side
GET /api/equipments
# Filter 1000 items in browser

# GOOD - Server-side filtering
GET /api/equipments?type=Laptop&status=1
```

### 3. Handle Errors Gracefully

```javascript
try {
  const response = await fetch('/api/equipments', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(equipment)
  });
  
  if (!response.ok) {
    const error = await response.json();
    console.error('Validation errors:', error.details);
    // Show user-friendly error messages
  }
} catch (error) {
  console.error('Network error:', error);
}
```

### 4. Cache QR Codes

```javascript
// QR code không thay đổi
// Cache locally sau khi load lần đầu
const qrCode = localStorage.getItem(`qr_${equipmentId}`);
if (!qrCode) {
  const equipment = await fetchEquipment(id);
  localStorage.setItem(`qr_${equipmentId}`, equipment.qrCodeBase64);
}
```

### 5. Validate Before Submit

```javascript
// Client-side validation trước khi gọi API
if (!equipment.code || equipment.price < 0) {
  showError('Invalid data');
  return;
}

// API call
await createEquipment(equipment);
```

---

## 📱 Mobile App Integration

### QR Code Format

```
Equipment Code: LAP001
```

### Offline Support

```javascript
// 1. Store audits locally
localStorage.setItem('pending_audits', JSON.stringify([
  { equipmentId, result, checkDate, ... }
]));

// 2. Sync when online
if (navigator.onLine) {
  const pending = JSON.parse(localStorage.getItem('pending_audits'));
  for (const audit of pending) {
    await POST('/api/audits', audit);
  }
  localStorage.removeItem('pending_audits');
}
```

---

## 🔄 Versioning

API versioning sẽ được implement trong tương lai:

```http
GET /api/v1/equipments
GET /api/v2/equipments
```

Hoặc via headers:

```http
GET /api/equipments
Accept: application/vnd.equipmentapi.v1+json
```

---

## 📞 Support

- **Swagger UI:** http://localhost:8080/swagger
- **Postman Collection:** `docs/postman/Equipment-Management-API.postman_collection.json`
- **Issues:** GitHub Issues
