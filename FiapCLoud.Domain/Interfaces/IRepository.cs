namespace FiapCLoud.Domain.Interfaces;

public interface IRepository<T>
{
    Task<bool> CommitAsync();
}
