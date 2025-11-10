using System.ComponentModel.DataAnnotations;
using Domain.ValueObjects;

namespace Application.Dtos.User;

public record UserCreateDto(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Name,

    [Required(ErrorMessage = "O email é obrigatório.")]
    string Email
)
{
    public Domain.Entities.User ToEntity()
        => new Domain.Entities.User(Name, new Email(Email));
}

public record UserUpdateDto(
    [Required(ErrorMessage = "O ID é obrigatório.")]
    Guid Id,

    [Required(ErrorMessage = "O nome é obrigatório.")]
    string Name,

    [Required(ErrorMessage = "O email é obrigatório.")]
    string Email
)
{
    public Domain.Entities.User ToEntity()
        => new Domain.Entities.User(Name, new Email(Email))
        {
            Id = Id
        };
}

public record UserReponseDto(
    Guid Id,
    string Name,
    string Email
)
{

    public static UserReponseDto FromEntity(Domain.Entities.User user)
        => new UserReponseDto(
            user.Id,
            user.Name,
            user.Email.Endereco
        );
}
