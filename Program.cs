using System.Text;

using FinanceTracker.Data;
using FinanceTracker.Data.Interceptors;

using FinanceTracker.DTOs.Common;
using FinanceTracker.Common;
using FinanceTracker.BackgroundServices;
using FinanceTracker.Repositories;
using FinanceTracker.Repositories.Interfaces;

using FinanceTracker.Services;
using FinanceTracker.Services.Interfaces;

using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
// Background Service
builder.Services.AddHostedService<RecurringTransactionProcessor>();
builder.Services.AddHostedService<NotificationProcessor>();

// -------------------------------------------------------
// Interceptor — Singleton
builder.Services.AddSingleton<AuditInterceptor>();

// -------------------------------------------------------
// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------------------------------------------
// Repositories
builder.Services.AddScoped<IUserRepository,                 UserRepository>();
builder.Services.AddScoped<IAccountRepository,              AccountRepository>();
builder.Services.AddScoped<ICategoryRepository,             CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository,          TransactionRepository>();
builder.Services.AddScoped<IBudgetRepository,               BudgetRepository>();
builder.Services.AddScoped<IRecurringTransactionRepository, RecurringTransactionRepository>();
builder.Services.AddScoped<IGoalRepository,                 GoalRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// -------------------------------------------------------
// Services
builder.Services.AddScoped<IAuthService,                    AuthService>();
builder.Services.AddScoped<IAccountService,                 AccountService>();
builder.Services.AddScoped<ICategoryService,                CategoryService>();
builder.Services.AddScoped<ITransactionService,             TransactionService>();
builder.Services.AddScoped<IBudgetService,                  BudgetService>();
builder.Services.AddScoped<IGoalService,                    GoalService>();
builder.Services.AddScoped<IRecurringTransactionService,    RecurringTransactionService>();
builder.Services.AddScoped<IStatisticsService,              StatisticsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// IMemoryCache for caching exchange rates
builder.Services.AddMemoryCache();

// HttpClient for making requests to an external currency exchange rate API
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();

// Register CurrencyService
builder.Services.AddScoped<ICurrencyService, CurrencyService>();

// -------------------------------------------------------
// JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// -------------------------------------------------------
// Controllers + FluentValidation + ApiResponse format
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Intercept the automatic 400 response from [ApiController]
        // and return it in our ApiResponse format
        options.InvalidModelStateResponseFactory = context =>
        {
            var details = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                );

            var response = ApiResponse<object>.Fail(
                ErrorCodes.ValidationError,
                "One or more validation errors occurred.",
                details);

            return new BadRequestObjectResult(response);
        };
    });

// We automatically register all validators from the build
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// -------------------------------------------------------
// Swagger + Bearer token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "Finance Tracker API",
        Version = "v1",
        Description = "Personal finance tracking REST API"
    });

    // Add an “Authorize” button to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// -------------------------------------------------------
var app = builder.Build();
// -------------------------------------------------------

// Exception Handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Migrationd for dev
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Finance Tracker API v1");
    });
}


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();