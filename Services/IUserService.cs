using UserManagementApi.DTOs;

namespace UserManagementApi.Services;

public interface IUserService
{
    Task<UserReadDto> CreateAsync(UserCreateDto dto);

    Task<IEnumerable<UserReadDto>> GetAllAsync();

    Task<UserReadDto?> GetByIdAsync(int id);

    Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto dto);

    Task<bool> DeleteAsync(int id);

    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
}
