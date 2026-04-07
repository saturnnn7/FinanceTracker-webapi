using FinanceTracker.Common;
using FinanceTracker.DTOs.Auth;

namespace FinanceTracker.Services.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct = default);
}