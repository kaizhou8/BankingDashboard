# Banking Dashboard

A full-stack banking dashboard application built with ASP.NET Core MVC and Entity Framework Core.

## Screenshots

### Dashboard Overview
![Dashboard Overview](docs/images/p1.png)
*Main dashboard showing account overview and recent transactions*

### Transaction History
![Transaction History](docs/images/p2.png)
*Detailed transaction history with filtering and sorting capabilities*

### Account Management
![Account Management](docs/images/p3.png)
*Account management interface for handling multiple accounts*

### Security Features
![Security Features](docs/images/p4.png)
*Advanced security features and authentication system*


## Features

* Secure user authentication and authorization
* Account management
* Transaction history
* Fund transfers between accounts
* Real-time balance updates
* Responsive design
* Dashboard with account overview
* User registration and login
* Multi-account support


## Technology Stack

* **Frontend**: ASP.NET Core MVC, Bootstrap 5, JavaScript
* **Backend**: .NET 8, Entity Framework Core
* **Database**: SQL Server
* **Authentication**: JWT Authentication
* **Development Tools**: Visual Studio 2022, Git


## Project Structure

* **BankingDashboard.Core**: Contains domain models, interfaces, and business logic  
   * Domain entities (Account, Transaction, User)  
   * Service interfaces  
   * Value objects and DTOs
* **BankingDashboard.Infrastructure**: Implements data access and services  
   * Entity Framework Core configurations  
   * Repository implementations  
   * Service implementations  
   * Database migrations  
   * JWT Authentication service
* **BankingDashboard.Web**: Web API and MVC application  
   * Controllers and Views  
   * ViewModels  
   * JWT Authentication middleware  
   * Frontend assets (CSS, JavaScript)
* **BankingDashboard.Tests**: Unit and integration tests  
   * Service tests  
   * Controller tests  
   * Repository tests


## Development Progress

### Completed Features

* Basic project structure and architecture
* User authentication system with JWT
* Account management functionality
* Transaction history
* Dashboard view with account overview
* Registration and login pages
* Security implementations
* Database setup and migrations

### In Progress

* Transaction processing improvements
* Account balance calculations
* User interface enhancements
* Error handling and validation
* Performance optimizations

### Planned Features

* Email notifications
* Account statements
* Mobile-responsive design improvements
* Advanced transaction filtering
* Export functionality for transactions
