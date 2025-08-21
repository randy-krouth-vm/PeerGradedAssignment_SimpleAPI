# PeerGradedAssignment_SimpleAPI
*A simple API built with ASP.NET Core minimal APIs (no controllers)*

---

## 📌 Project Overview
This project is a **User Management API** created using **ASP.NET Core 6+ minimal APIs**.  
It demonstrates:
- Building APIs **without controllers**
- Using **middleware** for logging, authentication, and error handling
- Implementing **CRUD operations** for a `User` resource

The final output is a working API that supports user management while meeting corporate requirements for **auditing**, **security**, and **reliability**.

---

## 🛠 Features

### ✅ CRUD Endpoints
- **GET /api/users** → Retrieve all users
- **GET /api/users/{id}** → Retrieve a single user by ID
- **POST /api/users** → Add a new user
- **PUT /api/users/{id}** → Update an existing user
- **DELETE /api/users/{id}** → Remove a user by ID

### ✅ Middleware
- **Error Handling Middleware**
  - Catches unhandled exceptions
  - Returns standardized JSON: `{ "error": "Internal server error." }`
- **Authentication Middleware**
  - Validates a simple token from the `Authorization` header
  - Requires: `Authorization: Bearer mysecrettoken`
  - Returns `401 Unauthorized` if missing/invalid
- **Logging Middleware**
  - Logs HTTP method, request path, and response status code

### ✅ Validation
- Prevents adding users with missing names or invalid emails

---

## 🚀 Getting Started

### Prerequisites
- [.NET 6 SDK or later](https://dotnet.microsoft.com/download)
- Git

### Run
Clone and run:
```bash
git clone https://github.com/yourusername/PeerGradedAssignment_SimpleAPI.git
cd PeerGradedAssignment_SimpleAPI
dotnet run

