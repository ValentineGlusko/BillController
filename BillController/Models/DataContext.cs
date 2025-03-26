using System.Reflection;
using BillController.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BillController.Models
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Bill> Bills => Set<Bill>();
        public DbSet<Category> Categories => Set<Category>();
        public  DbSet<CurrentAccount> CurrentAccounts => Set<CurrentAccount>();

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(BillConfiguration)));
        }
    }
}
 
