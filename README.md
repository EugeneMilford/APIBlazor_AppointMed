# AppointMed - Healthcare Appointment Management System

A comprehensive healthcare management platform built with ASP.NET Core 8 and Blazor Server, designed to streamline patient appointments, prescriptions, and medical billing.

```
    ___                      _       _   __  __          _ 
   / _ \                    (_)     | | |  \/  |        | |
  / /_\ \_ __  _ __   ___  _ _ __  | |_| \  / | ___  __| |
  |  _  | '_ \| '_ \ / _ \| | '_ \ | __| |\/| |/ _ \/ _` |
  | | | | |_) | |_) | (_) | | | | || |_| |  | |  __/ (_| |
  \_| |_/ .__/| .__/ \___/|_|_| |_| \__|_|  |_|\___|\__,_|
        | |   | |                                          
        |_|   |_|                                          

              Healthcare Management & Appointment Platform
        ASP.NET Core 8 • Blazor Server • Automated Billing • JWT Security
```

## 🚀 Quick Start

### 1. Prerequisites

* .NET 8.0 SDK or later
* SQL Server (Express or higher)
* Visual Studio 2022 or VS Code
* Git

### 2. Clone & Configure

```bash
# Clone the repository
git clone https://github.com/EuegenMilford/AppointMed.git
cd AppointMed

# Update connection string in appsettings.json (API project)
# Edit "AppointMedDbConnectionString" to point to your SQL Server
```

### 3. Initialize Database

```bash
# Navigate to API project
cd AppointMed.API

# Apply database migrations
dotnet ef database update

# This creates the database schema and seeds:
# - Administrator account (admin@appointmed.com / Admin@2024)
# - Sample doctors and medicines
# - Initial roles (Administrator, User)
```

### 4. Run the Application

```
# Terminal 1 - Run the API
cd AppointMed.API
dotnet run

# Terminal 2 - Run the Blazor UI
cd AppointMed.Blazor.Web.UI
dotnet run
```

**Access the application:**
- **Blazor UI:** https://localhost:7138
- **API Swagger:** https://localhost:7017/swagger

---

## 📋 Overview

### Core Features

* ✅ **Appointment Management** — Schedule, view, and manage patient appointments with doctors
* ✅ **Doctor Directory** — Comprehensive doctor profiles with specializations and contact info
* ✅ **Prescription System** — Create prescriptions, track fulfillment, and manage medicine inventory
* ✅ **Medicine Catalog** — Maintain database of medicines with pricing and stock information
* ✅ **Automated Billing** — Automatic charging for appointments (R200) and prescriptions
* ✅ **Refund Processing** — Built-in refund system for cancelled appointments and prescriptions
* ✅ **Account Management** — Track patient account balances and complete transaction history
* ✅ **Role-Based Security** — JWT authentication with Administrator and Patient roles
* ✅ **Audit Trail** — Complete transaction log for financial transparency
* ✅ **Cascade Operations** — Automatic cleanup and refunds when deleting appointments

---

## 👥 Roles & Permissions

### 👑 Administrator — Full System Access

**User Management:**
* ✅ View all user accounts and profiles
* ✅ Manage system roles and permissions

**Medical Staff:**
* ✅ Create, edit, and delete doctor profiles
* ✅ Manage doctor specializations and availability
* ✅ View all doctor appointments

**Inventory:**
* ✅ Add, update, and remove medicines
* ✅ Set medicine prices and manage stock
* ✅ View complete medicine catalog

**Prescriptions:**
* ✅ Create prescriptions for any patient
* ✅ View all prescriptions system-wide
* ✅ Delete prescriptions (with automatic refunds)
* ✅ Access prescription fulfillment status

**Appointments:**
* ✅ View all appointments across the system
* ✅ Delete appointments (cascades to prescriptions)
* ✅ Automatic refund processing on cancellation

**Financial:**
* ✅ View all patient accounts
* ✅ Access complete transaction histories
* ✅ Monitor system-wide billing

**Default Admin Credentials:**
- Email: `admin@appointmed.com`
- Password: `Admin@2024`

---

### 👤 Patient (User) — Standard Patient Access

**Appointments:**
* ✅ View personal appointment history
* ✅ Book new appointments with available doctors
* ✅ Update appointment details
* ❌ Cannot delete appointments (admin only)

**Prescriptions:**
* ✅ View prescriptions linked to appointments
* ✅ Fulfill prescriptions (triggers medicine charge)
* ✅ Track prescription status
* ❌ Cannot create prescriptions (admin only)
* ❌ Cannot delete prescriptions

**Account & Billing:**
* ✅ View personal account balance
* ✅ Access complete transaction history
* ✅ See itemized charges (appointments, medicines)
* ✅ View refund transactions
* ❌ Cannot access other patients' accounts

**Doctors:**
* ✅ Browse doctor directory
* ✅ View doctor profiles and specializations
* ❌ Cannot create, edit, or delete doctors

**Medicines:**
* ✅ View medicine catalog and prices
* ❌ Cannot modify medicine database

**Demo Patient Credentials:**
- Email: `lisa.wilson@email.com`
- Password: `Lisa@2024`

---

## 💰 Billing System

### Transaction Types

| Type | Amount | Trigger | Refundable |
|------|--------|---------|------------|
| **Appointment** | R200.00 | Creating appointment | ✅ Yes (on delete) |
| **Prescription** | Variable | Fulfilling prescription | ✅ Yes (if deleted) |
| **Refund** | Negative | Deleting appointment/prescription | N/A |

### Transaction Flow

```
1. Patient books appointment
   └─> Account charged R200.00 (AppointmentId linked)

2. Admin creates prescription for appointment
   └─> No charge (prescription pending)

3. Patient fulfills prescription
   └─> Account charged medicine cost (PrescriptionId linked)

4. Admin deletes appointment
   ├─> Refunds R200.00 appointment charge
   ├─> Refunds all fulfilled prescription charges
   └─> Deletes all related prescriptions (cascade)

5. Admin deletes fulfilled prescription
   └─> Refunds medicine cost
```

### Account Balance Calculation

```csharp
Account Balance = SUM(All Transactions)

Example:
+ R200.00  (Appointment charge)
+ R150.00  (Prescription: Paracetamol)
+ R300.00  (Prescription: Antibiotics)
- R150.00  (Refund: Cancelled prescription)
─────────
= R500.00  (Current balance)
```

---

## 🛠️ Technology Stack

### Frontend

| Technology | Version | Purpose |
|------------|---------|---------|
| **Blazor Server** | .NET 8.0 | Interactive web UI framework |
| **Bootstrap** | 5.3.2 | Responsive design & components |
| **Blazored.LocalStorage** | 4.5.0 | Browser storage for JWT tokens |
| **AutoMapper** | 13.0.1 | Object-to-object mapping |

### Backend (API)

| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 8.0 | RESTful API framework |
| **Entity Framework Core** | 8.0.22 | ORM for database operations |
| **ASP.NET Core Identity** | 8.0.22 | User authentication & management |
| **JWT Bearer** | 8.0.22 | Token-based authentication |
| **SQL Server** | Latest | Primary database |
| **AutoMapper** | 13.0.1 | DTO mapping |
| **Serilog** | 8.0.3 | Structured logging |
| **Swashbuckle** | 6.6.2 | API documentation (Swagger) |

### Development Tools

| Tool | Version | Purpose |
|------|---------|---------|
| **NSwag** | Latest | OpenAPI client generation |
| **Newtonsoft.Json** | 13.0.3 | JSON serialization |

---

## 📁 Project Structure

```
AppointMed/
├── AppointMed.API/                    # Backend REST API
│   ├── Controllers/
│   │   ├── AccountsController.cs      # Account & billing endpoints
│   │   ├── AppointmentsController.cs  # Appointment management
│   │   ├── AuthController.cs          # Login & registration
│   │   ├── DoctorsController.cs       # Doctor CRUD operations
│   │   ├── MedicinesController.cs     # Medicine catalog
│   │   └── PrescriptionsController.cs # Prescription management
│   ├── Data/
│   │   ├── AppointMedDbContext.cs     # EF Core database context
│   │   ├── Account.cs                 # Account entity
│   │   ├── AccountTransaction.cs      # Transaction records
│   │   ├── Appointment.cs             # Appointment entity
│   │   ├── Doctor.cs                  # Doctor entity
│   │   ├── Medicine.cs                # Medicine entity
│   │   └── Prescription.cs            # Prescription entity
│   ├── Models/
│   │   ├── Account/                   # Account DTOs
│   │   ├── Appointment/               # Appointment DTOs
│   │   ├── Doctor/                    # Doctor DTOs
│   │   ├── Medicine/                  # Medicine DTOs
│   │   ├── Prescription/              # Prescription DTOs
│   │   └── User/                      # Auth DTOs
│   ├── Repository/
│   │   ├── Interface/                 # Repository interfaces
│   │   └── Implementation/            # Repository implementations
│   ├── Static/
│   │   ├── CustomClaimTypes.cs        # JWT claim types
│   │   └── Messages.cs                # Error messages
│   ├── Configurations/
│   │   └── MapperConfig.cs            # AutoMapper profiles
│   ├── Migrations/                    # EF Core migrations
│   ├── Program.cs                     # API entry point
│   └── appsettings.json               # API configuration
│
├── AppointMed.Blazor.Web.UI/          # Blazor Server UI
│   ├── Components/                    # Reusable UI components
│   │   └── Layout/
│   │       ├── MainLayout.razor       # Main app layout
│   │       └── NavMenu.razor          # Navigation sidebar
│   ├── Pages/
│   │   ├── Account/
│   │   │   └── Index.razor            # Account balance & transactions
│   │   ├── Appointments/
│   │   │   ├── Index.razor            # Appointment list
│   │   │   ├── Create.razor           # Book appointment
│   │   │   ├── Update.razor           # Edit appointment
│   │   │   └── Details.razor          # Appointment details
│   │   ├── Doctors/
│   │   │   ├── Index.razor            # Doctor directory
│   │   │   ├── Create.razor           # Add doctor (admin)
│   │   │   ├── Update.razor           # Edit doctor (admin)
│   │   │   └── Details.razor          # Doctor profile
│   │   ├── Medicines/
│   │   │   ├── Index.razor            # Medicine catalog
│   │   │   ├── Create.razor           # Add medicine (admin)
│   │   │   └── Update.razor           # Edit medicine (admin)
│   │   ├── Prescriptions/
│   │   │   ├── Index.razor            # Prescription list
│   │   │   └── Create.razor           # Create prescription (admin)
│   │   ├── Users/
│   │   │   ├── Login.razor            # User login
│   │   │   ├── Register.razor         # User registration
│   │   │   └── Logout.razor           # User logout
│   │   ├── Home.razor                 # Landing page
│   │   └── About.razor                # System information
│   ├── Services/
│   │   ├── Base/
│   │   │   ├── ServiceClient.cs       # NSwag generated API client
│   │   │   ├── BaseHttpService.cs     # Base service with auth
│   │   │   └── Response.cs            # API response wrapper
│   │   ├── Authentication/
│   │   │   ├── IAuthenticationService.cs
│   │   │   └── AuthenticationService.cs
│   │   ├── IAccountService.cs
│   │   ├── AccountService.cs
│   │   ├── IAppointmentService.cs
│   │   ├── AppointmentService.cs
│   │   ├── IDoctorService.cs
│   │   ├── DoctorService.cs
│   │   ├── IMedicineService.cs
│   │   ├── MedicineService.cs
│   │   ├── IPrescriptionService.cs
│   │   └── PrescriptionService.cs
│   ├── Providers/
│   │   └── ApiAuthenticationStateProvider.cs  # JWT auth state
│   ├── Configurations/
│   │   └── MapperConfig.cs            # AutoMapper profiles
│   ├── wwwroot/
│   │   ├── css/                       # Custom stylesheets
│   │   └── favicon.ico                # App icon
│   ├── Program.cs                     # Blazor entry point
│   ├── App.razor                      # Root component
│   ├── _Imports.razor                 # Global using statements
│   └── appsettings.json               # UI configuration
│
└── README.md                          # This file
```

---

## 🗄️ Database Schema

### Core Tables

```sql
-- Users & Authentication
AspNetUsers          -- Identity user accounts
AspNetRoles          -- User roles (Administrator, User)
AspNetUserRoles      -- User-role mappings

-- Medical Entities
Doctors              -- Doctor profiles and specializations
Medicines            -- Medicine catalog with pricing
Appointments         -- Patient appointments with doctors
Prescriptions        -- Prescriptions linked to appointments

-- Financial
Accounts             -- Patient account records
AccountTransactions  -- All billing transactions (charges & refunds)
```

### Entity Relationships

```
User (1) ────────< (N) Appointments
                         │
                         │ (1)
                         │
                         ↓
                        (N) Prescriptions
                         │
                         │ (N)
                         │
                         ↓
                        (1) Medicine

User (1) ────────< (1) Account
                         │
                         │ (1)
                         │
                         ↓
                        (N) AccountTransactions
                               │
                               ├── AppointmentId (FK)
                               └── PrescriptionId (FK)

Doctor (1) ──────< (N) Appointments
```

---

## 🔐 Authentication & Security

### JWT Token-Based Authentication

AppointMed uses **JWT (JSON Web Token)** for secure, stateless authentication

```

**Security Features:**
* 🔒 Passwords hashed with ASP.NET Core Identity (PBKDF2)
* 🎫 JWT tokens stored in browser LocalStorage
* ⏰ Configurable token expiration
* 🚫 Protected API endpoints with `[Authorize]` attribute
* 👮 Role-based authorization (`[Authorize(Roles = "Administrator")]`)

---
