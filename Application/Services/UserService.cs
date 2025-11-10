using Application.Dtos.User;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Intefaces.Repositories;
using Domain.Intefaces;
using Application.Common;
using System.Linq;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<User>> Add(UserCreateDto dto)
    {
        var user = dto.ToEntity();
        
        // Validação da entidade
        var validation = user.EhValido();
        if (!validation.IsValid)
            return ApiResponse<User>.ValidationFailure(validation);

        // Regra de negócio: email único
        if (await ExistsByEmail(user.Email.Endereco))
            return ApiResponse<User>.Error(400, "E-mail já cadastrado.", "Email");

        var addedUser = await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<User>.Success(addedUser);
    }

    public async Task<ApiResponse<User>> Update(Guid id, UserUpdateDto dto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser is null)
            return ApiResponse<User>.NotFound("Usuário não encontrado.");

        var entity = dto.ToEntity();
        entity.Id = id; // Garante que o Id seja o correto
        
        // Validação da entidade
        var validation = entity.EhValido();
        if (!validation.IsValid)
            return ApiResponse<User>.ValidationFailure(validation);

        // Regra de negócio: email único (se mudou)
        if (existingUser.Email.Endereco != entity.Email.Endereco && 
            await ExistsByEmail(entity.Email.Endereco))
        {
            return ApiResponse<User>.Error(400, "E-mail já cadastrado.", "Email");
        }

        var updatedUser = await _userRepository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<User>.Success(updatedUser);
    }

    public async Task<ApiResponse<User>> GetById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return ApiResponse<User>.NotFound("Usuário não encontrado.");

        return ApiResponse<User>.Success(user);
    }

    public async Task<ApiResponse<bool>> Delete(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return ApiResponse<bool>.NotFound("Usuário não encontrado.");

        await _userRepository.DeleteAsync(user);
        var affected = await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Success(affected > 0);
    }

    public async Task<ApiResponse<IEnumerable<User>>> GetAll(int pageNumber, int pageSize)
    {
        var users = await _userRepository.GetAllAsync();
        return ApiResponse<IEnumerable<User>>.Success(users);
    }
    
    private async Task<bool> ExistsByEmail(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email.Endereco == email);
        return users.Any();
    }
}