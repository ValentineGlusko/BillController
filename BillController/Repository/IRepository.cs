using System.Diagnostics;
using BillController.Configurations;
using BillController.Models;
using Microsoft.EntityFrameworkCore;

namespace BillController.Repository
{
    public interface IRepository<T > where T : class

    {
        DatabaseScopeConfigurations ScopeNaming { get; }
        ILogger<T> Logger { get; }
        Activity CurrentActivity { get; }
        ProblemRecorder Recorder { get; }
    DbSet<T> EntitySet { get; }
    public DataContext Context { get; }
    public Task<bool> AddAsync(T entity);
    public Task<bool> Update(T entity);
    public Task<bool> Delete(T entity);
    public Task<T?> Get(Guid id);
    }
}
