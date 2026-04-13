# 🏥 Clinic Management System API

A Clean Architecture-based RESTful API built with ASP.NET Core for managing a clinic system including users, patients, doctors, and appointments with JWT authentication and role-based authorization.

---

## 📌 Features

- 🔐 JWT Authentication & Authorization
- 👥 Role-based access (Admin / Doctor / Receptionist)
- 🧱 Clean Architecture (Domain / Application / Infrastructure / API)
- 🗄️ Entity Framework Core with SQL Server
- 📦 Repository & Service Pattern
- ⚡ Dependency Injection
- 🧪 Swagger API Documentation
- 🚨 Global Exception Handling Middleware
- 📊 FluentValidation for input validation

---

## 🏗️ Project Architecture
- Clinic.Project.Api
- Clinic.Application
- Clinic.Domain
- Clinic.Infrastructure
- Clinic.Application.Tests
```

HospitalSystem.sln
├── Hospital.Domain
│   ├── Entities
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   ├── Appointment.cs
│   │   ├── Invoice.cs
│   │   ├── MedicalRecord.cs
│   │   └── User.cs 
│   ├── Enums
│   │   ├── AppointmentStatus.cs
│   │   └── UserRole.cs
│
├── Hospital.Application
│   ├── DTOs 
│   │   ├── Auth/
│   │   ├── Doctors/
│   │   ├── Invoices/
│   │   └── ...
│   ├── Interfaces ( Repositories)
│   │   ├── IPatientRepository.cs
│   │   ├── IDoctorRepository.cs
│   │   ├── IAppointmentRepository.cs
│   │   ├── IInvoiceRepository.cs
│   │   └── IMedicalRecordRepository.cs
│   └── UseCases ( Handlers)
│       ├── Patients/
│       ├── Doctors/
│       ├── Appointments/
│       └── ...
│
├── Hospital.Infrastructure
│   ├── Data
│   │   └── AppDbContext.cs
│   ├── Repositories (تنفيذ الـ Interfaces)
│   │   ├── PatientRepository.cs
│   │   ├── DoctorRepository.cs
│   │   ├── AppointmentRepository.cs
│   │   └── ...
│   ├── Configurations
│   └── Migrations
│
└── Hospital.API (Presentation Layer)
    ├── Controllers
    │   ├── AuthController.cs
    │   ├── DoctorsController.cs
    │   ├── AppointmentsController.cs
    │   ├── InvoicesController.cs
    │   ├── MedicalRecordsController.cs
    │   ├── PatientsController.cs
    │   └── AdminUsersController.cs
    ├── Program.cs
    └── appsettings.json
  

---
```
  ## 🔐 Authentication Flow

1. User registers / logs in
2. Server returns JWT Token
3. Token is used in requests:

```

Authorization:  YOUR\_TOKEN ("without Bearer " )

👤 Roles

Admin → Full access
Doctor → Manage patients & medical data
Receptionist → Manage appointments

⚙️ Tech Stack
ASP.NET Core Web API
Entity Framework Core
SQL Server
JWT Authentication
AutoMapper

FluentValidation
Swagger / OpenAPI
