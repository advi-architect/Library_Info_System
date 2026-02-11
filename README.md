# Library Book Reservation System

A microservices-based Library Management System built using:

- .NET 8 Web API
- Entity Framework Core (InMemory / SQL Ready)
- JWT Authentication
- Role-Based Authorization (Admin / Member)
- Clean Architecture (Controller → Service → Data Layer)

---

## Architecture Overview

This solution contains two services:

### 1️⃣ BookService
Responsible for:
- Add Book (Admin)
- Update Book (Admin)
- Delete Book (Admin)
- Search Books (Member/Admin)
- Reserve Book (Member)
- View My Reserved Books

### 2️⃣ MemberService
Responsible for:
- User Registration
- Login
- JWT Token Generation
- Reservation Handling

---

##  Authentication & Authorization

- JWT-based authentication
- Role-based access control
- Roles:
  - **Admin** → Manage books
  - **Member** → Reserve & View books
