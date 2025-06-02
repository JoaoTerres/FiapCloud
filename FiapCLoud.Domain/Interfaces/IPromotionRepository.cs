using FiapCLoud.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiapCLoud.Domain.Interfaces;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task AddAsync(Promotion promotion);
    Task<Promotion?> GetByIdAsync(Guid promotionId);
    Task<bool> ExistsByNameAsync(string name);
    Task<IEnumerable<Promotion>> GetAllAsync();
}