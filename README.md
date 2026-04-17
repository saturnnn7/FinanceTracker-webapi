# Finance Tracker API

A personal finance tracking REST API built with ASP.NET Core 8.

## Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: SQLite (via Entity Framework Core 8)
- **Authentication**: JWT Bearer tokens
- **Password hashing**: BCrypt
- **Validation**: FluentValidation
- **Documentation**: Swagger / OpenAPI
- **CSV Export**: CsvHelper
- **Currency rates**: exchangerate-api.com (cached in-memory)

## Features

### Core
- User registration and JWT authentication
- Multi-currency accounts (PLN, USD, EUR, GBP, RUB)
- Income / Expense / Transfer transactions
- Category management (system + custom)
- Budget limits with real-time progress tracking
- Savings goals with automatic progress calculation
- Transaction filtering, sorting and pagination
- CSV export for any date range

### Automation
- **Recurring transactions** — templates that auto-create transactions on schedule (daily / weekly / monthly / yearly)
- **In-app notifications** — budget warnings at 80% and 100%, reminders 24h before recurring transactions

### Statistics
- Expense breakdown by category for any date range
- Monthly income vs expense summary
- Account balance history by day (for chart visualization)

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

### Setup

**1. Clone the repository**
```bash
git clone https://github.com/your-username/FinanceTracker.git
cd FinanceTracker
```

**2. Set JWT secret key via user-secrets**
```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "your-secret-key-min-32-characters!!"
```

**3. Run the application**
```bash
dotnet run
```

The app will automatically apply database migrations on startup.

**4. Open Swagger UI**

---

## API Overview

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Accounts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/accounts` | Get all accounts with current balance |
| GET | `/api/accounts/{id}` | Get account by id |
| POST | `/api/accounts` | Create a new account |
| PUT | `/api/accounts/{id}` | Update account |
| DELETE | `/api/accounts/{id}` | Delete account |

### Transactions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/transactions` | Get transactions with filters and pagination |
| GET | `/api/transactions/{id}` | Get transaction by id |
| POST | `/api/transactions` | Create transaction (Income / Expense / Transfer) |
| PUT | `/api/transactions/{id}` | Update transaction |
| DELETE | `/api/transactions/{id}` | Delete transaction |
| GET | `/api/transactions/export` | Export to CSV |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories (system + custom) |
| POST | `/api/categories` | Create custom category |
| DELETE | `/api/categories/{id}` | Delete custom category |

### Budgets
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/budgets` | Get budgets for month/year |
| GET | `/api/budgets/{id}` | Get budget with spent amount |
| POST | `/api/budgets` | Create budget limit |
| PUT | `/api/budgets/{id}` | Update budget limit |
| DELETE | `/api/budgets/{id}` | Delete budget |

### Goals
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/goals` | Get all savings goals with progress |
| GET | `/api/goals/{id}` | Get goal by id |
| POST | `/api/goals` | Create savings goal |
| PUT | `/api/goals/{id}` | Update goal |
| DELETE | `/api/goals/{id}` | Delete goal |

### Recurring Transactions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/recurring-transactions` | Get all templates |
| GET | `/api/recurring-transactions/{id}` | Get template by id |
| POST | `/api/recurring-transactions` | Create template |
| PATCH | `/api/recurring-transactions/{id}/toggle` | Toggle active/inactive |
| DELETE | `/api/recurring-transactions/{id}` | Delete template |

### Statistics
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/statistics/categories` | Expense breakdown by category |
| GET | `/api/statistics/monthly` | Monthly income vs expense |
| GET | `/api/statistics/balance-history` | Balance history by day |

### Notifications
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | Get all notifications |
| GET | `/api/notifications/unread-count` | Get unread count |
| PATCH | `/api/notifications/mark-all-read` | Mark all as read |

---

## Project Structure
```text
FinanceTracker/
├── BackgroundServices/
│   ├── RecurringTransactionProcessor.cs
│   └── NotificationProcessor.cs
|
├── Common/
│   ├── Result.cs
│   ├── NotificationTypes.cs
│   └── ExceptionHandlerMiddleware.cs
|
├── Controllers/
│   ├── BaseController.cs
│   ├── AuthController.cs
│   ├── AccountsController.cs
│   ├── TransactionsController.cs
│   ├── CategoriesController.cs
│   ├── BudgetsController.cs
│   ├── GoalsController.cs
│   ├── RecurringTransactionsController.cs
│   ├── StatisticsController.cs
│   └── NotificationsController.cs
|
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── Interceptors/
│       └── AuditInterceptor.cs
|
├── DTOs/
│   ├── Common/
│   ├── Auth/
│   ├── Account/
│   ├── Transaction/
│   ├── Category/
│   ├── Budget/
│   ├── Goal/
│   ├── RecurringTransaction/
│   ├── Statistics/
│   └── Notification/
|
├── Models/
│   ├── Enums/
│   ├── Interfaces/
│   └── External/
|
├── Repositories/
│   └── Interfaces/
|
├── Services/
│   └── Interfaces/
|
├── Validators/
│   ├── Auth/
│   ├── Account/
│   ├── Transaction/
│   ├── Budget/
│   ├── Goal/
│   └── RecurringTransaction/
|
├── appsettings.json
├── GlobalUsings.cs
└── Program.cs
```

---

## Business Rules

**Balance calculation**
Account balance is never stored directly. It is always computed as:
CurrentBalance = InitialBalance + SUM(Income) - SUM(Expense)

**Transfer transactions**
A transfer between accounts creates two linked transactions — an Expense on the source account and an Income on the destination. Both are deleted together.

**System categories**
10 system categories are seeded on first run and cannot be deleted. Users can create custom categories, which can only be deleted if they have no transactions.

**Budget uniqueness**
Only one budget per category per month is allowed.

**Recurring transactions**
The `RecurringTransactionProcessor` runs immediately on startup and then every 60 minutes. It creates a transaction for every active template where `NextRunAt <= UtcNow` and updates `NextRunAt` based on the interval.

**Notifications**
The `NotificationProcessor` runs every 6 hours. It creates a `BudgetWarning` when spending reaches 80% of the limit, `BudgetExceeded` at 100%, and `RecurringReminder` 24 hours before the next scheduled transaction. Duplicate notifications within the same day are suppressed.

---

## Configuration

`appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=finance.db"
  },
  "Jwt": {
    "Issuer": "FinanceTracker",
    "ExpiresInHours": "24"
  },
  "CurrencyApi": {
    "BaseUrl": "https://api.exchangerate-api.com/v4/latest/",
    "CacheDurationMinutes": "60"
  }
}
```

`Jwt:Key` must be set via user-secrets or environment variable — never in `appsettings.json`.

---

## Known Limitations

- SQLite does not support `SUM()` on `decimal` columns at the database level — aggregations are performed in-memory after fetching data
- No refresh token flow — JWT tokens expire after 24 hours and require re-login
- Currency conversion uses current rates only — historical rates at transaction time are not stored
- No email notifications — only in-app notifications are implemented