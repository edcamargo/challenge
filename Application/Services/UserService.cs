using Application.Custom;
using Application.Dtos.User;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Intefaces.Repositories;
using Domain.Intefaces;
using Domain.Validations;
using FluentValidation.Results;

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

    public async Task<OperationResult<User>> Add(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        var user = dto.ToEntity();
        var userValid = user.EhValido();

        if (!userValid.IsValid)
            return new OperationResult<User>(user, userValid!);

        var addUser = await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new OperationResult<User>(addUser, userValid);
    }

    public async Task<OperationResult<User>> Update(Guid id, UserUpdateDto dto, CancellationToken cancellationtoken = default)
    {
        var entity = dto.ToEntity();
        var uservalid = entity.EhValido();

        if (!uservalid.IsValid)
            return new OperationResult<User>(entity, uservalid!);

        var user = await _userRepository.GetByIdAsync(id, cancellationtoken);

        if (user is null)
        {
            var validationresult = new ValidationResult();
            ActionErrorCustom.ActionError(validationresult, "id", "usuário não encontrado.");
            return new OperationResult<User>(Guid.Empty, validationresult);
        }

        var updateduser = await _userRepository.UpdateAsync(entity, cancellationtoken);
        await _unitOfWork.SaveChangesAsync(cancellationtoken);
        return new OperationResult<User>(updateduser, uservalid);
    }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<OperationResult<bool>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            var validationresult = new ValidationResult();
            ActionErrorCustom.ActionError(validationresult, "id", "usuário não encontrado.");
            return new OperationResult<bool>(Guid.Empty, validationresult);
        }

        var resultado = user.EhValido();
        if (!resultado.IsValid)
            return new OperationResult<bool>(false, resultado!);

        await _userRepository.DeleteAsync(user, cancellationToken);
        var affected = await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new OperationResult<bool>(affected > 0, resultado);
    }

    public async Task<IEnumerable<User>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }
}