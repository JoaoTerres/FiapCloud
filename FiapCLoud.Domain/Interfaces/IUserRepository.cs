using FiapCLoud.Domain.Entities;

namespace FiapCLoud.Domain.Interfaces;

public interface IUserRepository : IRepository<DomainUser>
{
    Task<bool> EmailExistsAsync(string email);
    Task<DomainUser?> GetByIdAsync(Guid userId);
    Task<DomainUser?> GetByApplicationUserIdAsync(Guid applicationUserId);
    Task AddAsync(DomainUser user);
}
