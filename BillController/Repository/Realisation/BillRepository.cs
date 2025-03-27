using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using BillController.Configurations;
using BillController.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace BillController.Repository.Realisation
{
    public class BillRepository : IRepository<Bill>
    {
        public BillRepository(IOptionsSnapshot<DatabaseScopeConfigurations> scopeNaming,ILogger<Bill> logger , ProblemRecorder recorder, DataContext context)
        {
            ScopeNaming = scopeNaming.Value;
            Logger = logger;
            CurrentActivity = Activity.Current;
            Recorder = recorder;
            Context = context;
            EntitySet = Context.Bills;
        }

        public DatabaseScopeConfigurations ScopeNaming { get; }
        public ILogger<Bill> Logger { get; }
        public Activity CurrentActivity { get; }
        public ProblemRecorder Recorder { get; }
        public DbSet<Bill> EntitySet { get; }
        public DataContext Context { get; }

        public async Task<bool> AddAsync(Bill entity)
        {
            Log.ForContext("Test",  entity);
            try
            {
                Context.Bills.Add(entity);
                await Context.SaveChangesAsync();
                Log.Write(LogEventLevel.Information, "Enttiy Added");
                return true;
            }
            catch (Exception e)
            {
                Log.Write(LogEventLevel.Information, $"Enttiy can't be Added\n {e.StackTrace}");
                return false;
            }
        }

        public async Task<bool> Update(Bill entity)
        {
         var result = await   EntitySet.Where(e => e.BillId == entity.BillId)
                .ExecuteUpdateAsync(e =>
                    e.SetProperty(e => e.Name, entity.Name)
                        .SetProperty(v => v.AccountGuid, entity.AccountGuid)
                        .SetProperty(e => e.Amount, entity.Amount)
                        .SetProperty(v => v.BillId, entity.BillId)
                        .SetProperty(b => b.CategoryId, entity.CategoryId)
                        .SetProperty(b => b.CurrentAccount, entity.CurrentAccount)
                        .SetProperty(e => e.DueDate, entity.DueDate)
                        .SetProperty(x => x.Name, entity.Name)
                        .SetProperty(b => b.Category, v => v.Category)
                        .SetProperty(b => b.IsPayed, entity.IsPayed)
                        .SetProperty(c => c.PayedDate, entity.PayedDate)
                );
         return result != 0;
        }

        public async Task<bool> Delete(Guid id)
        {
          await  Context.Bills.Where(b => b.BillId == id).ExecuteDeleteAsync();
          var invert = Context.Bills.Any(e => e.BillId == id);
            return !invert;
        }

        public Task<Bill?> Get(Guid id)
        {
            Task<Bill?> objects = EntitySet.SingleAsync(e => e.BillId == id);
            return objects;
        }

        
    }
}
