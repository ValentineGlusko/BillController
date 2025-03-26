using System.Diagnostics;
using BillController.Configurations;
using BillController.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace BillController.Repository.Realisation
{
    public class CategoryRepository : IRepository<Category>
    {
        public DatabaseScopeConfigurations ScopeNaming { get; }
        public ProblemRecorder Recorder { get; set; }
       public Activity CurrentActivity { get; }
        public ILogger<Category> Logger { get; }
        public DbSet<Category> EntitySet { get; }
        public DataContext Context { get; }
        public CategoryRepository(ILogger<Category> logger   , DataContext context, ProblemRecorder recorder , IOptions<DatabaseScopeConfigurations> config)
        {
            Logger = logger;
            EntitySet = context.Categories;
            Context = context;
            CurrentActivity = Activity.Current;
            Recorder = recorder;
            ScopeNaming = config.Value;

        }

        public async Task<bool> AddAsync(Category entity)
        {
            using var scope = Logger.BeginScope("Category:{Category}\nDate:{Date}", $"{ScopeNaming.CommonOperations}",
                       $"{DateTime.Now}");
            Logger.LogInformation($"Trying to add Entity:{nameof(entity)} to database");
            try
            {
                await EntitySet.AddAsync(entity);
                await Context.SaveChangesAsync();
                Logger.LogInformation($"Entity:{nameof(entity)} saved to the database\nDate: {DateTime.Now}");
                return true;
            }
            catch (Exception e)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Impossible to save Entity",
                    Detail = e.Message,
                    Instance = e.Source,
                    Extensions = new Dictionary<string, object?>()
                        {
                            { "Display Name: ", CurrentActivity.DisplayName },
                            { "Source:", CurrentActivity.Source },
                            { "Operation Name:", CurrentActivity.OperationName }
                        }
                };
                Recorder.SaveProblem(problem, functionForScope: (f) => f.BeginScope("Category:{Category}",
                    $"{ScopeNaming.NotSuccessfulOperation}"));
                return false;
            }
 
        }

        public async Task<bool> Update(Category entity)
        {
            using var scope = Logger.BeginScope("Category:{Category}\nDate:{Date}", $"{ScopeNaming.CommonOperations}", $"{DateTime.Now}");
            Logger.LogInformation($"Trying to add Entity:{nameof(entity)} to database");
            try
            {
                var result = await Context.Categories.Where(e => e.Id == entity.Id)
                    .ExecuteUpdateAsync(e =>
                        e.SetProperty(p => p.Bills, entity.Bills)
                            .SetProperty(p => p.Name, entity.Name));
                if (result == 0)
                {
                    Logger.LogWarning($"Entity with Id {entity.Id} not found in database.");
                    return false;
                }
            }
            catch (Exception e)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Impossible to Update Entity",
                    Detail = e.Message,
                    Instance = e.Source,
                    Extensions = new Dictionary<string, object?>()
                    {
                        {"Display Name: ",CurrentActivity.DisplayName},
                        {"Source:" ,CurrentActivity.Source},
                        {"Operation Name:", CurrentActivity.OperationName}
                    }
                };

                Recorder.SaveProblem(problem, functionForScope: (f) => f.BeginScope("Category:{Category}",
                    $"{ScopeNaming.NotSuccessfulOperation}"));
                return false;
            }
            Logger.LogInformation($"Entity:{nameof(entity)} updated to the database\nDate: {DateTime.Now}");
            return true;
        }

        public async Task<bool> Delete(Category entity)
        {
            using var scope = Logger.BeginScope("Category:{Category}\nDate:{Date}", $"{ScopeNaming.CommonOperations}", $"{DateTime.Now}");
            try
            {
                var res =   await EntitySet.Where(e => e.Id == entity.Id).ExecuteDeleteAsync();
                if (res == 1)
                {
                    Logger.LogWarning($"Entity with Id {entity.Id} not found in database.");
                    return false;
                }
            }
            
            catch (Exception e)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Impossible to Delete Entity",
                    Detail = e.Message,
                    Instance = e.Source,
                    Extensions = new Dictionary<string, object?>()
                    {
                        {"Display Name: ",CurrentActivity.DisplayName},
                        {"Source:" ,CurrentActivity.Source},
                        {"Operation Name:", CurrentActivity.OperationName}
                    }
                };

                Recorder.SaveProblem(problem, functionForScope: (f) => f.BeginScope("Category:{Category}",
                    $"{ScopeNaming.NotSuccessfulOperation}"));
                return false;
            }
            Logger.LogInformation($"Entity:{nameof(entity)} updated to the database\nDate: {DateTime.Now}");
            return true;


        }

        public async Task<Category?> Get(Guid id)
        {
            Category? entity;
            using var scope = Logger.BeginScope("Category:{Category}\nDate:{Date}", $"{ScopeNaming.CommonOperations}", $"{DateTime.Now}");
            try
            {
                 entity = await EntitySet.FindAsync(id);
                if (entity is null)
                {
                    Logger.LogWarning($"Entity with Id {entity.Id} not found in database.");
                }
            }
            catch (Exception e)
            {
                var problem = new ProblemDetails()
                {
                    Title = "Impossible to Get Entity",
                    Detail = e.Message,
                    Instance = e.Source,
                    Extensions = new Dictionary<string, object?>()
                    {
                        {"Display Name: ",CurrentActivity.DisplayName},
                        {"Source:" ,CurrentActivity.Source},
                        {"Operation Name:", CurrentActivity.OperationName}
                    }
                };

                Recorder.SaveProblem(problem, functionForScope: (f) => f.BeginScope("Category:{Category}",
                    $"{ScopeNaming.NotSuccessfulOperation}"));
                return null;
            }
            Logger.LogInformation($"Entity:{nameof(entity)} found in database\nDate: {DateTime.Now}");
            return entity;
        }
    }
}