namespace Application.Dtos.Task;

public record TaskCreateDto
(
    string Title,
    string? Description, 
    DateTime CreatedAt,
    DateTime? DueDate,
    Guid UserId)
{
    public Domain.Entities.Tasks ToEntity()
        => new Domain.Entities.Tasks(Title, Description, DueDate, UserId);
}

public record TaskUpdateDto
(
    Guid Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime? DueDate,
    Guid UserId)
{
    public Domain.Entities.Tasks ToEntity()
    {
        var task = new Domain.Entities.Tasks(Title, Description, DueDate, UserId)
        {
            Id = Id
        };
        
        return task;
    }
}

public record TaskResponseDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime CreatedAt,
    DateTime? DueDate,
    Guid UserId,
    UserInfoDto? User,
    bool IsCompleted)
{
    public static TaskResponseDto FromEntity(Domain.Entities.Tasks task)
        => new TaskResponseDto(
            task.Id,
            task.Title,
            task.Description,
            task.CreatedAt,
            task.DueDate,
            task.UserId,
            task.User != null ? UserInfoDto.FromEntity(task.User) : null,
            task.IsCompleted);
}

public record UserInfoDto(
    Guid Id,
    string Name,
    string Email)
{
    public static UserInfoDto FromEntity(Domain.Entities.User user)
        => new UserInfoDto(
            user.Id,
            user.Name,
            user.Email.Endereco);
}
