using FiapCloud.Infra.Context;
using FiapCLoud.Domain.Entities;
using FiapCLoud.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FiapCloud.Infra.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Promotion promotion)
    {
        await _context.Promotions.AddAsync(promotion);
    }

    public async Task<Promotion?> GetByIdAsync(Guid promotionId)
    {
        return await _context.Promotions
                             .Include(p => p.GamePromotions)
                             .FirstOrDefaultAsync(p => p.Id == promotionId);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Promotions.AnyAsync(p => EF.Functions.ILike(p.Name, name));
    }

    public async Task<IEnumerable<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
                             .Include(p => p.GamePromotions)
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task<bool> CommitAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }


}