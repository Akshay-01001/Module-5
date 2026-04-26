using UserManagementApi.Models;

namespace UserManagementApi.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}
