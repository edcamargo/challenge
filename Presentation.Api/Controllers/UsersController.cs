using Application.Dtos.User;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.Common;
using Presentation.Api.Extensions;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserReponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _userService.GetAll(1, 100);
        return response.ToActionResult(UserReponseDto.FromEntity);
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(ApiResponse<UserReponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _userService.GetById(id);
        return response.ToActionResult(UserReponseDto.FromEntity);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserReponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var response = await _userService.Add(dto);
        return response.ToCreatedAtActionResult(
            nameof(GetById), 
            UserReponseDto.FromEntity, 
            userDto => new { id = userDto.Id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserReponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        var response = await _userService.Update(id, dto);
        return response.ToActionResult(UserReponseDto.FromEntity);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _userService.Delete(id);
        return response.ToNoContentResult();
    }
}
