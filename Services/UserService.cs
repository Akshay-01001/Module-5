using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.DTOs;
using UserManagementApi.Exceptions;
using UserManagementApi.Models;

namespace UserManagementApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;

    public UserService(
        ApplicationDbContext dbContext,
        IMapper mapper,
        IJwtTokenService jwtTokenService,
        IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _jwtTokenService = jwtTokenService;
        _passwordService = passwordService;
    }

    public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var emailExists = await _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail);
        if (emailExists)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        user.Password = _passwordService.HashPassword(user, dto.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserReadDto>(user);
    }

    public async Task<IEnumerable<UserReadDto>> GetAllAsync()
    {
        // Optimized LINQ query with Copilot help: AsNoTracking avoids change tracking for read-only list responses.
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderByDescending(user => user.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserReadDto>>(users);
    }

    public async Task<UserReadDto?> GetByIdAsync(int id)
    {
        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);
        return user is null ? null : _mapper.Map<UserReadDto>(user);
    }

    public async Task<UserReadDto?> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        if (user is null)
        {
            return null;
        }

        var emailTaken = await _dbContext.Users.AnyAsync(existing =>
            existing.Id != id && existing.Email == dto.Email.Trim().ToLowerInvariant());

        if (emailTaken)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        user.Name = dto.Name.Trim();
        user.Email = dto.Email.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.Password = _passwordService.HashPassword(user, dto.Password);
        }

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserReadDto>(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        if (user is null)
        {
            return false;
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail);

        // Fixed null reference issue using AI suggestion: check user before verifying the hashed password.
        if (user is null || !_passwordService.VerifyPassword(user, dto.Password))
        {
            return null;
        }

        var tokenResult = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            User = _mapper.Map<UserReadDto>(user)
        };
    }
}
