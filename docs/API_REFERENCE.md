# 🔌 API Reference

## Equipment Management System REST API

> **API Version:** 1.0  
> **Base URL:** `https://localhost:7072/api` (Development)  
> **Protocol:** HTTPS  
> **Format:** JSON

---

## 📋 Table of Contents

- [Authentication](#authentication)
- [Response Format](#response-format)
- [Pagination](#pagination)
- [Error Codes](#error-codes)
- [Equipments API](#equipments-api)
- [Warehouses API](#warehouses-api)
- [Assignments API](#assignments-api)
- [Maintenances API](#maintenances-api)
- [Liquidations API](#liquidations-api)
- [Audits API](#audits-api)

---

## 🔐 Authentication

Currently, the API does not require authentication. Future versions will implement:
- JWT Bearer Token authentication
- Role-based authorization (Admin, Manager, Technician, Auditor)

**Planned Header**:
```http
Authorization: Bearer {token}
```

---

## 📦 Response Format

### Success Response

**HTTP 200 OK** (GET/PUT):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Dell Latitude 5420",
  "code": "EQ-2025-001",
  "status": "Available"
}
```

**HTTP 201 Created** (POST):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Error Response

**HTTP 400 Bad Request** (Validation):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Equipment name is required"],
    "Price": ["Price must be greater than zero"]
  }
}
```

**HTTP 404 Not Found**:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Equipment with ID 3fa85f64-5717-4562-b3fc-2c963f66afa6 not found"
}
```

**HTTP 500 Internal Server Error**:
```json
{
  "error": "An error occurred",
  "message": "Database connection failed"
}
```

---

## 📄 Pagination

All list endpoints support pagination with the following query parameters:

| Parameter | Type | Default | Max | Description |
|-----------|------|---------|-----|-------------|
| `pageNumber` | int | 1 | - | Page number (1-indexed) |
| `pageSize` | int | 10 | 100 | Items per page |

**Example Request**:
```http
GET /api/equipments?pageNumber=2&pageSize=20
```

**Paginated Response**:
```json
{
  "items": [
    { "id": "...", "name": "..." }
  ],
  "pageNumber": 2,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

---

## ⚠️ Error Codes

| HTTP Code | Description | Common Cause |
|-----------|-------------|--------------|
| 400 | Bad Request | Validation failed |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Business rule violation |
| 500 | Internal Server Error | Server/database error |

---

## 🖥️ Equipments API

Manage IT equipment inventory (laptops, desktops, printers, etc.)

### List Equipments

```http
GET /api/equipments
```

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 10, max: 100) |
| status | string | No | Filter by status: `New`, `Available`, `Assigned`, `InMaintenance`, `Liquidated` |
| type | string | No | Filter by equipment type |
| searchTerm | string | No | Search in name, code, or manufacturer |

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "code": "EQ-2025-001",
      "name": "Dell Latitude 5420",
      "type": "Laptop",
      "manufacturer": "Dell",
      "serialNumber": "SN123456789",
      "status": "Available",
      "price": 25000000.00,
      "purchaseDate": "2025-01-15T00:00:00Z",
      "warrantyEndDate": "2028-01-15T00:00:00Z",
      "notes": "High-performance laptop",
      "qrCode": "data:image/png;base64,iVBOR...",
      "createdAt": "2025-01-15T08:30:00Z",
      "updatedAt": "2025-01-15T08:30:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Get Equipment by ID

```http
GET /api/equipments/{id}
```

**Path Parameters**:
- `id` (guid) - Equipment ID

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "code": "EQ-2025-001",
  "name": "Dell Latitude 5420",
  "type": "Laptop",
  "manufacturer": "Dell",
  "serialNumber": "SN123456789",
  "status": "Available",
  "price": 25000000.00,
  "purchaseDate": "2025-01-15T00:00:00Z",
  "warrantyEndDate": "2028-01-15T00:00:00Z",
  "notes": "High-performance laptop",
  "qrCode": "data:image/png;base64,iVBOR...",
  "createdAt": "2025-01-15T08:30:00Z",
  "updatedAt": "2025-01-15T08:30:00Z"
}
```

**Errors**:
- `404 Not Found` - Equipment not found

### Create Equipment

```http
POST /api/equipments
```

**Request Body**:
```json
{
  "code": "EQ-2025-001",
  "name": "Dell Latitude 5420",
  "type": "Laptop",
  "manufacturer": "Dell",
  "serialNumber": "SN123456789",
  "price": 25000000.00,
  "purchaseDate": "2025-01-15T00:00:00Z",
  "warrantyEndDate": "2028-01-15T00:00:00Z",
  "notes": "High-performance laptop"
}
```

**Validation Rules**:
- `code`: Required, max 50 characters, unique
- `name`: Required, max 200 characters
- `type`: Required, max 100 characters
- `price`: Required, > 0
- `purchaseDate`: Required, ≤ current date
- `warrantyEndDate`: Optional, > purchaseDate

**Response**: `201 Created`
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Headers**:
```http
Location: /api/equipments/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Errors**:
- `400 Bad Request` - Validation failed
- `409 Conflict` - Equipment code already exists

### Update Equipment

```http
PUT /api/equipments/{id}
```

**Path Parameters**:
- `id` (guid) - Equipment ID

**Request Body**:
```json
{
  "name": "Dell Latitude 5420 (Updated)",
  "type": "Laptop",
  "manufacturer": "Dell",
  "serialNumber": "SN123456789",
  "price": 26000000.00,
  "purchaseDate": "2025-01-15T00:00:00Z",
  "warrantyEndDate": "2028-01-15T00:00:00Z",
  "notes": "Updated specifications"
}
```

**Response**: `200 OK`

**Errors**:
- `404 Not Found` - Equipment not found
- `400 Bad Request` - Validation failed

### Delete Equipment (Soft Delete)

```http
DELETE /api/equipments/{id}
```

**Path Parameters**:
- `id` (guid) - Equipment ID

**Response**: `204 No Content`

**Errors**:
- `404 Not Found` - Equipment not found
- `409 Conflict` - Cannot delete equipment with active assignments

### Update Equipment Status

```http
PUT /api/equipments/{id}/status
```

**Request Body**:
```json
{
  "status": "Available"
}
```

**Valid Statuses**: `New`, `Available`, `Assigned`, `InMaintenance`, `Liquidated`

**Response**: `200 OK`

---

## 📦 Warehouses API

Manage warehouse inventory and transactions

### List Warehouse Items

```http
GET /api/warehouses
```

**Query Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| pageNumber | int | Page number (default: 1) |
| pageSize | int | Items per page (default: 10) |
| type | string | Filter by equipment type |
| minQuantity | int | Minimum quantity filter |

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "equipmentType": "Laptop",
      "manufacturer": "Dell",
      "model": "Latitude 5420",
      "quantity": 15,
      "unitPrice": 25000000.00,
      "totalValue": 375000000.00,
      "location": "Warehouse A - Shelf B3",
      "minimumStockLevel": 5,
      "createdAt": "2025-01-01T00:00:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 20,
  "totalPages": 2
}
```

### Get Warehouse Item by ID

```http
GET /api/warehouses/{id}
```

**Response**: `200 OK`

### Create Warehouse Item

```http
POST /api/warehouses
```

**Request Body**:
```json
{
  "equipmentType": "Laptop",
  "manufacturer": "Dell",
  "model": "Latitude 5420",
  "quantity": 15,
  "unitPrice": 25000000.00,
  "location": "Warehouse A - Shelf B3",
  "minimumStockLevel": 5
}
```

**Response**: `201 Created`

### Record Warehouse Transaction

```http
POST /api/warehouses/transactions
```

**Request Body**:
```json
{
  "warehouseItemId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "transactionType": "Import",
  "quantity": 10,
  "reason": "New stock arrival",
  "performedBy": "John Doe"
}
```

**Transaction Types**: `Import`, `Export`

**Response**: `201 Created`

### Get Low Stock Items

```http
GET /api/warehouses/low-stock
```

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "equipmentType": "Mouse",
      "quantity": 3,
      "minimumStockLevel": 10,
      "deficit": 7
    }
  ]
}
```

---

## 👤 Assignments API

Manage equipment assignments to employees

### List Assignments

```http
GET /api/assignments
```

**Query Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| pageNumber | int | Page number |
| pageSize | int | Items per page |
| status | string | `Active`, `Returned` |
| technicianName | string | Filter by technician |

**Response**: `200 OK`

### Get Assignment by ID

```http
GET /api/assignments/{id}
```

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "equipmentCode": "EQ-2025-001",
  "equipmentName": "Dell Latitude 5420",
  "technicianId": "tech-001",
  "technicianName": "Nguyen Van A",
  "assignedDate": "2025-02-01T00:00:00Z",
  "returnDate": null,
  "notes": "For field work",
  "status": "Active"
}
```

### Create Assignment

```http
POST /api/assignments
```

**Request Body**:
```json
{
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "technicianId": "tech-001",
  "technicianName": "Nguyen Van A",
  "assignedDate": "2025-02-01T00:00:00Z",
  "notes": "For field work"
}
```

**Business Rules**:
- Equipment must have status `Available`
- Equipment cannot already be assigned
- Equipment automatically changes to `Assigned` status

**Response**: `201 Created`

### Return Assignment

```http
PUT /api/assignments/{id}/return
```

**Request Body**:
```json
{
  "returnDate": "2025-03-01T00:00:00Z",
  "condition": "Good condition",
  "notes": "Returned on time"
}
```

**Business Rules**:
- Assignment must have status `Active`
- returnDate must be ≥ assignedDate
- Equipment status changes back to `Available`

**Response**: `200 OK`

---

## 🔧 Maintenances API

Manage equipment maintenance requests and tracking

### List Maintenance Requests

```http
GET /api/maintenances
```

**Query Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| status | string | `Pending`, `InProgress`, `Completed`, `Cancelled` |
| equipmentId | guid | Filter by equipment |

**Response**: `200 OK`

### Get Maintenance by ID

```http
GET /api/maintenances/{id}
```

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "equipmentCode": "EQ-2025-001",
  "equipmentName": "Dell Latitude 5420",
  "issueDescription": "Screen flickering",
  "requestedDate": "2025-02-15T00:00:00Z",
  "scheduledDate": "2025-02-20T00:00:00Z",
  "completedDate": null,
  "cost": 2000000.00,
  "performedBy": "Tech Support Team",
  "notes": "Warranty repair",
  "status": "InProgress"
}
```

### Create Maintenance Request

```http
POST /api/maintenances
```

**Request Body**:
```json
{
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "issueDescription": "Screen flickering",
  "requestedDate": "2025-02-15T00:00:00Z",
  "scheduledDate": "2025-02-20T00:00:00Z",
  "estimatedCost": 2000000.00,
  "notes": "Urgent repair needed"
}
```

**Response**: `201 Created`

### Update Maintenance Status

```http
PUT /api/maintenances/{id}/status
```

**Request Body**:
```json
{
  "status": "Completed",
  "actualCost": 1500000.00,
  "completedDate": "2025-02-22T00:00:00Z",
  "resolution": "Replaced screen panel",
  "performedBy": "Tech Support Team"
}
```

**Status Transitions**:
- `Pending` → `InProgress`, `Cancelled`
- `InProgress` → `Completed`, `Cancelled`

**Response**: `200 OK`

---

## 🗑️ Liquidations API

Manage equipment disposal and liquidation workflow

### List Liquidation Requests

```http
GET /api/liquidations
```

**Query Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| status | string | `Pending`, `Approved`, `Rejected` |
| fromDate | datetime | Filter from date |
| toDate | datetime | Filter to date |

**Response**: `200 OK`

### Get Liquidation by ID

```http
GET /api/liquidations/{id}
```

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "equipmentCode": "EQ-2020-001",
  "equipmentName": "Old Dell Desktop",
  "reason": "End of life, frequent breakdowns",
  "requestedBy": "IT Manager",
  "requestedDate": "2025-03-01T00:00:00Z",
  "approvedBy": null,
  "approvedDate": null,
  "status": "Pending",
  "estimatedValue": 1000000.00,
  "notes": "5 years old, no longer cost-effective to repair"
}
```

### Create Liquidation Request

```http
POST /api/liquidations
```

**Request Body**:
```json
{
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "reason": "End of life, frequent breakdowns",
  "requestedBy": "IT Manager",
  "estimatedValue": 1000000.00,
  "notes": "5 years old, no longer cost-effective to repair"
}
```

**Response**: `201 Created`

### Approve Liquidation

```http
PUT /api/liquidations/{id}/approve
```

**Request Body**:
```json
{
  "approvedBy": "Finance Director",
  "notes": "Approved for disposal"
}
```

**Business Rules**:
- Status must be `Pending`
- Equipment status changes to `Liquidated`
- Warehouse export transaction created

**Response**: `200 OK`

### Reject Liquidation

```http
PUT /api/liquidations/{id}/reject
```

**Request Body**:
```json
{
  "rejectedBy": "Finance Director",
  "rejectionReason": "Equipment still usable, requires maintenance only"
}
```

**Response**: `200 OK`

### Get Pending Liquidations

```http
GET /api/liquidations/pending
```

**Response**: `200 OK` - List of liquidations with status `Pending`

---

## 📋 Audits API

Manage equipment audits and verification (optimized for mobile apps)

### List Audit Records

```http
GET /api/audits
```

**Query Parameters**:
| Parameter | Type | Description |
|-----------|------|-------------|
| pageNumber | int | Page number |
| pageSize | int | Items per page |
| fromDate | datetime | Filter from date |
| toDate | datetime | Filter to date |
| result | string | `Match`, `NotMatch`, `Missing` |

**Response**: `200 OK`

### Get Audit by ID

```http
GET /api/audits/{id}
```

**Response**: `200 OK`
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
  "equipmentCode": "EQ-2025-001",
  "equipmentName": "Dell Latitude 5420",
  "auditDate": "2025-04-01T10:30:00Z",
  "auditor": "Nguyen Van B",
  "expectedLocation": "Office Floor 3",
  "actualLocation": "Office Floor 3",
  "expectedStatus": "Assigned",
  "actualStatus": "Assigned",
  "condition": "Good",
  "result": "Match",
  "notes": "All checks passed"
}
```

### Batch Create Audit Records (Mobile)

```http
POST /api/audits/batch
```

**Purpose**: Optimize mobile app offline workflow - upload multiple audit records at once

**Request Body**:
```json
{
  "auditRecords": [
    {
      "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "auditDate": "2025-04-01T10:30:00Z",
      "auditor": "Nguyen Van B",
      "actualLocation": "Office Floor 3",
      "actualStatus": "Assigned",
      "condition": "Good",
      "notes": "OK"
    },
    {
      "equipmentId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
      "auditDate": "2025-04-01T10:35:00Z",
      "auditor": "Nguyen Van B",
      "actualLocation": "Office Floor 3",
      "actualStatus": "Assigned",
      "condition": "Minor scratches",
      "notes": "Cosmetic damage"
    }
  ]
}
```

**Validation Rules**:
- Maximum 1000 records per batch
- Each record validated individually
- Partial success supported (some records may fail)

**Response**: `200 OK`
```json
{
  "totalRecords": 2,
  "successCount": 2,
  "failureCount": 0,
  "results": [
    {
      "index": 0,
      "success": true,
      "auditRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "error": null
    },
    {
      "index": 1,
      "success": true,
      "auditRecordId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "error": null
    }
  ]
}
```

**Partial Failure Example**:
```json
{
  "totalRecords": 3,
  "successCount": 2,
  "failureCount": 1,
  "results": [
    {
      "index": 0,
      "success": true,
      "auditRecordId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "error": null
    },
    {
      "index": 1,
      "success": false,
      "auditRecordId": null,
      "error": "Equipment with ID 5fa85f64... not found"
    },
    {
      "index": 2,
      "success": true,
      "auditRecordId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "error": null
    }
  ]
}
```

### Get Audits by Equipment

```http
GET /api/audits/equipment/{equipmentId}
```

**Path Parameters**:
- `equipmentId` (guid) - Equipment ID

**Response**: `200 OK` - List of all audits for the equipment

### Get Audits for Sync (Mobile Incremental Sync)

```http
GET /api/audits/sync
```

**Query Parameters**:
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| sinceDate | datetime | No | Get audits created/updated after this date |
| pageNumber | int | No | Page number (default: 1) |
| pageSize | int | No | Items per page (default: 50, max: 1000) |

**Purpose**: Mobile app incremental data sync - only download new/changed records

**Example Request**:
```http
GET /api/audits/sync?sinceDate=2025-04-01T00:00:00Z&pageSize=100
```

**Response**: `200 OK`
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "equipmentId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "equipmentCode": "EQ-2025-001",
      "auditDate": "2025-04-01T10:30:00Z",
      "auditor": "Nguyen Van B",
      "result": "Match",
      "updatedAt": "2025-04-01T10:35:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 100,
  "totalCount": 45
}
```

**Mobile Sync Strategy**:
1. **Initial Sync**: Call without `sinceDate` to get all records
2. **Incremental Sync**: Call with `sinceDate` = last sync timestamp
3. **Store** `updatedAt` of latest record for next sync

### Update Audit Record

```http
PUT /api/audits/{id}
```

**Request Body**:
```json
{
  "actualLocation": "Office Floor 4 (Moved)",
  "actualStatus": "Assigned",
  "condition": "Good",
  "notes": "Equipment relocated"
}
```

**Response**: `200 OK`

---

## 📊 Common Query Patterns

### Filtering Examples

**Multiple statuses**:
```http
GET /api/equipments?status=Available&status=Assigned
```

**Date range**:
```http
GET /api/maintenances?fromDate=2025-01-01&toDate=2025-12-31
```

**Search with pagination**:
```http
GET /api/equipments?searchTerm=Dell&pageNumber=1&pageSize=20
```

### Sorting

Default sorting: `CreatedAt DESC` (newest first)

Future support for custom sorting:
```http
GET /api/equipments?sortBy=name&sortOrder=asc
```

---

## 🔧 Rate Limiting

Currently no rate limiting. Future implementation:
- **100 requests/minute** per IP
- **1000 requests/hour** per IP
- **10,000 requests/day** per IP

**Headers**:
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 2025-04-01T11:00:00Z
```

---

## 📱 Mobile API Best Practices

### Batch Operations

Use batch endpoints for offline scenarios:
- ✅ `/api/audits/batch` - Upload up to 1000 audit records at once
- ✅ Partial success handling - some records can fail while others succeed

### Incremental Sync

Use sync endpoints with `sinceDate` parameter:
- ✅ `/api/audits/sync?sinceDate=2025-04-01T10:00:00Z`
- ✅ Only downloads new/changed records since last sync
- ✅ Reduces bandwidth and improves performance

### Pagination for Large Datasets

- ✅ Use reasonable `pageSize` (50-100 for mobile)
- ✅ Implement infinite scroll/load more
- ✅ Cache results locally

---

## 🧪 Testing the API

### Postman Collections

Import Postman collections from `/postman/` directory:
- `Equipment-Management-API.postman_collection.json`
- `Warehouse-Management-API.postman_collection.json`
- `Assignment-Management-API.postman_collection.json`
- `Maintenance-Management-API.postman_collection.json`
- `Liquidation-Management-API.postman_collection.json`
- `Audit-Management-API.postman_collection.json`

Each collection includes:
- Pre-configured requests
- Example responses
- Test scripts
- Environment variables

### Swagger UI

Access interactive API documentation:
```
https://localhost:7072/swagger
```

Features:
- Try out endpoints
- View request/response schemas
- Download OpenAPI specification

---

## 📚 Related Documentation

- [Architecture Guide](ARCHITECTURE.md) - System design and patterns
- [Development Guidelines](GUIDELINES.md) - Coding standards
- [Getting Started](GETTING_STARTED.md) - Setup instructions

---

**API Version**: 1.0  
**Last Updated**: December 12, 2025  
**Maintainer**: API Team
