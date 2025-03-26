using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillController.Models.Configuration
{
    public class AccountConfiguration : IEntityTypeConfiguration<CurrentAccount>
    {
        public void Configure(EntityTypeBuilder<CurrentAccount> builder)
        {
            builder.HasKey(e => e.AccountId);
            builder.HasMany(e => e.Bills).WithOne(e => e.CurrentAccount).HasForeignKey(e => e.AccountGuid);
            builder.Property(e => e.Amount).HasPrecision(10, 2);
            builder.Property(e=> e.Version).IsRowVersion();

        }
    }
}
