# Library Management System - OOP Project

## 📚 Project Overview
A comprehensive library management system built with ASP.NET Core MVC, featuring book management, member registration, and loan tracking with advanced business rules.

## 🏗️ Architecture

### Layers
1. **Presentation Layer (Library.MVC)** - ASP.NET Core MVC
   - Controllers: BooksController, MembersController, LoansController, AdminController
   - Views: Bootstrap 5 responsive UI
   
2. **Domain Layer (Library_Domain)** - Business Entities
   - Book entity with availability tracking
   - Member entity for library members
   - Loan entity with loan tracking and due dates
   
3. **Data Layer (Library.MVC.Data)** - Entity Framework Core
   - DbContext: ApplicationDbContext (with Identity integration)
   - Database: SQL Server
   - Seeding: Bogus-generated fake data

## 🚀 Features

✅ **Books Management**
- Create, read, update, delete books
- Track book availability status
- View book categories and ISBN

✅ **Members Management**
- Register new library members
- Store contact information
- View member loan history

✅ **Loans Management**
- Create loans for available books
- Mark loans as returned
- Track loan dates and due dates
- Overdue detection

✅ **Admin Features**
- Manage roles and permissions
- Role-based access control (RBAC)
- Admin dashboard

✅ **Business Rules Enforced**
- Cannot loan already loaned books
- Returned loans mark books as available
- Overdue loans are flagged
- One 14-day default loan period

## 🧪 Testing

### Unit Tests (xUnit)
Comprehensive test coverage with 10+ tests covering:
- Loan creation and restrictions
- Book availability tracking
- Member registration
- Overdue loan detection
- CRUD operations

**Run tests locally:**
```bash
dotnet test
```

**Run specific test file:**
```bash
dotnet test Library.Tests/LoanTests.cs
```

## 🔄 CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci.yml`) automatically:
- ✅ Builds on push/PR to main/master
- ✅ Runs all xUnit tests
- ✅ Reports build status

**View CI status:** GitHub Actions tab in repository

## 🛠️ Tech Stack

- **.NET Version:** 10.0
- **Framework:** ASP.NET Core MVC
- **Database:** SQL Server
- **ORM:** Entity Framework Core 10.0.2
- **Authentication:** ASP.NET Core Identity
- **UI Framework:** Bootstrap 5
- **Testing:** xUnit + Moq
- **Fake Data:** Bogus

## 📋 Project Structure

```
oop-s2-1-mvc-77487/
├── Library Domain/           # Domain entities
│   ├── Book.cs
│   ├── Member.cs
│   └── Loan.cs
├── Library.MVC/              # Presentation layer
│   ├── Controllers/
│   ├── Views/
│   ├── Data/
│   ├── Program.cs
│   └── appsettings.json
├── Library.Tests/            # Unit tests
│   ├── LoanTests.cs
│   ├── BookTests.cs
│   └── MemberTests.cs
├── .github/workflows/        # CI/CD
│   └── ci.yml
└── README.md
```

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (local or connection string configured)

### Installation

1. **Clone the repository:**
```bash
git clone https://github.com/Weszs/oop-s2-1-mvc-77487-2025-03-10.git
cd oop-s2-1-mvc-77487-2025-03-10
```

2. **Restore dependencies:**
```bash
dotnet restore
```

3. **Apply migrations:**
```bash
dotnet ef database update --project Library.MVC --startup-project Library.MVC
```

4. **Run the application:**
```bash
dotnet run --project Library.MVC
```

5. **Access the app:**
Navigate to `https://localhost:5001`

### Default Admin Account
- **Email:** admin@library.local
- **Password:** Admin123!

## 📊 Database Schema

### Books Table
- Id (PK)
- Title (string, required)
- Author (string, required)
- ISBN (string, nullable)
- Category (string, nullable)
- IsAvailable (bool)

### Members Table
- Id (PK)
- FullName (string, required)
- Email (string, required)
- Phone (string, nullable)

### Loans Table
- Id (PK)
- BookId (FK to Books)
- MemberId (FK to Members)
- LoanDate (DateTime)
- DueDate (DateTime)
- ReturnedDate (DateTime, nullable)

## 🧠 Key Design Patterns

1. **MVC Pattern** - Separation of concerns with Controllers, Views, and Models
2. **Repository Pattern** - Data access abstraction via EF Core DbContext
3. **Dependency Injection** - Constructor injection of ApplicationDbContext
4. **Role-Based Authorization** - [Authorize(Roles = "Admin")] attribute
5. **Entity Relationships** - One-to-many between Books↔Loans and Members↔Loans

## 📝 Assignment Requirements Met

✅ **30 marks** - EF Core models with relationships and migrations
✅ **16 marks** - Fake data seeding with Bogus
✅ **20 marks** - Books management page (Index, Create, Edit, Delete)
✅ **14 marks** - Loans workflow with business rules
✅ **10 marks** - Admin role management
✅ **10+ marks** - xUnit tests (10 comprehensive tests)
✅ **CI/CD** - GitHub Actions workflow

## 📚 References

- [ASP.NET Core MVC Documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [xUnit Testing Framework](https://xunit.net/)
- [Bogus - Fake Data Generator](https://github.com/bchavez/Bogus)

## 👤 Author
Created for OOP Course - Assignment 77487 (2025-03-10)

## 📄 License
This project is for educational purposes.
