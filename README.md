# EduMart - E-commerce Web Application

## Overview
EduMart is an ASP.NET MVC e-commerce web application designed to manage and sell online courses (referred to as "Skills"). The application allows users to browse, purchase, and manage courses, while administrators can manage mentors, platforms, and course organizers. The application also includes features like user authentication, shopping cart functionality, and order management.

## Features
- **User Authentication**: Users can register, log in, and manage their profiles. Roles include Admin and User.
- **Course Management**: Admins can create, update, and delete courses (Skills), including details like course name, description, price, start/end dates, and associated mentors.
- **Shopping Cart**: Users can add courses to their shopping cart, view the cart, and proceed to checkout.
- **Order Management**: Users can view their order history, and admins can view all orders.
- **Mentor Management**: Admins can manage mentors, including their profiles and the courses they are associated with.
- **Platform Management**: Admins can manage platforms where courses are hosted.
- **Course Organizer Management**: Admins can manage organizations that organize the courses.
- **Search and Filter**: Users can search for courses by name or description.

## Technologies Used
- **ASP.NET Core MVC**: The application is built using the ASP.NET Core MVC framework.
- **Entity Framework Core**: Used for database operations and migrations.
- **Identity Framework**: Handles user authentication and authorization.
- **CSS**: Custom stylesheet (`wwwroot/css/site.css` with `tokens.css` and `base.css`), no CSS framework.
- **MySQL**: The database used to store application data (via `MySql.EntityFrameworkCore`).
- **jQuery**: Used for client-side scripting and AJAX requests.

## Directory Structure
```
toimur678-aspnet-mvc-ecommerce-webapp/
|-- EduMart.sln
|-- EduMart/
    |-- Controllers/
    |-- Data/
    |-- Migrations/
    |-- Models/
    |-- Properties/
    |-- Views/
    |-- wwwroot/
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- MySQL 8.4 (e.g. `brew install mysql@8.4 && brew services start mysql@8.4`)
- EF Core CLI: `dotnet tool install --global dotnet-ef`
- Any editor: VS Code with C# Dev Kit, Rider, or Visual Studio

### Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/toimur678/aspnet-mvc-ecommerce-webapp.git
   cd aspnet-mvc-ecommerce-webapp/EduMart
   ```
2. **Create the MySQL database and user** (run `mysql -u root`):
   ```sql
   CREATE DATABASE edumart CHARACTER SET utf8mb4;
   CREATE USER 'edumart'@'localhost' IDENTIFIED BY 'Dev_Passw0rd123';
   GRANT ALL PRIVILEGES ON edumart.* TO 'edumart'@'localhost';
   ```
3. **Set the connection string** (stored in user secrets, not in git):
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=edumart;User Id=edumart;Password=Dev_Passw0rd123;"
   ```
4. **Create the schema**:
   ```bash
   dotnet ef database update
   ```
5. **Run the application**:
   ```bash
   dotnet dev-certs https --trust   # once per machine
   dotnet watch run --launch-profile https
   ```
   Open https://localhost:7215. On first start, `AppDbInitializer.cs` creates the roles and the default users below.

### Default Users (Development only)
These accounts are only created when `ASPNETCORE_ENVIRONMENT` is `Development`, because their passwords are public. In any other environment no sample learners are created, and the admin account is only created if you set `Seed__AdminEmail` and `Seed__AdminPassword` as environment variables (or `Seed:AdminEmail` / `Seed:AdminPassword` in another configuration source).

- **Admin User**:
  - Email: `admin@gmail.com`
  - Password: `Admin@1234`
- **Sample learners** (role User, all with password `User@1234`):
  - `leila.haddad@example.com`, `marco.bianchi@example.com`, `aisha.bello@example.com`, `ethan.clarke@example.com`
  - `sofia.petrova@example.com`, `kenji.watanabe@example.com`, `nadia.rahman@example.com`, `oliver.grant@example.com`

On first start the seeder also fills an empty catalogue with 5 platforms, 5 agencies, 12 mentors, 15 courses and 10 sample orders. Each table is only seeded when it is empty, and course dates are set relative to the day the seed runs.

## Usage
- **Admin Role**: Admins can manage courses, mentors, platforms, and course organizers. They can also view all orders.
- **User Role**: Users can browse courses, add them to their shopping cart, and place orders. They can also view their order history.

## Contributing
Contributions are welcome! Please fork the repository and create a pull request with your changes.

## Acknowledgments
- ASP.NET Core Documentation
- Bootstrap Documentation
- Entity Framework Core Documentation

## Contact
For any questions or issues, please open an issue on the GitHub repository.

---

**Note**: This is a sample project for educational purposes. It is not intended for production use without further testing and security enhancements. In particular, checkout uses the PayPal sandbox and the payment is only confirmed in the browser: the server records the order without checking with PayPal that it was paid. A real store must verify the payment server-side (PayPal Orders API) before calling `CompleteOrder`.
