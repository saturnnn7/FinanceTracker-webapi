using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using FinanceTracker.Common;
using FinanceTracker.DTOs.Auth;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration  _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _config         = config;
        _logger         = logger;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(
        RegisterDto dto, CancellationToken ct = default)
    {
        // Checking the uniqueness of the email address and username
        if (await _userRepository.ExistsByEmailAsync(dto.Email, ct))
            return Result<AuthResponseDto>.Fail(
                ErrorCodes.Conflict, "Email is already in use.");

        if (await _userRepository.ExistsByUsernameAsync(dto.Username, ct))
            return Result<AuthResponseDto>.Fail(
                ErrorCodes.Conflict, "Username is already in use.");

        var user = new User
        {
            Username     = dto.Username.ToLower(),
            Email        = dto.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            BaseCurrency = dto.BaseCurrency.ToUpper()
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

        _logger.LogInformation("New user registered: {Email}", user.Email);

        return Result<AuthResponseDto>.Ok(GenerateToken(user));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(
        LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email, ct);

        // Intentionally identical prompts for email and password
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Fail(
                ErrorCodes.Unauthorized, "Invalid email or password.");

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return Result<AuthResponseDto>.Ok(GenerateToken(user));
    }

    // -------------------------------------------------------

    private AuthResponseDto GenerateToken(User user)
    {
        var key       = _config["Jwt:Key"]!;
        var issuer    = _config["Jwt:Issuer"]!;
        var expiresIn = double.Parse(_config["Jwt:ExpiresInHours"] ?? "24");
        var expiresAt = DateTime.UtcNow.AddHours(expiresIn);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Email,          user.Email),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds      = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           issuer,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            Username  = user.Username,
            Email     = user.Email,
            Currency  = user.BaseCurrency,
            ExpiresAt = expiresAt
        };
    }
}