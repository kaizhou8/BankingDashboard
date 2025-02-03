# Banking Dashboard

A full-stack banking dashboard application built with ASP.NET Core MVC and Entity Framework Core.

## Features

- Secure user authentication and authorization
- Account management
- Transaction history
- Fund transfers between accounts
- Real-time balance updates
- Responsive design
- Dashboard with account overview
- User registration and login
- Multi-account support

## Technology Stack

- **Frontend**: ASP.NET Core MVC, Bootstrap 5, JavaScript
- **Backend**: .NET 8, Entity Framework Core
- **Database**: SQL Server
- **Authentication**: JWT Authentication
- **Development Tools**: Visual Studio 2022, Git

## Project Structure

- **BankingDashboard.Core**: Contains domain models, interfaces, and business logic
  - Domain entities (Account, Transaction, User)
  - Service interfaces
  - Value objects and DTOs
- **BankingDashboard.Infrastructure**: Implements data access and services
  - Entity Framework Core configurations
  - Repository implementations
  - Service implementations
  - Database migrations
  - JWT Authentication service
- **BankingDashboard.Web**: Web API and MVC application
  - Controllers and Views
  - ViewModels
  - JWT Authentication middleware
  - Frontend assets (CSS, JavaScript)
- **BankingDashboard.Tests**: Unit and integration tests
  - Service tests
  - Controller tests
  - Repository tests

## Development Progress

### Completed Features
- Basic project structure and architecture
- User authentication system with JWT
- Account management functionality
- Transaction history
- Dashboard view with account overview
- Registration and login pages
- Security implementations
- Database setup and migrations

### In Progress
- Transaction processing improvements
- Account balance calculations
- User interface enhancements
- Error handling and validation
- Performance optimizations

### Planned Features
- Email notifications
- Account statements
- Mobile-responsive design improvements
- Advanced transaction filtering
- Export functionality for transactions

## Security Features

- JWT-based authentication
- Password hashing with salt
- HTTPS enforcement
- Cross-Site Request Forgery (CSRF) protection
- Role-based authorization
- Input validation and sanitization
- Secure password requirements
- Token-based session management

## Setup Instructions

1. Prerequisites:
   - .NET 8 SDK
   - SQL Server
   - Visual Studio 2022 (recommended)

2. Database Setup:
   ```bash
   dotnet ef database update
   ```

3. Configuration:
   - Update connection string in appsettings.json
   - Configure JWT settings in appsettings.json:
     ```json
     "JwtSettings": {
       "SecretKey": "your-256-bit-secret",
       "Issuer": "BankingDashboard",
       "Audience": "BankingDashboardClient",
       "ExpirationMinutes": 60
     }
     ```

4. Running the Application:
   ```bash
   dotnet run --project BankingDashboard.Web
   ```

## API Endpoints

### Authentication
- POST /api/auth/register - User registration
- POST /api/auth/login - User login (returns JWT token)
- POST /api/auth/logout - User logout

### Accounts
- GET /api/account/{id} - Get account details
- GET /api/account/user/{userId} - Get user's accounts
- POST /api/account - Create new account
- GET /api/account/{id}/balance - Get account balance

### Transactions
- GET /api/transaction/{id} - Get transaction details
- GET /api/transaction/account/{accountId} - Get account transactions
- POST /api/transaction/transfer - Transfer funds
- POST /api/transaction/deposit - Make deposit
- POST /api/transaction/withdraw - Make withdrawal

## Authentication Flow

1. Registration:
   - User submits registration details
   - Server creates user account
   - Returns JWT token

2. Login:
   - User submits credentials
   - Server validates credentials
   - Returns JWT token

3. API Requests:
   - Client includes JWT token in Authorization header
   - Server validates token for each request
   - Returns requested data if authorized

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

For any questions or suggestions, please open an issue in the repository.

---
Last Updated: February 3, 2025
