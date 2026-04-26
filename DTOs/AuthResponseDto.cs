namespace UserManagementApi.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public UserReadDto User { get; set; } = new();
}
