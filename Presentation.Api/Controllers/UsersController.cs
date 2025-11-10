using Application.Dtos.User;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Common;

namespace Presentation.Api.Controllers
{
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
        [ProducesResponseType(typeof(IEnumerable<UserReponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAll(1, 100, cancellationToken);
            var dtos = (users ?? Enumerable.Empty<Domain.Entities.User>()).Select(UserReponseDto.FromEntity);
            return ResultMapper.From(Result<IEnumerable<UserReponseDto>>.Ok(dtos));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserReponseDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetById(id, cancellationToken);
            if (user == null) 
                return ResultMapper.From(Result<object>.Fail("not found"));

            var dto = UserReponseDto.FromEntity(user);
            return ResultMapper.From(Result<UserReponseDto>.Ok(dto));
        }
    }
}
