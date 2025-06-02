// Em FiapCloud.Infra/Repositories/UserRepository.cs
using FiapCloud.Infra.Context;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FiapCloud.Infra.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(DomainUser user)
    {
        await _context.DomainUsers.AddAsync(user);
    }

    public async Task<DomainUser?> GetByIdAsync(Guid userId)
    {
        var user = await _context.DomainUsers
                             .Include(u => u.Library.Sales)
                             .Include(u => u.Wallet)
                             .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            _logger.LogWarning("Repositório: DomainUser ID {DomainUserId} não encontrado.", userId);
        }
        return user;
    }

    public async Task<DomainUser?> GetByApplicationUserIdAsync(Guid applicationUserId)
    {
        var user = await _context.DomainUsers
                             .Include(u => u.Library.Sales)
                             .Include(u => u.Wallet)
                             .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);
        if (user == null)
        {
            _logger.LogWarning("Repositório: DomainUser para ApplicationUserID {ApplicationUserId} não encontrado.", applicationUserId);
        }
        return user;
    }

    public async Task<bool> CommitAsync()
    {
        try
        {
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                _logger.LogInformation("Repositório: CommitAsync não resultou em alterações no banco de dados.");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repositório: Erro ao tentar persistir alterações (CommitAsync).");
            throw;
        }
    }
}