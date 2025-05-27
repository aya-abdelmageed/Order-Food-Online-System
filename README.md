# 🛒 Online Food Ordering System

A web-based food ordering system built with ASP.NET Core MVC, allowing users to browse menus, place orders, and pay online. The system is organized using layered architecture for maintainability and scalability.

## 🚀 Features

- User-friendly interface for customers to browse and order food.
- Admin dashboard to manage orders and menu items.
- Real-time order status tracking.
- Online payments via Stripe.
- Redis caching for faster data access.
- Secure and scalable system design.

## 🧰 Technologies & Tools

- **ASP.NET Core MVC** – Web application framework.
- **Entity Framework Core** – ORM for database interactions.
- **C#** – Primary programming language.
- **SQL Server** – Database backend.
- **HTML/CSS/JavaScript** – Frontend development.
- **LINQ** – For querying data.
- **Redis** – Used for caching.
- **Stripe** – Payment gateway integration.

## 🧱 Architecture & Patterns

- **Layered Architecture (PL, BLL, DAL, Service)** – Clean separation of concerns.
- **Generic Repository Pattern** – For reusable data access logic.
- **Unit of Work Pattern** – For transaction management.
- **Dependency Injection** – For loosely coupled components.
- **Union Architecture** – Combining layered and clean architecture principles.

## ⚙️ How to Run

1. Clone or download the repository.
2. Open `OrderFood.sln` in Visual Studio.
3. Restore NuGet packages.
4. Update connection string in `appsettings.json`.
5. Run the project.
