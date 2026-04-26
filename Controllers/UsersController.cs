using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApi.DTOs;
using UserManagementApi.Services;

namespace UserManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserReadDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserReadDto>>> Create([FromBody] UserCreateDto dto)
    {
        var user = await _userService.CreateAsync(dto);
        var response = ApiResponse<UserReadDto>.Ok(user, "User created successfully.");

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserReadDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserReadDto>>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserReadDto>>.Ok(users, "Users retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserReadDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserReadDto>>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound(ApiResponse<UserReadDto>.Fail("User not found."));
        }

        return Ok(ApiResponse<UserReadDto>.Ok(user, "User retrieved successfully."));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserReadDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserReadDto>>> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);
        if (user is null)
        {
            return NotFound(ApiResponse<UserReadDto>.Fail("User not found."));
        }

        return Ok(ApiResponse<UserReadDto>.Ok(user, "User updated successfully."));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("User not found."));
        }

        return Ok(ApiResponse<object>.Ok(new { id }, "User deleted successfully."));
    }
}
