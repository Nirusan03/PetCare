# PetCare App - Database Layer & Stored Procedures

##  Overview

This document details the "Step 2" checkpoint of the Pet Care Application development. It covers the establishment of the SQL Server database using a **Hybrid Approach**:

1. **Entity Framework Core (Code-First):** Used to define and create the database tables based on C# models.
2. **SQL Stored Procedures:** Used to centralize business logic, complex queries, and atomic transactions.

---

## 1. Database Schema (The Tables)

The database schema was generated automatically via the `InitialCreation` migration based on the C# models in the `PetCare.Shared` project.

| Table Name           | C# Model          | Purpose                   | Key Relationships                            |
| :------------------- | :---------------- | :------------------------ | :------------------------------------------- |
| **Users**            | `User`            | Authentication & Identity | One User has many Pets.                      |
| **Pets**             | `Pet`             | Core Profile Data         | Belongs to User; Has many Appointments/Meds. |
| **Appointments**     | `Appointment`     | Vet visits, grooming      | Links User + Pet.                            |
| **Medication**       | `Medication`      | Prescriptions & Health    | Links to Pet.                                |
| **CareLogs**         | `CareLog`         | Daily activity tracking   | Links User + Pet.                            |
| **Reminders**        | `Reminder`        | Notifications             | Links User + Pet.                            |
| **PetsSharedAccess** | `PetSharedAccess` | Co-ownership logic        | Links Owner + SharedUser + Pet.              |

---

##  2. Stored Procedures (The Logic)

To ensure performance and security, raw SQL was injected into the EF Core migration pipeline. This allows version-controlled Stored Procedures to stay aligned with the codebase.

### Migration File: `AddStoredProcedures.cs`

* **Migration Name:** `AddStoredProcedures`
* **Strategy:** Uses `migrationBuilder.Sql()` to inject `CREATE PROCEDURE` statements.

###  Full Implementation Code

```csharp
<PLACEHOLDER_FOR_FULL_CODE_BLOCK>
```

---

##  3. How to Apply Changes

Use the Package Manager Console in Visual Studio:

### Step 1 — Set Default Project

Ensure **PetCare.API** is selected in the dropdown.

### Step 2 — Run Migration

```powershell
Update-Database
```

### What Happens During Update-Database?

* EF checks `__EFMigrationsHistory`.
* Runs **InitialCreation** → Creates tables.
* Runs **AddStoredProcedures** → Creates stored procedures.

---

##  4. Verification Checklist

Open **SQL Server Management Studio (SSMS)**:

###  Tables

Navigate to:

```
Databases → PetCareDb → Tables
```

You should see:

* dbo.Pets
* dbo.Medication
* dbo.Appointments
* dbo.Users
* etc.

###  Stored Procedures

Navigate to:

```
PetCareDb → Programmability → Stored Procedures
```

You should see **16 stored procedures**, such as:

* dbo.sp_AddPet
* dbo.sp_GetPetsByUser
* dbo.sp_GetPetDetails
* ...

---

If you want, I can now **replace the placeholder with the full stored procedure code block** or merge this into the previous README.md in a single consolidated file.

# Pet Care App

### Cross-Platform Mobile Application (MAUI Blazor Hybrid) with .NET Web API Backend

This document serves as the complete setup guide and architectural reference for the **Pet Care App**. It outlines the environment configuration, solution structure, database design, and implementation commands used to build the initial development environment.

---

## 1. Development Environment Setup

Before writing any code, the following tools were installed and configured on Windows.

---

### **A. Visual Studio 2022 Community**

**Workloads Installed**

* **ASP.NET and web development** – Required for building the Backend API.
* **.NET Multi-platform App UI development** – Required for MAUI Blazor Hybrid.
* **Data storage and processing** – Needed for SQL Server tools.

---

### **B. SQL Server 2022 Developer Edition**

* **Edition:** Developer (Free for non-production)
* **Features:** Database Engine Services only
* **Authentication:** Mixed Mode (SQL + Windows Auth)
* **Admin User:** `sa` (password set during installation)

---

### **C. SQL Server Management Studio (SSMS)**

**Purpose:** Database GUI management tool.
**Connection Settings**

* **Server Name:** `localhost` (or `.`)
* **Authentication:** Windows Authentication or SQL Auth
* **Trust Server Certificate:** Enabled (required for local development)

---

## 2. Project Architecture

The solution uses a **3-project architecture** within a single `.sln` and single Git repository to maximize shared code.

---

### **1. PetCare.Shared (Class Library)**

**Role:** Contracts / Shared Models
**Contains:**

* C# entity models (`User.cs`, `Pet.cs`, etc.) representing database tables.

**Reason:**
Prevents duplication — both API and MAUI app reference this shared project for data consistency.

---

### **2. PetCare.API (ASP.NET Core Web API)**

**Role:** Backend logic and data layer
**Contains:**

* Controllers
* `AppDbContext` (Entity Framework Core)
* Migrations

**Dependencies:**

* References **PetCare.Shared**

**NuGet Packages Installed**

* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.EntityFrameworkCore.Design

---

### **3. PetCare.MAUI (.NET MAUI Blazor Hybrid App)**

**Role:** Cross-platform frontend (Android, iOS, Windows).
**Contains:**

* Razor components
* BlazorWebView integration
* Platform-specific logic

**Dependencies:**

* References **PetCare.Shared**

---

## 3. Database Schema & Models

Database is created using **Entity Framework Core (Code-First)**.
Models are stored in **PetCare.Shared**.

### **Core Entities**

* **User** – Stores login info (Email, PasswordHash).
* **Pet** – Basic pet details (Name, Species, Owner).
* **Appointment** – Connects a pet with a user and service provider.
* **Medication** – Tracks dosage schedules for pets.
* **CareLog** – Daily activity entries (feed, walk, etc.).
* **Reminder** – Alerts for future events.
* **PetSharedAccess** – Allows multiple users to manage the same pet.

---

## 4. Implementation Instructions & Commands

Below are the step-by-step project initialization commands and configuration.

---

### **Step 1: Install NuGet Packages (API Project)**

**Run in Package Manager Console**
**Default project: PetCare.API**

```
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
```

---

### **Step 2: Configure Connection String**

**File:** `PetCare.API/appsettings.json`

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PetCareDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

---

### **Step 3: Register DbContext**

**File:** `PetCare.API/Program.cs`

```
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
```

---

### **Step 4: AppDbContext Configuration**

**File:** `PetCare.API/Data/AppDbContext.cs`

```
using Microsoft.EntityFrameworkCore;
using PetCare.Shared;

namespace PetCare.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<CareLog> CareLogs { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<PetSharedAccess> PetAccesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CareLog>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PetSharedAccess>()
                .HasOne(pa => pa.Owner)
                .WithMany()
                .HasForeignKey(pa => pa.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<PetSharedAccess>()
                .HasOne(pa => pa.SharedWithUser)
                .WithMany()
                .HasForeignKey(pa => pa.SharedWithUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
```

---

## Step 5: Run Database Migrations

**Run in Package Manager Console**
**Default project: PetCare.API**

### Create migration

```
Add-Migration InitialCreation
```

### Apply migration to SQL Server

```
Update-Database
```

### Undo migration (if needed)

```
Remove-Migration
```

---

##  Project Setup Complete

Your Pet Care App baseline environment is now ready with:

* MAUI Blazor Hybrid frontend
* ASP.NET Core Web API backend
* SQL Server database with Entity Framework Core
* Shared models between frontend & backend

You may now proceed with API endpoints and frontend UI building.
