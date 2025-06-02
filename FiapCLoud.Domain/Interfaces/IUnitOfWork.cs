namespace FiapCloud.Infra.Context;

public interface IUnitOfWork
{
    Task<bool> CommitAsync();
}
