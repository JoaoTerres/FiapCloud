using System.Net;
using FiapCLoud.Domain.Exceptions; 
using FiapCLoud.Domain.Interfaces; 

namespace FiapCLoud.Domain.Services;


public class UserDomainService : IUserDomainService 
{
    private readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task EnsureEmailIsUniqueAsync(string email)
    {
        if (await _userRepository.EmailExistsAsync(email)) 
            throw new DomainException("E-mail já cadastrado.", HttpStatusCode.BadRequest); 
    }

    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId); 

        if (user == null)
        {
            throw new DomainException("Usuário não encontrado.", HttpStatusCode.NotFound); 
        }

        if (user.IsActive)
        {
            return false;
        }

        user.Activate(); 
        await _userRepository.CommitAsync(); 
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId); 

        if (user == null)
        {
            throw new DomainException("Usuário não encontrado.", HttpStatusCode.NotFound); 
        }

        if (!user.IsActive)
        {
            return false;
        }

        user.Deactivate(); 
        await _userRepository.CommitAsync(); 
        return true;
    }
}