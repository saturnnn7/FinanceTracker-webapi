using FinanceTracker.Data;
using FinanceTracker.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

using FluentValidation;
using FluentValidation.AspNetCore;
using FinanceTracker.DTOs.Common;

using FinanceTracker.Repositories;
using FinanceTracker.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Interceptor
builder.Services.AddSingleton<AuditInterceptor>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IRecurringTransactionRepository, RecurringTransactionRepository>();
builder.Services.AddScoped<IGoalRepository, GoalRepository>();

// Add services to the container.
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

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
        };
    });

// We automatically register all validators from the build
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Migrationd for dev
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();