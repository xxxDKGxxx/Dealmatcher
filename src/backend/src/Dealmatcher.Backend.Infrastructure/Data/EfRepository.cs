namespace Dealmatcher.Backend.Infrastructure.Data;
public sealed class EfRepository<T>(AppDbContext dbContext) :
    RepositoryBase<T>(dbContext),
    IReadRepository<T>,
    IRepository<T> where T : class, IAggregateRoot
{
}
