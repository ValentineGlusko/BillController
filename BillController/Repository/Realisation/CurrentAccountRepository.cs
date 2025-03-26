using System.Diagnostics;
using BillController.Configurations;
using BillController.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BillController.Repository.Realisation
{
    public class CurrentAccountRepository : IRepository<CurrentAccount>
    {
        public CurrentAccountRepository(IOptionsSnapshot<DatabaseScopeConfigurations> scopeNaming, ILogger<CurrentAccount> logger, ProblemRecorder recorder, DataContext context)
        {
            ScopeNaming = scopeNaming.Value;
            Logger = logger;
            CurrentActivity = Activity.Current;
            Recorder = recorder;
            Context = context;
            EntitySet = Context.CurrentAccounts;
        }
        public DatabaseScopeConfigurations ScopeNaming { get; }
        public ILogger<CurrentAccount> Logger { get; }
        public Activity CurrentActivity { get; }
        public ProblemRecorder Recorder { get; }
        public DbSet<CurrentAccount> EntitySet { get; }
        public DataContext Context { get; }
        public async Task<bool> AddAsync(CurrentAccount entity)
        {
            try
            {
                await EntitySet.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.Log( logLevel: LogLevel.Critical  ,$"Impossible to save entity {ex.StackTrace}");
                return false;
            }

            await Context.SaveChangesAsync();
;            return true;
        }

        public async Task<bool> Update(CurrentAccount entity)
        {
            try
            {
              await  EntitySet.Where(e => e.AccountId == entity.AccountId).ExecuteUpdateAsync(u =>
                        u.SetProperty(p => p.Name, entity.Name)
                            .SetProperty(p => p.AccountId, entity.AccountId)
                                .SetProperty(p => p.Bills, entity.Bills)
                                    .SetProperty(p => p.Amount, entity.Amount));
            }
            catch (Exception e){Logger.Log(LogLevel.Warning, $"Impossible to update entities{e.StackTrace}"); return false; }
            

            return true;
        }

        public async Task<bool> Delete(CurrentAccount entity)
        {
            try
            {
                await EntitySet.Where(e => e.AccountId == entity.AccountId).ExecuteDeleteAsync();
            }
            catch(Exception e){Logger.Log(LogLevel.Critical, $"Impossible to Delete entity\n {e.StackTrace}");
                return false;

            }

            return true;
        }

        public async Task<CurrentAccount> Get(Guid id)
        {
            return await EntitySet.Where(e => e.AccountId == id).FirstAsync();
        }
    }
}
